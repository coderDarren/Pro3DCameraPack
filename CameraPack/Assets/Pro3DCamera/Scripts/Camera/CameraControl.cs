using UnityEngine;
using System.Collections;

namespace Pro3DCamera
{
    [RequireComponent(typeof(CollisionHandler))]
    public class CameraControl : MonoBehaviour {

        #region Initialization

        /// <summary>
        /// Initialize the camera based on the awakeCam set from the editor
        /// If you want to manually set a camera at the start of application, you can do it on Start(), since it executes after Awake()
        /// </summary>
        void Awake()
        {
            collision = GetComponent<CollisionHandler>();

            SetCameraTarget(target);

            zoomInput = mouseOrbitInput = 0;

            if (target)
            {
                int awakeID = PlayerPrefs.GetInt("ActiveAwakeCam");
                switch (awakeID)
                {
                    case 0: InitRPG(); break;
                    case 1: InitFPS(); break;
                    case 2: InitRTS(); break;
                    case 3: InitTopDown(); break;
                }


                collision.Initialize(Camera.main);
                //collision.UpdateCollisionHandler(destination, targetPos);
                //multiTouch = new MultitouchHandler(dataManager.rtsData.mobile.minimumPinchDelta, dataManager.rtsData.mobile.minimumTurnAngle);
                //singleTouch = new TouchHandler();
            }
        }

        /// <summary>
        /// Sets the activeCam to RPG and prepares _lookSpeed and _smoothTime for the camera transition
        /// </summary>
        void InitRPG()
        {
            activeCam = CameraType.RPG;
            if (!dataManager.rpgData)
            {
                Debug.LogWarning("Unable to initialize RPG Camera. RPG Data does not exist.");
                return;
            }
            //dataManager.rpgData.pos.newDistance = (dataManager.rpgData.pos.maxZoom + dataManager.rpgData.pos.minZoom) / 4.0f;
            //dataManager.rpgData.pos.distanceFromTarget = (dataManager.rpgData.pos.maxZoom + dataManager.rpgData.pos.minZoom) / 4.0f;
            _lookSpeed = 1;
            _smoothTime = 1;
            _collisionSmoothTime = 1;
            dataManager.rpgData.pos.adjustedTargetPosOffset = dataManager.rpgData.pos.targetPosOffset;
            MoveToTarget(dataManager.rpgData.pos, dataManager.rpgData.orbit, dataManager.rpgData.debug.useCollision);
        }

        /// <summary>
        /// Sets the activeCam to FPS and hides the target
        /// </summary>
        void InitFPS()
        {
            activeCam = CameraType.FPS;
            if (!dataManager.fpsData)
            {
                Debug.LogWarning("Unable to initialize FPS Camera. FPS Data does not exist.");
                return;
            }
            bouncePosition = Vector2.zero;
            dataManager.fpsData.pos.newDistance = dataManager.fpsData.pos.maxZoom;
            dataManager.fpsData.pos.distanceFromTarget = dataManager.fpsData.pos.maxZoom;   
            ToggleShowTarget(false);
        }

        /// <summary>
        /// Sets the activeCam to RTS and initializes the position and rotation of the camera.
        /// </summary>
        void InitRTS()
        {
            activeCam = CameraType.RTS;
            if (!dataManager.rtsData)
            {
                Debug.LogWarning("Unable to initialize RTS Camera. RTS Data does not exist.");
                return;
            }
            transform.position = dataManager.rtsData.pos.initialPos;
            targetPos = transform.position;
            HandleCameraDistance();
            RTSRotate();
            RTSZoom();
        }

        /// <summary>
        /// Sets the activeCam to TOP_DOWN and prepares _lookSpeed and _smoothTime for the camera transition
        /// </summary>
        void InitTopDown()
        {
            activeCam = CameraType.TOP_DOWN;
            if (!dataManager.topDownData)
            {
                Debug.LogWarning("Unable to initialize Top Down Camera. Top Down Data does not exist.");
                return;
            }
            _smoothTime = 1;
            _lookSpeed = 1;
           //dataManager.topDownData.orbit.xRotation = dataManager.topDownData.orbit.xRotation;
            dataManager.topDownData.orbit.yRotation = dataManager.topDownData.orbit.defaultYAngle;
            MoveToTarget(dataManager.topDownData.pos, dataManager.topDownData.orbit, dataManager.topDownData.debug.useCollision);
        }

        #endregion

        #region Update

        /// <summary>
        /// Handles updates for input, zooming and transitioning to FPS
        /// </summary>
        void UpdateRPG()
        {
            if (!dataManager.rpgData)
                return;

            GetInput(dataManager.rpgData.input);
            if (dataManager.rpgData.pos.allowZoom)
                ZoomInOnTarget(ref dataManager.rpgData.pos);
            if (dataManager.rpgData.pos.rpgFpsTransition)
            {
                if (dataManager.rpgData.pos.distanceFromTarget <= dataManager.rpgData.pos.maxZoom + 0.01f)
                {
                    dataManager.rpgData.pos.newDistance = dataManager.rpgData.pos.maxZoom + 0.1f;
                    dataManager.rpgData.pos.distanceFromTarget = dataManager.rpgData.pos.maxZoom + 0.1f;
                    SetCameraType(CameraType.FPS);
                }
            }
            //_lookSpeed = Mathf.Lerp(_lookSpeed, 100, 1 * Time.deltaTime);
            DrawDebugLines(dataManager.rpgData.debug);
        }

        /// <summary>
        /// Handles updates for input, zooming, bouncing and transitioning to RPG
        /// </summary>
        void UpdateFPS()
        {
            if (!dataManager.fpsData)
                return;

            if (Input.GetKeyUp(KeyCode.Tab))
                Cursor.visible = !Cursor.visible;
            GetInput(dataManager.fpsData.input);
            if (dataManager.fpsData.pos.allowZoom)
                ZoomInOnTarget(ref dataManager.fpsData.pos);
            if (dataManager.fpsData.useBounce)
                Bounce();
            else {
                bouncePosition = Vector2.zero;
            }
            if (dataManager.fpsData.pos.rpgFpsTransition)
            {
                if (dataManager.fpsData.pos.distanceFromTarget >= dataManager.fpsData.pos.minZoom - 0.1f)
                {
                    dataManager.fpsData.pos.newDistance = dataManager.fpsData.pos.minZoom - 0.1f;
                    dataManager.fpsData.pos.distanceFromTarget = dataManager.fpsData.pos.minZoom - 0.1f;   
                    SetCameraType(CameraType.RPG);
                }
            }
        }

        /// <summary>
        /// Handles updates for input, zooming, rotating, panning, and bounds maintainence
        /// </summary>
        void UpdateRTS()
        {
            if (!dataManager.rtsData)
                return;

            //updating input
            GetInput(dataManager.rtsData.input);
            //if (!dataManager.rtsData.mobile.useMobileInput)
            //{
            if (dataManager.rtsData.pos.allowZoom)
                RTSZoom();
            //rotating
            RTSRotate();
            //panning
            PanWorld();
            //}
            /*else {
                if (dataManager.rtsData.pos.allowZoom)
                    RTSZoom();
                if (dataManager.rtsData.orbit.allowOrbit)
                    RTSRotateMobile();
                PanWorldMobile();
            }*/

            if (dataManager.rtsData.pos.useBoundary)
            {
                if (dataManager.rtsData.pos.useElasticBoundary)
                    RTSHandleBoundsElastic();
                else
                    RTSHandleBounds();
            }
        }

        /// <summary>
        /// Handles updates for input and zooming
        /// </summary>
        void UpdateTopDown()
        {
            if (!dataManager.topDownData)
                return;

            GetInput(dataManager.topDownData.input);
            if (dataManager.topDownData.pos.allowZoom)
            {
                ZoomInOnTarget(ref dataManager.topDownData.pos);
            }
        }
        
        /// <summary>
        /// Handles updating camera types based on the active camera.
        /// Prevents the camera from being switched more than once within a specified threshold.
        /// </summary>
        void Update()
        {
            //update datas

            switch (activeCam)
            {
                case CameraType.RPG: UpdateRPG(); break;
                case CameraType.FPS: UpdateFPS(); break;
                case CameraType.RTS: UpdateRTS(); break;
                case CameraType.TOP_DOWN: UpdateTopDown(); break;
            }
            if (controller)
            {
                if (activeCam == CameraType.FPS)
                    controller.firstPersonMode = true;
                else
                    controller.firstPersonMode = false;
            }
            timeSinceLastCameraSwitch += Time.deltaTime; //keeping track of time between camera switching       
        }

        #endregion

        #region FixedUpdate

        Vector3 collisionTargetPos;
        float collisionTimer; //keeps track of length of a collision
        /// <summary>
        /// Handles fixed updates for positioning, rotation and collision
        /// </summary>
        void FixedUpdateRPG()
        {
            if (!dataManager.rpgData)
                return;

            //moving
            MoveToTarget(dataManager.rpgData.pos, dataManager.rpgData.orbit, dataManager.rpgData.debug.useCollision);
            //rotating
            LookAtTarget();
            //player input orbit
            if (dataManager.rpgData.orbit.allowOrbit)
                MouseOrbitTarget(ref dataManager.rpgData.orbit, ref dataManager.rpgData.pos);

            if (dataManager.rpgData.debug.useCollision)
            {
                collisionTargetPos = target.position;
                collisionTargetPos.y += dataManager.rpgData.pos.targetPosOffset.y;

                collision.UpdateCollisionHandler(destination, collisionTargetPos);
            }
        }

        /// <summary>
        /// Handles fixed updates for positioning and looking
        /// </summary>
        void FixedUpdateFPS()
        {
            if (!dataManager.fpsData)
                return;

            Vector3 zoomFactor = (dataManager.fpsData.useZoom) ? Vector3.forward * -dataManager.fpsData.pos.distanceFromTarget : Vector3.zero;
            targetPos = target.transform.position +
                        Vector3.up * dataManager.fpsData.pos.targetPosOffset.y +
                        Vector3.up * bouncePosition.y +
                        transform.TransformDirection(zoomFactor) +
                        transform.TransformDirection(Vector3.right * bouncePosition.x);

            transform.position = Vector3.Slerp(transform.position, targetPos + transform.TransformDirection(shakeOffset), 20 * Time.deltaTime);

            TurnTarget();
            LookUpDown();
        }

        /// <summary>
        /// Handles fixed updates for distance maintainence
        /// </summary>
        void FixedUpdateRTS()
        {
            if (!dataManager.rtsData)
                return;

            HandleCameraDistance();
        }

        /// <summary>
        /// Handles fixed updates for positioning and rotation
        /// </summary>
        void FixedUpdateTopDown()
        {
            if (!dataManager.topDownData)
                return;

            MoveToTarget(dataManager.topDownData.pos, dataManager.topDownData.orbit, dataManager.topDownData.debug.useCollision);
            //if (!shaking)
            LookAtTarget();
            if (dataManager.topDownData.orbit.allowOrbit)
            {
                MouseOrbitTarget(ref dataManager.topDownData.orbit, ref dataManager.topDownData.pos);
            }
        }

        /// <summary>
        /// Handles fixed updates for the active camera
        /// </summary>
        void FixedUpdate()
        {
            switch (activeCam)
            {
                case CameraType.RPG: FixedUpdateRPG(); break;
                case CameraType.FPS: FixedUpdateFPS(); break;
                case CameraType.RTS: FixedUpdateRTS(); break;
                case CameraType.TOP_DOWN: FixedUpdateTopDown(); break;
            }
        }

        #endregion

        #region Camera Functionality

        float _smoothTime;
        float _collisionSmoothTime;
        RaycastHit _hit;
        Vector3 testTargetPos = Vector3.zero; //the ideal target pos, unchanged by any collision adjustments
        bool _shouldShift = false; //is the camera X offset causing occlusion?
        bool _closeQuartersCollision = false; //is the camera moving upward due to close quarters?
        /// <summary>
        /// Main function for updating the RPG and Top Down camera positions.
        /// Locks the camera on the target's center during collision.
        /// Smooths toward a destination determined by current rotation and zoom values.
        /// Makes position adjustments when the target is backed up against a wall.
        /// Toggles the target to fade when the camera is too close.
        /// </summary>
        void MoveToTarget(CameraData.PositionSet positionSet, CameraData.OrbitSet orbitSet, bool useCollision)
        {
            //keeps the camera locked on the target when the X offset is high
            AdjustCameraCenter(positionSet);
            
            targetPos = target.position + Vector3.up * positionSet.targetPosOffset.y +
                                          transform.TransformDirection(Vector3.forward * positionSet.targetPosOffset.z) +
                                          transform.TransformDirection(Vector3.right * positionSet.adjustedTargetPosOffset.x); 
            if (orbitSet.rotateWithTarget)
            {
                destination = Quaternion.Euler(orbitSet.xRotation + shakeRotationOffset.x, 
                                               orbitSet.yRotation + target.eulerAngles.y + shakeRotationOffset.y, 
                                               0) * -Vector3.forward * positionSet.distanceFromTarget;
            }
            else
            {
                destination = Quaternion.Euler(orbitSet.xRotation + shakeRotationOffset.x, 
                                               orbitSet.yRotation + shakeRotationOffset.y, 
                                               0) * -Vector3.forward * positionSet.distanceFromTarget;
            }
            destination += targetPos;

            _smoothTime = Mathf.Lerp(_smoothTime, positionSet.smooth, 3 * Time.deltaTime);
            _collisionSmoothTime = Mathf.Lerp(_collisionSmoothTime, positionSet.collisionSmooth, 3 * Time.deltaTime);

            if (collision.colliding && useCollision)
            {
                //attaining the distance the camera should be from the target. Determined by the distance the wall is from the target pos
                positionSet.adjustmentDistance = collision.GetAdjustedDistanceWithRayFrom(collisionTargetPos);

                //adjusting the distance with padding. If padding surpasses distance, the distance is set to 0, placing the camera at the target pos
                if (positionSet.adjustmentDistance - dataManager.rpgData.debug.collisionPadding > 0)
                    positionSet.adjustmentDistance -= dataManager.rpgData.debug.collisionPadding;
                else {
                    positionSet.adjustmentDistance = 0.1f;
                }

                //calculating the new destination based on our adjustment distance and current rotations
                int i = (activeCam == CameraType.RPG) ? -1 : 1;
                if (orbitSet.rotateWithTarget)
                {
                    //if rotating with the target, our y angle depends on the target y angle
                    adjustedDestination = Quaternion.Euler(orbitSet.xRotation + shakeRotationOffset.x, 
                                                           orbitSet.yRotation + target.eulerAngles.y + shakeRotationOffset.y, 
                                                           0) * Vector3.forward * positionSet.adjustmentDistance * i;
                }
                else
                {
                    //if not rotating with the target, our y angle is independent
                    adjustedDestination = Quaternion.Euler(orbitSet.xRotation + shakeRotationOffset.x, 
                                                           orbitSet.yRotation + shakeRotationOffset.y, 
                                                           0) * Vector3.forward * positionSet.adjustmentDistance * i;
                }

                //handle the adjusted destination when the player is backed against the wall
                HandleCloseQuarters();
                
                //add our adjustments to the target's position
                adjustedDestination += targetPos;

                //move the camera position toward the adjustment position
                if (positionSet.smoothFollow)
                {
                    //use smooth damp function
                    transform.position = Vector3.SmoothDamp(transform.position, 
                                                            adjustedDestination + transform.TransformDirection(shakeOffset), 
                                                            ref camVel, 
                                                            _collisionSmoothTime);
                }
                else
                    transform.position = adjustedDestination + transform.TransformDirection(shakeOffset);
            }
            else
            {
                //move the camera toward the calculated position
                if (positionSet.smoothFollow)
                {
                    //use smooth damp function
                    transform.position = Vector3.SmoothDamp(transform.position, 
                                                            destination + transform.TransformDirection(shakeOffset), 
                                                            ref camVel, 
                                                            _smoothTime);
                }
                else
                    transform.position = destination + transform.TransformDirection(shakeOffset);
            }

            if (dataManager.rpgData.debug.useCloseQuartersFade){
                if (Vector3.Distance(collisionTargetPos, transform.position) <= dataManager.rpgData.debug.distanceToFadeTarget)
                    ToggleFadeTarget(true);
                else 
                    ToggleFadeTarget(false);
            }
        }

        float _lookSpeed = 100;
        Vector3 _lookPos;
        /// <summary>
        /// Updates the camera rotation to face the target at a rate of _lookSpeed.
        /// </summary>
        void LookAtTarget()
        {
            if (shaking){
                _lookSpeed = 40;
            }else if (smoothingOffset)
            {
                _lookSpeed = 1;
            }
            else {
                _lookSpeed = Mathf.Lerp(_lookSpeed, 100, 0.25f * Time.deltaTime);
            }
            _lookPos = Vector3.Lerp(transform.position + transform.forward * 20, targetPos, 30 * Time.deltaTime);
            targetRotation = Quaternion.LookRotation(_lookPos - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _lookSpeed * Time.deltaTime);
        }

        float returnAngleXVel = 0, returnAngleYVel = 0;
        float xRot, yRot;
        /// <summary>
        /// Modifies the camera's rotation based on current orbiting input values.
        /// </summary>
        void MouseOrbitTarget(ref CameraData.OrbitSet orbitSet, ref CameraData.PositionSet positionSet)
        {
            if (mouseOrbitInput > 0)
            {
                if (activeCam == CameraType.RPG)
                    xRot = Input.GetAxisRaw("Mouse Y") * -orbitSet.xOrbitSmooth;
                yRot = Input.GetAxisRaw("Mouse X") * orbitSet.yOrbitSmooth;
                orbitSet.xRotation = Mathf.SmoothDampAngle(orbitSet.xRotation, orbitSet.xRotation + xRot, ref returnAngleXVel, 0.2f);
                orbitSet.yRotation = Mathf.SmoothDampAngle(orbitSet.yRotation, orbitSet.yRotation + yRot, ref returnAngleYVel, 0.2f);
            }
            else
            {
                if (orbitSet.alwaysFindYAngle)
                    orbitSet.yRotation = Mathf.SmoothDampAngle(orbitSet.yRotation, orbitSet.defaultYAngle, ref returnAngleYVel, orbitSet.timeToRevertY);
                if (orbitSet.alwaysFindXAngle)
                    orbitSet.xRotation = Mathf.SmoothDampAngle(orbitSet.xRotation, orbitSet.defaultXAngle, ref returnAngleXVel, orbitSet.timeToRevertX);
            }
            if (activeCam == CameraType.RPG)
                CheckVerticalRotation(ref orbitSet);
        }

        /// <summary>
        /// Modifies the camera's distance based on the current zoom input value.
        /// </summary>
        void ZoomInOnTarget(ref CameraData.PositionSet positionSet)
        {
            positionSet.newDistance += positionSet.zoomStep * -zoomInput;

            if (positionSet.newDistance > positionSet.minZoom)
                positionSet.newDistance = positionSet.minZoom;
            if (positionSet.newDistance < positionSet.maxZoom)
                positionSet.newDistance = positionSet.maxZoom;


            positionSet.distanceFromTarget = Mathf.Lerp(positionSet.distanceFromTarget, positionSet.newDistance, positionSet.zoomSmooth * Time.deltaTime);
        }

        /// <summary>
        /// Keeps the camera locked on the target.
        /// Useful when the camera's X offset is high
        /// </summary>
        void AdjustCameraCenter(CameraData.PositionSet positionSet)
        {
            //Debug.DrawRay(collisionTargetPos, Vector3.Normalize(testTargetPos - collisionTargetPos) * positionSet.targetPosOffset.x, Color.green);
            int dir = (positionSet.targetPosOffset.x < 0) ? -1 : 1;
            testTargetPos = collisionTargetPos + positionSet.targetPosOffset.x * ((dir == 1) ? target.right : -target.right);
            if (Physics.Raycast(collisionTargetPos, Vector3.Normalize(testTargetPos - collisionTargetPos) * dir, out _hit, Mathf.Abs(positionSet.targetPosOffset.x), collision.collisionLayer)){
                _shouldShift = true;
            }
            else if (!collision.colliding && !_closeQuartersCollision) {
                _shouldShift = false;
            }

            if (_shouldShift)
            {
                positionSet.adjustedTargetPosOffset.x = Mathf.Lerp(positionSet.adjustedTargetPosOffset.x, 0, 5 * Time.deltaTime);
            }
            else {
                positionSet.adjustedTargetPosOffset.x = Mathf.Lerp(positionSet.adjustedTargetPosOffset.x, positionSet.targetPosOffset.x, 1 * Time.deltaTime);
            }
        }

        /// <summary>
        /// If the close quarters feature is enabled, 
        /// ..pushes the camera upward when the target is backed against the wall
        /// </summary>
        void HandleCloseQuarters()
        {
            float adjustedHeight = 0;
            if (dataManager.rpgData.debug.useCloseQuartersTechnique)
            {
                if (Physics.Raycast(collisionTargetPos, -target.forward, dataManager.rpgData.debug.closeQuartersDistance, collision.collisionLayer)) {
                    float height = dataManager.rpgData.debug.closeQuartersHeightAdjust;
                    adjustedHeight = height;
                    if (Physics.Raycast(targetPos, Vector3.up, out _hit)){
                        //Debug.Log("Hit distance: "+_hit.distance);
                        adjustedHeight = _hit.distance;
                    }    
                    _shouldShift = true; 
                    _closeQuartersCollision = true;
                    adjustedDestination += Vector3.up * adjustedHeight;
                }
                else {
                    _closeQuartersCollision = false;
                }
            }
        }

        #region FPS

        //FPS only
        /// <summary>
        /// 
        /// </summary>
        void LookUpDown()
        {
            rotation.x -= look.y * dataManager.fpsData.YSensitivity * Time.deltaTime;
            rotation.x = Mathf.Clamp(rotation.x, -60, 80);
            if (dataManager.fpsData.smoothLook)
            {
                targetRotation = Quaternion.Slerp(targetRotation, target.rotation * Quaternion.Euler(rotation), dataManager.fpsData.smoothLookTime * Time.deltaTime);
                transform.rotation = targetRotation;
            }
            else {
                transform.rotation = target.rotation * Quaternion.Euler(rotation + shakeRotationOffset);
            }
        }

        //FPS only
        /// <summary>
        /// 
        /// </summary>
        void TurnTarget()
        {
            target.rotation *= Quaternion.AngleAxis(look.x * dataManager.fpsData.XSensitivity * Time.deltaTime, Vector3.up);
        }

        //FPS only
        /// <summary>
        /// 
        /// </summary>
        float targetX = 0, targetY = 0;
        void Bounce()
        {
            if (!controller)
            {
                Debug.LogWarning("Since your target does not use a PlayerController component, the bounce feature has been disabled.\nThis is because bouncing requires information about moving and being grounded.\nDouble click this to comment it out if it is unwanted.");
                return;
            }
            //Controller needs a grounded bool - or a method called 'Grounded' that returns a bool.
            //Controller needs some float for forward movement and a float for strafe movement
            if (controller.Grounded() && (Mathf.Abs(controller.forwardInput) > 0.1f || Mathf.Abs(controller.turnInput) > 0.1f))
            {
                targetX = Mathf.PingPong(dataManager.fpsData.bounceFrequency * Time.time, dataManager.fpsData.bounceAmplitude);
                targetY = Mathf.PingPong(dataManager.fpsData.bounceFrequency * Time.time, dataManager.fpsData.bounceAmplitude / 2f);
                targetX -= (dataManager.fpsData.bounceAmplitude / 2f);
                targetY -= (dataManager.fpsData.bounceAmplitude / 4f);
                bouncePosition.x = Mathf.Lerp(bouncePosition.x, targetX, dataManager.fpsData.bounceFrequency * Time.deltaTime);
                bouncePosition.y = Mathf.Lerp(bouncePosition.y, targetY, dataManager.fpsData.bounceFrequency * Time.deltaTime);
            }
        }

        #endregion

        #region RTS

        //RTS only
        Vector3 dragVel;
        /// <summary>
        /// Updates the position of the camera based on the pan input value.
        /// Updates the camera's forward based on the rotation of the camera.
        /// </summary>
        void PanWorld()
        {
            //targetPos = transform.position;

            if (dataManager.rtsData.pos.invertPan)
                panDirection = -1;
            else
                panDirection = 1;

            if (panInputValue > 0)
            {
                targetPos += transform.right * look.x * dataManager.rtsData.pos.panSmooth * panDirection * Time.deltaTime;
                targetPos += Vector3.Cross(transform.right, Vector3.up) * look.y * dataManager.rtsData.pos.panSmooth * panDirection * Time.deltaTime;
            }

            transform.position = Vector3.SmoothDamp(transform.position, targetPos + destination - boundsOffset + transform.TransformDirection(shakeOffset), ref dragVel, dataManager.rtsData.pos.panDrag);
        }

        //RTS only
        /*void PanWorldMobile()
        {
            targetPos = transform.position;

            if (dataManager.rtsData.pos.invertPan)
                panDirection = -1;
            else
                panDirection = 1;

            if (singleTouch.TouchInput.phase == TouchPhase.Moved &&
                Input.touchCount == 1)
            {
                targetPos += transform.right * singleTouch.DragInput().x * dataManager.rtsData.pos.panSmooth * panDirection * Time.deltaTime;
                targetPos += Vector3.Cross(transform.right, Vector3.up) * singleTouch.DragInput().y * dataManager.rtsData.pos.panSmooth * panDirection * Time.deltaTime;
            }
            transform.position = Vector3.SmoothDamp(transform.position, targetPos + destination - boundsOffset + transform.TransformDirection(shakeOffset), ref dragVel, dataManager.rtsData.pos.panDrag);
        }*/

        //RTS only
        RaycastHit hit;
        Ray ray;
        Vector3 newDestination;
        /// <summary>
        /// Maintain camera distance from the ground using the groundLayer mask
        /// </summary>
        void HandleCameraDistance()
        {
            ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, 1000, dataManager.rtsData.groundLayer))
            {
                newDestination = Vector3.Normalize(transform.position - hit.point) * dataManager.rtsData.pos.distanceFromGround;
                newDestination += hit.point;
                newDestination -= transform.position;
                destination = Vector3.SmoothDamp(destination, newDestination, ref camVel, 0.03f);
            }
        }

        //RTS only
        /// <summary>
        /// Update the camera's distance from the ground based on the current zoom input value
        /// </summary>
        void RTSZoom()
        {
            dataManager.rtsData.pos.newDistance += -zoomInput * dataManager.rtsData.pos.zoomStep;
            if (dataManager.rtsData.pos.newDistance > dataManager.rtsData.pos.minZoom)
                dataManager.rtsData.pos.newDistance = dataManager.rtsData.pos.minZoom;
            if (dataManager.rtsData.pos.newDistance < dataManager.rtsData.pos.maxZoom)
                dataManager.rtsData.pos.newDistance = dataManager.rtsData.pos.maxZoom;

            dataManager.rtsData.pos.distanceFromGround = Mathf.Lerp(dataManager.rtsData.pos.distanceFromGround, dataManager.rtsData.pos.newDistance, dataManager.rtsData.pos.zoomSmooth * Time.deltaTime);
        }

        //RTS only
        /// <summary>
        /// Update the rotation of the camera based on the current orbit input value.
        /// </summary>
        void RTSRotate()
        {
            if (mouseOrbitInput > 0 && dataManager.rtsData.orbit.allowOrbit)
            {
                dataManager.rtsData.orbit.yRotation += look.x * dataManager.rtsData.orbit.yOrbitSmooth * Time.deltaTime;
            }
            transform.rotation = Quaternion.Euler(dataManager.rtsData.orbit.xRotation + shakeRotationOffset.x, dataManager.rtsData.orbit.yRotation + shakeRotationOffset.y, 0);
        }

        //RTS only
        /*
        void RTSRotateMobile()
        {
            dataManager.rtsData.orbit.yRotation += mouseOrbitInput * dataManager.rtsData.orbit.yOrbitSmooth * 6.5f * Time.deltaTime;
        }*/

        //RTS only
        /// <summary>
        /// Updates the current state of collision events based on the boundary settings.
        /// </summary>
        void CalculateBoundaryCollision(ref Vector3 vec)
        {
            if (vec.x <= dataManager.rtsData.pos.minBoundary.x)
                lowerXIsColliding = true;
            else
                lowerXIsColliding = false;

            if (vec.x >= dataManager.rtsData.pos.maxBoundary.x)
                upperXIsColliding = true;
            else
                upperXIsColliding = false;

            if (vec.z <= dataManager.rtsData.pos.minBoundary.y)
                lowerZIsColliding = true;
            else
                lowerZIsColliding = false;

            if (vec.z >= dataManager.rtsData.pos.maxBoundary.y)
                upperZIsColliding = true;
            else
                upperZIsColliding = false;
        }

        //RTS only
        Vector3 boundsOffset;
        bool lowerXIsColliding = false, upperXIsColliding = false, lowerZIsColliding = false, upperZIsColliding = false;
        /// <summary>
        /// Clamps the camera to the min and max boundaries
        /// Using current collision events, prevents the camera from moving beyond the boundary
        /// </summary>
        void RTSHandleBounds()
        {
            boundsOffset = Vector3.zero;
            Vector3 tempPos = transform.position;
            tempPos.x = Mathf.Clamp(tempPos.x, dataManager.rtsData.pos.minBoundary.x, dataManager.rtsData.pos.maxBoundary.x);
            tempPos.z = Mathf.Clamp(tempPos.z, dataManager.rtsData.pos.minBoundary.y, dataManager.rtsData.pos.maxBoundary.y);

            transform.position = tempPos;

            CalculateBoundaryCollision(ref tempPos);

            if (lowerXIsColliding || upperXIsColliding || lowerZIsColliding || upperZIsColliding)
                targetPos = tempPos;
        }

        /// <summary>
        /// Clamps the camera to the min and max boundaries while considering the allowance of elasticity.
        /// </summary>
        void RTSHandleBoundsElastic()
        {
            boundsOffset = Vector3.zero;
            Vector3 tempPos = transform.position;
            tempPos.x = Mathf.Clamp(tempPos.x,
                                    dataManager.rtsData.pos.minBoundary.x - dataManager.rtsData.pos.boundaryElasticity,
                                    dataManager.rtsData.pos.maxBoundary.x + dataManager.rtsData.pos.boundaryElasticity);
            tempPos.z = Mathf.Clamp(tempPos.z,
                                    dataManager.rtsData.pos.minBoundary.y - dataManager.rtsData.pos.boundaryElasticity,
                                    dataManager.rtsData.pos.maxBoundary.y + dataManager.rtsData.pos.boundaryElasticity);

            CalculateBoundaryCollision(ref tempPos);

            //functions normally with extra padding on bounds
            boundsOffset = Vector3.zero;

            if (lowerXIsColliding || upperXIsColliding || lowerZIsColliding || upperZIsColliding)
                targetPos = tempPos;

            if (panInputValue == 0)
            {
                if (upperZIsColliding)
                    boundsOffset.z = transform.position.z - dataManager.rtsData.pos.maxBoundary.y;
                if (lowerZIsColliding)
                    boundsOffset.z = transform.position.z - dataManager.rtsData.pos.minBoundary.y;
                if (upperXIsColliding)
                    boundsOffset.x = transform.position.x - dataManager.rtsData.pos.maxBoundary.x;
                if (lowerXIsColliding)
                    boundsOffset.x = transform.position.x - dataManager.rtsData.pos.minBoundary.x;
            }
            else
            {
                transform.position = tempPos;
            }
        }

        #endregion

        #endregion

        #region Misc Functions

        /// <summary>
        /// Gathers zoom, orbit, and look input.
        /// </summary>
        void GetInput(CameraData.InputSet inputSet)
        {
            zoomInput = Input.GetAxisRaw(inputSet.ZOOM);
            try
            {
                if (Input.GetKey((KeyCode)inputSet.MOUSE_ORBIT))
                    mouseOrbitInput = 1;
                else
                    mouseOrbitInput = 0;
            }
            catch (System.Exception) { Debug.Log("Invalid orbit input. Please insert a valid value such as Fire1"); }

            look.x = Input.GetAxis("Mouse X");
            look.y = Input.GetAxis("Mouse Y");

            if (activeCam == CameraType.RTS)
            {
                GetMobileInput();
            }
        }

        
        //RTS only
        void GetMobileInput()
        {
            //if (!dataManager.rtsData.mobile.useMobileInput)
            //{
            try {
                if (Input.GetKey((KeyCode)dataManager.rtsData.input.PAN))
                    panInputValue = 1;
                else
                    panInputValue = 0;
            }
            catch (System.Exception) { Debug.Log("Invalid pan input. Please insert a valid value such as Fire1"); }
            try {
                if (Input.GetKey((KeyCode)dataManager.rtsData.input.ORBIT_Y))
                    mouseOrbitInput = 1;
                else
                    mouseOrbitInput = 0;
            }
            catch (System.Exception) { Debug.Log("Invalid orbit input. Please insert a valid value such as Fire2"); }

            zoomInput = Input.GetAxisRaw(dataManager.rtsData.input.ZOOM);
            //}
            /*else {
                singleTouch.UpdateTouchInput();
                multiTouch.UpdateMultiTouchInput();
                zoomInput = -multiTouch.PinchInput;
                mouseOrbitInput = multiTouch.SpinInput;
            }*/
        }

        /// <summary>
        /// Clamps vertical rotation based on min and max X rotation values.
        /// </summary>
        void CheckVerticalRotation(ref CameraData.OrbitSet orbitSet)
        {
            if (orbitSet.xRotation < orbitSet.minXRotation)
                orbitSet.xRotation = orbitSet.minXRotation;
            if (orbitSet.xRotation > orbitSet.maxXRotation)
                orbitSet.xRotation = orbitSet.maxXRotation;
        }

        /// <summary>
        /// Draws the desired and adjusted camera collision rays.
        /// </summary>
        void DrawDebugLines(CameraData.DebugSet debugSet)
        {
            for (int i = 0; i < 5; i++)
            {
                if (debugSet.drawDesiredCollisionLines)
                {
                    Debug.DrawLine(collision.collisionCheckPos, collision.desiredCameraClipPoints[i], Color.white);
                }
                if (debugSet.drawAdjustedCollisionLines)
                {
                    Debug.DrawLine(collision.collisionCheckPos, collision.adjustedCameraClipPoints[i], Color.green);
                }
            }
        }

        /// <summary>
        /// Reveals or hides the target based on the value of show.
        /// The target's color will not be affected unless it's materials use the Standard Shader or some variation of it
        /// </summary>
        void ToggleShowTarget(bool show)
        {
            if (targetVisible == show)
                return;
            targetVisible = show;
            SkinnedMeshRenderer[] skinnedRenderers = target.GetComponentsInChildren<SkinnedMeshRenderer>();
            MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();
            foreach (SkinnedMeshRenderer skin in skinnedRenderers)
                skin.enabled = targetVisible;
            foreach (MeshRenderer mesh in renderers)
                mesh.enabled = targetVisible;
        }

        Color _matColor;
        bool _targetFaded = false;
        /// <summary>
        /// Sets the target's opacity value based on the value of fade
        /// The target's color will not be affected unless it's materials use the Standard Shader or some variation of it
        /// </summary>
        void ToggleFadeTarget(bool fade)
        {
            if (_targetFaded == fade)
                return;
            _targetFaded = fade;
            SkinnedMeshRenderer[] skinnedRenderers = target.GetComponentsInChildren<SkinnedMeshRenderer>();
            MeshRenderer[] renderers = target.GetComponentsInChildren<MeshRenderer>();
            foreach (SkinnedMeshRenderer skin in skinnedRenderers){
                foreach(Material m in skin.materials)
                {
                    if (fade) SetMaterialBlendMode(m, "Fade"); 
                    else SetMaterialBlendMode(m, "Opaque");

                    _matColor = m.color;
                    _matColor.a = fade ? dataManager.rpgData.debug.targetFadeAlpha : 1.0f;
                    m.color = _matColor;
                }
            }
            foreach (MeshRenderer mesh in renderers){
                foreach(Material m in mesh.materials)
                {
                    if (fade) SetMaterialBlendMode(m, "Fade"); 
                    else SetMaterialBlendMode(m, "Opaque");

                    _matColor = m.color;
                    _matColor.a = fade ? dataManager.rpgData.debug.targetFadeAlpha : 1.0f;
                    m.color = _matColor;
                }
            }
        }

        /// <summary>
        /// Returns a shake sequence specified by _shakeName
        /// </summary>
        CameraShakeData.ShakeSequence FindShakeSequence(string _shakeName)
        {
            for (int i = 0; i < dataManager.shakeData.shakeSequences.Count; i++)
            {
                if (dataManager.shakeData.shakeSequences[i].name == _shakeName)
                    return dataManager.shakeData.shakeSequences[i];
            }
            return null;
        }

        Vector3 shakeOffset;
        Vector3 shakeRotationOffset;
        /// <summary>
        /// A process which updates the X and Y positions of the camera along curves over time.
        /// </summary>
        IEnumerator ShakeWithCurve(CameraShakeData.ShakeSequence sequence)
        {
            shaking = true;
            float time = 0;
            int posXKey = 0;
            int posYKey = 0;
            float shakeIntensity = sequence.intensity;
            float shakeDecay = sequence.decay;
            while (time < sequence.duration)
            {
                if (sequence.curve_posX.keys.Length > 0)
                    shakeOffset.x = sequence.curve_posX.keys[posXKey].value * shakeIntensity;
                if (sequence.curve_posY.keys.Length > 0)
                    shakeOffset.y = sequence.curve_posY.keys[posYKey].value * shakeIntensity;

                time += Time.deltaTime;
                posXKey++;
                posYKey++;
                if (posXKey >= sequence.curve_posX.length)
                    posXKey = 0;
                if (posYKey >= sequence.curve_posY.length)
                    posYKey = 0;
                if (shakeIntensity > shakeDecay)
                    shakeIntensity -= shakeDecay;
                if (_lookSpeed < 100)
                    _lookSpeed ++;
                yield return new WaitForSeconds(0f);
            }

            shaking = false;
            shakeOffset = Vector3.zero;
            shakeRotationOffset = Vector3.zero;
        }

        /// <summary>
        /// Accepts a material, which needs to use the Standard Shader or some variation of it.
        /// Sets the material blend mode to either opaque or fade
        /// </summary>
        void SetMaterialBlendMode(Material material, string BLEND_MODE)
        {
            switch (BLEND_MODE)
            {
                case "Opaque":
                    material.SetFloat("_Mode", 0);
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case "Fade":
                    material.SetFloat("_Mode", 2); //set to fade mode
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }

        #endregion

        #region System Variables

        public enum CameraType
        {
            RPG,
            FPS,
            RTS,
            TOP_DOWN
        }
        public CameraType activeCam, awakeCam;

        public Transform target;

        //data value set from CameraSettingsEditorWindow
        public CameraDataManager dataManager;

        //rpg
        CollisionHandler collision;
        Vector3 targetPos = Vector3.zero;
        Vector3 destination = Vector3.zero;
        Vector3 adjustedDestination = Vector3.zero;
        Vector3 camVel = Vector3.zero;
        float zoomInput, mouseOrbitInput;

        //fps
        Vector2 look = Vector2.zero;
        Quaternion targetRotation;
        Vector3 rotation = Vector3.zero;
        Vector2 bouncePosition = Vector2.zero;
        bool targetVisible = true;
        //This package uses a PlayerController. You will need to modify this based on your own controller.
        public PlayerController controller;

        //rts,
        //MultitouchHandler multiTouch;
        //TouchHandler singleTouch;
        float panInputValue;
        int panDirection;

        //shake
        bool shaking = false;

        #endregion

        #region API

        //Functions

        /// <summary>
        /// Modifies RPG Camera Offset
        /// Values locked [-10, 10]
        /// Permanently modifies data to 'x' and 'y'
        /// Data can be readjusted from the editor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void OffsetCamera(float x, float y)
        {
            if (!dataManager.rpgData)
            {
                Debug.LogWarning("Attempting to modify the camera's offset, but no RPG data could be found.");
                return;
            }
            x = Mathf.Clamp(x, -10, 10);
            y = Mathf.Clamp(y, -10, 10);
            if (smoothingOffset)
                StopCoroutine(SmoothOffsetParameters(x, y));
            StartCoroutine(SmoothOffsetParameters(x, y));
        }
        bool smoothingOffset = false;
        /// <summary>
        /// Helper process for smoothing offset commands so the camera rotation doesn't jerk
        /// </summary>
        IEnumerator SmoothOffsetParameters(float _x, float _y)
        {
            float currX = dataManager.rpgData.pos.targetPosOffset.x;
            float currY = dataManager.fpsData.pos.targetPosOffset.y;
            _lookSpeed = 0;
            while (Mathf.Abs(currX - _x) > 0.1f || Mathf.Abs(currY - _y) > 0.1f)
            {
                currX = Mathf.Lerp(currX, _x, 500 * Time.deltaTime);
                currY = Mathf.Lerp(currY, _y, 500 * Time.deltaTime);
                dataManager.rpgData.pos.targetPosOffset.x = currX;
                dataManager.rpgData.pos.targetPosOffset.y = currY;
                smoothingOffset = true;
                yield return new WaitForSeconds(0f);
            }
            smoothingOffset = false;
            dataManager.rpgData.pos.targetPosOffset.x = _x;
            dataManager.rpgData.pos.targetPosOffset.y = _y;
            dataManager.rpgData.pos.adjustedTargetPosOffset = dataManager.rpgData.pos.targetPosOffset;
        }

        /// <summary>
        /// Assigns the camera's focus to a new target 't'
        /// Attempts to assign a new PlayerController variable.
        /// If the new target does not have a PlayerController component, some functionality will be lost
        /// </summary>
        /// <param name="t"></param>
        public void SetCameraTarget(Transform t)
        {
            if (t == null)
            {
                Debug.LogError("Your camera needs a target. The attempted transform does not exist or has been destroyed");
                return;
            }
            target = t;
            try
            {
                controller = target.GetComponent<PlayerController>();
            }
            catch
            {
                Debug.LogWarning("The FPS camera receives information from the PlayerController.cs script. Your target does not use a PlayerController. Refer to docs for information on how to use the FPS camera with your own player controller.");
            }
        }

        float timeSinceLastCameraSwitch = 0;
        float switchTimeLimit = 1;
        /// <summary>
        /// Changes the type of camera being used
        /// Initializes the camera settings for 'cam'
        /// Hides the target if FPS is chosen
        /// </summary>
        /// <param name="cam"></param>
        public void SetCameraType(CameraType cam)
        {
            if (activeCam == cam)
                return;
            if (timeSinceLastCameraSwitch < switchTimeLimit)
                return;

            timeSinceLastCameraSwitch = 0;

            activeCam = cam;
            if (activeCam == CameraType.FPS)
            {
                ToggleShowTarget(false);
            }
            else {
                ToggleShowTarget(true);
            }

            switch (cam)
            {
                case CameraType.RPG: InitRPG(); break;
                case CameraType.FPS: InitFPS(); break;
                case CameraType.RTS: InitRTS(); break;
                case CameraType.TOP_DOWN: InitTopDown(); break;
            }
        }

        /// <summary>
        /// Modifies FPS Sensitivity
        /// Values locked [10, 500]
        /// Permanently modifies data to 'x' and 'y'
        /// Data can be readjusted from the editor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetSensitivity(float x, float y)
        {
            if (!dataManager.fpsData)
            {
                Debug.LogWarning("Attempting to modify the camera's sensitivity, but no FPS data could be found.");
                return;
            }
            x = Mathf.Clamp(x, 10, 500);
            y = Mathf.Clamp(y, 10, 500);
            dataManager.fpsData.XSensitivity = x;
            dataManager.fpsData.YSensitivity = y;
        }

        ///<summary>
        /// Sets a target distance for the current camera to move to.
        /// _dist is locked between the current camera's max and min zoom values [maxZoom, minZoom]
        ///</summary>
        public void SetDistance(float _dist)
        {
            CameraData _data = (activeCam == CameraType.RPG) ? dataManager.rpgData : 
                               (activeCam == CameraType.FPS) ? dataManager.fpsData : 
                               (activeCam == CameraType.RTS) ? dataManager.rtsData : 
                               (activeCam == CameraType.TOP_DOWN) ? dataManager.topDownData : dataManager.rpgData;
            _dist = Mathf.Clamp(_dist, _data.pos.maxZoom, _data.pos.minZoom);
            _data.pos.distanceFromTarget = _dist;      
            _data.pos.newDistance = _dist;
        }

        /// <summary>
        /// Sets the obstruction handler to be active based on the value '_active' that was passed.
        /// All components with the obstruction handler will be set to _active.
        /// If _active is set to false, all existing obstructions will fade in and be removed.
        /// Does nothing if obstruction data cannot be found.
        /// </summary>
        public void SetObstructionHandlerActive(bool _active)
        {
            if (dataManager.obstructionData == null)
            {
                Debug.LogWarning("Attempting to modify the obstruction handler, but no obstruction data could be found.");
                return;
            }

            dataManager.obstructionData.obstructionSet.active = _active;
        }

        /// <summary>
        /// Modifies orbit input based on the current active camera
        /// Permanently modifies data 'orbitInput'
        /// Data can be readjusted from the editor
        /// </summary>
        /// <param name="forCam"></param>
        /// <param name="input"></param>
        public void SetOrbitInput(CameraType forCam, CameraData.InputOption input)
        {
            if (forCam == CameraType.FPS)
            {
                Debug.LogWarning("Attempting to modify orbit input for FPS data, but the FPS camera does not utilize orbit input.");
                return;
            }
            switch (forCam)
            {
                case CameraType.RPG: dataManager.rpgData.input.MOUSE_ORBIT = input; break;
                case CameraType.RTS: dataManager.rtsData.input.MOUSE_ORBIT = input; break;
                case CameraType.TOP_DOWN: dataManager.topDownData.input.MOUSE_ORBIT = input; break;
            }
        }

        /// <summary>
        /// Modifies X orbit speed based on the current active camera
        /// Value locked [0.1, 50]
        /// Permanently modifies data 'xOrbitSpeed'
        /// Data can be readjusted from the editor
        /// </summary>
        /// <param name="forCam"></param>
        /// <param name="speed"></param>
        public void SetOrbitSpeedX(CameraType forCam, float speed)
        {
            speed = Mathf.Clamp(speed, 0.1f, 50);
            if (forCam == CameraType.FPS)
            {
                Debug.LogWarning("Attempting to modify orbitSpeedX for FPS data, but the FPS camera does not utilize orbitSpeedX.");
                return;
            }
            if (forCam == CameraType.RTS || forCam == CameraType.TOP_DOWN)
            {
                Debug.LogWarning("Attempting to modify orbitSpeedX for RTS or TopDown data, but the RTS or TopDown camera does not utilize orbitSpeedX.\nDid you mean to use SetOrbitSpeedY()?");
                return;
            }
            if (!dataManager.rpgData)
            {
                Debug.LogWarning("Attempting to modify orbitSpeedX for RPG camera, but no RPG data could be found.");
                return;
            }

            dataManager.rpgData.orbit.xOrbitSmooth = speed;
        }

        /// <summary>
        /// Modifies Y orbit speed based on the current active camera
        /// Value locked [0.1, 50]
        /// Permanently modifies data 'yOrbitSpeed'
        /// Data can be readjusted from the editor
        /// </summary>
        /// <param name="forCam"></param>
        /// <param name="speed"></param>
        public void SetOrbitSpeedY(CameraType forCam, float speed)
        {
            speed = Mathf.Clamp(speed, 0.1f, 50);
            if (forCam == CameraType.FPS)
            {
                Debug.LogWarning("Attempting to modify orbitSpeedY for FPS data, but the FPS camera does not utilize orbitSpeedY.");
                return;
            }
            switch (forCam)
            {
                case CameraType.RPG:
                    if (!dataManager.rpgData)
                    {
                        Debug.LogWarning("Attempting to modify orbitSpeedY for RPG camera, but no RPG data could be found.");
                        return;
                    }
                    dataManager.rpgData.orbit.yOrbitSmooth = speed;
                    break;
                case CameraType.RTS:
                    if (!dataManager.rtsData)
                    {
                        Debug.LogWarning("Attempting to modify orbitSpeedY for RTS camera, but no RTS data could be found.");
                        return;
                    }
                    dataManager.rtsData.orbit.yOrbitSmooth = speed;
                    break;
                case CameraType.TOP_DOWN:
                    if (!dataManager.topDownData)
                    {
                        Debug.LogWarning("Attempting to modify orbitSpeedY for TopDown camera, but no TopDown data could be found.");
                        return;
                    }
                    dataManager.topDownData.orbit.yOrbitSmooth = speed;
                    break;
            }
        }

        /// <summary>
        /// Shakes the camera based on shake data specified by '_shakeName'
        /// Will exit upon failing to locate shake information
        /// </summary>
        /// <param name="_shakeName"></param>
        public void ShakeCamera(string _shakeName)
        {
            if (dataManager.shakeData == null)
            {
                Debug.LogWarning("Attempt to shake camera failed. No shake data could be found.");
                return;
            }
            CameraShakeData.ShakeSequence seq = FindShakeSequence(_shakeName);
            if (seq != null)
            {
                StopAllCoroutines();
                StartCoroutine(ShakeWithCurve(seq));
            }
            else
            {
                Debug.LogWarning("Attempting to shake camera. Shake data named '" + _shakeName + "' does not exist.");
            }
        }

        //Properties

        /// <summary>
        /// Get
        /// Returns the camera currently in use by the camera control system.
        /// </summary>
        public CameraType activeCamera { get { return activeCam; } }

        /// <summary>
        /// Get/Set
        /// Modifies FPS Bounce Frequency
        /// Value locked [0.01f, 10]
        /// Permanently modifies 'bounce frequency' data
        /// Data can be readjusted from the editor
        /// </summary>
        public float bounceFrequency
        {
            get {
                if (!dataManager.fpsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's bounce frequency, but no FPS data could be found.");
                    return 0;
                }
                return dataManager.fpsData.bounceFrequency;
            }
            set
            {
                if (!dataManager.fpsData)
                    Debug.LogWarning("Attempting to modify the camera's bounce frequency, but no FPS data could be found.");
                
                value = Mathf.Clamp(value, 0.01f, 10);
                dataManager.fpsData.bounceFrequency = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies RPG Camera Offset X
        /// Value locked [-10, 10]
        /// Permanently modifies 'offset x' data
        /// Data can be readjusted from the editor
        /// </summary>
        public float cameraOffsetX
        {
            get {
                if (!dataManager.rpgData)
                {
                    Debug.LogWarning("Attempting to modify the camera's offset, but no RPG data could be found.");
                    return 0;
                }
                return dataManager.rpgData.pos.targetPosOffset.x;
            }
            set
            {
                if (!dataManager.rpgData)
                    Debug.LogWarning("Attempting to modify the camera's offset, but no RPG data could be found.");
                
                value = Mathf.Clamp(value, -10, 10);
                dataManager.rpgData.pos.targetPosOffset.x = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies RPG Camera Offset Y
        /// Value locked [-10, 10]
        /// Permanently modifies 'offset y' data
        /// Data can be readjusted from the editor
        /// </summary>
        public float cameraOffsetY
        {
            get {
                if (!dataManager.rpgData)
                {
                    Debug.LogWarning("Attempting to modify the camera's offset, but no RPG data could be found.");
                    return 0;
                }
                return dataManager.rpgData.pos.targetPosOffset.y;
            }
            set
            {
                if (!dataManager.rpgData)
                    Debug.LogWarning("Attempting to modify the camera's offset, but no RPG data could be found.");
                
                value = Mathf.Clamp(value, -10, 10);
                dataManager.rpgData.pos.targetPosOffset.y = value;
            }
        }

        /// <summary>
        /// Get
        /// Returns true if the camera is in a collision state.
        /// </summary>
        public bool isColliding{ get { return collision.colliding; } }

        /// <summary>
        /// Get/Set
        /// Toggles RTS camera invertPan
        /// Permanently modifies 'invertPan' data
        /// Data can be readjusted from the editor
        /// </summary>
        public bool invertPan
        {
            get {
                if (!dataManager.rtsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's pan settings, but no RTS data could be found.");
                    return false;
                }
                return dataManager.rtsData.pos.invertPan;
            }
            set {
                if (!dataManager.rpgData)
                    Debug.LogWarning("Attempting to modify the camera's pan settings, but no RTS data could be found.");

                dataManager.rtsData.pos.invertPan = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies time taken for FPS camera to look
        /// Value locked [-0.01, 30]
        /// Permanently modifies 'lookSpeed' data
        /// Data can be readjusted from the editor
        /// </summary>
        public float lookSpeed
        {
            get {
                if (!dataManager.fpsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's fps lookSpeed, but no FPS data could be found.");
                    return 0;
                }
                return dataManager.fpsData.smoothLookTime;
            }
            set
            {
                if (!dataManager.fpsData)
                    Debug.LogWarning("Attempting to modify the camera's fps lookSpeed, but no FPS data could be found.");

                value = Mathf.Clamp(value, 0.01f, 30);
                dataManager.fpsData.smoothLookTime = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies the minimum X-Z position of the RTS camera
        /// Permanently modifies 'minBoundary' data
        /// Data can be readjusted from the editor
        /// </summary>
        public Vector2 minBounds
        {
            get {
                if (!dataManager.rtsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's boundary settings, but no RTS data could be found.");
                    return Vector2.zero;
                }
                return dataManager.rtsData.pos.minBoundary;
            }
            set {
                if (!dataManager.rtsData)
                    Debug.LogWarning("Attempting to modify the camera's boundary settings, but no RTS data could be found.");
                
                dataManager.rtsData.pos.minBoundary = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies the maximum X-Z position of the RTS camera
        /// Permanently modifies 'maxBoundary' data
        /// Data can be readjusted from the editor
        /// </summary>
        public Vector2 maxBounds
        {
            get {
                if (!dataManager.rtsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's boundary settings, but no RTS data could be found.");
                    return Vector2.zero;
                }
                return dataManager.rtsData.pos.maxBoundary;
            }
            set {
                if (!dataManager.rtsData)
                    Debug.LogWarning("Attempting to modify the camera's boundary settings, but no RTS data could be found.");

                dataManager.rtsData.pos.maxBoundary = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies pan input for the RTS camera
        /// Permanently modifies 'panInput' data
        /// Data can be readjusted from the editor
        /// </summary>
        public CameraData.InputOption panInput
        {
            get {
                if (!dataManager.rtsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's input settings, but no RTS data could be found.");
                    return CameraData.InputOption.LEFT_MOUSE;
                }
                return dataManager.rtsData.input.PAN;
            }
            set {
                if (!dataManager.rtsData)
                    Debug.LogWarning("Attempting to modify the camera's input settings, but no RTS data could be found.");

                dataManager.rtsData.input.PAN = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies pan speed for the RTS camera
        /// Value locked [0.1, 500]
        /// Permanently modifies 'panSpeed' data
        /// Data can be readjusted from the editor
        /// </summary>
        public float panSpeed
        {
            get {
                if (!dataManager.rtsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's pan settings, but no RTS data could be found.");
                    return 0;
                }
                return dataManager.rtsData.pos.panSmooth;
            }
            set
            {
                if (!dataManager.rtsData)
                    Debug.LogWarning("Attempting to modify the camera's pan settings, but no RTS data could be found.");
                 
                value = Mathf.Clamp(value, 0.1f, 500);
                dataManager.rtsData.pos.panSmooth = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies responsiveness to FPS mouse input
        /// Value locked [10, 500]
        /// Permanently modifies 'XSensitivity' data
        /// Data can be readjusted from the editor
        /// </summary>
        public float sensitivityX
        {
            get {
                if (!dataManager.fpsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's fps sensitivity, but no FPS data could be found.");
                    return 0;
                }
                return dataManager.fpsData.XSensitivity;
            }
            set
            {
                if (!dataManager.fpsData)
                    Debug.LogWarning("Attempting to modify the camera's fps sensitivity, but no FPS data could be found.");
                
                value = Mathf.Clamp(value, 10, 500);
                dataManager.fpsData.XSensitivity = value;
            }
        }

        /// <summary>
        /// Get/Set
        /// Modifies responsiveness to FPS mouse input
        /// Value locked [10, 500]
        /// Permanently modifies 'YSensitivity' data
        /// Data can be readjusted from the editor
        /// </summary>
        public float sensitivityY
        {
            get {
                if (!dataManager.fpsData)
                {
                    Debug.LogWarning("Attempting to modify the camera's fps sensitivity, but no FPS data could be found.");
                    return 0;
                }
                return dataManager.fpsData.YSensitivity;
            }
            set
            {
                if (!dataManager.fpsData)
                    Debug.LogWarning("Attempting to modify the camera's fps sensitivity, but no FPS data could be found.");
                
                value = Mathf.Clamp(value, 10, 500);
                dataManager.fpsData.YSensitivity = value;
            }
        }

        #endregion

    }
}