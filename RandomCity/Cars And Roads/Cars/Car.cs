using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private static readonly float[] Angles = new float[] { 0f/*, -10f, 10f, -20f, 20f,  -30f, 30f*/ };
    private static readonly float[] Offsets = new float[] { 0f, /*-.25f, .25f,*/ -.1f, .1f };
    public static readonly float TinyDistance = 1f;
    private static readonly float WaitTime = 1f;

    [SerializeField]
    private GameObject NextPoint;

    //[SerializeField]
    //private float RotationSpeed = 100;
    //[SerializeField, Range(0, 5)]
    //private float Speed = 3;
    [SerializeField]
    private LayerMask CarMask;
    [SerializeField]
    private LayerMask StartPointMask;
    private float RandomSpeedUp = 10;
    // private Queue<RoadPoint> PointQueue = new Queue<RoadPoint>();

    private TrafficLightRoadController TrafficLightController = null;
    private bool IsNS = false;
    private int RoadNumber;
    private bool IsRestricted = false;

    [SerializeField]
    private WheelCollider WheelColliderFL;
    [SerializeField]
    private WheelCollider WheelColliderFR;
    [SerializeField]
    private WheelCollider WheelColliderRL;
    [SerializeField]
    private WheelCollider WheelColliderRR;
    [SerializeField, Range(0f,200f)]
    private float MotorTorque = 20;
    private Rigidbody Rigidbody;
    private float Angle = 35;

    private bool IsWaiting = false;

    private void Wait()
    {
        IsWaiting = false;
    }

    // private Rigidbody rigidbody;
    private float GetVector2Distance(Vector3 vec1, Vector3 vec2)
    {
        return Vector2.Distance(new Vector2(vec1.x, vec1.z),
            new Vector2(vec2.x, vec2.z));
    }
    void Start()
    {
        // RandomSpeedUp = Random.Range(0.5f, 1.5f);
        Rigidbody = GetComponent<Rigidbody>();
        Rigidbody.centerOfMass = new Vector3(0, -0.3f, 0);
    }
    private float CalculateSpeedPerSecond()
    {
        return ((WheelColliderRR.rpm * WheelColliderRR.radius * 2 * Mathf.PI +
                    WheelColliderRL.rpm * WheelColliderRL.radius * 2 * Mathf.PI) / 2 / 60);
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (NextPoint != null)
    //    {
    //        return;
    //    }
    //    var RoadPoint = other.gameObject.GetComponent<RoadPoint>();
    //    if (RoadPoint != null)
    //    {
    //        NextPoint = RoadPoint.RequestForRoadPoints(out RoadNumber, out TrafficLightController,
    //            out IsNS, out IsRestricted)?.gameObject;
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.name.Contains("EndPoint") && NextPoint.name.Contains("EndPoint"))
        //{
        //    NextPoint = null;
        //}
        var RoadPoint = other.gameObject.GetComponent<RoadPoint>();
        if (RoadPoint != null)
        {
            NextPoint = RoadPoint.RequestForRoadPoints(this, out RoadNumber, out TrafficLightController,
                out IsNS, out IsRestricted)?.gameObject;
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(collision.collider.gameObject.name);
    //    if (collision.collider.gameObject.name.Contains("RoadMark") && PointQueue.Count != 0)
    //    {
    //        NextPoint = PointQueue.Dequeue().gameObject;
    //    }
    //}

    // Update is called once per frame
    void FixedUpdate()
    {
        //foreach (var item in Offsets)
        //{
        //    Debug.DrawRay(transform.position + transform.right * item,
        //        Quaternion.Euler(0, item, 0) * transform.forward * 5,
        //        Color.black);
        //}
        //if (NextPoint != null &&
        //    GetVector2Distance(transform.position, NextPoint.transform.position) < TinyDistance &&
        //    PointQueue.Count != 0)
        //{
        //    NextPoint = PointQueue.Dequeue().gameObject;
        //}
        if (Vector3.Dot(transform.up, Vector3.down) > -0.7f)
        {
            transform.rotation = Quaternion.Euler(0,0,0);
        }
        bool isBraked = false;
        if (NextPoint != null)
        {
            // Debug.Log(CheckMovable());
            if (CheckMovable())
            {
                /*transform.position = Vector3.MoveTowards(transform.position,
                    NextPoint.gameObject.transform.position,
                    (Speed + RandomSpeedUp) * Time.deltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    NextPoint.transform.rotation,
                    RotationSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, 0.40f, transform.position.z);*/
                Vector3 vec = transform.InverseTransformPoint(NextPoint.transform.position);
                float steer = (vec.x / vec.magnitude);
                ReleaseBrake(true);
                // Debug.Log(steer);
                WheelColliderFL.steerAngle = steer * Angle;
                WheelColliderFR.steerAngle = steer * Angle;
                WheelColliderRL.motorTorque = MotorTorque * Mathf.Max(1 - Mathf.Abs(steer) , 0.1f);
                WheelColliderRR.motorTorque = MotorTorque * Mathf.Max(1 - Mathf.Abs(steer) , 0.1f);
                // WheelColliderRL.brakeTorque = MotorTorque * 0.3f;
                // WheelColliderRR.brakeTorque = MotorTorque * 0.3f;
                //if(Mathf.Abs(steer) >= 0.7f)
                //{
                //    Brake(false);
                //}
                //else
                //{
                //    ReleaseBrake(false);
                //}
            }
            else
            {
                isBraked = true;
                Brake(true);
            }
        }
        if(NextPoint == null)
        {
            isBraked = true;
            Brake(true);
        }
        //if (!isBraked && NextPoint.tag == "StartPoint")
        //{
        //    var ray = new Ray(transform.position, transform.forward);
        //    RaycastHit hit;
        //    Physics.Raycast(ray, out hit, 3f, StartPointMask);
        //    if (hit.collider != null)
        //    {
        //        if (!hit.collider.GetComponent<RoadPoint>().IsTrafficLightRoad
        //        && Rigidbody.velocity.magnitude >= 20) //WheelColliderRL.rpm >= 100)
        //        {
        //            PartiallyBrake();
        //        }
        //        else
        //        {
        //            ReleaseBrake(true);
        //        }
        //    }
        //}
    }
    private void PartiallyBrake()
    {
        WheelColliderRL.brakeTorque = WheelColliderRL.motorTorque * 2;
        WheelColliderRR.brakeTorque = WheelColliderRR.motorTorque * 2;
    }
    private void Brake(bool FullBrake)
    {
        if (FullBrake)
        {
            //if (Rigidbody.velocity.magnitude > 10)
            //{
            //    WheelColliderRL.motorTorque = -Mathf.Infinity;
            //    WheelColliderRR.motorTorque = -Mathf.Infinity;
            //}
            //else
            //{
            //    WheelColliderRL.motorTorque = 0;
            //    WheelColliderRR.motorTorque = 0;
            //}
            Rigidbody.isKinematic = true;
            WheelColliderRL.motorTorque = 0;
            WheelColliderRR.motorTorque = 0;
            WheelColliderRL.brakeTorque = Mathf.Infinity;
            WheelColliderRR.brakeTorque = Mathf.Infinity;
            return;
        }
        WheelColliderRL.motorTorque = Mathf.Max(0, WheelColliderRL.motorTorque - 10);
        WheelColliderRR.motorTorque = Mathf.Max(0, WheelColliderRR.motorTorque - 10);
        WheelColliderRL.brakeTorque = WheelColliderRL.motorTorque * 20f;
        WheelColliderRR.brakeTorque = WheelColliderRR.motorTorque * 20f;
    }
    private void ReleaseBrake(bool FullRelease)
    {
        if(FullRelease)
        {
            Rigidbody.isKinematic = false;
            WheelColliderRL.motorTorque = 0;
            WheelColliderRR.motorTorque = 0;
            WheelColliderRL.brakeTorque = 0;
            WheelColliderRR.brakeTorque = 0;
            return;
        }
        WheelColliderRL.brakeTorque = Mathf.Max(0, WheelColliderRL.brakeTorque - 1);
        WheelColliderRR.brakeTorque = Mathf.Max(0, WheelColliderRL.brakeTorque - 1);
    }
    private bool CheckGreenLight()
    {
        if (!IsRestricted)
        {
            if (TrafficLightController != null && RoadNumber != 0 &&
                TrafficLightController?.CheckLightStatus(IsNS) != TrafficLight.LightStatus.Green)
            {
                return false;
            }
        }
        else
        {
            if (TrafficLightController != null &&
                TrafficLightController?.CheckLightStatus(IsNS) != TrafficLight.LightStatus.Green)
            {
                return false;
            }
        }
        return true;
    }
    private bool CheckMovable()
    {
        if(IsWaiting)
        {
            return false;
        }
        //if(!CheckGreenLight())
        //{
        //    return false;
        //}
        TrafficLightController = null;
        int count = 0;
        foreach (var angleItem in Angles)
        {
            foreach (var offetItem in Offsets)
            {
                var ray = new Ray(transform.position + transform.right * offetItem,
                    Quaternion.Euler(0, angleItem, 0) * transform.forward);
                if (Physics.Raycast(ray,
                    //CalculateSpeedPerSecond() * 1.5f
                    1f
                    , CarMask))
                {
                    count++;
                }
                if (count >= 1)
                {
                    IsWaiting = true;
                    Invoke(nameof(Wait), WaitTime);
                    return false;
                }
            }
        }
        return true;
    }

    //public void SetNextPoint(GameObject sender, GameObject nextPoint)
    //{
    //    if (NextPoint != null && Vector3.Distance(sender.transform.position, 
    //        NextPoint.transform.position) >= TinyDistance)
    //    {
    //        return;
    //    }
    //    NextPoint = nextPoint.gameObject;
    //}

    public void SetNextPoint(GameObject nextPoint)
    {
        NextPoint = nextPoint;
    }
    public GameObject GetNextPoint()
    {
        return NextPoint;
    }
}
