using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    [SerializeField]
    private float Speed = 50;
    [SerializeField]
    private float RotationSpeed = 100;
    [SerializeField]
    public float FlyingTime = 10;
    public AirportPoint NextPoint;
    private bool IsReadyToGo = true;
    private bool IsLanding = false;
    private float percentageOfSpeed = 1;
    private Quaternion TakingOffRotation;
    public Airport TargetAirport { set; get; }

    private WorldGrid WorldCenter;

    // Use this for initialization
    void Start ()
    {
        Invoke(nameof(RequestToLand), FlyingTime);
        TakingOffRotation = transform.rotation;
        WorldCenter = GameObject.FindGameObjectWithTag("WorldCenter").GetComponent<WorldGrid>();
    }
	// Update is called once per frame
	void Update ()
    {
		if(IsReadyToGo && IsLanding && NextPoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, NextPoint.gameObject.transform.position, Speed * percentageOfSpeed * Time.deltaTime);
            if(NextPoint.IsLandingPoint)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(NextPoint.gameObject.transform.position - transform.position) ,
                    RotationSpeed * percentageOfSpeed * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, NextPoint.gameObject.transform.rotation, RotationSpeed * percentageOfSpeed * Time.deltaTime);
            }
        }
        if(!IsLanding)
        {
            transform.position += transform.rotation * Vector3.forward * Speed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                            TakingOffRotation,
                            RotationSpeed * percentageOfSpeed * Time.deltaTime);

            transform.position = Vector3.up * Mathf.Clamp(transform.position.y + 20 * Time.deltaTime, 0, 250)
                + new Vector3(transform.position.x, 0, transform.position.z); //Take off.
            if(Vector3.Distance(transform.position, WorldCenter.transform.position) >= 10000)
            {
                TakingOffRotation = Quaternion.LookRotation(WorldCenter.transform.position - transform.position);
            }
        }
	}
    private void RequestToLand()
    {
        TargetAirport = WorldCenter.Airports[Random.Range(0, WorldCenter.Airports.Length)];
        if (!TargetAirport.RequestToLandForAirplane(this))
        {
            Invoke(nameof(RequestToLand), 10f);
            TargetAirport = null;
        }
        else
        {
            IsLanding = true;
        }
    }
    public void SetNextPoint(AirportPoint nextPoint, float WaitTime, float PercentageOfSpeed)
    {
        NextPoint = nextPoint;
        IsReadyToGo = false;
        percentageOfSpeed = Mathf.Clamp(PercentageOfSpeed, 0, 1);
        Invoke(nameof(GetReady), WaitTime);
    }

    private void GetReady()
    {
        IsReadyToGo = true;
    }

    public void TakeOff()
    {
        IsLanding = false;
        TakingOffRotation = Quaternion.Euler(0, Random.Range(0,360), 0);
        NextPoint = null;
        TargetAirport = null;
        Invoke(nameof(RequestToLand), FlyingTime);
    }
}
