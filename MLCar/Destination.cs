using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        other.transform.parent.GetComponent<CarAgent>()?.GetDestination();
    }
}
