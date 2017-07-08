using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {

    StandardCamera standard;
    TopDownCamera topDown;
    RTSCamera rts;

    public StandardCameraSettings standardSettings;
    public TopDownCameraSettings topdownSettings;
    public RTSCameraSettings rtsSettings;

    public void Init()
    {
        Profiler.enabled = false;
        standard = Camera.main.GetComponent<StandardCamera>();
        topDown = Camera.main.GetComponent<TopDownCamera>();
        rts = Camera.main.GetComponent<RTSCamera>();

        standardSettings.InitializeSettings();
        topdownSettings.InitializeSettings();
        rtsSettings.InitializeSettings();

        SwitchToCamera(PlayerPrefs.GetInt(CameraSettings.Instance.CAMERA_TYPE));
    }

    void SwitchToStandard()
    {
        standard.enabled = true;
        standardSettings.gameObject.SetActive(true);
        topDown.enabled = false;
        topdownSettings.gameObject.SetActive(false);
        rts.enabled = false;
        rtsSettings.gameObject.SetActive(false);
        standardSettings.InitializeSettings();
    }

    void SwitchToTopDown()
    {
        standard.enabled = false;
        standardSettings.gameObject.SetActive(false);
        topDown.enabled = true;
        topdownSettings.gameObject.SetActive(true);
        rts.enabled = false;
        rtsSettings.gameObject.SetActive(false);
        topdownSettings.InitializeSettings();
    }

    void SwitchToRTS()
    {
        standard.enabled = false;
        standardSettings.gameObject.SetActive(false);
        topDown.enabled = false;
        topdownSettings.gameObject.SetActive(false);
        rts.enabled = true;
        rtsSettings.gameObject.SetActive(true);
        rtsSettings.InitializeSettings();

        try
        {
            standard.target.GetComponentInChildren<ObstructionHandler>().obstructionSetting.active = false;
            standard.target.GetComponentInChildren<ObstructionHandler>().obstructionSetting.changeTargetColor = false;
        }
        catch (System.Exception) { }
        try
        {
            topDown.target.GetComponentInChildren<ObstructionHandler>().obstructionSetting.active = false;
            topDown.target.GetComponentInChildren<ObstructionHandler>().obstructionSetting.changeTargetColor = false;
        }
        catch (System.Exception) { }
    }

    public void SwitchToCamera(int cam_type)
    {
        switch(cam_type)
        {
            case 0: SwitchToStandard(); break;
            case 1: SwitchToTopDown(); break;
            case 2: SwitchToRTS(); break;
        }
    }
}
