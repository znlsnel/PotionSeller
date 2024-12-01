using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Notifications.iOS;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum ECustomerState
{
        MovingToStore,
        Completed
}

public class Customer : MonoBehaviour
{
	#region CUSTOM STYLE
	[SerializeField] List<GameObject> body;
        [SerializeField] List<GameObject> back;
        [SerializeField] List<GameObject> headAccessories;
        [SerializeField] List<GameObject> glass;
        [SerializeField] List<GameObject> faceAccessories;
        [SerializeField] List<GameObject> eye;
        [SerializeField] List<GameObject> mouth;
        [SerializeField] List<GameObject> hair;
        [SerializeField] List<GameObject> head;
        [SerializeField] List<GameObject> headArmor;
        [SerializeField] List<GameObject> eyebow;
        [SerializeField] List<GameObject> hat;
        [Space(10)]
	#endregion
	


	[NonSerialized] public PickupManager _pickup;
        [NonSerialized] public int _requireItem = 0;
	[NonSerialized] public UnityEvent _onRelase = new UnityEvent();

	Counter _counter;
	Transform _endPos;

	NavMeshAgent _agent;

        private ECustomerState _state;


	private void Awake()
	{ 
		_agent = GetComponent<NavMeshAgent>();
		_pickup = GetComponent<PickupManager>();
	}
	private void Start()
	{
                SetCustomerStyle();
	}

        public void SetState(ECustomerState next)
	{
                _state = next;
		switch (_state)
                {
                        case ECustomerState.MovingToStore:
                                MovingToStoreState();
				break;

                        case ECustomerState.Completed:
                                CompletedState();
				break;
                }
	}

        public void MoveToTarget(Vector3 targetPos)
        {
                _agent.SetDestination(targetPos);
        }

        void MovingToStoreState()
        {
                _counter.EnqueueCustomer(this);
	}

        void CompletedState()
        {
                _agent.SetDestination(_endPos.position);
                StartCoroutine(CheckArrival());
	}

        public void InitCustomer(Counter counter, Transform endPos, UnityAction relaseAction)
        { 
                _onRelase.RemoveAllListeners();
                _onRelase.AddListener(relaseAction); 

                _counter = counter;
                _endPos = endPos;

                SetState(ECustomerState.MovingToStore);
	}

        IEnumerator CheckArrival()
        {
                if (_agent.remainingDistance == 0)
                {
                        Utils.instance.SetTimer(()=>RelaseCustomer(), 1.0f);
                        yield break;
                }
                yield return new WaitForSeconds(0.3f);
        }

        public void RelaseCustomer()
        {
                _pickup.ClearItem(); 
		_onRelase?.Invoke();
        }
	public void SetCustomerStyle()
        {
                bool hasHeadArmor = Random.Range(1, 10) > 9;

                SetList(faceAccessories, !hasHeadArmor, false);
                SetList(headArmor, hasHeadArmor);
                SetList(eyebow, !hasHeadArmor);
                SetList(mouth, !hasHeadArmor);
                SetList(head, !hasHeadArmor);
                SetList(body, true); 
                SetList(glass, !hasHeadArmor, false);
                SetList(back, !hasHeadArmor, false);
                SetList(eye, !hasHeadArmor);
		
                bool hasHat =  Random.Range(1, 10) > 7;

                SetList(hat, !hasHeadArmor && hasHat); 
                SetList(hair, !hasHeadArmor && !hasHat);
                SetList(headAccessories, !hasHeadArmor && !hasHat, false);
        } 
          
        void SetList(List<GameObject> list, bool active, bool isEssential = true)
        {
                int idx = active ? Random.Range(isEssential ? 0 : -list.Count*2, list.Count) : -1;
                for (int i = 0; i < list.Count; i++) 
                        list[i].SetActive(i == idx);
        }
}
