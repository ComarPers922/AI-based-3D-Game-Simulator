using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirportPoint : MonoBehaviour
{
    [SerializeField]
    private float WaitTime;
    [SerializeField]
    private AirportPoint NextPointForLanding;
    [SerializeField]
    private AirportPoint NextPointForTakingOff;
    [SerializeField]
    private float PercentageOfSpeed = 1;
    [SerializeField]
    private bool LandingPoint = false;
    [SerializeField]
    private bool BridgePoint = false;
    [SerializeField]
    private bool TakingOffPoint = false;

    public bool IsLandingPoint
    {
        get
        {
            return LandingPoint;
        }
    }
    private Airport Airport;
    private void Start()
    {
        Airport = transform.root.GetComponent<Airport>();
    }
    private void OnTriggerEnter(Collider other)
    {
        var airplane = other.transform.root.GetComponent<AirplaneController>();
        if(!airplane.TargetAirport.Equals(Airport))
        {
            return;
        }
        if (airplane != null)
        {
            if (BridgePoint)
            {
                Airport.WillTakeOff = true;
            }
            if (TakingOffPoint && Airport.WillTakeOff)
            {
                Airport.WillTakeOff = false;
                Airport.IsOccupiedForAirplane = false;
                airplane.TakeOff();
            }
            if(Airport.WillTakeOff && NextPointForTakingOff != null)
            {
                airplane.SetNextPoint(NextPointForTakingOff, WaitTime, PercentageOfSpeed);
            }
            else if (!Airport.WillTakeOff && NextPointForLanding != null)
            {
                airplane.SetNextPoint(NextPointForLanding, WaitTime, PercentageOfSpeed);
            }
        }
    }
}
