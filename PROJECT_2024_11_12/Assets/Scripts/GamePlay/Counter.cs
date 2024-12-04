using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Counter : MonoBehaviour, IPlayerSensor
{

	[SerializeField] GameObject _player;
	IItemSender _playerSender;
        [SerializeField] Transform _waitingStartPos;
        [SerializeField] Transform _waitingEndPos;
        [SerializeField] float _waitingInterval = 1.0f;

	IItemSender _itemSender;
	IItemReceiver _itemReceiver;
        Queue<Customer> _customers = new Queue<Customer>();

	private void Awake()
	{
		_itemSender = GetComponent<SendItemManager>();
		_itemReceiver = GetComponent<PickupManager>();
		_playerSender = _player.GetComponent<SendItemManager>();
	}
	private void Start()
	{
                StartCoroutine(Checkout());
	}

	public void EnqueueCustomer(Customer customer)
        {
                customer.MoveToTarget(GetWaitingPos());
		_customers.Enqueue(customer);
	}

        IEnumerator Checkout()  
        {
                while (true)
                {
			if (_customers.Count > 0 && _itemSender.GetItemCount() > _customers.Peek()._requireItem)
                        {
				_itemSender.SendItem(_customers.Peek()._pickup, _customers.Peek()._requireItem);

				Utils.instance.SetTimer(() =>
				{
					_customers.Peek().SetState(ECustomerState.Completed);
					_customers.Dequeue();
				}, 2.0f);
                               

                                int idx = 0;
                                foreach (Customer customer in _customers)
                                        customer.MoveToTarget(GetWaitingPos(idx++));
			}
			
			yield return new WaitForSeconds(2.0f);
		}
                
        }
	Vector3 GetWaitingPos(int idx = -1)
	{
		return _waitingStartPos.position +
			(_waitingInterval * (idx == -1 ? _customers.Count : idx) * (_waitingEndPos.position - _waitingStartPos.position).normalized);
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

	public void EnterPlayer()
	{
		_playerSender.SendItem(_itemReceiver);
	}

	public void ExitPlayer()
	{
		_playerSender.CancelSend();
	}
}
