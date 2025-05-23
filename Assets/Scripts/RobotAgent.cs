using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

using static RobotController;
using static EnvManager;
using static TreeManager;
using static EpisodeManager;

public class RobotAgent : Agent
{
    private Vector3 initPos;
    private Quaternion initRot;
    public string agent_id;

    public BehaviorParameters bps;
    public Rigidbody rb;
    public Transform tf;

    public RobotController robotCtrl;
    public GameObject target;
    public GameObject alternative;
    public GameObject peerAgent;
    public RobotAgent peerScript;
    public Animator animator;
    private bool isHarvesting = false;

    public float priorityWeight = 0.5f;

    public EnvManager env;
    public EpisodeManager episode;

    public float peerDistance;
    public float targetDistance;
    public float targetDistanceBeforeAction;
    public float targetDistanceAfterAction;
    public float alternativeDistance;

    public override void Initialize()
    {
        env = FindObjectOfType<EnvManager>();
        episode = FindObjectOfType<EpisodeManager>();
        agent_id = this.gameObject.name;
        bps = GetComponent<BehaviorParameters>();
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        robotCtrl = GetComponent<RobotController>();
        if (agent_id == "RobotAgent1")
        {
            peerAgent = GameObject.Find("RobotAgent2");
        }
        else
        {
            peerAgent = GameObject.Find("RobotAgent1");
        }
        peerScript = peerAgent.GetComponent<RobotAgent>();
        initPos = tf.localPosition;
        initRot = tf.rotation;
        animator = GetComponent<Animator>();
    }

    public override void OnEpisodeBegin()
    {
        tf.localPosition = initPos;
        tf.rotation = initRot;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        robotCtrl.acc = 0;
        robotCtrl.turn = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 각 관측값의 최소최대 구하여 0 ~ 1 사이 값으로 normalize 진행
        sensor.AddObservation((rb.velocity.x + 10f) / 40f); //-10 ~ 30
        sensor.AddObservation((rb.velocity.z + 10f) / 40f); //-10 ~ 30
        sensor.AddObservation((rb.angularVelocity.y + 5.5f) / 11f); //-5.5 ~ +5.5 정도

        peerDistance = CalculateDistance(peerAgent);
        sensor.AddObservation(peerDistance / 180f); //최대 거리는 180쯤

        float targetPriority = 0;
        float alternativePriority = 0;
        foreach (TreeManager tree in env.trees) //모든 트리 순회
        {
            if (tree.IsFruitRot()) //제때 수확 못해 썩은 열매 있다면 각 열매에 대해 네거티브 보상
            {
                AddReward(-0.5f);
            }

            if ((env.fruitTrees != null) && (env.fruitTrees.Contains(tree))) //열매 열린 애들 순회
            {
                Vector3 offset = tree.transform.position - tf.position;
                float distance = offset.magnitude;
                float stepsAfterRipen = tree.fruitStepCounter;

                //거리 및 익은 이후의 스텝수 사용해 중요도 계산
                float priority = (1 - priorityWeight) * stepsAfterRipen + priorityWeight / distance;

                if (priority >= targetPriority)
                {
                    targetPriority = priority;
                    target = tree.gameObject;
                }
                else if ((priority < targetPriority) && (priority >= alternativePriority))
                {
                    alternativePriority = priority;
                    alternative = tree.gameObject;
                }
                else
                {
                    // 암것도 안함
                }
            }
        }

        // 열매 열린 애들 순회 끝났는데 아직 target/alternative 둘다 또는 alternative만 비어있다면
        if (alternative == null)
        {
            float firstPriority = 0;
            float secondPriority = 0;
            foreach (TreeManager tree in env.trees)
            {
                if ((env.fruitTrees != null) && (env.fruitTrees.Contains(tree)))
                {
                    continue;
                }

                Vector3 offset = tree.transform.position - tf.position;
                float distance = offset.magnitude;
                float currentHP = tree.HP;

                //체력과 거리 사용해 중요도 계산
                float priority = (1 - priorityWeight) * currentHP + priorityWeight / distance;

                if (target == null)
                {
                    if (priority >= firstPriority)
                    {
                        firstPriority = priority;
                        target = tree.gameObject;
                    }
                    else if ((priority < firstPriority) && (priority >= secondPriority))
                    {
                        secondPriority = priority;
                        alternative = tree.gameObject;
                    }
                    else
                    {

                    }
                }
                else
                {
                    if (priority >= secondPriority)
                    {
                        secondPriority = priority;
                        alternative = tree.gameObject;
                    }
                    else
                    {

                    }
                }
            }
        }

        targetDistance = CalculateDistance(target);
        alternativeDistance = CalculateDistance(alternative);
        sensor.AddObservation(targetDistance / 180f); //최대 거리는 180쯤
        sensor.AddObservation(alternativeDistance /180f); //최대 거리는 180쯤
    }

    public float CalculateDistance(GameObject opponent)
    {
        Vector3 offset = opponent.transform.position - tf.position;
        float dx = offset.x;
        float dz = offset.z;
        float distance = Mathf.Sqrt(dx * dx + dz * dz);
        return distance;
    }

    public (string tagName, TreeManager tree) GetTagAndTree()
    {
        // 바닥을 센싱하여 만약 sensor 위에 있다면, 해당 나무 오브젝트의 TreeManager script와 함께 반환
        string tagName = "None";
        TreeManager tree = null;
        Vector3 origin = tf.position + Vector3.up * 0.5f;
        Vector3 direction = Vector3.down;
        float rayDistance = 2.0f;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            tagName = hit.collider.tag;
            if (tagName == "sensor")
            {
                GameObject parentTree = hit.collider.transform.parent.gameObject;
                tree = parentTree.GetComponent<TreeManager>();
            }
        }
        return (tagName, tree);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var continuousAction = actions.ContinuousActions;

        robotCtrl.acc = continuousAction[0];
        robotCtrl.turn = continuousAction[1];

        targetDistanceAfterAction = targetDistance;

        // Harvest action 도중 움직이면 negative reward
        if (isHarvesting)
        {
            float penalty = Mathf.Abs(robotCtrl.acc) + Mathf.Abs(robotCtrl.turn);
            AddReward(-penalty);
        }

        // 타겟 나무에 접근 시 positive reward
        if (target != peerScript.target)
        {
            targetDistanceAfterAction = CalculateDistance(target);
            if (targetDistanceAfterAction < targetDistanceBeforeAction)
            {
                AddReward(+0.2f);
            }
        }

        int discreteAction = actions.DiscreteActions[0];
        if (discreteAction == 1 && !isHarvesting)
        {
            StartCoroutine(HarvestRoutine());
            isHarvesting = false;
        }

        AddReward(env.CalculateGlobalReward());
        targetDistanceBeforeAction = targetDistanceAfterAction;
    }

    IEnumerator HarvestRoutine()
    {
        isHarvesting = true;
        var (tag, tree) = GetTagAndTree();
        Debug.Log(agent_id + "-> received tag " + tag);
        if (tree != null)
        {
            Debug.Log(agent_id + "-> harvesting " + tree.name);
        }

        if (tag != "sensor") //엉뚱한 곳에서 수확 시도 시 negative reward
        {
            AddReward(-1.4f);
        }
        else // 센서 범위 내에서 수확 시도 -> 해당 나무의 정보를 리턴 받은 상태
        {
            if (tree.IsHarvestable()) // 수확하려는 나무가 수확 가능한 상태! 수확하고 pos reward 부여
            {
                tree.Harvest();

                // 열매 익고 난지 얼마 안되었을수록 큰 보상 부여
                float StepsAfterRipen = tree.fruitStepCounter;
                if (StepsAfterRipen != 0)
                {
                    AddReward(4.0f + 1 / StepsAfterRipen);
                }
                else
                {
                    AddReward(4.0f);
                }

                env.HarvestCounter();
                if (episode.episodeCounter <= 10)
                {
                    episode.EndAllEpisodes();
                    episode.episodeCounter++;
                }
                else if ((episode.episodeCounter > 10) && (episode.episodeCounter <= 100))
                {
                    if (env.harvestCounter >= 7)
                    {
                        episode.EndAllEpisodes();
                        episode.episodeCounter++;
                    }
                }
                else
                {
                    if (env.harvestCounter >= 20)
                    {
                        episode.EndAllEpisodes();
                        episode.episodeCounter++;
                    }
                }
            }
            else // 수확하려는 나무가 수확 불가한 상태. Negative reward 부여
            {
                AddReward(-0.7f);
            }
        }

        animator.SetTrigger("HarvestTrigger");
        yield return new WaitForSeconds(2.44f);
    }

    //애니메이션 이벤트 요구 함수
    public void AnimEventShoot()
    {
    }
    public void AnimEventReloadStarted()
    {
    }
    public void AnimEventReloadFinished()
    {
    }

    public void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == peerAgent.name) //동료 에이전트와 충돌
        {
            Debug.Log(agent_id + "-> Collided with " + col.gameObject.name);
            AddReward(-1.0f);
            episode.EndAllEpisodes();
        }
        if (col.collider.CompareTag("home"))
        {
            Debug.Log(agent_id + "-> Collided with Home");
            AddReward(-0.5f);
        }
        if (col.collider.CompareTag("wall"))
        {
            Debug.Log(agent_id + "-> Collided with Wall");
            AddReward(-0.5f);
        }
        if (col.collider.CompareTag("tree"))
        {
            Debug.Log(agent_id + "-> Collided with Tree");
            AddReward(-0.3f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 테스트용 수동 입력
        var continuous = actionsOut.ContinuousActions;

        float drive = Input.GetAxis("Vertical"); //z
        float steer = Input.GetAxis("Horizontal"); //x

        continuous[0] = drive;
        continuous[1] = steer;

        var discrete = actionsOut.DiscreteActions;
        discrete[0] = Input.GetKey(KeyCode.H) ? 1 : 0;
        if (discrete[0] == 1)
        {
            StartCoroutine(HarvestRoutine());
        }
    }    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
