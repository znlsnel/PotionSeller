using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PotionTable : MonoBehaviour
{
        ObjectPool<GameObject> _itemPool;
        GameObject _itemPrefab;

	[SerializeField] SendItemManager _ingredientSpot;
        PickupManager _pickup;
	PickupManager __ingredientPickup;

	int ingredientCnt = 0;
	void Start()
	{
		_itemPool = new ObjectPool<GameObject>(
			createFunc: () => Instantiate<GameObject>(_itemPrefab),
			defaultCapacity: 0
			);
		 
		_pickup = GetComponent<PickupManager>();
		_pickup.destoryItem = true;
		_pickup._onGetItem += () => { ingredientCnt++; }; 

		__ingredientPickup = _ingredientSpot.gameObject.GetComponent<PickupManager>();
		StartCoroutine(MoveToMagicJar());
	}

	// Update is called once per frame
	IEnumerator MoveToMagicJar()
	{
		while (true) 
		{
			if (__ingredientPickup.isReceivingItem == false)
				_ingredientSpot.SendItem(_pickup);

			yield return new WaitForSeconds(1.0f);
		}
	}


}
