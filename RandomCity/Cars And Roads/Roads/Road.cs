using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Road : MonoBehaviour, IRoadMark
{
    public virtual void RequestForNextRoadMarks(Car car, bool isOnTheRight)
    {
        throw new System.NotImplementedException();
    }
    
    public virtual void RequestForNextRoadMarksAtEndPoint(Car car, bool isOnTheRight)
    {
        throw new System.NotImplementedException();
    }
}
