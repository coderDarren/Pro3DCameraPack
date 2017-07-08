using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraTypeSetting : MonoBehaviour {

    public GameObject CameraSettingsManager;

    Dropdown dropDown;
    CameraSwitch cam_switcher;

    void Start()
    {
        dropDown = GetComponent<Dropdown>();
        cam_switcher = CameraSettingsManager.GetComponent<CameraSwitch>();
        cam_switcher.Init();
        dropDown.value = PlayerPrefs.GetInt(CameraSettings.Instance.CAMERA_TYPE);
        SetCameraType();
    }

    public void SetCameraType()
    {
        cam_switcher.SwitchToCamera(dropDown.value);
        PlayerPrefs.SetInt(CameraSettings.Instance.CAMERA_TYPE, dropDown.value);
    }
}
