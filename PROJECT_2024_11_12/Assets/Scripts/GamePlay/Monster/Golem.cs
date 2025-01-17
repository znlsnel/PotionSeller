using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using static UnityEngine.ParticleSystem;


public class Golem : BossMonster
{
	[Space(10)]
	[SerializeField] GameObject _monsterPrefab;
	[SerializeField] GameObject _spawnEffect;
	[SerializeField] Transform _spawnPos;
	[SerializeField] AudioClip _spawnAudio;

 	ObjectPool<GameObject> _monsterPool;
	HashSet<GameObject> _activeMonsters = new HashSet<GameObject>();
	private void Start()
	{
		_monsterPool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate<GameObject>(_monsterPrefab),
			actionOnGet: ActionOnGet,
			actionOnRelease: (obj) => obj.SetActive(false)
		) ;
	}

	void ActionOnGet(GameObject obj) 
	{
		MonsterController mc = obj.GetComponent<MonsterController>();
		mc.SetPlayerInThisStage(isPlayerInThisStage);
		mc.InitMonster(_spawnPos.position, () => {
			_monsterPool.Release(obj);
			_activeMonsters.Remove(obj);
		});
		_activeMonsters.Add(obj);
	}

	public override void SetPlayerInThisStage(bool b)
	{
		base.SetPlayerInThisStage(b);

		foreach (GameObject obj in _activeMonsters)
			obj.GetComponent<MonsterController>().SetPlayerInThisStage(b);

		if (b == false)
		{
			foreach (GameObject obj in _activeMonsters)
			{
				Utils.instance.SetTimer(() => {
					obj.GetComponent<MonsterController>().OnDead(false);
				}, 0.1f);
			}
		}	
	}

	void AE_OnSpawnMonsterSkill()
	{
		var go = Instantiate<GameObject>(_spawnEffect);
		Vector3 pos = transform.position; pos.y = 2;
		go.transform.position = pos;
		Utils.instance.SetTimer(()=>Destroy(go), 3.0f);

		Utils.instance.SetTimer(() =>
		{
			if (isPlayerInThisStage)
			{
				_monsterPool.Get();
				AudioManager.instance.PlayAudioClip(_spawnAudio);
			}

		}, 1.0f);
		
	}
}
