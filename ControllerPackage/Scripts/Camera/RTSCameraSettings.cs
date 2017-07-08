using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RTSCameraSettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Toggle invertPanField;
    public Slider panSmoothField;
    public InputField panInputField;
    public Slider distanceField;
    public Toggle allowZoomField;
    public Slider zoomSmoothField, zoomStepField;
    public Slider maxZoomField, minZoomField;

    public Slider xRotationField, yOrbitSmoothField;
    public Toggle allowOrbitField;
    public InputField orbitInputField;

    RTSCamera camera;

    public void InitializeSettings()
    {
        camera = Camera.main.GetComponent<RTSCamera>();

        int invertPan = PlayerPrefs.GetInt(CameraSettings.Instance.RTS_INVERT_PAN);
        if (invertPan == 1)
            invertPanField.isOn = true;
        else
            invertPanField.isOn = false;
        panSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_PAN_SMOOTH);
        panInputField.text = PlayerPrefs.GetString(CameraSettings.Instance.RTS_PAN_INPUT);
        distanceField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_DISTANCE);
        int allowZoom = PlayerPrefs.GetInt(CameraSettings.Instance.RTS_ALLOW_ZOOM);
        if (allowZoom == 1)
            allowZoomField.isOn = true;
        else
            allowZoomField.isOn = false;
        zoomSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_ZOOM_SMOOTH);
        zoomStepField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_ZOOM_STEP);
        maxZoomField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_MAX_ZOOM);
        minZoomField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_MIN_ZOOM);

        xRotationField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_X_ROTATION);
        yOrbitSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.RTS_Y_ORBIT_SMOOTH);
        int allowOrbit = PlayerPrefs.GetInt(CameraSettings.Instance.RTS_ALLOW_ORBIT);
        if (allowOrbit == 1)
            allowOrbitField.isOn = true;
        else
            allowOrbitField.isOn = false;
        orbitInputField.text = PlayerPrefs.GetString(CameraSettings.Instance.RTS_ORBIT_INPUT);

    }

    public void SetInverPan()
    {
        camera.position.invertPan = invertPanField.isOn;
        if (camera.position.invertPan)
            PlayerPrefs.SetInt(CameraSettings.Instance.RTS_INVERT_PAN, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.RTS_INVERT_PAN, 0);
    }

    public void SetPanSmooth()
    {
        camera.position.panSmooth = panSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_PAN_SMOOTH, panSmoothField.value);
    }

    public void SetPanInput()
    {
        camera.input.PAN = panInputField.text;
        PlayerPrefs.SetString(CameraSettings.Instance.RTS_PAN_INPUT, panInputField.text);
    }

    public void SetDistance()
    {
        camera.position.distanceFromGround = distanceField.value;
        camera.position.newDistance = distanceField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_DISTANCE, distanceField.value);
    }

    public void SetAllowZoom()
    {
        camera.position.allowZoom = allowZoomField.isOn;
        if (camera.position.allowZoom)
            PlayerPrefs.SetInt(CameraSettings.Instance.RTS_ALLOW_ZOOM, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.RTS_ALLOW_ZOOM, 0);
    }

    public void SetZoomSmooth()
    {
        camera.position.zoomSmooth = zoomSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_ZOOM_SMOOTH, zoomSmoothField.value);
    }

    public void SetZoomStep()
    {
        camera.position.zoomStep = zoomStepField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_ZOOM_STEP, zoomStepField.value);
    }

    public void SetMaxZoom()
    {
        camera.position.maxZoom = maxZoomField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_MAX_ZOOM, maxZoomField.value);
    }

    public void SetMinZoom()
    {
        camera.position.minZoom = minZoomField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_MIN_ZOOM, minZoomField.value);
    }

    public void SetXRotation()
    {
        camera.orbit.xRotation = xRotationField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_X_ROTATION, xRotationField.value);
    }

    public void SetYOrbitSmooth()
    {
        camera.orbit.yOrbitSmooth = yOrbitSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_Y_ORBIT_SMOOTH, yOrbitSmoothField.value);
    }

    public void SetAllowOrbit()
    {
        camera.orbit.allowYOrbit = allowOrbitField.isOn;
        if (camera.orbit.allowYOrbit)
            PlayerPrefs.SetInt(CameraSettings.Instance.RTS_ALLOW_ORBIT, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.RTS_ALLOW_ORBIT, 0);
    }

    public void SetOrbitInput()
    {
        camera.input.ORBIT_Y = orbitInputField.text;
        PlayerPrefs.SetString(CameraSettings.Instance.RTS_ORBIT_INPUT, orbitInputField.text);
    }


    public void Reset()
    {
        PlayerPrefs.SetInt(CameraSettings.Instance.RTS_INVERT_PAN, 1);
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_PAN_SMOOTH, 75);
        PlayerPrefs.SetString(CameraSettings.Instance.RTS_PAN_INPUT, "Fire1");
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_DISTANCE, 40);
        PlayerPrefs.SetInt(CameraSettings.Instance.RTS_ALLOW_ZOOM, 1);
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_ZOOM_SMOOTH, 2);
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_ZOOM_STEP, 10);
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_MAX_ZOOM, 25);
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_MIN_ZOOM, 80);
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_X_ROTATION, 50);
        PlayerPrefs.SetFloat(CameraSettings.Instance.RTS_Y_ORBIT_SMOOTH, 100);
        PlayerPrefs.SetInt(CameraSettings.Instance.RTS_ALLOW_ORBIT, 1);
        PlayerPrefs.SetString(CameraSettings.Instance.RTS_ORBIT_INPUT, "Fire2");
        InitializeSettings();
    }


    public delegate void EditingDelegate(bool editing);
    public static event EditingDelegate IsEditing;

    public void OnPointerEnter(PointerEventData ped)
    {
        IsEditing(true);
    }

    public void OnPointerExit(PointerEventData ped)
    {
        IsEditing(false);
    }
}
