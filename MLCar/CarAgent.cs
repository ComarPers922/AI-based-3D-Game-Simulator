using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAgent : Agent
{
    private Driver CarDriver;
    private RayPerception Ray;
    [SerializeField]
    private GameObject StartPoint;
    [SerializeField]
    private GameObject TargetPoint;
    [SerializeField]
    private GameObject[] CheckPoints;

    private float StartDistance;
    // private float FencePenalty = 0;
    private float CumulativeCheckPointReward = 0.05f;

    private Vector3 LastCheckPointTransformPosition;
    private Quaternion LastCheckPointTransformRotation;

    public override void AgentReset()
    {
        base.AgentReset();
        chances = 5;
        CarDriver.Brake();
        CarDriver.GetComponent<Rigidbody>().velocity = Vector3.zero;
        CarDriver.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = StartPoint.transform.position;
        transform.rotation = StartPoint.transform.rotation;
        CarDriver.Brake();
        StartDistance = Vector3.Distance(transform.position,
                                        TargetPoint.transform.position);
        CumulativeCheckPointReward = 0.05f;
        foreach (var item in CheckPoints)
        {
            item.SetActive(true);
        }
        // FencePenalty = 0;
    }
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        CarDriver = GetComponent<Driver>();
        Ray = GetComponent<RayPerception>();
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        var forward = vectorAction[0];
        var rotation = vectorAction[1];
        switch (forward)
        {
            case 1:
                CarDriver.Accelerate(1);
                break;
            case 2:
                CarDriver.Accelerate(-1);
                break;
            default:
                CarDriver.Accelerate(0);
                CarDriver.Brake();
                break;
        }
        switch(rotation)
        {
            case 1:
                CarDriver.Steer(-1);
                break;
            case 2:
                CarDriver.Steer(1);
                break;
            default:
                CarDriver.Steer(0);
                break;
        }
        AddReward(-(1f / agentParameters.maxStep)*(Vector3.Distance(transform.position,
                                        TargetPoint.transform.position) / 
                                        StartDistance));
        var angle = Mathf.Abs(Vector3.Dot(transform.forward,
            transform.InverseTransformVector(TargetPoint.transform.position)));
        //if(angle <= 30)
        //{
        //    AddReward(0.03f);
        //}
        //else
        //{
        //    AddReward(-0.03f);
        //}
    }

    public override void CollectObservations()
    {
        var rayDistance = 5f;
        float[] rayAngles = {/* 0f, 22.5F,*/ /*135f, 45f,*/ 112.5f,   90f, 67.5f, -90f};
        var detectableObjects = new[] {"fence", "destination", "checkpoint", "badpoint"};
        AddVectorObs(Ray.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        AddVectorObs(Ray.Perceive(rayDistance, rayAngles, detectableObjects, .23f, 0f));
        AddVectorObs(Vector3.Distance(transform.position,
                                        TargetPoint.transform.position));
        AddVectorObs(CarDriver.GetComponent<Rigidbody>().velocity);
        AddVectorObs(CarDriver.GetComponent<Rigidbody>().angularVelocity);
        //AddVectorObs(Mathf.Abs(Vector3.Dot(transform.forward,
        //    transform.InverseTransformVector(TargetPoint.transform.position))));
        // AddVectorObs(transform.forward);
        // AddVectorObs(transform.position);
        // AddVectorObs(TargetPoint.transform.position);
        // AddVectorObs(transform.InverseTransformVector(TargetPoint.transform.position));
        // AddVectorObs(TargetPoint.transform.position);
    }
    
    public void GetDestination()
    {
        // AddReward(5);
        AddReward(CumulativeCheckPointReward * 2);
        Done();
    }

    public void GetCheckPoint(GameObject sender)
    {
        // AddReward(2);
        AddReward(CumulativeCheckPointReward);
        CumulativeCheckPointReward += 0.05f;
        sender.SetActive(false);
        StartDistance = Vector3.Distance(transform.position,
                                TargetPoint.transform.position);
        
        LastCheckPointTransformPosition = sender.transform.position + Vector3.up * 2;
        LastCheckPointTransformRotation = sender.transform.rotation;
    }
    private int chances = 5;
    public void GetBadPoint()
    {
        AddReward(-CumulativeCheckPointReward);
        CumulativeCheckPointReward = 0.05f;
        if (chances-- <= 0)
        {
            Done();
        }
        CarDriver.GetComponent<Rigidbody>().velocity = Vector3.zero;
        CarDriver.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = LastCheckPointTransformPosition;
        transform.rotation = LastCheckPointTransformRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.transform.parent?.tag == "fence")
        {
            AddReward(-CumulativeCheckPointReward);
            CumulativeCheckPointReward = 0.05f;
            if (chances-- <= 0)
            {
                Done();
            }
            CarDriver.GetComponent<Rigidbody>().velocity = Vector3.zero;
            CarDriver.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = LastCheckPointTransformPosition;
            transform.rotation = LastCheckPointTransformRotation;
        }
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.collider.transform.parent?.tag == "fence")
    //    {
    //        AddReward(-0.002f);
    //        FencePenalty += 0.002f;
    //    }
    //    if(FencePenalty >= 0.8f)
    //    {
    //        AddReward(-1);
    //        Done();
    //    }
    //}
}
