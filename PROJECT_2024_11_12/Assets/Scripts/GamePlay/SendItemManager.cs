using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SendItemManager : MonoBehaviour
{
	PickupManager _pickupManager;
	[SerializeField] EItemType _pickupItemType;
	[SerializeField] float _sendTime = 1.0f;

	Stack<GameObject> _item;
	private void Start()
	{
		_pickupManager = GetComponent<PickupManager>();
		_item = _pickupManager.GetItemStack();
	}

	Coroutine sendCT = null;
	public void SendItem(PickupManager target)
	{
		sendCT = StartCoroutine(Send(target));
	}

	public void CancelSend()
	{
		if (sendCT != null)
		{
			StopCoroutine(sendCT);
			sendCT = null;
		}
	}

	IEnumerator Send(PickupManager target)
	{
		int size = _pickupManager.GetItemStack().Count;
		if (size == 0 || _pickupManager.GetItemStack().Peek().GetComponent<Item>()._itemType != _pickupItemType)
			yield break;

		float t = _sendTime / size;
		while (_pickupManager.GetItemStack().Count > 0)
		{
			target.PickUpItem(_pickupManager.GetItemStack().Peek());
			_pickupManager.GetItemStack().Pop();

			yield return new WaitForSeconds(t);
		}

	}
}
