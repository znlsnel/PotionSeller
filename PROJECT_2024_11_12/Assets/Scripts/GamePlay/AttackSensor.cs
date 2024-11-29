using UnityEngine;

public class AttackSensor : MonoBehaviour
{
        [SerializeField] string _findLayer = "";
	[SerializeField] HealthEntity _parent;

	private void OnTriggerEnter(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask(_findLayer); // 비트마스크 값 생성
		if ((findLayerMask.value & (1 << other.gameObject.layer)) != 0)
		{
			HealthEntity he = other.GetComponent<HealthEntity>();
			if (he != null && he.HP == 0)
				return;

			_parent.TargetEnter(other.gameObject);

		}

	}

	private void OnTriggerExit(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask(_findLayer); // 비트마스크 값 생성
		if ((findLayerMask.value & (1 << other.gameObject.layer)) != 0)
			_parent.TargetExit(other.gameObject);
		
	} 
} 
