using UnityEngine;

public class JoystickController : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] RectTransform _joystickUI;
	[SerializeField] RectTransform _dirUI;
	[SerializeField] RectTransform _touchUI;


	void Start()
	{
		gameObject.SetActive(false);
	}

	public void UpdateJoystick(Vector2 startPos, Vector2 touchPos)
	{
		_joystickUI.localPosition = _joystickUI.parent.InverseTransformPoint(startPos);
		_touchUI.localPosition = _touchUI.parent.InverseTransformPoint(touchPos);
		 
		Vector3 dir = (_dirUI.parent.InverseTransformPoint(touchPos) - Vector3.zero).normalized;

		_dirUI.localPosition = dir * 30;

		if (gameObject.activeSelf == false)
			gameObject.SetActive(true); 
	}

	public void DisableJoystickUI()
	{
	//	Utils.instance.SetTimer(() => { gameObject.SetActive(false); }, 0.1f);
		gameObject.SetActive(false); 
	}

	public void EnableJoystickUI()
	{
		//	Utils.instance.SetTimer(() => { gameObject.SetActive(true); }, 0.1f);

		gameObject.SetActive(true);

	}
}
