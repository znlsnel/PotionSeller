using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
	HashSet<MonsterController> _monsters = new HashSet<MonsterController>();
	PlayerController _playerCtrl;
	Animator _anim;

	int _upperBodyIdx = -1;

	GameObject _lookTarget;
	public bool _isAttacking { get { return _monsters.Count > 0; }}
	public bool isInHuntZone = false;

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_upperBodyIdx = _anim.GetLayerIndex("Upper Body");
		_playerCtrl = GetComponent<PlayerController>();
		StartCoroutine(LookAtMonster());
		StartCoroutine(AttackCheck()); 
	}

	public void SetActiveUpperBody(bool on)
	{
		if (on)
			_anim.SetLayerWeight(_upperBodyIdx, 1.0f);
		else
			_anim.SetLayerWeight(_upperBodyIdx, 0.0f);

	}

	public void EnterMonster(GameObject monster)
	{
		MonsterController mc = monster.GetComponent<MonsterController>();
		if (mc != null)
		{

			_monsters.Add(mc);
			mc._onDead.AddListener(() => RemoveMonster(mc));

			if (_monsters.Count == 1)
				_lookTarget = monster;
		}
	}

	public void ExitMonster(GameObject monster)
	{
		MonsterController mc = monster.GetComponent<MonsterController>();
		if (mc != null)
		{
			RemoveMonster(mc);
		} 
	}

	void RemoveMonster(MonsterController mc)
	{
		_monsters.Remove(mc);
		if (_monsters.Count == 0)
		{
			_lookTarget = null;
		}
	}

	private void FixedUpdate()
	{
		if (_isAttacking && _playerCtrl.isDead == false)
		{
			if ((_lookTarget.transform.position - transform.position).magnitude > 0.1)
				transform.LookAt(_lookTarget.transform);
			 
		}

	}

	public void AE_Attack()
	{
		foreach(MonsterController mc in _monsters)
		{
			mc.OnDamage(gameObject, 1);  
		}
	}

	IEnumerator LookAtMonster()
	{
		while (true)
		{ 
			MonsterController target = null;
			float dist = -1f;
			foreach (MonsterController mc in _monsters)
			{
			//	if (mc == null)
			///		continue; 

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
				_lookTarget = target.gameObject;

			yield return new WaitForSeconds(0.3f);
		}
	}

	IEnumerator AttackCheck()
	{
		while (true)
		{
			SetActiveUpperBody(_monsters.Count > 0 && isInHuntZone && _playerCtrl.isDead == false);
			_anim.SetBool("attacking", _monsters.Count > 0 && isInHuntZone);
			yield return new WaitForSeconds(0.3f); 
		} 
	}
}
