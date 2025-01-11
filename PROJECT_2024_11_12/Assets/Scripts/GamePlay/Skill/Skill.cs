using UnityEngine;

public abstract class Skill : MonoBehaviour
{
	[SerializeField] protected  int _damage;
	protected GameObject _target;
	public void SetTarget(GameObject target)
	{
		_target = target;
	}

	public abstract void HitTarget(); 
} 
