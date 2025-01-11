using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;


public class MonsterSpawner : MonoBehaviour
{
        [SerializeField] GameObject _monsterPrefab;
        ObjectPool<GameObject> _pool;
	HashSet<MonsterController> _activeMonster = new HashSet<MonsterController>();
	[SerializeField] int _maxCount = 20;
	[SerializeField, Range(2, 120)] float _spawnInterval = 2.0f;

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
			yield return new WaitForSeconds(2.0f);
		}
	}

	public void InitMonsters()
	{
		foreach (MonsterController monster in _activeMonster)
		{
			if (monster.HP > 0)
				monster.InitHp();

		} 

	}
}
