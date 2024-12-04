using Unity.VisualScripting;
using UnityEngine;

public class PotionCraftingTable : MonoBehaviour, IPlayerSensor
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] GameObject _player;
	IItemReceiver _playerItemReceiver;
	SendItemManager _itemSender;

	public void EnterPlayer()
	{
		_itemSender.SendItem(_player, _playerItemReceiver);
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
		LayerMask findLayerMask = LayerMask.GetMask("Player"); // 비트마스크 값 생성
		if ((findLayerMask.value & (1 << other.gameObject.layer)) == 0)
			return;
		EnterPlayer();
	}
	private void OnTriggerExit(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player"); // 비트마스크 값 생성
		if ((findLayerMask.value & (1 << other.gameObject.layer)) == 0)
			return;
		ExitPlayer();
	}
}
