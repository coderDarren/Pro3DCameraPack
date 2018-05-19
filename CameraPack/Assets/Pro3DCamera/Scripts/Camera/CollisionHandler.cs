using UnityEngine;
using System.Collections;
using Pro3DCamera;

public class CollisionHandler : MonoBehaviour
{

    public CameraDataManager dataManager;

    [HideInInspector]
    public LayerMask collisionLayer;
    [HideInInspector]
    public bool colliding = false;
    [HideInInspector]
    public Vector3[] adjustedCameraClipPoints;
    [HideInInspector]
    public Vector3[] desiredCameraClipPoints;

    Camera _camera;
    CameraControl _camControl;
    [HideInInspector]
    public Vector3 collisionCheckPos;

    /// <summary>
    /// Initializes the camera member and the clip point arrays.
    /// </summary>
    public void Initialize(Camera cam)
    {
        _camera = cam;
        adjustedCameraClipPoints = new Vector3[5];
        desiredCameraClipPoints = new Vector3[5];
        _camControl = GetComponent<CameraControl>();
    }

    /// <summary>
    /// Updates adjusted and desired clip point arrays and checks if the camera is colliding.
    /// Main update function called from CameraControl
    /// </summary>
    public void UpdateCollisionHandler(Vector3 cameraDestination, Vector3 targetPos)
    {
        switch (_camControl.activeCamera)
        {
            case CameraControl.CameraType.RPG: collisionLayer = dataManager.rpgData.collisionLayer; break;
            case CameraControl.CameraType.TOP_DOWN: collisionLayer = dataManager.topDownData.collisionLayer; break;
        }
        collisionCheckPos = targetPos;
        //collisionCheckPos.x = 0;
        UpdateCameraClipPoints(transform.position, transform.rotation, ref adjustedCameraClipPoints);
        UpdateCameraClipPoints(cameraDestination, transform.rotation, ref desiredCameraClipPoints);
        CheckColliding(collisionCheckPos); //using raycasts here
    }

    /// <summary>
    /// Calculates the clip points based on the camera's near clip plane, field of view and aspect ratio.
    /// Places the calculated clip points in intoArray
    /// </summary>
    void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
    {
        if (!_camera)
            return;

        //clear the contents of intoArray
        intoArray = new Vector3[5];

        float z = _camera.nearClipPlane;
        float x = Mathf.Tan(_camera.fieldOfView / 3.41f) * z;
        float y = x / _camera.aspect;

        //top left
        intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; //added and rotated the point relative to camera
        //top right
        intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition; //added and rotated the point relative to camera
        //bottom left
        intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition; //added and rotated the point relative to camera
        //bottom right
        intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition; //added and rotated the point relative to camera
        //camera's position
        intoArray[4] = cameraPosition - _camera.transform.forward;
    }

    /// <summary>
    /// Returns true if any of the clip point rays return a hit.
    /// </summary>
    bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
    {
        for (int i = 0; i < clipPoints.Length; i++)
        {
            Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
            float distance = Vector3.Distance(clipPoints[i], fromPosition);
            if (Physics.Raycast(ray, distance, collisionLayer))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the distance closest to the target which is derived from one of the 5 rays that returns.. 
    /// ..a hit casted from the camera's clip points and center.
    /// </summary>
    public float GetAdjustedDistanceWithRayFrom(Vector3 from)
    {
        float distance = -1;

        for (int i = 0; i < desiredCameraClipPoints.Length; i++)
        {
            Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (distance == -1)
                    distance = hit.distance;
                else
                {
                    if (hit.distance < distance)
                        distance = hit.distance;
                }
            }
        }

        if (distance == -1)
            return 0;
        else
            return distance;
    }

    /// <summary>
    /// Returns true when a collision is detected from one of the 5 rays casted from the desired clip points and center
    /// </summary>
    void CheckColliding(Vector3 targetPosition)
    {
        if (CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
        {
            colliding = true;
        }
        else
        {
            colliding = false;
        }
    }
}

