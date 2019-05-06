using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airport : MonoBehaviour
{
    [SerializeField]
    private AirportPoint LandingPointForAirplane;
    [SerializeField]
    private BalloonPoint LandingPointForBalloon;
    [SerializeField]
    private AirportPoint BridgePoint;

    public bool IsOccupiedForAirplane { set; get; } = false;

    public bool IsOccupiedForBalloon { set; get; } = false;

    public bool WillTakeOff { set; get; } = false;

    public bool RequestToLandForAirplane(AirplaneController airplane)
    {
        if(IsOccupiedForAirplane)
        {
            return false;
        }
        airplane.SetNextPoint(LandingPointForAirplane, 0, 1);
        IsOccupiedForAirplane = true;
        return true;
    }

    public bool RequestToLandForBalloon(BalloonController balloon)
    {
        if(IsOccupiedForBalloon)
        {
            return false;
        }
        balloon.SetLandingPoint(LandingPointForBalloon);
        IsOccupiedForBalloon = true;
        return true;
    }
}
