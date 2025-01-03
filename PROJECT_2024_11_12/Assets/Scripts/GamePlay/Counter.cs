using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class Counter : MonoBehaviour, IPlayerSensor
{

	[SerializeField] GameObject _porter;
	[SerializeField] CoinSpawner _coinSpawner;
        [SerializeField] Transform _waitingStartPos;
        [SerializeField] Transform _waitingEndPos;
        [SerializeField] float _waitingInterval = 1.0f;

	IItemSender _playerSender;
	IItemSender _itemSender;
	IItemReceiver _itemReceiver;
        Queue<Customer> _customers = new Queue<Customer>();

	public bool isActive = false;
	private void Awake()
	{
		_itemSender = GetComponent<SendItemManager>();
		_itemReceiver = GetComponent<PickupManager>();
		_playerSender = _porter.GetComponent<SendItemManager>();
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
	bool CheckCounterArrival(GameObject go)
	{
		Vector3 pos = go.transform.position;
		pos.y = _waitingStartPos.position.y; 
		return (pos - _waitingStartPos.position).magnitude < 0.1f;
	}

	IEnumerator Checkout()  
        {
                while (true)
                {
			if (_customers.Count > 0 && _itemSender.GetItemCount() >= _customers.Peek()._requireItem)
                        {
				isActive = true;
				while (CheckCounterArrival(_customers.Peek().gameObject) == false)
					yield return new WaitForSeconds(1.0f);

				_itemSender.SendItem(_customers.Peek()._pickup, _customers.Peek()._requireItem);

				Utils.instance.SetTimer(() =>
				{
					_coinSpawner.AddCoin(_customers.Peek()._requireItem * 10);
					_customers.Peek().SetState(ECustomerState.Completed);
					_customers.Dequeue();
					int idx = 0;
					foreach (Customer customer in _customers)
						customer.MoveToTarget(GetWaitingPos(idx++));
				}, 1.0f);
                               

                                
			}
			else
				isActive = false;

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
