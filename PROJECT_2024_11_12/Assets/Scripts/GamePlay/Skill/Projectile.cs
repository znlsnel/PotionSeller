using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
	[SerializeField] protected float _speed;
	[SerializeField] int _damage;
	[SerializeField] string _targetLayer;
	[SerializeField] GameObject _effectPrefab;
	
	Vector3 _moveDir;
	bool _isActive = false;
	float _spawnTime = 0;

	[NonSerialized] public UnityEvent _onRelease = new UnityEvent();
	private void Update()
	{
		if (_isActive)
		{
			MoveProjectile();

			if (Time.time - _spawnTime > 5.0f)
			{
				gameObject.SetActive(false); 
			}
		}
	} 

	public  void FireProjectile(Vector3 targetPosition)
	{
		targetPosition.y = transform.position.y; 
		_moveDir = (targetPosition - transform.position);
		_moveDir = _moveDir.normalized;

		transform.LookAt(targetPosition);
		_isActive = true;
		_spawnTime = Time.time;
	}

	public virtual void MoveProjectile()
	{
		transform.position += _moveDir * _speed * Time.deltaTime;
	}

	public void StopMovement()
	{
		_isActive = false;
		_onRelease?.Invoke();
		gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player"); // 비트마스크 값 생성
		if ((findLayerMask.value & (1 << other.gameObject.layer)) != 0)
		{
			other.GetComponent<PlayerController>().OnDamage(gameObject, _damage);
			GameObject go = Instantiate<GameObject>(_effectPrefab);
			go.transform.position = transform.position;
			Utils.instance.SetTimer(() => { Destroy(go); }, 3.0f);
			StopMovement();
		}
	}
}

