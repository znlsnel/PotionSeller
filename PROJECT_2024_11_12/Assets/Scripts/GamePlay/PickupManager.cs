using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IItemReceiver
{
	public void ReceiveItem(GameObject item, float speed = 0.2f);
	public bool CheckItemType(EItemType type);

	public bool isReceivable();
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
	[SerializeField] int _maxCarrySize = 8;
	[SerializeField] bool _destoryItem = false;

	[Space(10)]
	[SerializeField] AudioClip _pickupAudio;
	
	public UnityEvent _onPickedItem = new UnityEvent();	

	Stack<GameObject> _items = new Stack<GameObject>();


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


	public EItemType GetItemType()
	{
		if (_items.Count == 0)
			return EItemType.None;
		return _items.Peek().GetComponent<Item>()._itemType;
	} 

	public int _leftCarryCap { get { return _maxCarrySize - (_items.Count); } }
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

	Vector3 GetItemOffset()
	{
		Vector3 pos = Vector3.zero;
		Renderer renderer = _items.Count == 0 ? null : _items.Peek().GetComponent<Renderer>();
		if (renderer != null)
		{
			int size = _carrySizeX * _carrySizeZ;
			int yIdx = (_items.Count) / size;
			int zIdx = (_items.Count) % size / _carrySizeX;
			int xIdx = (_items.Count) % size % _carrySizeX;

			pos.y += yIdx * (renderer.bounds.size.y + _yOffset);
			pos.z += _sortDir.y * zIdx * (renderer.bounds.size.z + _zOffset);
			pos.x += _sortDir.x * xIdx * (renderer.bounds.size.x + _xOffset);
			
		}
		return pos;
	}
	public void ReceiveItem(GameObject go, float speed = 0.2f)
	{
		Item item = go.GetComponent<Item>();
		if (isEnable == false || _leftCarryCap == 0 || !CheckItemType(item._itemType))
			return;

		go.GetComponent<IngredientItem>()?.OnHand();
		 
		go.transform.SetParent(null);
		go.transform.rotation = _handPos.rotation;
		AudioManager.instance.PlayAudioClip(_pickupAudio);

		if (item.isMoving)
		{
			Vector3 offset = GetItemOffset();
			item._onMoveStop += () => { StartCoroutine(MoveInParabola(go, go.transform.position, offset, _destoryItem)); };
		}
		else
		{
			StartCoroutine(MoveInParabola(go, go.transform.position, GetItemOffset(), _destoryItem));
			item.isMoving = true;
		}
		if (!_destoryItem)
		{
			go.transform.SetParent(_handPos);
			_items.Push(go);
		} 
	}

	IEnumerator MoveInParabola(GameObject go, Vector3 start, Vector3 offset, bool detory = false)
	{
		float height = 2.0f;
		float duration = 0.4f;

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

		Item item = go.GetComponent<Item>();
		item.isMoving = false;
		item._onMoveStop?.Invoke();
		item._onMoveStop = null;
		_onPickedItem?.Invoke();


		if (detory)
			go.GetComponent<Item>()?.Relase(); 
	} 



	public void ClearItem()
	{
		foreach (var item in _items)
			item.GetComponent<Item>()?.Relase();
		_items.Clear();
		
	}
}
