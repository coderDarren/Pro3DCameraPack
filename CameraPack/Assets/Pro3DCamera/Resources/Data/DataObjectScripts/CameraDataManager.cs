using UnityEngine;
using System.Collections;

namespace Pro3DCamera {
	[CreateAssetMenu()]
	public class CameraDataManager : ScriptableObject {

	    public CameraData rpgData;
	    public CameraData fpsData;
	    public CameraData rtsData;
	    public CameraData topDownData;
	    public CameraShakeData shakeData;
	    public PlayerControllerData playerControllerData;
	    public ObstructionHandlerData obstructionData;
	}
}