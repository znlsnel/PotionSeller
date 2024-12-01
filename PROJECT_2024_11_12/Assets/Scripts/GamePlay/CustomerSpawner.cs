using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class CustomerSpawner : MonoBehaviour
{
        [SerializeField] Counter _counter;
        [SerializeField] GameObject _customerPrefab;
        [SerializeField] Transform _spawnPos;
        [SerializeField] Transform _endPos;

        [SerializeField] int _maxCustomer = 10;
        ObjectPool<GameObject> _customerPool;

	private void Awake()
	{
		_customerPool = new ObjectPool<GameObject>(
		createFunc: () => Instantiate<GameObject>(_customerPrefab),
			actionOnGet: (obj) => { 
                                obj.transform.position = _spawnPos.position;
                                obj.GetComponent<Customer>()?.InitCustomer(_counter, _endPos, () => { _customerPool.Release(obj); 
                                }); }
		);
	}
	void Start()
        { 
                StartCoroutine(SpawnCustomer());
	}

        IEnumerator SpawnCustomer()
        {
                while (true)
                {
                        if (_customerPool.CountActive < _maxCustomer)
                                _customerPool.Get(); 

			yield return new WaitForSeconds(Random.Range(2, 5));
                }
        }
}
