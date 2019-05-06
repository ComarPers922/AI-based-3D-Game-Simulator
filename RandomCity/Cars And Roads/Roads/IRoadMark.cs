using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoadMark
{
    void RequestForNextRoadMarks(Car car, bool isOnTheRight);
    void RequestForNextRoadMarksAtEndPoint(Car car, bool isOnTheRight);
}
