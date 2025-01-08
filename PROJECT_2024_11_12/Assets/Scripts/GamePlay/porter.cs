using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class porter : MonoBehaviour
{

        [SerializeField] RectTransform _max;
        [SerializeField] Transform _maxUIPos;

        [SerializeField] GameObject _player;
        NavMeshAgent _agent;
        PickupManager _pickup;
        void Start()
        { 
                _pickup = GetComponent<PickupManager>();
		_pickup.SetCarrySize(DataBase.instance._maxCarryItemCnt.GetValue());
                DataBase.instance._maxCarryItemCnt._onChangedLevel.AddListener(() =>
                {
			GetComponent<PickupManager>().SetCarrySize(DataBase.instance._maxCarryItemCnt.GetValue());
		});
		_agent = GetComponent<NavMeshAgent>();
          //      StartCoroutine(GointToPlayer());

		_agent.speed = (5 * DataBase.instance._speed.GetValue()) / 100;
                DataBase.instance._speed._onChangedLevel.AddListener(() =>
                {
                        _agent.speed = (7 * DataBase.instance._speed.GetValue()) / 100;
                });
	}


	IEnumerator GointToPlayer()
        {
                while( true)
                {
			if ((_agent.transform.position - _player.transform.position).magnitude > 10)
				_agent.transform.position = _player.transform.position;
                        else
			        _agent.SetDestination(_player.transform.position);

                        
                         
			yield return new WaitForSeconds(0.5f);
		}
        }

	private void FixedUpdate() 
	{
                if (_pickup._leftCarryCap == 0)
                {
                        Vector2 targetPos = Camera.main.WorldToScreenPoint(_maxUIPos.position);
                        _max.localPosition = _max.parent.InverseTransformPoint(targetPos);
                }

		if ((_agent.transform.position - _player.transform.position).magnitude > 10)
			_agent.transform.position = _player.transform.position;
		else
			_agent.SetDestination(_player.transform.position);

		_max.gameObject.SetActive(_pickup._leftCarryCap == 0);

	}
}
