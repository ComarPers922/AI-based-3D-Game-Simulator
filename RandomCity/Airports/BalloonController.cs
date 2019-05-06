using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    [SerializeField]
    private float Speed = 10f;
    [SerializeField]
    private float FlyingTime = 20f;
    private BalloonPoint LandingPoint;
    private WorldGrid WorldCenter;
    private Airport TargetAirport;

    private enum BalloonStatus
    {
        Flying, Approaching, Landing
    }

    private BalloonStatus CurrentStatus = BalloonStatus.Flying;

    private Vector3 Direction = Vector3.zero;
	// Use this for initialization
	void Start ()
    {
        WorldCenter = GameObject.FindGameObjectWithTag("WorldCenter").GetComponent<WorldGrid>();
        Direction = Vector3.right * Random.Range(-1f, 1f) + Vector3.forward * Random.Range(-1f, 1f);
        Invoke(nameof(RequestToLand), FlyingTime);
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (CurrentStatus)
        {
            case BalloonStatus.Flying:
                transform.position += Direction.normalized * Speed * Time.deltaTime;
                transform.position = Vector3.up * Mathf.Clamp(transform.position.y + 20 * Time.deltaTime, 0, 120)
                        + new Vector3(transform.position.x, 0, transform.position.z);
                if (Vector3.Distance(transform.position, WorldCenter.transform.position) >= 10000)
                {
                    Direction = -Direction;
                }
                break;
            case BalloonStatus.Approaching:
                transform.position = Vector3.MoveTowards(transform.position,
                    LandingPoint.gameObject.transform.position + Vector3.up * 100,
                    Speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, LandingPoint.gameObject.transform.position) <= 110)
                {
                    CurrentStatus = BalloonStatus.Landing;
                }
                break;
            case BalloonStatus.Landing:
                transform.position = Vector3.MoveTowards(transform.position,
                    LandingPoint.gameObject.transform.position,
                    Speed * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    public void SetLandingPoint(BalloonPoint landingPoint)
    {
        LandingPoint = landingPoint;
        CurrentStatus = BalloonStatus.Approaching;
    }
    public void TakeOff()
    {
        Invoke(nameof(_TakeOff), 5);
    }
    private void _TakeOff()
    {
        CurrentStatus = BalloonStatus.Flying;
        Direction = Vector3.right * Random.Range(-1f, 1f) + Vector3.forward * Random.Range(-1f, 1f);
        Invoke(nameof(RequestToLand), FlyingTime);
        TargetAirport.IsOccupiedForBalloon = false;
        TargetAirport = null;
    }
    private void RequestToLand()
    {
        TargetAirport = WorldCenter.Airports[Random.Range(0, WorldCenter.Airports.Length)];
        if (!TargetAirport.RequestToLandForBalloon(this))
        {
            Invoke(nameof(RequestToLand), 10f);
            TargetAirport = null;
        }
        else
        {
            CurrentStatus = BalloonStatus.Approaching;
        }
    }
}
