using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{

	[SerializeField] float _attackRange = 1.0f;
	[SerializeField] float _monsterSpeed = 1.0f;
	 
	Animator _anim;
        Rigidbody _rigid;
	NavMeshAgent _agent;

        GameObject _target;

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
		   //     _rigid.MovePosition(transform.position + dir.normalized * Time.fixedDeltaTime * _monsterSpeed);

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
