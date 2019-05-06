using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadL : Road
{
    private List<GameObject> RightRoadMarks;
    private List<GameObject> LeftRoadMarks;

    void Start()
    {
        RightRoadMarks = new List<GameObject>();
        LeftRoadMarks = new List<GameObject>();

        var rightMark = transform.Find("StartPointR").gameObject;
        RightRoadMarks.Add(rightMark);
        foreach (Transform item in rightMark.transform)
        {
            RightRoadMarks.Add(item.gameObject);
        }

        var leftMark = transform.Find("StartPointL").gameObject;
        LeftRoadMarks.Add(leftMark);
        foreach (Transform item in leftMark.transform)
        {
            LeftRoadMarks.Add(item.gameObject);
        }
    }

    public override void RequestForNextRoadMarks(Car car, bool isOnTheRight)
    {
        /*if (isOnTheRight)
        {
            car.EnqueueRoadMarks(RightRoadMarks.GetRange(1, RightRoadMarks.Count - 1).ToArray());
        }
        else
        {
            car.EnqueueRoadMarks(LeftRoadMarks.GetRange(1, LeftRoadMarks.Count - 1).ToArray());
        }*/
    }

    public override void RequestForNextRoadMarksAtEndPoint(Car car, bool isOnTheRight)
    {

    }
}
