using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IItemSender
{
	public void SendItem(IItemReceiver receiver, int cnt = 99999);
	public void CancelSend();
	public int GetItemCount();
}

public class SendItemManager : MonoBehaviour, IItemSender
{
	PickupManager _pickupManager;
	[SerializeField] float _sendTime = 1.0f;

	Stack<GameObject> _items; 
	private void Awake()
	{
		_pickupManager = GetComponent<PickupManager>();
		_items = _pickupManager.GetItemStack();
	}

	public int GetItemCount()
	{
		return _items.Count; ;
	}

	Coroutine sendCT = null;
	public void SendItem(IItemReceiver receiver, int cnt = 99999)
	{
		sendCT = StartCoroutine(Send(receiver, cnt));
	}

	public void CancelSend()
	{
		if (sendCT != null)
		{
			StopCoroutine(sendCT);
			sendCT = null;
		}
	}
	 
	IEnumerator Send(IItemReceiver receiver, int cnt)
	{ 
		int size = Mathf.Min(_pickupManager.GetItemStack().Count, cnt);
		if (size == 0 || !receiver.CheckItemType(_items.Peek().GetComponent<Item>()._itemType))
			yield break;

		while (_pickupManager.isReceivingItem)
			yield return new WaitForSeconds(0.3f);  

		float t = _sendTime / size; 
		while (cnt-- > 0 && _pickupManager.GetItemStack().Count > 0)
		{
			receiver.ReceiveItem(_pickupManager.GetItemStack().Peek());
			_pickupManager.GetItemStack().Pop();

			yield return new WaitForSeconds(t);
		}
	}
}
