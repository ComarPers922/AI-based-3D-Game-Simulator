using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField]
    private Camera MainCamera;
    [SerializeField]
    private Camera MapCamera;

    private static bool IsMain = true;


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.M))
        {
            MainCamera.gameObject.SetActive(IsMain = !IsMain);
            MapCamera.gameObject.SetActive(!IsMain);
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 100, 50), "Camera Switch"))
    //    {
    //        MainCamera.gameObject.SetActive(IsMain = !IsMain);
    //        MapCamera.gameObject.SetActive(!IsMain);
    //    }
    //}
}
