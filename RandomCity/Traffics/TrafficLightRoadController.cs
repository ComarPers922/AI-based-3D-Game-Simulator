using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightRoadController : MonoBehaviour {
    [SerializeField]
    private float LightTime = 10;
    [SerializeField]
    private float YellowLightTime = 5;

    [SerializeField]
    private TrafficLight NorthernLight;
    [SerializeField]
    private TrafficLight SouthernLight;
    [SerializeField]
    private TrafficLight WesternLight;
    [SerializeField]
    private TrafficLight EasternLight;

    private bool AreNSGreen = false;
    void Start ()
    {
        AreNSGreen = Random.Range(0, 2) == 1;
        UpdateLights();
	}
    private void UpdateLights()
    {
        NorthernLight?.ChangeStatus(AreNSGreen ? TrafficLight.LightStatus.Green : TrafficLight.LightStatus.Red);
        SouthernLight?.ChangeStatus(AreNSGreen ? TrafficLight.LightStatus.Green : TrafficLight.LightStatus.Red);
        WesternLight?.ChangeStatus(!AreNSGreen ? TrafficLight.LightStatus.Green : TrafficLight.LightStatus.Red);
        EasternLight?.ChangeStatus(!AreNSGreen ? TrafficLight.LightStatus.Green : TrafficLight.LightStatus.Red);
        Invoke(nameof(UpdateYellowLights), LightTime);
    }
    private void UpdateYellowLights()
    {
        if(AreNSGreen)
        {
            NorthernLight?.ChangeStatus(TrafficLight.LightStatus.Yellow);
            SouthernLight?.ChangeStatus(TrafficLight.LightStatus.Yellow);
        }
        else
        {
            WesternLight?.ChangeStatus(TrafficLight.LightStatus.Yellow);
            EasternLight?.ChangeStatus(TrafficLight.LightStatus.Yellow);
        }
        Invoke(nameof(UpdateRedLights), YellowLightTime);
    }
    private void UpdateRedLights()
    {
        if (AreNSGreen)
        {
            NorthernLight?.ChangeStatus(TrafficLight.LightStatus.Red);
            SouthernLight?.ChangeStatus(TrafficLight.LightStatus.Red);
        }
        else
        {
            WesternLight?.ChangeStatus(TrafficLight.LightStatus.Red);
            EasternLight?.ChangeStatus(TrafficLight.LightStatus.Red);
        }
        AreNSGreen = !AreNSGreen;
        Invoke(nameof(UpdateLights), 2);
    }
    public TrafficLight.LightStatus CheckLightStatus(bool isNS)
    {
        return isNS ? NorthernLight.GetLightStatus() : EasternLight.GetLightStatus();
    }
}
