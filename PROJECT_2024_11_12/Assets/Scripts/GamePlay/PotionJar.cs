using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class PotionJar : MonoBehaviour
{
        ObjectPool<GameObject> _itemPool;
        [SerializeField] GameObject _itemPrefab;
        [SerializeField] GameObject _ingredientSpot;
        [SerializeField] GameObject _potionTable;
	[Space(10)]


	[SerializeField] Transform _potionPos;
	[SerializeField] Transform _potionSpawnPos;

	[Space(10)]
	[SerializeField] float _createSpeed = 2.0f;

	IItemSender _IGItemSender;
	IItemReceiver _tableItemReceiver;

        PickupManager _pickup;
	 
	void Start() 
	{
		_IGItemSender = _ingredientSpot.GetComponent<SendItemManager>();
		_tableItemReceiver = _potionTable.GetComponent<PickupManager>();

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
			if (_IGItemSender.GetItemCount() > 0)
			{
				_IGItemSender.SendItem(_pickup, 1);

				yield return new WaitForSeconds(_createSpeed / 2);

				GameObject item = _itemPool.Get();
				item.transform.position = _potionSpawnPos.position;
				item.GetComponent<Item>().AddReleaseAction(() => { _itemPool.Release(item); });
				_tableItemReceiver.ReceiveItem(item);

				yield return new WaitForSeconds(_createSpeed / 2);
			}
			else
				yield return new WaitForSeconds(_createSpeed);
		}
	}
}
