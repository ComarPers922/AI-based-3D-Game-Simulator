using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadI : Road
{
    private GameObject RightRoadMark;
    private GameObject LeftRoadMark;

    void Start()
    {
        RightRoadMark = transform.Find(nameof(RightRoadMark)).gameObject;
        LeftRoadMark = transform.Find(nameof(LeftRoadMark)).gameObject;
    }

    void Update()
    {
        
    }

    public override void RequestForNextRoadMarks(Car car, bool isOnTheRight)
    {
        
    }

    public override void RequestForNextRoadMarksAtEndPoint(Car car, bool isOnTheRight)
    {
        
    }
}
