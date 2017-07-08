using UnityEngine;
using System.Collections;

public class CameraSettings : MonoBehaviour {

    public static CameraSettings Instance;

    string camera_type;

    string standard_local_offset_x;
    string standard_local_offset_y;
    string standard_local_offset_z;
    string standard_distance;
    string standard_zoom_smooth;
    string standard_zoom_step;
    string standard_max_zoom;
    string standard_min_zoom;
    string standard_follow_smooth;
    string standard_use_smooth_follow;
    string standard_max_x_orbit;
    string standard_min_x_orbit;
    string standard_x_orbit_smooth;
    string standard_y_orbit_smooth;
    string standard_allow_orbit;
    string standard_rotate_with_target;
    string standard_orbit_input;
    string standard_use_collision;
    string standard_fade_obstructions;
    string standard_highlight_target;

    string topdown_distance;
    string topdown_allow_zoom;
    string topdown_zoom_smooth;
    string topdown_zoom_step;
    string topdown_max_zoom;
    string topdown_min_zoom;
    string topdown_follow_smooth;
    string topdown_use_smooth_follow;
    string topdown_x_rotation;
    string topdown_y_orbit_smooth;
    string topdown_allow_orbit;
    string topdown_orbit_input;
    string topdown_fade_obstructions;
    string topdown_highlight_target;

    string rts_invert_pan;
    string rts_pan_smooth;
    string rts_pan_input;
    string rts_distance;
    string rts_allow_zoom;
    string rts_zoom_smooth;
    string rts_zoom_step;
    string rts_max_zoom;
    string rts_min_zoom;
    string rts_x_rotation;
    string rts_y_orbit_smooth;
    string rts_allow_orbit;
    string rts_orbit_input;

    public string CAMERA_TYPE { get { return camera_type; } }

    public string STANDARD_LOCAL_OFFSET_X { get { return standard_local_offset_x; } }
    public string STANDARD_LOCAL_OFFSET_Y { get { return standard_local_offset_y; } }
    public string STANDARD_LOCAL_OFFSET_Z { get { return standard_local_offset_z; } }
    public string STANDARD_DISTANCE { get { return standard_distance; } }
    public string STANDARD_ZOOM_SMOOTH { get { return standard_zoom_smooth; } }
    public string STANDARD_ZOOM_STEP { get { return standard_zoom_step; } }
    public string STANDARD_MAX_ZOOM { get { return standard_max_zoom; } }
    public string STANDARD_MIN_ZOOM { get { return standard_min_zoom; } }
    public string STANDARD_FOLLOW_SMOOTH { get { return standard_follow_smooth; } }
    public string STANDARD_USE_SMOOTH_FOLLOW { get { return standard_use_smooth_follow; } }
    public string STANDARD_MAX_X_ORBIT { get { return standard_max_x_orbit; } }
    public string STANDARD_MIN_X_ORBIT { get { return standard_min_x_orbit; } }
    public string STANDARD_X_ORBIT_SMOOTH { get { return standard_x_orbit_smooth; } }
    public string STANDARD_Y_ORBIT_SMOOTH { get { return standard_y_orbit_smooth; } }
    public string STANDARD_ALLOW_ORBIT { get { return standard_allow_orbit; } }
    public string STANDARD_ROTATE_WITH_TARGET { get { return standard_rotate_with_target; } }
    public string STANDARD_ORBIT_INPUT { get { return standard_orbit_input; } }
    public string STANDARD_USE_COLLISION { get { return standard_use_collision; } }
    public string STANDARD_FADE_OBSTRUCTIONS { get { return standard_fade_obstructions; } }
    public string STANDARD_HIGHLIGHT_TARGET { get { return standard_highlight_target; } }

    public string TOPDOWN_DISTANCE { get { return topdown_distance; } }
    public string TOPDOWN_ALLOW_ZOOM { get { return topdown_allow_zoom; } }
    public string TOPDOWN_ZOOM_SMOOTH { get { return topdown_zoom_smooth; } }
    public string TOPDOWN_ZOOM_STEP { get { return topdown_zoom_step; } }
    public string TOPDOWN_MAX_ZOOM { get { return topdown_max_zoom; } }
    public string TOPDOWN_MIN_ZOOM { get { return topdown_min_zoom; } }
    public string TOPDOWN_FOLLOW_SMOOTH { get { return topdown_follow_smooth; } }
    public string TOPDOWN_USE_SMOOTH_FOLLOW { get { return topdown_use_smooth_follow; } }
    public string TOPDOWN_X_ROTATION { get { return topdown_x_rotation; } }
    public string TOPDOWN_Y_ORBIT_SMOOTH { get { return topdown_y_orbit_smooth; } }
    public string TOPDOWN_ALLOW_ORBIT { get { return topdown_allow_orbit; } }
    public string TOPDOWN_ORBIT_INPUT { get { return topdown_orbit_input; } }
    public string TOPDOWN_FADE_OBSTRUCTIONS { get { return topdown_fade_obstructions; } }
    public string TOPDOWN_HIGHLIGHT_TARGET { get { return topdown_highlight_target; } }

    public string RTS_INVERT_PAN { get { return rts_invert_pan; } }
    public string RTS_PAN_SMOOTH { get { return rts_pan_smooth; } }
    public string RTS_PAN_INPUT { get { return rts_pan_input; } }
    public string RTS_DISTANCE { get { return rts_distance; } }
    public string RTS_ALLOW_ZOOM { get { return rts_allow_zoom; } }
    public string RTS_ZOOM_SMOOTH { get { return rts_zoom_smooth; } }
    public string RTS_ZOOM_STEP { get { return rts_zoom_step; } }
    public string RTS_MAX_ZOOM { get { return rts_max_zoom; } }
    public string RTS_MIN_ZOOM { get { return rts_min_zoom; } }
    public string RTS_X_ROTATION { get { return rts_x_rotation; } }
    public string RTS_Y_ORBIT_SMOOTH { get { return rts_y_orbit_smooth; } }
    public string RTS_ALLOW_ORBIT { get { return rts_allow_orbit; } }
    public string RTS_ORBIT_INPUT { get { return rts_orbit_input; } }

    void Awake()
    {
        Instance = this;
        InitializeKeys();
    }

    void InitializeKeys()
    {
        camera_type = "CameraType";
        standard_local_offset_x = "StandardLocalOffsetX";
        standard_local_offset_y = "StandardLocalOffsetY";
        standard_local_offset_z = "StandardLocalOffsetZ";
        standard_distance = "StandardDistance";
        standard_zoom_smooth = "StandardZoomSmooth";
        standard_zoom_step = "StandardZoomStep";
        standard_max_zoom = "StandardMaxZoom";
        standard_min_zoom = "StandardMinZoom";
        standard_follow_smooth = "StandardFollowSmooth";
        standard_use_smooth_follow = "StandardUseSmoothFollow";
        standard_max_x_orbit = "StandardMaxXOrbit";
        standard_min_x_orbit = "StandardMinXOrbit";
        standard_x_orbit_smooth = "StandardXOrbitSmooth";
        standard_y_orbit_smooth = "StandardYOrbitSmooth";
        standard_allow_orbit = "StandardAllowOrbit";
        standard_rotate_with_target = "StandardRotateWithTarget";
        standard_orbit_input = "StandardOrbitInput";
        standard_use_collision = "StandardUseCollision";
        standard_fade_obstructions = "StandardFadeObstructions";
        standard_highlight_target = "StandardHighlightTarget";

        topdown_distance = "TopDownDistance";
        topdown_allow_zoom = "TopDownAllowZoom";
        topdown_zoom_smooth = "TopDownZoomSmooth";
        topdown_zoom_step = "TopDownZoomStep";
        topdown_max_zoom = "TopDownMaxZoom";
        topdown_min_zoom = "TopDownMinZoom";
        topdown_follow_smooth = "TopDownFollowSmooth";
        topdown_use_smooth_follow = "TopDownUseSmoothFollow";
        topdown_x_rotation = "TopDownXRotation";
        topdown_y_orbit_smooth = "TopDownYOrbitSmooth";
        topdown_allow_orbit = "TopDownAllowOrbit";
        topdown_orbit_input = "TopDownOrbitInput";
        topdown_fade_obstructions = "TopDownFadeObstructions";
        topdown_highlight_target = "TopDownHighlightTarget";

        rts_invert_pan = "RTSInvertPan";
        rts_pan_smooth = "RTSPanSmooth";
        rts_pan_input = "RTSPanInput";
        rts_distance = "RTSDistance";
        rts_allow_zoom = "RTSAllowZoom";
        rts_zoom_smooth = "RTSZoomSmooth";
        rts_zoom_step = "RTSZoomStep";
        rts_max_zoom = "RTSMaxZoom";
        rts_min_zoom = "RTSMinZoom";
        rts_x_rotation = "RTSXRotation";
        rts_y_orbit_smooth = "RTSYOrbitSmooth";
        rts_allow_orbit = "RTSAllowOrbit";
        rts_orbit_input = "RTSOrbitInput";
    }

}
