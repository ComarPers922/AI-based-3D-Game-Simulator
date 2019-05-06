using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float Speed = 5;
    [SerializeField]
    private float Acceleration = 5;
    [SerializeField]
    private float MouseScrollSensitivity = 200;
    [SerializeField]
    private float RotationSpeed = 0.1f;
    [SerializeField]
    private float Low = 1.5f;
    [SerializeField]
    private float High = 200f;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        float forwardValue = Input.GetAxisRaw("Vertical");
        float leftValue = Input.GetAxisRaw("Horizontal");
        forwardValue += Input.GetAxis("Mouse ScrollWheel") * MouseScrollSensitivity;
        if(transform.position.y < Low)
        {
            transform.position = new Vector3(transform.position.x, Low, transform.position.z);
            forwardValue = 0;
        }
        else if (transform.position.y > High)
        {
            transform.position = new Vector3(transform.position.x, High, transform.position.z);
            forwardValue = 0;
        }

        float totalSpeed = Speed + (Input.GetKey(KeyCode.LeftShift) ? Acceleration : 0);
        transform.Translate(leftValue * totalSpeed * Time.deltaTime, 
            0, 
            forwardValue * totalSpeed * Time.deltaTime);
        if (Input.GetMouseButton(1))
        {
            var mousePositionDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);//Input.mousePosition - PreviousMousePosition;
            transform.Rotate(-mousePositionDelta.y * RotationSpeed * Time.deltaTime,
                0,
                0);
            transform.RotateAround(transform.position,
                Vector3.up,
                mousePositionDelta.x * RotationSpeed * Time.deltaTime);
        }
    }
}
