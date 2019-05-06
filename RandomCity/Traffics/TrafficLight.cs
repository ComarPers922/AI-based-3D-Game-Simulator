using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public enum LightStatus
    {
        Green, Red, Yellow
    }
    [SerializeField]
    private MeshRenderer GreenLight;
    [SerializeField]
    private MeshRenderer RedLight;
    [SerializeField]
    private MeshRenderer YellowLight;
    private TrafficLightMaterial Materials;

    [SerializeField]
    private LightStatus CurrentStatus;
    private void Awake()
    {
        Materials = GameObject.Find("Materials").GetComponent<TrafficLightMaterial>();
    }
    public LightStatus GetLightStatus()
    {
        return CurrentStatus;
    }
    public void ChangeStatus(LightStatus newStatus)
    {
        CurrentStatus = newStatus;
        switch (CurrentStatus)
        {
            case LightStatus.Green:
                GreenLight.material = Materials.LitGreenLightMaterial;
                RedLight.material = Materials.UnlitRedLightMaterial;
                YellowLight.material = Materials.UnlitYellowLightMaterial;
                break;
            case LightStatus.Red:
                GreenLight.material = Materials.UnlitGreenLightMaterial;
                RedLight.material = Materials.LitRedLightMaterial;
                YellowLight.material = Materials.UnlitYellowLightMaterial;
                break;
            case LightStatus.Yellow:
                GreenLight.material = Materials.UnlitGreenLightMaterial;
                RedLight.material = Materials.UnlitRedLightMaterial;
                YellowLight.material = Materials.LitYellowLightMaterial;
                break;
            default:
                break;
        }
    }
}
