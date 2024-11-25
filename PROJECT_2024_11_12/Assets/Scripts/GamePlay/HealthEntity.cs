using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class HealthEntity : MonoBehaviour
{
        [SerializeField] GameObject _hpBarPrefab;
	[SerializeField] Transform _hpPos;
	[SerializeField] protected int _initHp = 3;
	int _curHp = 0;

	[NonSerialized] public UnityEvent _onChangedHp = new UnityEvent();

	GameObject _hpBar;

	public int HP { get { return _curHp; } set { _curHp = Mathf.Max(Mathf.Min(value, _initHp), 0); _onChangedHp?.Invoke(); } }
	public float HpRate { get { return (float)_curHp / _initHp; } }

	protected virtual void Awake()
	{
		if (_hpBarPrefab != null)
		{
			_hpBar = Instantiate<GameObject>(_hpBarPrefab);
			_hpBar.GetComponent<HpBar>().InitHpBar(this, _hpPos); 
		}
		_curHp = _initHp;
	}

	public void OnDamage(GameObject target, int damage)
	{
		if (HP == 0)
			return;


		HP = HP - damage;
		if (HP == 0) 
			Utils.instance.SetTimer(()=>OnDead());
		
	}
	 
	public abstract void OnDead();

}
