using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Counter : MonoBehaviour
{

        [SerializeField] Transform _waitingStartPos;
        [SerializeField] Transform _waitingEndPos;
        [SerializeField] float _waitingInterval = 1.0f;

	SendItemManager _itemSend;
	PickupManager _pickup;
        Queue<Customer> _customers = new Queue<Customer>();

	private void Awake()
	{
		_itemSend = GetComponent<SendItemManager>();
		_pickup = GetComponent<PickupManager>();
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
			if (_customers.Count > 0 && _itemSend.itemCnt > _customers.Peek()._requireItem)
                        {
                                _itemSend.SendItem(_customers.Peek()._pickup, _customers.Peek()._requireItem);
                                _customers.Peek().SetState(ECustomerState.Completed);
                                _customers.Dequeue();

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

		other.GetComponent<SendItemManager>()?.SendItem(_pickup);
	}

	private void OnTriggerExit(Collider other)
	{
		PlayerController pc = other.GetComponent<PlayerController>();
		if (pc == null) return;
		other.GetComponent<SendItemManager>()?.CancelSend();

	}


}
