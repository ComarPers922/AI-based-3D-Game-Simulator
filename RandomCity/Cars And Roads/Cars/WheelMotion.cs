using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelMotion : MonoBehaviour
{
    [SerializeField]
    private WheelCollider Collider;
    private Vector3 WheelPosition;
    private Quaternion WheelRotation;
    void FixedUpdate()
    {
        Collider.GetWorldPose(out WheelPosition, out WheelRotation);
        transform.position = WheelPosition;
        transform.rotation = WheelRotation;
    }
}
