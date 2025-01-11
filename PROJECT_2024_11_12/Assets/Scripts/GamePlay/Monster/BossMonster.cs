using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[Serializable]
public struct MonsterSkill
{
        public float _range;
        public float _coolTime;
}
public class BossMonster : MonsterController
{
	[SerializeField] List<MonsterSkill> _skills = new List<MonsterSkill>();
	List<float> _lastUseTime = new List<float> {0, 0, 0, 0, 0, 0, 0};

	[SerializeField] MonsterSkill _attackInfo;
	Vector3 spawnPosition = new Vector3(0, 0, 0);

	bool isAttacking = false;
	public bool isPlayerInBossRoom = false;
        public override void FixedUpdate() 
        {
		_BTRoot.Execute();
	}

	public override void InitMonster(Vector3 pos, Action relaseListener)
	{
		transform.position = pos;
		spawnPosition = pos;
		isAttacking = false;

		InitHp();
		if (_onRelase != null)
			_onRelase.RemoveAllListeners();
		_onRelase.AddListener(() => relaseListener.Invoke());

		_anim.SetBool("die", false);
		Utils.instance.SetTimer(() => { gameObject.SetActive(true); }, 1.0f);
	}

	bool  isAttackable(int idx)
	{
		float dist = GetDistToPlayer();

		bool isInRange = dist >= 0.0f && dist <= _skills[idx]._range;
		bool isCoolDown = Time.time - _lastUseTime[idx] > _skills[idx]._coolTime;
		Debug.Log($"CoolTime : {_skills[idx]._coolTime}, Use Time : {_lastUseTime[idx]}");

		return isInRange && isCoolDown;
	}
	void OnAttack(int idx)
	{ 
		LookTarget(_target.transform.position);
		_lastUseTime[idx] = Time.time;
		_anim.Play($"attack_{idx + 1}");
	}

	public override void SetBehaviorTree()
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
		}
		attackMode.Add(new ActionNode(() => {
			_agent.isStopped = false;

			return MoveToTarget();
		})
		);

		_BTRoot = new Selector(new List<BTNode>
		{
			new Sequence(new List<BTNode>
			{
				// 유저가 보스룸에 입장 했는가?
				new ConditionNode(()=>{

					return isPlayerInBossRoom && !isAttacking;			
					 
				}),
			
				new Selector(attackMode)
			}),

			// 원래 위치로 돌아가기
			//new ActionNode(()=>{return MoveToTarget(spawnPosition); }),

			// Idle 상태 유지
			new ActionNode(()=>{
				_agent.isStopped = true;
				return BTNode.State.Running; 
			})
		});
	}


	void AE_StartAttack()
	{
		isAttacking = true;
	}

	void AE_EndAttack()
	{
		isAttacking = false;
	}

	public override void OnDead()
	{
		_agent.isStopped = true;
		_target = null;

		_onDead?.Invoke(); 
		_anim.SetBool("die", true);
	}
}
