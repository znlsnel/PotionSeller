using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class porter : MonoBehaviour
{
        [SerializeField] GameObject _player;
        NavMeshAgent _agent;
    void Start()
    {
		_agent = GetComponent<NavMeshAgent>();
                StartCoroutine(GointToPlayer());
    }

    // Update is called once per frame
        IEnumerator GointToPlayer()
        {
                while( true)
                {
			if ((_agent.transform.position - _player.transform.position).magnitude > 10)
				_agent.transform.position = _player.transform.position;
                        else
			        _agent.SetDestination(_player.transform.position);

                        
                         
			yield return new WaitForSeconds(0.1f);
		}
        }
}
