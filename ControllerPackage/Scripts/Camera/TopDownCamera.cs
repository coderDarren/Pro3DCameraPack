using UnityEngine;
using System.Collections;

public class TopDownCamera : MonoBehaviour {

    public Transform target;

    [System.Serializable]
    public class PositionSettings
    {
        //distance from our target
        //bools for zooming and smoothfollowing
        //min and max zoom settings
        public float distanceFromTarget = -50;
        public bool allowZoom = true;
        public float zoomSmooth = 100;
        public float zoomStep = 2;
        public float maxZoom = -30;
        public float minZoom = -60;
        public bool smoothFollow = true;
        public float smooth = 0.05f;

        [HideInInspector]
        public float newDistance = -50; //used for smooth zooming - gives us a "destination" zoom
    }

    [System.Serializable]
    public class OrbitSettings
    {
        //holding our current x and y rotation for our camera
        //bool for allowing orbit
        public float xRotation = -65;
        public float yRotation = -180;
        public bool allowOrbit = true;
        public float yOrbitSmooth = 0.5f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string MOUSE_ORBIT = "Fire1";
        public string ZOOM = "Mouse ScrollWheel";
    }

    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();

    Vector3 destination = Vector3.zero;
    Vector3 camVelocity = Vector3.zero;
    float mouseOrbitInput, zoomInput;

    bool editingSettings = false;

    void Start()
    {
        //setting camera target
        SetCameraTarget(target);

        if (target)
        {
            MoveToTarget();
        }
    }

    public void SetCameraTarget(Transform t)
    {
        //if we want to set a new target at runtime
        target = t;

        if (target == null)
        {
            Debug.LogError("Your camera needs a target");
        }
    }

    void GetInput()
    {
        //filling the values for our input variables
        if (!editingSettings)
        {
            try
            {
                mouseOrbitInput = Input.GetAxisRaw(input.MOUSE_ORBIT);
            }
            catch (System.Exception) { Debug.Log("Invalid orbit input. Please insert a valid value such as Fire1"); }
         
            zoomInput = Input.GetAxisRaw(input.ZOOM);
        }
    }

    void Update()
    {
        //getting input 
        //zooming
        GetInput();
        if (position.allowZoom && !editingSettings)
        {
            ZoomInOnTarget();
        }
    }

    void FixedUpdate()
    {
        //movetotarget
        //lookattarget
        //orbit
        if (target)
        {
            MoveToTarget();
            LookAtTarget();
            if (!editingSettings && orbit.allowOrbit)
                MouseOrbitTarget();
        }
    }

    void MoveToTarget()
    {
        //handling getting our camera to its destination position
        destination = target.position;
        destination += Quaternion.Euler(orbit.xRotation, orbit.yRotation, 0) * -Vector3.forward * position.distanceFromTarget;

        if (position.smoothFollow)
        {
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVelocity, position.smooth);
        }
        else
            transform.position = destination;
    }

    void LookAtTarget()
    {
        //handling getting our camera to look at the target at all times
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = targetRotation;
    }

    void MouseOrbitTarget()
    {
        if (mouseOrbitInput > 0)
        {
            orbit.yRotation += Input.GetAxis("Mouse X") * orbit.yOrbitSmooth;
        }
    }

    void ZoomInOnTarget()
    {
        //modifying the distancefromtarget to be closer or further away from our target
        position.newDistance += position.zoomStep * zoomInput;

        position.distanceFromTarget = Mathf.Lerp(position.distanceFromTarget, position.newDistance, position.zoomSmooth * Time.deltaTime);

        if (position.distanceFromTarget > position.maxZoom)
        {
            position.distanceFromTarget = position.maxZoom;
            position.newDistance = position.maxZoom;
        }
        if (position.distanceFromTarget < position.minZoom)
        {
            position.distanceFromTarget = position.minZoom;
            position.newDistance = position.minZoom;
        }
    }

    void OnEnable()
    {
        TopDownCameraSettings.IsEditing += IsEditing;
    }

    void OnDisable()
    {
        TopDownCameraSettings.IsEditing -= IsEditing;
    }

    void IsEditing(bool editing)
    {
        editingSettings = editing;
    }
}
