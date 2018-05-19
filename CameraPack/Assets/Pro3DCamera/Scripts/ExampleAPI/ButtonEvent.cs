using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEvent : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
	
	public Color restColor, hoverColor, clickColor;
	public Image _image;
	RectTransform _rect;

	protected void Init()
	{
		_rect = _image.GetComponent<RectTransform>();
		_image.color = restColor;
		_rect.localScale = Vector3.right * 1 + Vector3.up * 1;
	}

	public virtual void OnButtonClick(){}

	public void OnPointerClick(PointerEventData ped)
	{
		OnButtonClick();
	}

	public void OnPointerUp(PointerEventData ped)
	{
		_image.color = restColor;
	}

	public void OnPointerDown(PointerEventData ped)
	{
		_image.color = clickColor;	
	}

	public void OnPointerEnter(PointerEventData ped)
	{
		_image.color = hoverColor;
		_rect.localScale = Vector3.right * 1 + Vector3.up * 1.55f;
	}

	public void OnPointerExit(PointerEventData ped)
	{
		_image.color = restColor;
		_rect.localScale = Vector3.right * 1 + Vector3.up * 1;
	}
}
