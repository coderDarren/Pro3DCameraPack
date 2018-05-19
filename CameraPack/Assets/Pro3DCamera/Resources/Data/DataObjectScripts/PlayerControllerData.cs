using UnityEngine;
using System.Collections;

namespace Pro3DCamera
{
	[CreateAssetMenu()]
	public class PlayerControllerData : ScriptableObject {

		[System.Serializable]
	    public class MoveSettings
	    {
	        public float forwardVel = 12;
	        public float rotateVel = 3;
	        public float jumpVel = 5;
	    }

	    [System.Serializable]
	    public class PhysSettings
	    {
	        public float downAccel = 0.3f;
	        public float runAngleLimit = 120;
	        public Transform capsuleTop, capsuleBottom;
	        public float capsuleRadius;
	        public LayerMask ground;
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
	    
	}
}
