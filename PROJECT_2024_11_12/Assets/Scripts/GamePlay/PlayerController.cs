using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : HealthEntity
{
	PickupManager _pickupManager;
	PlayerCombatController _combatCtrl;
        JoystickController _joystick;
        Rigidbody _rigid;
	Animator _anim;

	InputAction _touchMove;
	InputAction _touch;

	[SerializeField] Transform _genPos; 
	[SerializeField] float _moveSpeed = 5.0f;

        Vector2 _touchStartPos;
	public bool isDead { get{ return HP == 0; } }

	Vector3 _lookTarget;
	float _lookTime;

	public UnityEvent _onDead = new UnityEvent();

	void Start()
        {
		_rigid = GetComponent<Rigidbody>();
		_anim = GetComponent<Animator>();
		_combatCtrl = GetComponent<PlayerCombatController>();
		_pickupManager = GetComponent<PickupManager>();
		_touch = InputManager.instance.Touch;
		_touchMove = InputManager.instance.TouchMove;
		_joystick = UIHandler.instance._joystick;

		_touch.performed += StartTouch;
		_touch.canceled += EndtTouch;
	} 
        
	private void FixedUpdate()
	{                 
		if (isDead == false &&  _touch.ReadValue<float>() != 0) 
                        OnMove();

	}

	void OnMove()
        {
		Vector2 touchPos = _touchMove.ReadValue<Vector2>();
		Vector2 dir = (touchPos - _touchStartPos).normalized;
		Vector3 movePos = _rigid.position + new Vector3(dir.x, 0.0f, dir.y) * Time.fixedDeltaTime * _moveSpeed;

		_rigid.MovePosition(movePos);
		_joystick.UpdateJoystick(_touchStartPos, touchPos); 
		
		if (!_combatCtrl._isAttacking) 
			LookAt(movePos);
	}
	 
	public void LookAt(Vector3 target, float speed = 360)
	{
		Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

		// 현재 회전에서 목표 회전으로 부드럽게 회전
		transform.rotation = Quaternion.RotateTowards(
		    transform.rotation,
		    targetRotation,
		    speed * Time.deltaTime
		);
	}

	void StartTouch(InputAction.CallbackContext context)
	{
		_anim.SetBool("moving", true);
		_joystick.EnableJoystickUI();
		_touchStartPos = _touchMove.ReadValue<Vector2>();
	}
	void EndtTouch(InputAction.CallbackContext context)
	{
		_anim.SetBool("moving", false);
		_joystick.DisableJoystickUI();
	}

	void InitPlayer()
	{
		_anim.SetBool("die", false);
		HP = _initHp;
		transform.position = _genPos.position;
		_pickupManager.SetActive(true);
	}

	public override void OnDead()
	{
		_pickupManager.SetActive(false);
		_onDead?.Invoke();
		_anim.SetBool("die", true);
		_combatCtrl.SetActiveUpperBody(false);
		_pickupManager.ClearItem();

		Utils.instance.SetTimer(() => {
			InitPlayer();
		}, 2.0f); 
	}

	public override void TargetEnter(GameObject go)
	{
		_combatCtrl.EnterMonster(go);
	}

	public override void TargetExit(GameObject go)
	{
		_combatCtrl.ExitMonster(go);
	}
} 
 