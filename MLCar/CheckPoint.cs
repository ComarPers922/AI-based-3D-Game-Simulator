using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        other.transform.parent.GetComponent<CarAgent>()?.GetCheckPoint(this.gameObject);
    }
}
