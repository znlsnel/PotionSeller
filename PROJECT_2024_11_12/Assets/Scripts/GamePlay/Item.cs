using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
	[SerializeField] public EItemType _itemType;
	[NonSerialized] public UnityEvent _onRelease = new UnityEvent();

	public bool isMoving = false;
	public UnityAction _onMoveStop;

	public void AddReleaseAction(Action ac)
	{
		_onRelease.AddListener(()=>ac?.Invoke());
	}
	public void Relase()
	{
		transform.SetParent(null);
		gameObject.SetActive(false);
		_onRelease?.Invoke();
		_onRelease.RemoveAllListeners();
	}
	
}
 