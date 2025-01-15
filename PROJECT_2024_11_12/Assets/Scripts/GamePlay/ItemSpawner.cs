using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public enum EItemType
{
	None,
	ALL,
	INGREDIENT_POTION,
	POTION,
	COIN,
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
	HashSet<GameObject> _activeItems = new HashSet<GameObject>();	

    // Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		foreach (var item in _items)
		{
			ObjectPool<GameObject> op = new ObjectPool<GameObject>(
				createFunc: ()=>Instantiate<GameObject>(item),
				actionOnGet: (obj) => { obj.SetActive(true); _activeItems.Add(obj); },
				actionOnRelease: (obj) => { obj.SetActive(false);  _activeItems.Remove(obj); },
				defaultCapacity: 0
			); 
			 
			_itemPools[item] = op;
		}
	}  

	GameObject GetItem(GameObject itemPrefab) => _itemPools[itemPrefab].Get();
	void RelaseItem(GameObject prefab, GameObject item) => _itemPools[prefab].Release(item); 

	public void InitDungeon()
	{
		List<GameObject> items = new List<GameObject>();
		foreach (GameObject item in _activeItems)
			items.Add(item);
		  
		foreach (GameObject item in items)
			item.GetComponent<IngredientItem>()?.InitDungeon(); 

	}
	// Update is called once per frame
    
	public void SpawnItem(GameObject itemPrefab, Vector3 spawnPos, int dropRate)
	{
		while (DungeonDoorway.instance.isPlayerInDungeon() && dropRate > 0)
		{
			int R = UnityEngine.Random.Range(1, 100);
			if (R < dropRate)
			{
				GameObject go = GetItem(itemPrefab);

				Vector3 pos = spawnPos;
				go.transform.position = new Vector3(pos.x + UnityEngine.Random.Range(-0.3f, 0.3f), pos.y, pos.z + UnityEngine.Random.Range(-0.3f, 0.3f));

				IngredientItem item = go.GetComponent<IngredientItem>();
				item.InitItem();
				item.AddReleaseAction(() => RelaseItem(itemPrefab, go));
			}
			dropRate -= 100;
		}
	}
}
