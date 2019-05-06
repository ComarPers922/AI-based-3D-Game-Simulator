using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Driver : MonoBehaviour
{

    private Rigidbody Rigidbody;

    [SerializeField]
    private float Torque = 1000f;
    [SerializeField, Range(0, 50f)]
    private float Angle = 30f;

    [SerializeField]
    private WheelCollider WheelFL;
    [SerializeField]
    private WheelCollider WheelFR;
    [SerializeField]
    private WheelCollider WheelRL;
    [SerializeField]
    private WheelCollider WheelRR;

    [SerializeField]
    private GameObject WheelFLRenderer;
    [SerializeField]
    private GameObject WheelFRRenderer;
    [SerializeField]
    private GameObject WheelRLRenderer;
    [SerializeField]
    private GameObject WheelRRRenderer;
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = Vector3.down * 0.3f;
    }

    void FixedUpdate()
    {
        //float forward = Input.GetAxisRaw("Vertical");
        //float rotation = Input.GetAxisRaw("Horizontal");

        //Accelerate(forward);
        //Steer(rotation);
        UpdateWheelPos();
    }

    private void UpdateWheelPos()
    {
        Vector3 pos;
        Quaternion rotation;

        WheelFL.GetWorldPose(out pos, out rotation);
        WheelFLRenderer.transform.position = pos + Vector3.up * 0.001f;
        WheelFLRenderer.transform.rotation = rotation;

        WheelFR.GetWorldPose(out pos, out rotation);
        WheelFRRenderer.transform.position = pos + Vector3.up * 0.001f;
        WheelFRRenderer.transform.rotation = rotation;

        WheelRL.GetWorldPose(out pos, out rotation);
        WheelRLRenderer.transform.position = pos + Vector3.up * 0.001f;
        WheelRLRenderer.transform.rotation = rotation;

        WheelRR.GetWorldPose(out pos, out rotation);
        WheelRRRenderer.transform.position = pos + Vector3.up * 0.001f;
        WheelRRRenderer.transform.rotation = rotation;
    }

    public void Accelerate(float rate)
    {
        WheelRL.brakeTorque = 0;
        WheelRR.brakeTorque = 0;
        rate = Mathf.Clamp(rate, -1.0f, 1.0f);
        WheelRL.motorTorque = Torque * rate;
        WheelRR.motorTorque = Torque * rate;
    }

    public void Steer(float rate)
    {
        WheelRL.brakeTorque = 0;
        WheelRR.brakeTorque = 0;
        rate = Mathf.Clamp(rate, -1.0f, 1.0f);
        WheelFL.steerAngle = Angle * rate;
        WheelFR.steerAngle = Angle * rate;
    }

    public void Brake()
    {
        WheelRL.brakeTorque = Mathf.Infinity;
        WheelRR.brakeTorque = Mathf.Infinity;
        WheelFL.steerAngle = 0;
        WheelFR.steerAngle = 0;
    }
}
