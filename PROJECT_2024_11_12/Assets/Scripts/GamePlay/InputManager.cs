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

		var inputSystem = Resources.Load<InputActionAsset>("Inputs/InputSystem");
		_gamePlayMap = inputSystem.FindActionMap("GamePlay");
		_touchMove = _gamePlayMap["TouchMove"];
		_touch = _gamePlayMap["Touch"];

		_touchMove.Enable();
		_touch.Enable();
	} 

}
