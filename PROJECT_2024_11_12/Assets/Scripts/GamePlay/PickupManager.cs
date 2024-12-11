using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IItemReceiver
{
	public void ReceiveItem(GameObject item, bool destroy = false);
	public bool CheckItemType(EItemType type);

	public bool isReceivable();
	public bool isReceiving();
}

public class PickupManager : MonoBehaviour, IItemReceiver
{
	[SerializeField] Transform _handPos;
	[SerializeField] EItemType _itemType;

	[SerializeField] int _carrySizeX = 1;
	[SerializeField] int _carrySizeZ = 1;

	[Space(10)]
	[SerializeField] float _xOffset = 0;
	[SerializeField] float _zOffset = 0;
	[SerializeField] float _yOffset = 0;

	[SerializeField] Vector2 _sortDir = new Vector2(1,1);

	[Space(10)]
	Stack<GameObject> _items = new Stack<GameObject>();

	[NonSerialized] public bool isReceivingItem = false;
	[SerializeField] int _maxCarrySize = 8;

	public bool destoryItem = false;
	public UnityAction _onGetItem;

	bool isEnable = true;

	public void SetActive(bool act)
	{
		isEnable = act;
	}

	public void SetCarrySize(int size)
	{
		_maxCarrySize = size;
	}

	public bool isReceivable()
	{
		return _items.Count < _maxCarrySize;
	}
	public bool isReceiving()
	{
		return isReceivingItem;
	}
	public EItemType GetItemType()
	{
		if (_items.Count == 0)
			return EItemType.None;
		return _items.Peek().GetComponent<Item>()._itemType;
	}
	public int _leftCarryCap { get { return _maxCarrySize - _items.Count; } }
	public Stack<GameObject> GetItemStack() { return _items; }
	public int GetItemCount() { return _items.Count; }


	public void InitPickupManager(Stack<GameObject> stack)
	{
		_items = stack;
	}

	public bool CheckItemType(EItemType type)
	{
		if (_itemType != EItemType.ALL)
			return type == _itemType;

		else if (_items.Count == 0)
			return true;
		 
		return _items.Peek().GetComponent<Item>()._itemType == type;
	}

	public void ReceiveItem(GameObject go, bool destroy = false)
	{
		Item item = go.GetComponent<Item>();
		if (isEnable == false || _leftCarryCap == 0 || !CheckItemType(item._itemType))
			return;

		isReceivingItem = true;

		go.GetComponent<IngredientItem>()?.OnHand();
		 
		go.transform.SetParent(null);
		go.transform.rotation = _handPos.rotation;
		Vector3 pos = Vector3.zero;

		Renderer renderer = _items.Count == 0 ? null : _items.Peek().GetComponent<Renderer>();
		if (renderer != null)
		{
			int yIdx = _items.Count / (_carrySizeX * _carrySizeZ);
			int zIdx = _items.Count % (_carrySizeX * _carrySizeZ) / _carrySizeX;
			int xIdx = _items.Count % (_carrySizeX * _carrySizeZ) % _carrySizeX;

			pos.y += yIdx * (renderer.bounds.size.y + _yOffset);
			pos.z += _sortDir.y *  zIdx * (renderer.bounds.size.z + _zOffset);
			pos.x += _sortDir.x * xIdx * (renderer.bounds.size.x + _xOffset);

		}

		StartCoroutine(MoveInParabola(go, go.transform.position, pos, destroy));

		if (destoryItem == false)
		{
			go.transform.SetParent(_handPos);
			_items.Push(go);
			 
		}

	}

	float moveEndTime = 0.0f;

	IEnumerator MoveInParabola(GameObject go, Vector3 start, Vector3 offset, bool destory, float height = 2.0f, float duration = 0.2f)
	{
		moveEndTime = Time.deltaTime + duration;
		float elapsedTime = 0f;
		while (elapsedTime < duration)
		{
			float t = elapsedTime / duration;
			Vector3 pos = _handPos.position;
			Vector3 currentPos = Vector3.Lerp(start, pos + offset, t);
			// 포물선 높이 추가
			currentPos.y += height * Mathf.Sin(Mathf.PI * t);

			go.transform.position = currentPos;

			elapsedTime += Time.deltaTime;
			yield return null;
		}

		Vector3 end =  _handPos.position;
		go.transform.position = end + offset;
		_onGetItem?.Invoke();

		if (destoryItem || destory)
			go.GetComponent<Item>()?.Relase();  
		

		yield return new WaitForSeconds(duration * 2);
		if (Time.time > moveEndTime)
			isReceivingItem = false;
	}

	public void ClearItem()
	{
		foreach (var item in _items)
			item.GetComponent<Item>()?.Relase();
		_items.Clear();
		
	}
}
