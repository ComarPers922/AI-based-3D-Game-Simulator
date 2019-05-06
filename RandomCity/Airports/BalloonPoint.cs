using UnityEngine;

public class BalloonPoint: MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Balloon")
        {
            other.gameObject.GetComponent<BalloonController>().TakeOff();
        }
    }
}