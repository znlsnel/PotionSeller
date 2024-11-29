using UnityEngine;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
	[SerializeField] public EItemType _itemType;

	SphereCollider _collider;
	public UnityEvent _onRelease = new UnityEvent();
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

	public void Relase()
	{
		_collider.enabled = true;
		_onRelease?.Invoke();
	}
}
