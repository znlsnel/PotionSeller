using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EItemType
{
	ALL,
	INGREDIENT_POSTION,
	POSTION,
}

public enum EItem
{
	POSTION_INGREDIENT,
	POSTION
}

public class ItemSpawner : Singleton<ItemSpawner>
{
	[SerializeField] List<GameObject> _items = new List<GameObject>();
        Dictionary<GameObject, ObjectPool<GameObject>> _itemPools = new Dictionary<GameObject, ObjectPool<GameObject>>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		foreach (var item in _items)
		{
			ObjectPool<GameObject> op = new ObjectPool<GameObject>(
				createFunc: ()=>Instantiate<GameObject>(item),
				defaultCapacity: 0
			); 
			 
			_itemPools[item] = op;
		}
	}  

	public GameObject GetItem(GameObject itemPrefab)
	{
		return _itemPools[itemPrefab].Get();
	}

	public void RelaseItem(GameObject prefab, GameObject item)
	{
		_itemPools[prefab].Release(item); 
	}

	// Update is called once per frame
    void Update()
    {
        
    }
}
