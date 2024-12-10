using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
	private InputActionMap _gamePlayMap;

	InputAction _touchMove;
	InputAction _touch;
	public InputAction TouchMove { get { return _touchMove; } private set { _touchMove = value; } }
	public InputAction Touch { get { return _touch; } private set { _touch = value; } }

	public override void Awake()  
	{
		//base.Awake();
		Application.targetFrameRate = 30;
		var inputSystem = Resources.Load<InputActionAsset>("Inputs/InputSystem");
		_gamePlayMap = inputSystem.FindActionMap("GamePlay");
		_touchMove = _gamePlayMap["TouchMove"];
		_touch = _gamePlayMap["Touch"];

		_touchMove.Enable();
		_touch.Enable();

		
	}

	//float time = 0.0f;
	//int cnt = 0;
	//int tCnt = 1;
	//private void Update()
	//{
	//	time += Time.deltaTime;
	//	cnt++;
	//	if (time >= 1.0f)
	//	{
	//		ScreenDebug.instance.DebugText($"[{tCnt++}] FPS : {cnt}");

	//		cnt = 0;
	//		time = 0.0f;
	//	}

	//}
}
