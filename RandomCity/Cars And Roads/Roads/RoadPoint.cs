using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 使用队列式，空间换时间
public class RoadPoint : MonoBehaviour
{
    // 规定：0号标记不受信号灯管制，除非受限制属性为真
    [SerializeField, Tooltip("The 0th road point is not affected by traffic lights!")]
    private RoadPoint[] NextRoadPoints;
    [SerializeField]
    private TrafficLightRoadController TrafficLightController = null;
    // 只有当NextRoadPoints的长度大于0时，此选项有效
    [SerializeField, Tooltip("This is effective if and only if the length of NextRoadPoints is more than 0")]
    private bool IsNS = true;
    [SerializeField]
    private bool IsRestricted = false;
    [SerializeField]
    private LayerMask StartPointMask;
    // private int RoadNumber = -1;
    private List<RoadPoint>[] RoadLists;
    public bool IsTrafficLightRoad = false;
    private TrafficPolice Police;

    private void Start()
    {
        foreach (var item in GetComponents<Collider>())
        {
            item.isTrigger = true;
        }
        if(name.Contains("EndPoint"))
        {
            var ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 5f, StartPointMask);
            if (hit.collider != null)
            {
                NextRoadPoints[0] = hit.collider.GetComponent<RoadPoint>();
            }
        }
        if(IsTrafficLightRoad)
        {
            Police = transform.parent.GetComponent<TrafficPolice>();
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    // Debug.Log(TrafficLightController?.CheckLightStatus(IsNS));
    //    if (other.gameObject.tag == "Car" && NextRoadPoints.Length != 0 && 
    //        TrafficLightController?.CheckLightStatus(IsNS) == TrafficLight.LightStatus.Green &&
    //        (RoadNumber != 0 || IsRestricted))
    //    {
    //        var car = other.transform.root.GetComponent<Car>();
    //        if (Vector3.Distance(car.GetNextPoint().transform.position, transform.position) >= Car.TinyDistance)
    //        {
    //            return;
    //        }
    //        car?.SetNextPoint(this.gameObject,
    //            NextRoadPoints[RoadNumber].gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
    }

    /*public RoadPoint[] RequestForRoadPoints()
    {
        if (!name.Contains("StartPoint"))
        {
            return null;
        }
        if (RoadNumber == -1)
        {
            RoadNumber = Random.Range(0, RoadLists.Length);
        }
        if (RoadNumber == 0)
        {
            RoadNumber = -1;
            return RoadLists[0].ToArray();
        }
        if (TrafficLightController?.CheckLightStatus(IsNS) == TrafficLight.LightStatus.Green)
        {
            var returnValue = RoadNumber;
            RoadNumber = -1;
            return RoadLists[returnValue].ToArray();
        }
        return null;
    }*/
    public RoadPoint RequestForRoadPoints(Car sender,out int roadNumber
        , out TrafficLightRoadController controller, out bool isNS, out bool isRestricted)
    {
        isNS = default;
        controller = default;
        isRestricted = default;
        roadNumber = Random.Range(0, NextRoadPoints.Length);

        if(NextRoadPoints[roadNumber] == null)
        {
            return null;
        }
        if (IsTrafficLightRoad)
        {
            Police.Enqueue(sender, NextRoadPoints[roadNumber], IsRestricted || roadNumber != 0
                ,IsNS);
            return null;
        }
        controller = /*NextRoadPoints[roadNumber].*/TrafficLightController;
        isNS = /*NextRoadPoints[roadNumber].*/IsNS;
        isRestricted = /*NextRoadPoints[roadNumber].*/IsRestricted;
        return NextRoadPoints[roadNumber];
    }

    public bool CheckGreenLight()
    {
        if(TrafficLightController == null)
        {
            return true;
        }
        return TrafficLightController.CheckLightStatus(IsNS) == TrafficLight.LightStatus.Green;
    }
}
