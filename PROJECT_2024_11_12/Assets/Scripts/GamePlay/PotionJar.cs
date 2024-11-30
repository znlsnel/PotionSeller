using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PotionJar : MonoBehaviour
{
        ObjectPool<GameObject> _itemPool;
        [SerializeField] GameObject _itemPrefab;
	[Space(10)]

	[SerializeField] SendItemManager _ingredientSpot;
	[SerializeField] PickupManager _postionTable;
	[SerializeField] Transform _potionPos;
	[SerializeField] Transform _potionSpawnPos;

	[Space(10)]
	[SerializeField] float _createSpeed = 2.0f;

        PickupManager _pickup;
	 
	void Start()
	{
		_itemPool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate<GameObject>(_itemPrefab),
			defaultCapacity: 0
			);
		 
		_pickup = GetComponent<PickupManager>();

		StartCoroutine(MoveToMagicJar());
	}
	 
	// Update is called once per frame
	IEnumerator MoveToMagicJar()
	{
		while (true) 
		{
			if (!_ingredientSpot.isEmpty)
			{
				_ingredientSpot.SendItem(_pickup, 1);

				yield return new WaitForSeconds(_createSpeed / 2);

				GameObject item = _itemPool.Get();
				item.transform.position = _potionSpawnPos.position;
				item.GetComponent<Item>().AddReleaseAction(() => { _itemPool.Release(item); });
				_postionTable.PickUpItem(item);

				yield return new WaitForSeconds(_createSpeed / 2);
			}
			else
				yield return new WaitForSeconds(_createSpeed);
		}
	}
}
