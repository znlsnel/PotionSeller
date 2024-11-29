using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class MonsterController : HealthEntity
{
	[Space(10)]
	[SerializeField] float _attackRange = 1.0f;
	[SerializeField] GameObject _dropItem;
	[SerializeField] int _damage = 10;
	  
	Animator _anim;
        Rigidbody _rigid;
	NavMeshAgent _agent;
	MonsterSpawner _ms;

        GameObject _target;

	UnityEvent _onRelase = new UnityEvent();
	public UnityEvent _onDead = new UnityEvent();
        protected override void Awake()
        {
		base.Awake();
                _anim = GetComponent<Animator>();
		_rigid = GetComponent<Rigidbody>();
		_agent = GetComponent<NavMeshAgent>();
	}

        void FixedUpdate()
        {
		if (_ms.isPlayerIn &&  _target != null)
		        AttackMove();
		
	}

	public void InitMonster(MonsterSpawner ms, Vector3 pos, Action relaseListener)
	{
		_ms = ms;
		HP = _initHp;
		if (_onRelase != null) 
			_onRelase.RemoveAllListeners();

		_onRelase.AddListener(()=>relaseListener.Invoke()); 

		_anim.SetBool("die", false);
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

		if (_ms.isPlayerIn == false || _target.GetComponent<PlayerController>().isDead)
		{
			_target = null;
			_anim.SetBool("move", false);
			_anim.SetBool("attack", false);
		}
	}


	public override void TargetEnter(GameObject go)
	{
		PlayerController pc = go.GetComponent<PlayerController>();
                if (pc == null)
                        return;

		_target = go;
	}

	public override void TargetExit(GameObject go)
	{
		PlayerController pc = go.GetComponent<PlayerController>();
		if (pc == null) 
			return;
		_target = null;
		_agent.isStopped = true;

		_anim.SetBool("attack", false);
		_anim.SetBool("move", false); 
	}

	public override void OnDead()
	{
		_agent.isStopped = true; 
		_target = null;

		_onDead?.Invoke();
		_onDead.RemoveAllListeners();
		_anim.SetBool("die", true);
	}

	public void AE_Attack() 
	{
		if (_target == null)
			return;

		Vector3 dir = (_target.transform.position - transform.position);
		float dist = dir.magnitude;

		if (dist < _attackRange)
			_target.GetComponent<HealthEntity>().OnDamage(gameObject, _damage);
	}
	 
	public void AE_Die()
	{ 
		GameObject go = ItemSpawner.instance.GetItem(_dropItem);

		Item item = go.GetComponent<Item>();
		item._onRelease.RemoveAllListeners();
		item._onRelease.AddListener(()=>ItemSpawner.instance.RelaseItem(_dropItem, go));

		go.transform.position = transform.position;
		  
		_onRelase?.Invoke();
	}
}
