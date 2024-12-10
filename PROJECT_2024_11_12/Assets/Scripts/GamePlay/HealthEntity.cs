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

	[NonSerialized] public GameObject _hpBar;
	[SerializeField] DamageUI _damageUI;
	public int HP { get { return _curHp; } set { if (value == HP) return;  _curHp = Mathf.Max(Mathf.Min(value, MaxHP()), 0); _onChangedHp?.Invoke(); } }
	public virtual int MaxHP() {  return _initHp;  } 
	public bool isDead { get { return HP == 0; } }

	protected virtual void Awake()
	{
		if (_hpBarPrefab != null)
		{
			_hpBar = Instantiate<GameObject>(_hpBarPrefab);
			_hpBar.GetComponent<HpBar>().SetParent(this);
			(_hpBar.GetComponent<HpBar>() as MonsterHpUI)?.SetParentTransform(_hpPos);

		}
		HP = _initHp; 
	}

	public void OnDamage(GameObject target, int damage)
	{
		if (HP == 0)
			return;

		_damageUI.SetDamage(damage); 

		HP = HP - damage;
		if (HP == 0)
		{
			Utils.instance.SetTimer(() => OnDead());
			Utils.instance.SetTimer(() => _damageUI.CloseUI(), 1.0f);

		}

	}
	 
	public abstract void OnDead();


	public abstract void TargetEnter(GameObject go);
	public abstract void TargetExit(GameObject go); 
}
