using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TopDownCameraSettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public Slider distanceField;
    public Toggle allowZoomField;
    public Slider zoomSmoothField, zoomStepField;
    public Slider maxZoomField, minZoomField;
    public Slider smoothFollowField;
    public Toggle useSmoothFollowField;

    public Slider xRotationField, yOrbitSmoothField;
    public Toggle allowOrbitField;
    public InputField orbitInputField;

    public Toggle fadeObstructionsField, highlightTargetField;

    TopDownCamera camera;
    ObstructionHandler obstructionHandler;

    public void InitializeSettings()
    {
        camera = Camera.main.GetComponent<TopDownCamera>();
        obstructionHandler = camera.target.GetComponentInChildren<ObstructionHandler>();

        distanceField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_DISTANCE);
        int allowZoom = PlayerPrefs.GetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ZOOM);
        if (allowZoom == 1)
            allowZoomField.isOn = true;
        else
            allowZoomField.isOn = false;
        zoomSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_ZOOM_SMOOTH);
        zoomStepField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_ZOOM_STEP);
        maxZoomField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_MAX_ZOOM);
        minZoomField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_MIN_ZOOM);
        smoothFollowField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_FOLLOW_SMOOTH);
        int useSmooth = PlayerPrefs.GetInt(CameraSettings.Instance.TOPDOWN_USE_SMOOTH_FOLLOW);
        if (useSmooth == 0)
            useSmoothFollowField.isOn = false;
        else
            useSmoothFollowField.isOn = true;

        xRotationField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_X_ROTATION);
        yOrbitSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.TOPDOWN_Y_ORBIT_SMOOTH);
        int allowOrbit = PlayerPrefs.GetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ORBIT);
        if (allowOrbit == 1)
            allowOrbitField.isOn = true;
        else
            allowOrbitField.isOn = false;
        orbitInputField.text = PlayerPrefs.GetString(CameraSettings.Instance.TOPDOWN_ORBIT_INPUT);

        int fadeObstructions = PlayerPrefs.GetInt(CameraSettings.Instance.TOPDOWN_FADE_OBSTRUCTIONS);
        if (fadeObstructions == 1)
            fadeObstructionsField.isOn = true;
        else
            fadeObstructionsField.isOn = false;
        int highlightTarget = PlayerPrefs.GetInt(CameraSettings.Instance.TOPDOWN_HIGHLIGHT_TARGET);
        if (highlightTarget == 1)
            highlightTargetField.isOn = true;
        else
            highlightTargetField.isOn = false;
    }


    public void SetDistance()
    {
        camera.position.distanceFromTarget = distanceField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_DISTANCE, distanceField.value);
    }

    public void SetAllowZoom()
    {
        camera.position.allowZoom = allowZoomField.isOn;
        if (camera.position.allowZoom)
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ZOOM, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ZOOM, 0);
    }

    public void SetZoomSmooth()
    {
        camera.position.zoomSmooth = zoomSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_ZOOM_SMOOTH, zoomSmoothField.value);
    }

    public void SetZoomStep()
    {
        camera.position.zoomStep = zoomStepField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_ZOOM_STEP, zoomStepField.value);
    }

    public void SetMaxZoom()
    {
        camera.position.maxZoom = maxZoomField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_MAX_ZOOM, maxZoomField.value);
    }

    public void SetMinZoom()
    {
        camera.position.minZoom = minZoomField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_MIN_ZOOM, minZoomField.value);
    }

    public void SetFollowSmooth()
    {
        camera.position.smooth = smoothFollowField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_FOLLOW_SMOOTH, smoothFollowField.value);
    }

    public void SetSmoothFollow()
    {
        camera.position.smoothFollow = useSmoothFollowField.isOn;
        if (camera.position.smoothFollow)
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_USE_SMOOTH_FOLLOW, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_USE_SMOOTH_FOLLOW, 0);
    }

    public void SetXRotation()
    {
        camera.orbit.xRotation = xRotationField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_X_ROTATION, xRotationField.value);
    }

    public void SetYOrbitSmooth()
    {
        camera.orbit.yOrbitSmooth = yOrbitSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_Y_ORBIT_SMOOTH, yOrbitSmoothField.value);
    }

    public void SetAllowOrbit()
    {
        camera.orbit.allowOrbit = allowOrbitField.isOn;
        if (camera.orbit.allowOrbit)
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ORBIT, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ORBIT, 0);
    }

    public void SetOrbitInput()
    {
        camera.input.MOUSE_ORBIT = orbitInputField.text;
        PlayerPrefs.SetString(CameraSettings.Instance.TOPDOWN_ORBIT_INPUT, orbitInputField.text);
    }

    public void SetFadeObstructions()
    {
        if (!obstructionHandler)
        {
            Debug.Log("Your target is missing the ObstructionHandler component.");
            return;
        }
        obstructionHandler.obstructionSetting.active = fadeObstructionsField.isOn;
        if (obstructionHandler.obstructionSetting.active)
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_FADE_OBSTRUCTIONS, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_FADE_OBSTRUCTIONS, 0);
    }

    public void SetHighlightTarget()
    {
        if (!obstructionHandler)
        {
            Debug.Log("Your target is missing the ObstructionHandler component.");
            return;
        }
        obstructionHandler.obstructionSetting.changeTargetColor = highlightTargetField.isOn;
        if (obstructionHandler.obstructionSetting.changeTargetColor)
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_HIGHLIGHT_TARGET, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_HIGHLIGHT_TARGET, 0);
    }



    public void Reset()
    {
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_DISTANCE, -50);
        PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ZOOM, 1);
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_ZOOM_SMOOTH, 2);
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_ZOOM_STEP, 10);
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_MAX_ZOOM, -30);
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_MIN_ZOOM, -60);
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_FOLLOW_SMOOTH, 0.05f);
        PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_USE_SMOOTH_FOLLOW, 1);
        PlayerPrefs.SetFloat(CameraSettings.Instance.TOPDOWN_Y_ORBIT_SMOOTH, 10);
        PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_ALLOW_ORBIT, 1);
        PlayerPrefs.SetString(CameraSettings.Instance.TOPDOWN_ORBIT_INPUT, "Fire1");
        PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_FADE_OBSTRUCTIONS, 0);
        PlayerPrefs.SetInt(CameraSettings.Instance.TOPDOWN_HIGHLIGHT_TARGET, 0);
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
