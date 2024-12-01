using UnityEngine;

public class PotionCraftingTable : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        SendItemManager _send;
	private void Awake()
	{
		_send = GetComponent<SendItemManager>();
	}
	private void OnTriggerEnter(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) return;

		_send.SendItem(pc.GetComponent<PickupManager>());
	}
	private void OnTriggerExit(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) return;
		_send.CancelSend(); 
	}
}
