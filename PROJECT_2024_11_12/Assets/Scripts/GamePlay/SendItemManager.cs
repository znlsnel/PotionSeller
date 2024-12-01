using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SendItemManager : MonoBehaviour
{
	PickupManager _pickupManager;
	[SerializeField] float _sendTime = 1.0f;

	Stack<GameObject> _items; 
	private void Awake()
	{
		_pickupManager = GetComponent<PickupManager>();
		_items = _pickupManager.GetItemStack();
	}

	public int itemCnt { get { return _items.Count; } }

	Coroutine sendCT = null;
	public void SendItem(PickupManager target, int cnt = 99999)
	{
		sendCT = StartCoroutine(Send(target, cnt));
	}

	public void CancelSend()
	{
		if (sendCT != null)
		{
			StopCoroutine(sendCT);
			sendCT = null;
		}
	}
	 
	IEnumerator Send(PickupManager target, int cnt)
	{ 
		int size = Mathf.Min(_pickupManager.GetItemStack().Count, cnt);
		if (size == 0 || !target.CheckItemType(_items.Peek().GetComponent<IngredientItem>()._itemType))
			yield break;

		while (_pickupManager.isReceivingItem)
			yield return new WaitForSeconds(0.3f); 

		float t = _sendTime / size; 
		while (cnt-- > 0 && _pickupManager.GetItemStack().Count > 0)
		{
			target.PickUpItem(_pickupManager.GetItemStack().Peek());
			_pickupManager.GetItemStack().Pop();

			yield return new WaitForSeconds(t);
		}
	}
}
