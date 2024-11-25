using System;
using UnityEngine;
using UnityEngine.Events;

public class HealthEntity : MonoBehaviour
{
        [SerializeField] GameObject _hpBarPrefab;
	[SerializeField] Transform _hpPos;
	[SerializeField] int _initHp = 3;
	int _curHp = 0;

	[NonSerialized] public UnityEvent _onChangedHp = new UnityEvent();

	public int HP { get { return _curHp; } set { _curHp = value; _onChangedHp?.Invoke(); } }
	public float HpRate { get { return (float)_curHp / _initHp; } }

	protected virtual void Awake()
	{
		if (_hpBarPrefab != null)
		{
			GameObject go = Instantiate<GameObject>(_hpBarPrefab);
			go.GetComponent<HpBar>().InitHpBar(this, _hpPos);
		}
	}


}
