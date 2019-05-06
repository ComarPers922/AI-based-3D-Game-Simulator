using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XTest : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;
    void Update()
    {
        Debug.Log(transform.InverseTransformPoint(Target.transform.position).normalized.x);
    }
}
