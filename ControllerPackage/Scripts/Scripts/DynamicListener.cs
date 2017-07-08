using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DynamicListener : MonoBehaviour {
	
	public string objectListeningTag = "Enter GameObject's tag";
	public bool thisIsListener = false;
	public bool parameter = false;
	public string sendMessage = "Enter GameObject's method name";
	public string messageParameter;
	
	Button b;
	GameObject objectListening;
	
	void Start()
	{
		b = GetComponent<Button>();
		GetObjectListening();
	}
	
	void GetObjectListening()
	{
		if (thisIsListener)
			objectListening = gameObject;
		else //if we are listening to another object
			objectListening = GameObject.FindGameObjectWithTag(objectListeningTag);
		
		if (objectListening)
			SetListener();
	}
	
	void SetListener()
	{
		if (b)
		{
			if (!parameter)
				b.onClick.AddListener(() => objectListening.SendMessage(sendMessage));
			else
				b.onClick.AddListener(() => objectListening.SendMessage(sendMessage, messageParameter));
		}
		else
			Debug.LogError("Dynamic Listeners belong on buttons.");
		
	}
}
