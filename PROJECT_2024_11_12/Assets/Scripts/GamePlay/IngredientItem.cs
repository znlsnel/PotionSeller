using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;

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
			_playerItemReceiver = other.GetComponent<PickupManager>();

		EnterPlayer();
	}

	public void OnHand() 
	{
		_collider.enabled = false;
	}
	public void InitItem()
	{
		transform.SetParent(null);
		_collider.enabled = true;

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
