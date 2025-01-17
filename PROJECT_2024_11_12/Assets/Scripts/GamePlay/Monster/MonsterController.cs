using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TreeEditor;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


[Serializable]
public struct SKILLINFO
{
	public float _range;
	public float _coolTime;

	public ObjectSensor sensor;
	public GameObject particle;
	public Transform spawnPos;
	public int damage;

	[Space(10)]
	public AudioClip _skillAudio;
}


public class MonsterController : HealthEntity
{
	[Space(10)]
	[SerializeField] List<SKILLINFO> _skills = new List<SKILLINFO>();
	[SerializeField] SKILLINFO _attackInfo;
	int _curSkillIdx = -1;

	[Space(10)]
	[SerializeField] GameObject _dropItem;
	[SerializeField] int _dropRate = 100;

	Rigidbody _rigid;
	Animator _anim;
	NavMeshAgent _agent;

	List<float> coolTimeChecker = new List<float>();
	float lastAttackTime = 0.0f;
	GameObject _target;
	Vector3 _spawnPosition;
	bool _isAttacking = false;
	bool _isPlayerInThisStage = false;

	UnityEvent _onRelase = new UnityEvent();
	UnityEvent _onDead = new UnityEvent();

	BTNode _BTRoot;

	// Funtions
	public virtual void SetPlayerInThisStage(bool b) => _isPlayerInThisStage = b;
	public bool isPlayerInThisStage => _isPlayerInThisStage;
	public void InitHp() => HP = _initHp;
	public void AddActionOnDead(Action ac) => _onDead.AddListener(() => ac?.Invoke());
	public Vector3 GetTargetPos()
	{
		if (_target != null)
			return _target.transform.position;

		return Vector3.zero;
	}

	#region Animation Events

	protected void AE_StartAttack()
	{
		_isAttacking = true;
	}
	protected void AE_EndAttack() => _isAttacking = false;
	public void AE_Die() => _onRelase?.Invoke();

	#endregion


	protected override void Awake()
        {
		base.Awake();
                _anim = GetComponent<Animator>();
		_rigid = GetComponent<Rigidbody>();
		_agent = GetComponent<NavMeshAgent>();
		_agent.stoppingDistance = 2.0f * transform.localScale.x;
		SetBehaviorTree();
	}

	public virtual void SetBehaviorTree()
	{
		_skills.Add(_attackInfo);
		List<BTNode> attackMode = new List<BTNode>();
		for (int i = 0; i < _skills.Count; i++)
		{
			int idx = i;
			attackMode.Add(new Sequence(new List<BTNode>
			{
				new ConditionNode(()=>isAttackable(idx)),
				new ActionNode(() =>
				{
					OnAttack(idx);
					_agent.isStopped = true;
					return BTNode.State.Running;
				})
			}));
			coolTimeChecker.Add(0.0f);
		}

		attackMode.Add(new ActionNode(() => {
			_agent.isStopped = false;

			if (_target != null)
				return MoveToTarget();
			else 
				return BTNode.State.Running;
		}));
		

		_BTRoot = new Selector(new List<BTNode>
		{
			new Sequence(new List<BTNode>
			{
				// 유저가 Stage에 입장했는가?
				new ConditionNode(()=>{

					return isPlayerVisible() && !_isAttacking;

				}),

				new Selector(attackMode)
			}), 

			// 원래 위치로 돌아가기
		//	new ActionNode(()=>{return MoveToTarget(_spawnPosition); }),

			// Idle 상태 유지 
			new ActionNode(()=>{
				_agent.isStopped = true; 
				return BTNode.State.Running;
			})
		});
	}

        public virtual void FixedUpdate() 
        {
		_BTRoot.Execute();
		_hpBar.GetComponent<MonsterHpUI>().SetActive(_target != null); 
	}

	public bool isPlayerVisible()
	{
		bool ret = isDead || _isPlayerInThisStage == false || _target == null ||
			_target.GetComponent<PlayerController>().isDead;

		_anim.SetBool("move", !ret);

		 
		return !ret;
	}

	bool isAttackable(int idx)
	{
		if (_target == null)
			return false;

		float dist =  (_target.transform.position - transform.position).magnitude;
		bool isInRange = dist >= 0.0f && dist <= _skills[idx]._range;
		bool isCoolDown = Time.time - coolTimeChecker[idx] > _skills[idx]._coolTime && Time.time - lastAttackTime > 1.0f ;
		bool isLookingAtTarget = Utils.AngleDifference(transform, _target.transform.position) < 1.0f;
		 
		return isInRange && isCoolDown && isLookingAtTarget;
	} 

	void OnAttack(int idx)
	{
		//LookTarget(_target.transform.position); 
		lastAttackTime = Time.time;
		coolTimeChecker[idx] = Time.time;

		_curSkillIdx = idx;
		_anim.Play($"attack_{idx + 1}"); 
	}

	public BTNode.State MoveToTarget() => MoveToTarget(_target.transform.position);
	public BTNode.State MoveToTarget(Vector3 targetPos)
	{
		LookTarget(targetPos);
		_agent.SetDestination(targetPos); 
		return BTNode.State.Running;
	}

	public void LookTarget(Vector3 target)
	{
		Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);
		// 현재 회전에서 목표 회전으로 부드럽게 회전
		transform.rotation = Quaternion.RotateTowards(
		    transform.rotation,
		    targetRotation,
		    720 * Time.deltaTime
		);
	}


	public virtual void InitMonster(Vector3 pos, Action relaseListener)
	{
		transform.position = pos;
		_spawnPosition = pos;

		if (gameObject.GetComponent<BossMonster>() == null)
			transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);
		
		InitHp(); 
		_onRelase.RemoveAllListeners();
		_onRelase.AddListener(() => relaseListener.Invoke());

		_anim.SetBool("die", false);
		_isAttacking = false;

		gameObject.SetActive(true); 
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
		OnDead(true);
	}

	public void OnDead(bool getItem)
	{
		HP = 0;
		_target = null;

		_onDead?.Invoke();
		_onDead.RemoveAllListeners();
		_anim.SetBool("die", true);

		if (getItem)
		{
			int rate = _dropRate * DataBase.instance._itemDropRate.GetValue() / 100;
			ItemSpawner.instance.SpawnItem(_dropItem, transform.position, rate);
		}
	}


	public void AE_OnSkill()
	{
		SKILLINFO skill = _skills[_curSkillIdx]; 
		if (skill.particle != null)
		{
			GameObject go = Instantiate<GameObject>(skill.particle);
			go.transform.position = skill.spawnPos.position;
			go.transform.rotation = skill.spawnPos.rotation;

			Utils.instance.SetTimer(() => { Destroy(go); }, 10.0f);
		}
		AudioManager.instance.PlayAudioClip(_skills[_curSkillIdx]._skillAudio);

		GameObject target = skill.sensor.FindTargetByLayer("Player");
		if (target != null)
			target.GetComponent<HealthEntity>()?.OnDamage(gameObject, skill.damage);
	}

}
