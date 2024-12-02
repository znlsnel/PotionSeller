using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	protected Slider _slider;
	protected HealthEntity _hpEntity;

	public virtual void Awake()
	{ 
		_slider = gameObject.GetComponentInChildren<Slider>();

	}

	public virtual void UpdateRate()
	{ 
		_slider.value = (float)_hpEntity.HP / _hpEntity.MaxHP;
	}

	public void SetParent(HealthEntity hpEntity)
	{
		_hpEntity = hpEntity;
		_hpEntity._onChangedHp.AddListener(() => UpdateRate());
	}
} 
 