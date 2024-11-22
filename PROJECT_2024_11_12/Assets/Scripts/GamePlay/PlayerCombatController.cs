using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
	HashSet<MonsterController> _monsters = new HashSet<MonsterController>();
	Animator _anim;

	int _upperBodyIdx = -1;

	GameObject _lookTarget;
	public bool _isAttacking { get { return _monsters.Count > 0; }}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_upperBodyIdx = _anim.GetLayerIndex("Upper Body");
		StartCoroutine(LookAtMonster());
	}

	private void OnTriggerEnter(Collider other)
	{
		
		MonsterController mc = other.GetComponent<MonsterController>();
		if (mc != null)
		{
			_anim.SetLayerWeight(_upperBodyIdx, 1.0f);
			_anim.SetBool("attacking", true);

			_monsters.Add(mc);
			if (_monsters.Count == 1)
				_lookTarget = other.gameObject;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		MonsterController mc = other.GetComponent<MonsterController>();
		if (mc != null ) 
		{
			_monsters.Remove(mc);
			if (_monsters.Count == 0)
			{
				_anim.SetLayerWeight(_upperBodyIdx, 0.0f);
				_anim.SetBool("attacking", false);
				_lookTarget = null;
			}
		}
	}

	private void FixedUpdate()
	{
		if (_isAttacking) 
			transform.LookAt(_lookTarget.transform); 
		
	}


	IEnumerator LookAtMonster()
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
				_lookTarget = target.gameObject;

			yield return new WaitForSeconds(0.3f);
		}
	}
}