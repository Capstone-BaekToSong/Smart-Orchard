using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public Rigidbody rb;

    public Transform[] propellers;
    private float moveForce = 40f;

    private float ascendForce = 40f;
    private float yawTorque = 20f;
    private float propellerSpinSpeed = 1500f;

    public float m, s, a, y; //외부로부터의 move, strafe, altitude, yaw 에 대한 input 값

    // Start is called before the first frame update
    void Start()
    {
    }

    void FixedUpdate()
    {
        MoveForwardBack();
        StrafeLeftRight();
        AdjustAltitude();
        RotateYaw();

        foreach (var prop in propellers)
        {
            prop.Rotate(Vector3.up, propellerSpinSpeed * Time.fixedDeltaTime);
        }
    }


    public void MoveForwardBack()
    {
        rb.AddForce(transform.forward * m * moveForce);
    }
    public void StrafeLeftRight()
    {
        rb.AddForce(transform.right * s * moveForce);
    }
    public void AdjustAltitude()
    {
        rb.AddForce(Vector3.up * a * ascendForce);
    }
    public void RotateYaw()
    {
        rb.AddTorque(Vector3.up * y * yawTorque);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
