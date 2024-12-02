using UnityEngine;
using UnityEngine.Events;

public interface IPlayerSensor
{
	public void EnterPlayer();
	public void ExitPlayer();
}
public class ItemSpotSensor : MonoBehaviour, IPlayerSensor
{
	[SerializeField] GameObject _player;
	IItemSender _playerItemSender;
	IItemReceiver _itemReceiver;
	private void Start()
	{
		_itemReceiver = GetComponent<PickupManager>();
		_playerItemSender = _player.GetComponent<SendItemManager>();
	}
	public void EnterPlayer()
	{
		_playerItemSender.SendItem(_itemReceiver); 
	}

	public void ExitPlayer()
	{
		_playerItemSender.CancelSend();
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) return;

		EnterPlayer();
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) return; 
		 
		ExitPlayer();
	}
}
