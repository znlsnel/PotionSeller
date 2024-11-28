using UnityEngine;

public class Item : MonoBehaviour
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
}
