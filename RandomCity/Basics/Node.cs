using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node
{
    public bool IsObstacle { set; get; }
    public Vector3 WorldPosition { set; get; }
    public int X { set; get; }
    public int Y { set; get; }

    public int GCost { set; get; }
    public int HCost { set; get; }
    public int UCost { set; get; }
    public int FCost { get { return HCost + GCost + UCost; } }

    public Node From { set; get; }

    public Node(bool isObstacle, Vector3 worldPos, int x, int y)
    {
        IsObstacle = isObstacle;
        WorldPosition = worldPos;
        X = x;
        Y = y;
    }
    public int CompareTo(Node nodeToCompare)
    {
        int compare = FCost.CompareTo(nodeToCompare.FCost);
        if (compare == 0)
        {
            compare = HCost.CompareTo(nodeToCompare.HCost);
        }
        return -compare;
    }
}