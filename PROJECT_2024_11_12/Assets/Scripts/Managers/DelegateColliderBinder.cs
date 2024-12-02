using UnityEngine;
using UnityEngine.Events;

public class DelegateColliderBinder : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public UnityEvent<GameObject> _triggerEnter;
        public UnityEvent<GameObject> _triggerExit;

	private void OnTriggerEnter(Collider other)
	{
		_triggerEnter?.Invoke(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{
		_triggerExit?.Invoke(other.gameObject); 
	}
}
