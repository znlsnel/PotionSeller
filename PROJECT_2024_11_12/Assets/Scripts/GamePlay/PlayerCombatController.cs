using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;

using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
	[SerializeField] int _damage = 100;

	HashSet<MonsterController> _monsters = new HashSet<MonsterController>();

	PlayerController _playerCtrl;
	Animator _anim;
	Rigidbody _rigid;

	GameObject _lookTarget;

	int _lowerBodyIdx = -1;

	public bool _isAttacking { get { return _monsters.Count > 0; }}
	[NonSerialized] public bool isInHuntZone = false;


	private void Start()
	{
		_anim = GetComponent<Animator>();
		_rigid = GetComponent<Rigidbody>();
		_lowerBodyIdx = _anim.GetLayerIndex("Lower Body");


		_playerCtrl = GetComponent<PlayerController>();
		StartCoroutine(FindCloseMonster());
		StartCoroutine(AttackCheck()); 
	}

	bool prev = false;
	public void SetActiveUpperBody(bool on)
	{
		_anim.SetBool("attack", on);
		_anim.SetLayerWeight(_lowerBodyIdx, on ? 1.0f : 0.0f);
		prev = on; 
	}

	public void EnterMonster(GameObject monster)
	{
		MonsterController mc = monster.GetComponent<MonsterController>();
		if (mc != null)
		{
			_monsters.Add(mc);
			mc._hpBar.GetComponent<MonsterHpUI>().SetHpBar(true);
			mc._onDead.AddListener(() => RemoveMonster(mc));
			 
			if (_monsters.Count == 1)
				_lookTarget = monster;
		}
	}

	public void ExitMonster(GameObject monster)
	{
		MonsterController mc = monster.GetComponent<MonsterController>();
		if (mc != null)
			RemoveMonster(mc);

	} 

	void RemoveMonster(MonsterController mc)
	{
		_monsters.Remove(mc);
		mc._hpBar.GetComponent<MonsterHpUI>().SetHpBar(false);

		if (_monsters.Count == 0)
		{
			_lookTarget = null;
		}
	}


	private void FixedUpdate()
	{
		if (_isAttacking && _playerCtrl.isDead == false)
		{
			float dist = (_lookTarget.transform.position - transform.position).magnitude;
			if (dist > 0.1)
				_playerCtrl.LookAt(_lookTarget.transform.position);
		}
	} 
	 

	public void AE_Attack(int rate)
	{
		foreach(MonsterController mc in _monsters)
		{ 
			mc.OnDamage(gameObject, (_damage * rate) / 100);  
		}
	}

	public void AE_SetLowerBody(int active)
	{
		_anim.SetLayerWeight(_lowerBodyIdx, active == 1 ? 1.0f : 0.0f);
	} 

	IEnumerator FindCloseMonster()
	{
		while (true)
		{ 
			MonsterController target = null;
			float dist = -1f;
			foreach (MonsterController mc in _monsters)
			{

				if (target == null)
				{
					target = mc;
					dist = (target.transform.position - gameObject.transform.position).magnitude;
				}

				else
				{
					float nxtMstDist = (mc.transform.position - gameObject.transform.position).magnitude;
					if (nxtMstDist < dist)
					{
						target = mc;
						dist = nxtMstDist;
					}
				}
			}

			if (target != null)
			{	
				_lookTarget = target.gameObject;
			}

			yield return new WaitForSeconds(0.3f);
		}
	}

	IEnumerator AttackCheck()
	{
		while (true)
		{
			SetActiveUpperBody(_monsters.Count > 0 && isInHuntZone && _playerCtrl.isDead == false);
			yield return new WaitForSeconds(0.3f); 
		} 
	}
}
