using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;


public class MonsterSpawner : MonoBehaviour
{
        [SerializeField] List<GameObject> _monsterPrefab;
        ObjectPool<GameObject> _pool;
	HashSet<MonsterController> _activeMonster = new HashSet<MonsterController>();
	[SerializeField] int _maxCount = 20;

	//public bool isPlayerIn = true;

	private void Awake()
	{
		_pool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate<GameObject>(_monsterPrefab[Random.Range(0, _monsterPrefab.Count)]),
			actionOnGet: (obj) => {
				obj.GetComponent<MonsterController>().InitMonster(this, GetSpawnPos(), () => { _pool.Release(obj); });
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

	// Update is called once per frame

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
		foreach (MonsterController monster in  _activeMonster)
			monster.InitHp();
		
	}

	//private void OnTriggerEnter(Collider other)
	//{
	//	PlayerCombatController pc = other.GetComponent<PlayerCombatController>();
	//	if (pc != null)
	//	{
	//	//	isPlayerIn = true;
	//		pc.isInHuntZone = true;
	//	}
	//}

	//private void OnTriggerExit(Collider other)
	//{
	//	PlayerCombatController pc = other.GetComponent<PlayerCombatController>();
	//	if (pc != null)
	//	{
	//	//	isPlayerIn = false; 
	//		pc.isInHuntZone = false; 
	//	}
	//}


}
