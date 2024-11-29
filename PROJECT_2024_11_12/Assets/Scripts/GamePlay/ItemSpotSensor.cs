using UnityEngine;
using UnityEngine.Events;

public class ItemSpotSensor : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created


	private void OnTriggerEnter(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) return;

		other.GetComponent<SendItemManager>()?.SendItem(GetComponent<PickupManager>());
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) return;

		other.GetComponent<SendItemManager>()?.CancelSend(); 

	}
}
