using UnityEngine;
using System.Collections;
using Pro3DCamera;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour {

    public CameraDataManager dataManager;

    PlayerControllerData.MoveSettings moveSetting = new PlayerControllerData.MoveSettings();
    PlayerControllerData.PhysSettings physSetting = new PlayerControllerData.PhysSettings();
    PlayerControllerData.InputSettings inputSetting = new PlayerControllerData.InputSettings();

    Vector3 velocity = Vector3.zero;
    Quaternion targetRotation;
    Rigidbody rBody;
    [HideInInspector]
    public float forwardInput, turnInput, jumpInput;
    [HideInInspector]
    public bool firstPersonMode = false; //Set to true when using FPSCamera
    Vector3 forward; //forward movement direction based on ground normals4
    [HideInInspector]
    public float timeInAir = 0;
    CapsuleCollider _capsule;

    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    float airTime = 0;
    public bool Grounded()
    {
        if (Physics.CheckCapsule(transform.position + _capsule.center + Vector3.up * (_capsule.height / 2f), transform.position + _capsule.center - Vector3.up * (_capsule.height / 2f), physSetting.capsuleRadius))
        { 
            airTime = 0;
            return true;
        }
        else {
            airTime += Time.deltaTime;
        }
        if (airTime > 0.1f){
            return false;
        }
        else {
            return true;
        }
    }

    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
            rBody = GetComponent<Rigidbody>();
        else
            Debug.LogError("The character needs a rigidbody.");

        forwardInput = turnInput = jumpInput = 0;
        _capsule = GetComponent<CapsuleCollider>();
        firstPersonMode = true;

    }

    void GetSettings()
    {
        moveSetting = dataManager.playerControllerData.moveSetting;
        physSetting = dataManager.playerControllerData.physSetting;
        inputSetting = dataManager.playerControllerData.inputSetting;
    }

    void GetInput()
    {
        forwardInput = Mathf.Lerp(forwardInput, Input.GetAxis(inputSetting.FORWARD_AXIS), 5f * Time.deltaTime); //interpolated
        turnInput = Mathf.Lerp(turnInput, Input.GetAxis(inputSetting.TURN_AXIS), moveSetting.rotateVel * 2.5f * Time.deltaTime); //interpolated
        jumpInput = Input.GetAxisRaw(inputSetting.JUMP_AXIS); //non-interpolated
    }

    void Update()
    {
        GetInput();
        forward = GetForward();
        if (forward == Vector3.zero)
            forward = transform.forward;

        if (Grounded())
        {
            timeInAir = 0;
        }
        else {
            timeInAir += Time.deltaTime * 15;
            //rBody.velocity = transform.TransformDirection(velocity);
        }
        rBody.velocity = forward * velocity.z + transform.right * velocity.x + Vector3.up * velocity.y;
    }

    void FixedUpdate()
    {
        GetSettings();
        Run();

        if (!firstPersonMode)
        {
            Turn();
            velocity.x = 0; //cant be strafing
        }
        else
        {
            //continue updating target rotation (which is only updated in Turn())
            targetRotation = transform.rotation;
            Strafe();
        }
        
        Jump();

        //handle a maximum slope situation, so a player cannot run up walls
        if (forwardGroundAngle > physSetting.runAngleLimit){
            if (velocity.z > 0)
                velocity.z = 0;
        }
        if (backGroundAngle > physSetting.runAngleLimit){
            if (velocity.z < 0)
                velocity.z = 0;
        }
        if (rightGroundAngle > physSetting.runAngleLimit){
            if (velocity.x > 0)
                velocity.x = 0;
        }
        if (leftGroundAngle > physSetting.runAngleLimit){
            if (velocity.x < 0)
                velocity.x = 0;
        }


        //Debug.Log("Grounded: "+Grounded());
        //Debug.Log("Velocity: "+velocity);
        //Debug.Log("Rigidbody Velocity: "+rBody.velocity);
        //Debug.Log("Vertical Offset: " +verticalOffset);
    }


    void Run()
    {
        if (Mathf.Abs(forwardInput) > inputSetting.inputDelay)
        {
            //move
            velocity.z = moveSetting.forwardVel * forwardInput;
        }
        else
            //zero velocity
            velocity.z = 0;
    }

    void Strafe()
    {
        if (Mathf.Abs(turnInput) > inputSetting.inputDelay)
        {
            //move
            velocity.x = moveSetting.forwardVel * turnInput;
        }
        else
            //zero velocity
            velocity.x = 0;
    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) > inputSetting.inputDelay)
        {
            targetRotation *= Quaternion.AngleAxis(moveSetting.rotateVel * turnInput, Vector3.up);
        }
        transform.rotation = targetRotation;
    }

    void Jump()
    {
        if (jumpInput > 0 && Grounded())
        {
            //jump
            velocity.y = moveSetting.jumpVel;
        }
        else if (jumpInput == 0 && Grounded())
        {
            //zero out our velocity.y
            velocity.y = 0;
        }
        else
        {
            //decrease velocity.y
            velocity.y -= physSetting.downAccel;
        }
    }

    RaycastHit hit;
    float forwardGroundAngle = 0;
    float rightGroundAngle = 0;
    float backGroundAngle = 0;
    float leftGroundAngle = 0;
    Vector3 GetForward()
    {
        Vector3 f = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out hit)){
            f = Vector3.Cross(transform.right, hit.normal);
            forwardGroundAngle = Vector3.Angle(transform.forward, hit.normal);
            backGroundAngle = Vector3.Angle(-transform.forward, hit.normal);
            rightGroundAngle = Vector3.Angle(transform.right, hit.normal);
            leftGroundAngle = Vector3.Angle(-transform.right, hit.normal);
        }
        return f;
    }
}
