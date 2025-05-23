using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Wheel Transforms (for visual rotation)")]
    public Transform leftWheel;
    public Transform rightWheel;

    private float motorForce = 500f;
    private float turnTorque = 300f;
    private float wheelSpinSpeed = 1000f;

    [Header("Input Values")] // [-1, 1] 범위
    public float acc;
    public float turn;

    void Start()
    {
        // 무게중심을 아래로 내리기 위해 Center of Mass 설정 (X,Z는 그대로, Y는 -0.5로 내림)
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
    }

    void FixedUpdate()
    {
        Drive();
        Steer();

        // 시각적 회전 연출 (바퀴 Mesh만 회전)
        if (leftWheel)  leftWheel.Rotate(Vector3.right,  wheelSpinSpeed * acc * Time.deltaTime);
        if (rightWheel) rightWheel.Rotate(Vector3.left,  wheelSpinSpeed * acc * Time.deltaTime);
    }

    public void Drive()
    {
        // 직진/후진 힘 적용
        if (acc >= 0) // 후진보다 전진 움직임을 지향하도록 유도
        {
            rb.AddForce(transform.forward * acc * motorForce);
        }
        else
        {
            rb.AddForce(transform.forward * acc * motorForce / 3);
        }
    }

    public void Steer()
    {
        // 회전 토크 적용 (Yaw 방향)
        rb.AddTorque(Vector3.up * turn * turnTorque);
    }

    void Update()
    {
    }


}
