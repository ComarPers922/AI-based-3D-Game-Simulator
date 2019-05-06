using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindGenerator : MonoBehaviour
{

    private void Start()
    {
        transform.root.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    void Update ()
    {
        transform.Rotate(new Vector3(0,0,50) * Time.deltaTime);
	}
}
