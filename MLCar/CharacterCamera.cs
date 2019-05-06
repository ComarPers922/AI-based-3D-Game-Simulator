using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCamera : MonoBehaviour
{
    private const float MinDistance = 0;
    [SerializeField]
    private float RotationSpeed = 50.0f;
    [SerializeField]
    private GameObject Target;
    [SerializeField, Range(MinDistance, float.MaxValue)]
    private float Distance = 100;
    [SerializeField]
    private float Height = 3.0f;
    [SerializeField]
    private float MouseScrollSensitivity = 50;
    // private Vector3 offset;
	// Use this for initialization
	void Start ()
    {
        //offset = (transform.position - Target.transform.position).normalized * Distance;
        //offset = Target.transform.position + (Target.transform.forward * Distance * -1);
    }
	
	// Update is called once per frame
	void Update ()
    {
        var targetPosition = (Target.transform.position + 
            (Target.transform.forward * Distance * -1)
            + Vector3.up * 0.5f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 3 * Time.deltaTime);
        //var scale = Input.GetAxis("Mouse ScrollWheel") * - MouseScrollSensitivity;
        //if(Mathf.Abs(scale) > 0)
        //{
        //    Distance += scale * MouseScrollSensitivity;
        //    Distance = Mathf.Max(MinDistance, Distance);
        //    offset = (transform.position - Target.transform.position).normalized * Distance;
        //}
        //if (Input.GetMouseButton(1))
        //{
        //    var mousePositionDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        //    transform.RotateAround(Target.transform.position,
        //        Vector3.up,
        //        mousePositionDelta.x * RotationSpeed * Time.deltaTime);
        //    transform.RotateAround(Target.transform.position,
        //        -transform.right,
        //        mousePositionDelta.y * RotationSpeed * Time.deltaTime);
        //    offset = (transform.position - Target.transform.position).normalized * Distance;
        //}
        var lookDirection = (Target.transform.position) - transform.position;
        var lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 2f * Time.deltaTime);
    }
}
