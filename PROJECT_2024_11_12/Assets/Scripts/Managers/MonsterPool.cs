using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;


public class MonsterPool : MonoBehaviour
{
        [SerializeField] GameObject _monsterPrefab;
	[SerializeField] int _maxCount = 20;

	HashSet<MonsterController> _activeMonster = new HashSet<MonsterController>();
        ObjectPool<GameObject> _pool;


	private void Awake() 
	{
		_pool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate<GameObject>( _monsterPrefab),
			actionOnGet: (obj) => {
				obj.SetActive(false);
				obj.GetComponent<MonsterController>().InitMonster(GetSpawnPos(), () => { _pool.Release(obj); });
				_activeMonster.Add(obj.GetComponent<MonsterController>());
				},

			actionOnRelease: (obj) => {
				_activeMonster.Remove(obj.GetComponent<MonsterController>());
				obj.SetActive(false); 
			},
			actionOnDestroy: (obj) => Destroy(obj), 
			defaultCapacity: 2,
			maxSize: 10
		);
	}
	 
	Vector3 GetSpawnPos()
	{ 
		float xOffset = transform.localScale.x / 2;
		float zOffset = transform.localScale.z / 2;
		 
		Vector3 ret = new Vector3(Random.Range(-xOffset, xOffset), 0.0f, Random.Range(-zOffset, zOffset)) + transform.position;
		return ret;
	}

	private void Start()
	{
		StartCoroutine(SpawnMonster());

	}

	IEnumerator SpawnMonster()
	{
		while (true)
		{
			int left = _maxCount - _pool.CountActive;
			if (left > 0)
			{
				
				int cnt = Random.Range(1, Mathf.Min(left+1, 3));
				while (cnt-- > 0)
					_pool.Get();
			}
			yield return new WaitForSeconds(Random.Range(2.0f, 3.0f));
		}
	}

	public void InitPool()
	{
		foreach (MonsterController monster in _activeMonster)
		{
			if (monster.HP > 0)
				monster.InitHp();
		} 
	}

	private void OnTriggerEnter(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player"); // ��Ʈ����ũ �� ����
		if ((findLayerMask.value & (1 << other.gameObject.layer)) != 0)
		{
			foreach (var m in _activeMonster)
				m.SetPlayerInThisStage(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player"); // ��Ʈ����ũ �� ����
		if ((findLayerMask.value & (1 << other.gameObject.layer)) != 0)
		{
			foreach (var m in _activeMonster)
				m.SetPlayerInThisStage(false);
		}
	}
}
