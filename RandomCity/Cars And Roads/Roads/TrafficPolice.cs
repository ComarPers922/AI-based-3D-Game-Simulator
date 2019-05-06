using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficPolice : MonoBehaviour
{
    [SerializeField]
    private RoadPoint StartPointN;
    [SerializeField]
    private RoadPoint StartPointE;

    private static readonly float WaitTime = 4;

    public enum Direction
    {
        Straight, Left, Right
    }

    public static Direction GetDirection(Transform from, Transform target)
    {
        var offset = from.InverseTransformPoint(target.transform.position).normalized.x;
        if(offset >= -0.2f && offset <= 0.2f)
        {
            return Direction.Straight;
        }
        else if(offset < -0.2)
        {
            return Direction.Left;
        }
        return Direction.Right;
    }

    private void Start()
    {
        Invoke(nameof(AssignCarPoint), WaitTime);
    }

    private void AssignCarPoint()
    {
        float timeToWait = WaitTime;
        if (queueWE.Count != 0 && ((!queueWE.Peek().IsRestrictedByRedLight && (queueNS.Count == 0
            || GetDirection(queueNS.Peek().Car.transform, queueNS.Peek().NextPoint.transform) != Direction.Straight)) || 
            StartPointE.CheckGreenLight()))
        {
            var carInfo = queueWE.Dequeue();
            carInfo.Car.SetNextPoint(carInfo.NextPoint.gameObject);
            if (queueWE.Count != 0 && GetDirection(carInfo.Car.transform, carInfo.NextPoint.transform) != Direction.Left)
            {
                var top = queueWE.Peek();
                var direction = GetDirection(top.Car.transform, top.NextPoint.transform);
                if (direction != Direction.Left)
                {
                    timeToWait = 0;
                }
            }
            Invoke(nameof(AssignCarPoint), timeToWait);
            return;
        }
        if (queueNS.Count != 0 && ((!queueNS.Peek().IsRestrictedByRedLight && (queueWE.Count == 0
            || GetDirection(queueWE.Peek().Car.transform, queueWE.Peek().NextPoint.transform) != Direction.Straight)) ||
            StartPointN.CheckGreenLight()))
        {
            var carInfo = queueNS.Dequeue();
            carInfo.Car.SetNextPoint(carInfo.NextPoint.gameObject);
            if (queueNS.Count != 0 && GetDirection(carInfo.Car.transform, carInfo.NextPoint.transform) != Direction.Left)
            {
                var top = queueNS.Peek();
                var direction = GetDirection(top.Car.transform, top.NextPoint.transform);
                if (direction != Direction.Left)
                {
                    timeToWait = 0;
                }
            }
        }
        Invoke(nameof(AssignCarPoint), timeToWait);
    }

    private struct CarInfo
    {
        public Car Car;
        public RoadPoint NextPoint;
        public bool IsRestrictedByRedLight;

        public CarInfo(Car car, RoadPoint nextPoint, bool isRestrictedByRedLight)
        {
            Car = car;
            NextPoint = nextPoint;
            IsRestrictedByRedLight = isRestrictedByRedLight;
        }
    }
    private Queue<CarInfo> queueNS = new Queue<CarInfo>();
    private Queue<CarInfo> queueWE = new Queue<CarInfo>();

    public void Enqueue(Car car, RoadPoint nextPoint, bool isRestrictedByRedLight, bool isNS)
    {
        if(isNS)
        {
            queueNS.Enqueue(new CarInfo(car, nextPoint, isRestrictedByRedLight));
            return;
        }
        queueWE.Enqueue(new CarInfo(car, nextPoint, isRestrictedByRedLight));
    }
}
