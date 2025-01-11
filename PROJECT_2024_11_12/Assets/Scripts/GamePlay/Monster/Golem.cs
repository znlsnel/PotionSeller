using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;

public class Golem : BossMonster
{
	[Space(10)]
	[SerializeField] SphereCollider _targetSensor;
	[SerializeField] GameObject _smashDownEP;
	[SerializeField] Transform _skillSpawnPos;
	[SerializeField] int _smashDownDamage;

	public void AE_SmashDown()
	{
		GameObject go = Instantiate<GameObject>(_smashDownEP);
		go.transform.position = _skillSpawnPos.position;

		Utils.instance.SetTimer(() => { Destroy(go); }, 10.0f);

		Vector3 center = _targetSensor.bounds.center;
		float radius = _targetSensor.radius;

	
		Collider[] result = Physics.OverlapSphere(center, radius);

		GameObject target = null;
		foreach (var t in result)
		{
			LayerMask findLayerMask = LayerMask.GetMask("Player");
			if ((findLayerMask.value & (1 << t.gameObject.layer)) == 0)
				continue;

			target = t.gameObject;
			break; 
		}
		if (target != null) 
			target.GetComponent<HealthEntity>()?.OnDamage(gameObject, _smashDownDamage);
	}

}
