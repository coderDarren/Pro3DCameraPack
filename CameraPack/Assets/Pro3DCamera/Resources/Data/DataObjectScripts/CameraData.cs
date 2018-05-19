using UnityEngine;
using System.Collections;

namespace Pro3DCamera {
    [CreateAssetMenu()]
    public class CameraData : ScriptableObject {

        [System.Serializable]
        public class PositionSet
        {
            public Vector3 initialPos;
            public Vector3 targetPosOffset = new Vector3(0, 3.4f, 0);
            public Vector2 maxBoundary;
            public Vector2 minBoundary;
            public bool useElasticBoundary = false;
            public bool useBoundary = true;
            public float boundaryElasticity = 1;
            public float distanceFromGround = 40;
            public bool invertPan = true;
            public float panSmooth = 7f;
            public float panDrag = 0.1f;
            public float distanceFromTarget = -8;
            public bool allowZoom = true;
            public bool rpgFpsTransition = false;
            public float zoomSmooth = 100;
            public float zoomStep = 2;
            public float maxZoom = -2;
            public float minZoom = -15;
            public bool smoothFollow = true;
            public float smooth = 0.05f;
            public float collisionSmooth = 0.05f;

            //[HideInInspector]
            public float newDistance = -8; //set by zoom input
            [HideInInspector]
            public float adjustmentDistance = -8;
            [HideInInspector]
            public Vector3 adjustedTargetPosOffset;
        }

        [System.Serializable]
        public class OrbitSet
        {
            public float xRotation = -20;
            public float yRotation = -180;
            public float maxXRotation = 25;
            public float minXRotation = -50;
            public float xOrbitSmooth = 0.5f;
            public float yOrbitSmooth = 0.5f;
            public bool allowOrbit = true;
            public bool rotateWithTarget = true;
            public bool alwaysFindXAngle = true;
            public bool alwaysFindYAngle = true;
            public float defaultXAngle = 10;
            public float defaultYAngle = 0;
            public float timeToRevertX = 0.5f;
            public float timeToRevertY = 0.5f;
        }

        public enum InputOption
        {
            LEFT_MOUSE = KeyCode.Mouse0,
            RIGHT_MOUSE = KeyCode.Mouse1
        }

        [System.Serializable]
        public class InputSet
        {
            public InputOption MOUSE_ORBIT = InputOption.LEFT_MOUSE;
            public string ZOOM = "Mouse ScrollWheel";
            public InputOption PAN = InputOption.LEFT_MOUSE;
            public InputOption ORBIT_Y = InputOption.RIGHT_MOUSE;
        }

        [System.Serializable]
        public class MobileSet
        {
            public bool useMobileInput = false;
            public float minimumPinchDelta = 1f;
            public float minimumTurnAngle = 0.05f;
        }

        [System.Serializable]
        public class DebugSet
        {
            public bool useCollision = true;
            public float collisionPadding = 2;
            public bool useCloseQuartersTechnique = true;
            public float closeQuartersDistance = 2;
            public float closeQuartersHeightAdjust = 5;
            public bool drawDesiredCollisionLines = true;
            public bool drawAdjustedCollisionLines = true;
            public bool useCloseQuartersFade = true;
            public float distanceToFadeTarget = 2;
            public float targetFadeAlpha = 0.1f;
        }

        //fps only
        public bool smoothLook = true;
        public float smoothLookTime = 20f;
        public float XSensitivity = 200f;
        public float YSensitivity = 320f;
        public bool useBounce = true;
        public bool useZoom = true;
        public float bounceFrequency = 2f;
        public float bounceAmplitude = 0.7f;

        //rts only
        public LayerMask groundLayer;

        //rpg and top down
        public LayerMask collisionLayer;

        public PositionSet pos = new PositionSet();
        public OrbitSet orbit = new OrbitSet();
        public InputSet input = new InputSet();
        //public MobileSet mobile = new MobileSet();
        public DebugSet debug = new DebugSet();

    }
}