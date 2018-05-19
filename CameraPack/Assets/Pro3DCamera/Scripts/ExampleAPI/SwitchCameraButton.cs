using UnityEngine;
using System.Collections;
using Pro3DCamera; //use this to access the Camera Control API

public class SwitchCameraButton : ButtonEvent {

	//The camera type you want to set the camera to transition to
	public CameraControl.CameraType desiredCamType;
	CameraControl _camControl;

	void Start()
	{
		Init();
		_camControl = Camera.main.GetComponent<CameraControl>();
	}

	public override void OnButtonClick()
	{
		//Transition to a new camera type
		//Nothing will happen if desiredCamType is equal to the currently active camera
		_camControl.SetCameraType(desiredCamType);

		if (desiredCamType == CameraControl.CameraType.TOP_DOWN)
		{
			_camControl.SetObstructionHandlerActive(true); //activate the obstruction handler for all components
		}
		else {
			_camControl.SetObstructionHandlerActive(false); //deactivate the obstruction handler for all components
		}
	}
}
