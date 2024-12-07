using System.Collections;
using UnityEngine;
using UnityEngine.Events;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class IngredientItem : Item, IPlayerSensor
{
	IItemReceiver _playerItemReceiver;
	SphereCollider _collider;
	void Awake() 
	{
		_collider = GetComponent<SphereCollider>();
		
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null)
			return;

		if (_playerItemReceiver == null)
			_playerItemReceiver = pc.GetPickupManager();

		EnterPlayer();
	}


	IEnumerator ItemAnim()
	{
		float time = 0f;
		float startY = transform.position.y;
		float endY = startY + 0.3f;
		bool isAscending = true;

		while (_collider.enabled)
		{
			if (_collider.enabled == false)
				yield break;

			time += (isAscending ? 1 : -1) * Time.deltaTime;

			if (time >= 1f || time <= 0f)
				isAscending = !isAscending;

			transform.position = new Vector3(
			    transform.position.x,
			    Mathf.Lerp(startY, endY, time),
			    transform.position.z
			);

			yield return null;
		}

	}

	public void OnHand() 
	{
		_collider.enabled = false;
	}
	public void InitItem()
	{
		transform.SetParent(null);
		_collider.enabled = true;
		StartCoroutine(ItemAnim());
	}

	public void InitDungeon()
	{
		if (_collider.enabled == true) 
			Relase();
	}
	public void EnterPlayer()
	{
		

		_playerItemReceiver.ReceiveItem(gameObject);
	}

	public void ExitPlayer()
	{
	}
}
