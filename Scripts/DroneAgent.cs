using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;

using static DroneController;

public class DroneAgent : Agent
{
    public Vector3 initPos;
    public Quaternion initRot;
    public string agent_id;

    public BehaviorParameters bps;
    public Rigidbody rb;
    public Transform tf;

    public DroneController droneCtrl;
    public GameObject target;
    public GameObject alternative;
    public GameObject peerAgent;

    public override void Initialize()
    {
        agent_id = this.gameObject.name;
        bps = GetComponent<BehaviorParameters>();
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        droneCtrl = GetComponent<DroneController>();
        if (agent_id == "DroneAgent1") {
            peerAgent = GameObject.Find("DroneAgent2");
        }
        else {
            peerAgent = GameObject.Find("DroneAgent1");
        }
        initPos = tf.localPosition;
        initRot = tf.rotation;
    }

    public override void OnEpisodeBegin()
    {
        tf.localPosition = initPos + new Vector3(0f, 6.85f, 0f);
        tf.rotation = initRot;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        droneCtrl.m = 0;
        droneCtrl.s = 0;
        droneCtrl.a = 0;
        droneCtrl.y = 0;

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

        droneCtrl.m = action[0];
        droneCtrl.s = action[1];
        droneCtrl.a = action[2];
        droneCtrl.y = action[3];

    }

    public void OnCollisionEnter(Collision other)
    {
        Debug.Log(agent_id + "-> Collided with " + other.gameObject.name);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // 테스트용 수동 입력
        var action = actionsOut.ContinuousActions;

        float forward = Input.GetAxis("Vertical"); //z
        float strafe = Input.GetAxis("Horizontal"); //x
        float altitude = Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftShift) ? -1f : 0f; // y
        float yaw = Input.GetKey(KeyCode.Q) ? -1f : Input.GetKey(KeyCode.E) ? 1f : 0f; // yaw

        action[0] = forward;
        action[1] = strafe;
        action[2] = altitude;
        action[3] = yaw;
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
