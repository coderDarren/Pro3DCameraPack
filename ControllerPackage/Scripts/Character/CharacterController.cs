using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {

    [System.Serializable]
    public class MoveSettings
    {
        public float forwardVel = 12;
        public float rotateVel = 3;
        public float mouseRotateVel = 0.1f;
        public float jumpVel = 5;
        public float distToGrounded = 0.1f;
        public Transform[] groundCheckPoints;
        public LayerMask ground;
    }

    [System.Serializable]
    public class PhysSettings
    {
        public float downAccel = 0.3f;
    }

    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string FORWARD_AXIS = "Vertical";
        public string TURN_AXIS = "Horizontal";
        public string JUMP_AXIS = "Jump";
    }

    public MoveSettings moveSetting = new MoveSettings();
    public PhysSettings physSetting = new PhysSettings();
    public InputSettings inputSetting = new InputSettings();

    Vector3 velocity = Vector3.zero;
    Vector3 terrainNormal = Vector3.zero;
    Quaternion targetRotation;
    Rigidbody rBody;
    public float forwardInput, turnInput, jumpInput;
    Vector3 previousMousePos = Vector3.zero;
    Vector3 currentMousePos = Vector3.zero;

    public Quaternion TargetRotation
    {
        get { return targetRotation; }
    }

    public bool Grounded()
    {
        foreach (Transform t in moveSetting.groundCheckPoints)
        {
            Debug.DrawLine(t.position, t.position + Vector3.down * moveSetting.distToGrounded, Color.green);
        }

        foreach (Transform t in moveSetting.groundCheckPoints)
        {
            Debug.DrawLine(t.position, t.position + Vector3.down * moveSetting.distToGrounded, Color.green);
            if (Physics.Raycast(t.position, Vector3.down, moveSetting.distToGrounded, moveSetting.ground))
                return true;
        }
        return false;
    }

    void Start()
    {
        targetRotation = transform.rotation;
        if (GetComponent<Rigidbody>())
            rBody = GetComponent<Rigidbody>();
        else
            Debug.LogError("The character needs a rigidbody.");

        forwardInput = turnInput = jumpInput = 0;
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis(inputSetting.FORWARD_AXIS); //interpolated
        turnInput = Input.GetAxis(inputSetting.TURN_AXIS); //interpolated
        jumpInput = Input.GetAxisRaw(inputSetting.JUMP_AXIS); //non-interpolated
    }

    void Update()
    {
        GetInput();
    }

    void FixedUpdate()
    {
        previousMousePos = currentMousePos;
        currentMousePos = Input.mousePosition;

        Run();
        Turn();
        velocity.x = 0; //cant be strafing
        Jump();

        rBody.velocity = transform.TransformDirection(velocity);
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
}
