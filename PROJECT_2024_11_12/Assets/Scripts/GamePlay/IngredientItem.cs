using UnityEngine;
using UnityEngine.Events;

public class IngredientItem : Item
{ 
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

		other.GetComponent<PickupManager>().PickUpItem(gameObject);
	}

	public void OnHand() 
	{
		_collider.enabled = false;
	}
	public void InitItem()
	{
		_collider.enabled = true;

	}


}
