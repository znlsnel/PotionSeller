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
	GameObject _target;
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
	public void SendItem(GameObject target, IItemReceiver receiver, int cnt = 99999)
	{
		sendCT = StartCoroutine(Send(receiver, cnt));
		_target = target;
	}

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
		_target = null;
	}
	 
	IEnumerator Send(IItemReceiver receiver, int cnt)
	{ 
		while (true)
		{
			int size = Mathf.Min(_pickupManager.GetItemStack().Count, cnt);
			if (size == 0 || !receiver.CheckItemType(_items.Peek().GetComponent<Item>()._itemType))
			{
				if (_target == null)
					yield break;
				else
				{
					yield return new WaitForSeconds(0.1f);
					continue; 
				}
			}

			while (_pickupManager.isReceivingItem)
				yield return new WaitForSeconds(0.3f);

			float t = Mathf.Min(_sendTime / size, 0.2f); 
			while (cnt-- > 0 && _pickupManager.GetItemStack().Count > 0)
			{
				receiver.ReceiveItem(_pickupManager.GetItemStack().Peek());
				_pickupManager.GetItemStack().Pop();

				yield return new WaitForSeconds(t);
			}

			if (_target != null) 
				yield return new WaitForSeconds(0.1f);
			else
				yield break;
		}

	}
}
