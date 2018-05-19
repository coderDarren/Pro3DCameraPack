using UnityEngine;
using System.Collections;
using Pro3DCamera; //use this to access the Camera Control API

public class ShakeCameraButton : ButtonEvent {

	//pass shakeID as the name of the shake sequence you defined from the editor
	public string shakeID;
	CameraControl _camControl;

	void Start()
	{
		Init();
		_camControl = Camera.main.GetComponent<CameraControl>();
	}

	public override void OnButtonClick()
	{
		_camControl.ShakeCamera(shakeID);
	}
}
