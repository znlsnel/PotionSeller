using Unity.VisualScripting;
using UnityEngine;

public class PotionCraftingTable : MonoBehaviour, IPlayerSensor
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] GameObject _player;
	IItemReceiver _playerItemReceiver;
        IItemSender _itemSender;

	public void EnterPlayer()
	{
		_itemSender.SendItem(_playerItemReceiver);
	}

	public void ExitPlayer()
	{
		_itemSender.CancelSend();
	}

	private void Awake()
	{
		_itemSender = GetComponent<SendItemManager>();
		_playerItemReceiver = _player.GetComponent<IItemReceiver>();
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
