using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollisionHandler))]

public class StandardCamera : MonoBehaviour
{
    public Transform target;
    public bool useCollision = true;

    [System.Serializable]
    public class PositionSettings
    {
        public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
        public float distanceFromTarget = -8;
        public float zoomSmooth = 100;
        public float zoomStep = 2;
        public float maxZoom = -2;
        public float minZoom = -15;
        public bool smoothFollow = true;
        public float smooth = 0.05f;

        [HideInInspector]
        public float newDistance = -8; //set by zoom input
        [HideInInspector]
        public float adjustmentDistance = -8;
    }

    [System.Serializable]
    public class OrbitSettings
    {
        public float xRotation = -20;
        public float yRotation = -180;
        public float maxXRotation = 25;
        public float minXRotation = -50;
        public float xOrbitSmooth = 0.5f;
        public float yOrbitSmooth = 0.5f;
        public bool allowOrbit = true;
        public bool rotateWithTarget = true;
    }

    [System.Serializable]
    public class InputSettings
    {
        public string MOUSE_ORBIT = "Fire1";
        public string ZOOM = "Mouse ScrollWheel";
    }

    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;
    }

    public PositionSettings position = new PositionSettings();
    public OrbitSettings orbit = new OrbitSettings();
    public InputSettings input = new InputSettings();
    public DebugSettings debug = new DebugSettings();

    CollisionHandler collision;
    Vector3 targetPos = Vector3.zero;
    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero; 
    Vector3 camVel = Vector3.zero; 
    float zoomInput, mouseOrbitInput;

    bool editingSettings = false;

    void Start()
    {
        collision = GetComponent<CollisionHandler>();

        SetCameraTarget(target);

        zoomInput = mouseOrbitInput = 0;

        if (target)
        {
            MoveToTarget();

            collision.Initialize(Camera.main);
            collision.UpdateCollisionHandler(destination, targetPos);
        }

    }

    void SetCameraTarget(Transform t)
    {
        target = t;

        if (target == null)
        {
            Debug.LogError("Your camera needs a target.");
        }
    }

    void GetInput()
    {
        zoomInput = Input.GetAxisRaw(input.ZOOM);
        if (!editingSettings)
        {
            try
            {
                mouseOrbitInput = Input.GetAxisRaw(input.MOUSE_ORBIT);
            }
            catch (System.Exception) { Debug.Log("Invalid orbit input. Please insert a valid value such as Fire1"); }
        }
    }

    void Update()
    {
        GetInput();
        if (!editingSettings)
            ZoomInOnTarget();
        DrawDebugLines();
    }

    void FixedUpdate() 
    {
        //moving
        MoveToTarget();
        //rotating
        LookAtTarget();
        //player input orbit
        if (!editingSettings)
        {
            if (orbit.allowOrbit)
                MouseOrbitTarget();
        }

        if (useCollision)
        {
            collision.UpdateCollisionHandler(destination, targetPos);
            position.adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(targetPos);
        }
    }

    void MoveToTarget()
    {
        targetPos = target.position + Vector3.up * position.targetPosOffset.y + 
                                      transform.TransformDirection(Vector3.forward * position.targetPosOffset.z) + 
                                      transform.TransformDirection(Vector3.right * position.targetPosOffset.x); //NEW
        if (orbit.rotateWithTarget)
            destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * -Vector3.forward * position.distanceFromTarget;
        else
            destination = Quaternion.Euler(orbit.xRotation, orbit.yRotation, 0) * -Vector3.forward * position.distanceFromTarget;
        destination += targetPos;

        if (collision.colliding && useCollision)
        {
            if (orbit.rotateWithTarget)
                adjustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotation + target.eulerAngles.y, 0) * Vector3.forward * position.adjustmentDistance;
            else
                adjustedDestination = Quaternion.Euler(orbit.xRotation, orbit.yRotation, 0) * Vector3.forward * position.adjustmentDistance;

            adjustedDestination += targetPos;

            if (position.smoothFollow)
            {
                //use smooth damp function
                transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref camVel, position.smooth);
            }
            else
                transform.position = adjustedDestination;
        }
        else
        {
            if (position.smoothFollow)
            {
                //use smooth damp function
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref camVel, position.smooth);
            }
            else
                transform.position = destination;
        }
    }

    void LookAtTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100 * Time.deltaTime);
    }

    void MouseOrbitTarget()
    {
        if (mouseOrbitInput > 0)
        {
            orbit.xRotation += Input.GetAxis("Mouse Y") * orbit.xOrbitSmooth;
            orbit.yRotation += Input.GetAxis("Mouse X") * orbit.yOrbitSmooth;
        }
        CheckVerticalRotation();
    }

    void CheckVerticalRotation()
    {
        if (orbit.xRotation > orbit.maxXRotation)
        {
            orbit.xRotation = orbit.maxXRotation;
        }
        if (orbit.xRotation < orbit.minXRotation)
        {
            orbit.xRotation = orbit.minXRotation;
        }
    }

    void ZoomInOnTarget()
    {
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


    void DrawDebugLines()
    {
        for (int i = 0; i < 5; i++)
        {
            if (debug.drawDesiredCollisionLines)
            {
                Debug.DrawLine(targetPos, collision.desiredCameraClipPoints[i], Color.white);
            }
            if (debug.drawAdjustedCollisionLines)
            {
                Debug.DrawLine(targetPos, collision.adjustedCameraClipPoints[i], Color.green);
            }
        }
    }

    void OnEnable()
    {
        StandardCameraSettings.IsEditing += IsEditing;
    }

    void OnDisable()
    {
        StandardCameraSettings.IsEditing -= IsEditing;
    }

    void IsEditing(bool editing)
    {
        editingSettings = editing;
    }
}
