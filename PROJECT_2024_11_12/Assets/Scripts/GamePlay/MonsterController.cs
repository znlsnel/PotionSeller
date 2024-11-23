using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MonsterController : MonoBehaviour
{

	[SerializeField] float _attackRange = 1.0f;
	[SerializeField] float _monsterSpeed = 1.0f;
	 
	Animator _anim;
        Rigidbody _rigid;
	NavMeshAgent _agent;

        GameObject _target;

	UnityEvent _onRelase = new UnityEvent();

        private void Awake()
        {
                _anim = GetComponent<Animator>();
		_rigid = GetComponent<Rigidbody>();
		_agent = GetComponent<NavMeshAgent>();
	}

        void FixedUpdate()
        {
		if (_target != null)
		        AttackMove();
	}

	public void InitMonster(Vector3 pos, Action relaseListener)
	{
		if (_onRelase != null) 
			_onRelase.RemoveAllListeners();
		_onRelase.AddListener(()=>relaseListener.Invoke()); 

		gameObject.SetActive(true);
		transform.position = pos;
	}

        void AttackMove()
        {
                transform.LookAt(_target.transform.position); 

                Vector3 dir = (_target.transform.position - transform.position);
		float dist = dir.magnitude;

		bool attack = dist < _attackRange;
		_anim.SetBool("move", !attack);
		_anim.SetBool("attack", attack);

		_agent.isStopped = attack; 

		if (!attack)
			_agent.SetDestination(_target.transform.position);
	}
        
	private void OnTriggerEnter(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
                if (pc == null)
                        return;


		_target = other.gameObject;
	}

        private void OnTriggerExit(Collider other)
        {
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) 
			return;
		_target = null;
		_agent.isStopped = true;

		_anim.SetBool("attack", false);
		_anim.SetBool("move", false); 
	}
	
}
