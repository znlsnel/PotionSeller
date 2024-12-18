
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : HealthEntity
{
	PickupManager _pickupManager;
	SendItemManager _sendItemManager;

	PlayerCombatController _combatCtrl;
        JoystickController _joystick;
        Rigidbody _rigid;
	Animator _anim;

	InputAction _touchMove;
	InputAction _touch;

	[SerializeField] GameObject _porter; 
	[SerializeField] Transform _genPos; 
	[SerializeField] float _moveSpeed = 5.0f;
	[Space(10)]
	[SerializeField] List<AudioClip> _footStepAudios = new List<AudioClip>();
	[SerializeField] AudioClip _initSound;
	[SerializeField] AudioClip _dieSound;
	[SerializeField] float _footVolume = 1.0f;

	Vector2 _touchStartPos;

	Vector3 _lookTarget;
	float _lookTime;

	[NonSerialized] public UnityEvent _onDead = new UnityEvent();

	public override int MaxHP()
	{
		return _initHp * DataBase.instance._hp.GetValue() / 100;
	}
	protected override void Awake()
	{
		base.Awake ();	

		_rigid = GetComponent<Rigidbody>();
		_anim = GetComponent<Animator>();
		_combatCtrl = GetComponent<PlayerCombatController>();
		_pickupManager = _porter.GetComponent<PickupManager>();
		_sendItemManager = _porter.GetComponent<SendItemManager>();

		HP = MaxHP();
		DataBase.instance._hp._onChangedLevel.AddListener(() =>
		{
			HP = MaxHP(); 
		});
	}
	void Start()
        {
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
		//_anim.SetBool("moving", _rigid.linearVelocity.magnitude > 0.1f);


	}

	void OnMove() 
        {
		if (UIHandler.instance._isOpenUI || isDead)
			return;

		Vector2 touchPos = _touchMove.ReadValue<Vector2>();
		Vector2 dir = (touchPos - _touchStartPos).normalized;

		float speed = _moveSpeed * DataBase.instance._speed.GetValue() / 100; 
		Vector3 movePos = _rigid.position + new Vector3(dir.x, 0.0f, dir.y) * Time.fixedDeltaTime * speed;

		_rigid.MovePosition(movePos);
		_joystick.UpdateJoystick(_touchStartPos, touchPos); 
		
		if (!_combatCtrl._isAttacking && isDead == false) 
			LookAt(movePos);
	}
	 
	public void LookAt(Vector3 target, float speed = 720)
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
		if (UIHandler.instance._isOpenUI || isDead)
			return;

		_anim.SetBool("moving", true); 
		//_joystick.EnableJoystickUI();
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
		HP = _initHp * DataBase.instance._hp.GetValue() / 100;
		transform.position = _genPos.position;
		_pickupManager.SetActive(true);
		AudioManager.instance.PlayAudioClip(_initSound); 
	}

	public void AE_FootStep()
	{
		AudioManager.instance.PlayAudioClip(_footStepAudios, _footVolume);  
	}

	public override void OnDead()
	{
		_pickupManager.SetActive(false);
		_onDead?.Invoke();
		_anim.SetBool("die", true);
		_combatCtrl.SetActiveUpperBody(false);
		_pickupManager.ClearItem();
		AudioManager.instance.PlayAudioClip(_dieSound);
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

	public PickupManager GetPickupManager()
	{
		return _pickupManager;
	}

	public SendItemManager GetSendItemManager()
	{
		return _sendItemManager;
	}
} 
 