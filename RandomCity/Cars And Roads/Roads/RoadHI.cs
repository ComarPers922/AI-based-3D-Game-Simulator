using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHI : Road
{ 
    private List<GameObject> RightRoadMarks;
    // private List<GameObject> LeftRoadMark;

    void Start()
    {
        RightRoadMarks = new List<GameObject>();
        // LeftRoadMark = new List<GameObject>();

        var rightMark = transform.Find("StartPoint").gameObject;
        RightRoadMarks.Add(rightMark);
        foreach (Transform item in rightMark.transform)
        {
            RightRoadMarks.Add(item.gameObject);
        }
    }

    void Update()
    {
        
    }

    public override void RequestForNextRoadMarks(Car car, bool isOnTheRight)
    {
        //car.EnqueueRoadMarks(RightRoadMarks.GetRange(1, RightRoadMarks.Count - 1).ToArray());
    }

    public override void RequestForNextRoadMarksAtEndPoint(Car car, bool isOnTheRight)
    {
        // 唯一能够改变车辆方向的地方
    }
}
