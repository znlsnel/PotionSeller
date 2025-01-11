using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class MonsterController : HealthEntity
{
	[Space(10)]
	[SerializeField] float _attackRange = 1.0f;
	[SerializeField] GameObject _dropItem;
	[SerializeField] int _damage = 10;
	[SerializeField] int _dropRate = 100;
	   
	protected Animator _anim;
        Rigidbody _rigid;
	protected NavMeshAgent _agent;


        protected GameObject _target;

	protected UnityEvent _onRelase = new UnityEvent();
	[NonSerialized] public UnityEvent _onDead = new UnityEvent();

	protected BTNode _BTRoot;

        protected override void Awake()
        {
		base.Awake();
                _anim = GetComponent<Animator>();
		_rigid = GetComponent<Rigidbody>();
		_agent = GetComponent<NavMeshAgent>();

		SetBehaviorTree();
	}

	public virtual void SetBehaviorTree()
	{
		_BTRoot = new Selector(new List<BTNode>
		{
			// �� �� �ϳ��� ���� ����
			// ���� ��� 
			new Sequence(new List<BTNode>
			{
				// �÷��̾ ���̴��� ( �Ⱥ��̸� return -> ����� )
				new ConditionNode(()=>{return isPlayerVisible();}),

				// �÷��̾ ���δ�
				new Selector(new List<BTNode>
				{
					new Sequence(new List<BTNode>
					{
						// �÷��̾ ���� ���� �ȿ� �ִ��� (���ٸ� return -> �̵� )
						new ConditionNode(()=>{return isAttackable(); }),

						// �÷��̾� ����
						new ActionNode(()=>{return BTNode.State.Running; })
					}),

					// �÷��̾�� �̵�
					new ActionNode(()=>{return MoveToTarget(); })
				}),
			}),

			// ��� ���
			new ActionNode(()=>{return BTNode.State.Running; })
		});
	}

        public virtual void FixedUpdate()
        {
		_BTRoot.Execute();
	}

	public bool isPlayerVisible()
	{
		if (isDead || DungeonDoorway.instance.isPlayerInDungeon() == false || _target == null || 
			(_target != null && _target.GetComponent<PlayerController>().isDead))
		{
			_anim.SetBool("attack", false); 
			_anim.SetBool("move", false);
			return false;
		}

		return true;
	}
	 
	public bool isAttackable()
	{
	
		Vector3 dir = (_target.transform.position - transform.position);
		float dist = dir.magnitude;
		bool attack = dist < _attackRange;

		_anim.SetBool("attack", attack);
		_anim.SetBool("move", !attack);

		return attack;
	}

	public BTNode.State MoveToTarget()
	{
		return MoveToTarget(_target.transform.position);
	}

	public BTNode.State MoveToTarget(Vector3 targetPos)
	{
		//transform.LookAt(_target.transform.position);
		if ((targetPos - transform.position).magnitude < 0.1f)
			return BTNode.State.Success;

		LookTarget(targetPos);
		_agent.SetDestination(targetPos); 
		return BTNode.State.Running;
	}

	public void LookTarget(Vector3 target)
	{
		Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
		// ���� ȸ������ ��ǥ ȸ������ �ε巴�� ȸ��
		transform.rotation = Quaternion.RotateTowards(
		    transform.rotation,
		    targetRotation,
		    720 * Time.deltaTime
		);
	}

	protected float GetDistToPlayer()
	{
		return _target == null  ? -1 : (_target.transform.position - transform.position).magnitude; 
	}
	public void InitHp()
	{
		HP = _initHp;
	}

	public virtual void InitMonster(Vector3 pos, Action relaseListener)
	{
		transform.position = pos;
		transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);

	//	_rigid.MovePosition(pos);

		InitHp();
		if (_onRelase != null)
			_onRelase.RemoveAllListeners();
		_onRelase.AddListener(() => relaseListener.Invoke());

		_anim.SetBool("die", false);
		Utils.instance.SetTimer(() =>{gameObject.SetActive(true);}, 1.0f); 
	}

	public override void TargetEnter(GameObject go)
	{
                if (go.GetComponent<PlayerController>() == null)
                        return;

		_target = go;
	}

	public override void TargetExit(GameObject go)
	{
		if (go.GetComponent<PlayerController>() == null)
			return;

		_target = null;
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
		Attack(_damage);
	} 

	public void AE_AttackRate(int rate)
	{
		int DAMAGE = _damage * rate / 100;

		Attack(DAMAGE);
	}
	
	void Attack(int damage)
	{
		if (_target == null)
			return;

		Vector3 dir = (_target.transform.position - transform.position);
		float dist = dir.magnitude;

		if (dist < _attackRange)
			_target.GetComponent<HealthEntity>().OnDamage(gameObject, damage);
	}

	public void AE_Die()
	{
		int rate = _dropRate * DataBase.instance._itemDropRate.GetValue() / 100; 
		while ( DungeonDoorway.instance.isPlayerInDungeon() &&  rate > 0)
		{
			int R = UnityEngine.Random.Range(1, 100);
			if (R < rate)
			{
				GameObject go = ItemSpawner.instance.GetItem(_dropItem);

				Vector3 pos = gameObject.transform.position;
				go.transform.position = new Vector3(pos.x + UnityEngine.Random.Range(-0.3f, 0.3f), pos.y, pos.z + UnityEngine.Random.Range(-0.3f, 0.3f));

				IngredientItem item = go.GetComponent<IngredientItem>();
				item.InitItem();
				item.AddReleaseAction(() => ItemSpawner.instance.RelaseItem(_dropItem, go));
			}
			rate -= 100;
		} 
		_onRelase?.Invoke();
	}
}
