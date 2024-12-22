using System.Collections;
using System.Collections.Generic;
using TMPro;
//using Unity.Android.Gradle.Manifest;
using UnityEngine;

public interface IItemSender 
{
	public void SendItem(IItemReceiver receiver, int cnt = 99999);
	public void CancelSend();
	public int GetItemCount();

	public bool isSending();
}

public class SendItemManager : MonoBehaviour, IItemSender
{
	PickupManager _myPickup;
	[SerializeField] float _sendTime = 1.0f;
	[SerializeField] AudioClip _sendAudio;

	Stack<GameObject> _items;
	GameObject _target;
	private void Awake()
	{
		_myPickup = GetComponent<PickupManager>();
		_items = _myPickup.GetItemStack();
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
	public bool isSending()
	{
		return sendCT != null;
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
			int size = Mathf.Min(_myPickup.GetItemStack().Count, cnt);
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

			float t = Mathf.Min(_sendTime / size, 0.2f); 
			while (cnt> 0 && _myPickup.GetItemStack().Count > 0 && receiver.isReceivable())
			{
				AudioManager.instance.PlayAudioClip(_sendAudio);
				receiver.ReceiveItem(_myPickup.GetItemStack().Peek());
				_myPickup.GetItemStack().Pop();
				cnt--;
				yield return new WaitForSeconds(t);
			}

			if (_target != null) 
				yield return new WaitForSeconds(0.1f);
			else
				yield break;
		}

	}
}
