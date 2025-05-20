using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

using static RobotController;
using static TreeManager;

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
    public Animator animator;
    private bool isHarvesting = false;

    public override void Initialize()
    {
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

        // Environment 초기화 진행
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(tf.rotation.y);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(rb.angularVelocity);

        //if (target != null)
        //{
        //    sensor.AddObservation(target.localPosition - tr.localPosition);
        //}
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.ContinuousActions;

        robotCtrl.acc = action[0];
        robotCtrl.turn = action[1];
        // Harvest action 도중 움직이면 negative reward
        if (isHarvesting)
        {
            float penalty = Mathf.Abs(robotCtrl.acc) + Mathf.Abs(robotCtrl.turn);
            AddReward(-penalty);
        }

        int discreteAction = actions.DiscreteActions[0];
        if (discreteAction == 1 && !isHarvesting)
        {
            StartCoroutine(HarvestRoutine());
        }

    }

    IEnumerator HarvestRoutine()
    {
        isHarvesting = true;
        animator.SetTrigger("HarvestTrigger");
        Debug.Log("Hi Noy");
        yield return new WaitForSeconds(2.44f);

        //if (targetTree != null && targetTree.IsHarvestable())
        //{
        //    targetTree.Harvest();
        //    AddReward(+1.0f);
        //}
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

    public void OnCollisionEnter(Collision other)
    {
        Debug.Log(agent_id + "-> Collided with " + other.gameObject.name);
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
