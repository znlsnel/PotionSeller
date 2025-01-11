using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleReporter : MonoBehaviour
{
	public List<UnityEvent> _action = new List<UnityEvent>();

	public void StartParticle(int idx)
	{
		if (_action.Count <= idx)
			return;

		_action[idx]?.Invoke();
	}
}
