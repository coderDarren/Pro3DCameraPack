using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StandardCameraSettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Slider xOffsetField, yOffsetField, zOffsetField;
    public Slider distanceField;
    public Slider zoomSmoothField, zoomStepField;
    public Slider maxZoomField, minZoomField;
    public Slider smoothFollowField;
    public Toggle useSmoothFollowField;

    public Slider maxXOrbitField, minXOrbitField;
    public Slider xOrbitSmoothField, yOrbitSmoothField;
    public Toggle allowOrbitField, rotateWithTargetField;
    public InputField orbitInputField;

    public Toggle useCollisionField, fadeObstructionsField, highlightTargetField;

    StandardCamera camera;
    ObstructionHandler obstructionHandler;

    public void InitializeSettings()
    {
        camera = Camera.main.GetComponent<StandardCamera>();
        obstructionHandler = camera.target.GetComponentInChildren<ObstructionHandler>();

        xOffsetField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_X);
        yOffsetField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_Y);
        zOffsetField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_Z);
        distanceField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_DISTANCE);
        zoomSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_ZOOM_SMOOTH);
        zoomStepField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_ZOOM_STEP);
        maxZoomField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_MAX_ZOOM);
        minZoomField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_MIN_ZOOM);
        smoothFollowField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_FOLLOW_SMOOTH);
        int useSmooth = PlayerPrefs.GetInt(CameraSettings.Instance.STANDARD_USE_SMOOTH_FOLLOW);
        if (useSmooth == 0)
            useSmoothFollowField.isOn = false;
        else
            useSmoothFollowField.isOn = true;

        maxXOrbitField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_MAX_X_ORBIT);
        minXOrbitField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_MIN_X_ORBIT);
        xOrbitSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_X_ORBIT_SMOOTH);
        yOrbitSmoothField.value = PlayerPrefs.GetFloat(CameraSettings.Instance.STANDARD_Y_ORBIT_SMOOTH);
        int allowOrbit = PlayerPrefs.GetInt(CameraSettings.Instance.STANDARD_ALLOW_ORBIT);
        if (allowOrbit == 1)
            allowOrbitField.isOn = true;
        else
            allowOrbitField.isOn = false;
        int rotateWithTarget = PlayerPrefs.GetInt(CameraSettings.Instance.STANDARD_ROTATE_WITH_TARGET);
        if (rotateWithTarget == 1)
            rotateWithTargetField.isOn = true;
        else
            rotateWithTargetField.isOn = false;
        orbitInputField.text = PlayerPrefs.GetString(CameraSettings.Instance.STANDARD_ORBIT_INPUT);

        int useCollision = PlayerPrefs.GetInt(CameraSettings.Instance.STANDARD_USE_COLLISION);
        if (useCollision == 1)
            useCollisionField.isOn = true;
        else
            useCollisionField.isOn = false;
        int fadeObstructions = PlayerPrefs.GetInt(CameraSettings.Instance.STANDARD_FADE_OBSTRUCTIONS);
        if (fadeObstructions == 1)
            fadeObstructionsField.isOn = true;
        else
            fadeObstructionsField.isOn = false;
        int highlightTarget = PlayerPrefs.GetInt(CameraSettings.Instance.STANDARD_HIGHLIGHT_TARGET);
        if (highlightTarget == 1)
            highlightTargetField.isOn = true;
        else
            highlightTargetField.isOn = false;
    }

    public void SetOffsetX()
    {
        camera.position.targetPosOffset.x = xOffsetField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_X, xOffsetField.value);
    }

    public void SetOffsetY()
    {
        camera.position.targetPosOffset.y = yOffsetField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_Y, yOffsetField.value);
    }

    public void SetOffsetZ()
    {
        camera.position.targetPosOffset.z = zOffsetField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_Z, zOffsetField.value);
    }

    public void SetDistance()
    {
        camera.position.distanceFromTarget = distanceField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_DISTANCE, distanceField.value);
    }

    public void SetZoomSmooth()
    {
        camera.position.zoomSmooth = zoomSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_ZOOM_SMOOTH, zoomSmoothField.value);
    }

    public void SetZoomStep()
    {
        camera.position.zoomStep = zoomStepField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_ZOOM_STEP, zoomStepField.value);
    }

    public void SetMaxZoom()
    {
        camera.position.maxZoom = maxZoomField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MAX_ZOOM, maxZoomField.value);
    }

    public void SetMinZoom()
    {
        camera.position.minZoom = minZoomField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MIN_ZOOM, minZoomField.value);
    }

    public void SetFollowSmooth()
    {
        camera.position.smooth = smoothFollowField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_FOLLOW_SMOOTH, smoothFollowField.value);
    }

    public void SetSmoothFollow()
    {
        camera.position.smoothFollow = useSmoothFollowField.isOn;
        if (camera.position.smoothFollow)
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_USE_SMOOTH_FOLLOW, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_USE_SMOOTH_FOLLOW, 0);
    }

    public void SetMaxXOrbit()
    {
        camera.orbit.maxXRotation = maxXOrbitField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MAX_X_ORBIT, maxXOrbitField.value);
    }

    public void SetMinXOrbit()
    {
        camera.orbit.minXRotation = minXOrbitField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MIN_X_ORBIT, minXOrbitField.value);
    }

    public void SetXOrbitSmooth()
    {
        camera.orbit.xOrbitSmooth = xOrbitSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_X_ORBIT_SMOOTH, xOrbitSmoothField.value);
    }

    public void SetYOrbitSmooth()
    {
        camera.orbit.yOrbitSmooth = yOrbitSmoothField.value;
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_Y_ORBIT_SMOOTH, yOrbitSmoothField.value);
    }

    public void SetAllowOrbit()
    {
        camera.orbit.allowOrbit = allowOrbitField.isOn;
        if (camera.orbit.allowOrbit)
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_ALLOW_ORBIT, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_ALLOW_ORBIT, 0);
    }

    public void SetRotateWithTarget()
    {
        camera.orbit.rotateWithTarget = rotateWithTargetField.isOn;
        if (camera.orbit.rotateWithTarget)
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_ROTATE_WITH_TARGET, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_ROTATE_WITH_TARGET, 0);
    }

    public void SetOrbitInput()
    {
        camera.input.MOUSE_ORBIT = orbitInputField.text;
        PlayerPrefs.SetString(CameraSettings.Instance.STANDARD_ORBIT_INPUT, orbitInputField.text);
    }

    public void SetCollisions()
    {
        camera.useCollision = useCollisionField.isOn;
        if (camera.useCollision)
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_USE_COLLISION, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_USE_COLLISION, 0);
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
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_FADE_OBSTRUCTIONS, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_FADE_OBSTRUCTIONS, 0);
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
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_HIGHLIGHT_TARGET, 1);
        else
            PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_HIGHLIGHT_TARGET, 0);
    }



    public void Reset()
    {
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_X, 0);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_Y, 0);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_LOCAL_OFFSET_Z, 0);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_DISTANCE, -8);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_ZOOM_SMOOTH, 2);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_ZOOM_STEP, 10);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MAX_ZOOM, -2);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MIN_ZOOM, -15);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_FOLLOW_SMOOTH, 0.05f);
        PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_USE_SMOOTH_FOLLOW, 1);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MAX_X_ORBIT, 25);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_MIN_X_ORBIT, -50);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_X_ORBIT_SMOOTH, 5);
        PlayerPrefs.SetFloat(CameraSettings.Instance.STANDARD_Y_ORBIT_SMOOTH, 5);
        PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_ALLOW_ORBIT, 1);
        PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_ROTATE_WITH_TARGET, 1);
        PlayerPrefs.SetString(CameraSettings.Instance.STANDARD_ORBIT_INPUT, "Fire1");
        PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_USE_COLLISION, 1);
        PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_FADE_OBSTRUCTIONS, 0);
        PlayerPrefs.SetInt(CameraSettings.Instance.STANDARD_HIGHLIGHT_TARGET, 0);
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
