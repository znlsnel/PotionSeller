using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] RectTransform _rectTransform;

	Slider _slider;
	HealthEntity _hpEntity;

	Transform _hpUIRootPos;

	void Awake()
	{
		_slider = GetComponentInChildren<Slider>();
		gameObject.SetActive(false);
	}

	public void InitHpBar(HealthEntity parent, Transform uipos)
	{
		_hpEntity = parent;
		_hpUIRootPos = uipos;
		MoveUI();
		gameObject.SetActive(true);
		parent._onChangedHp.AddListener(() => UpdateRate());
	}

	public void UpdateRate()
	{ 
		_slider.value = _hpEntity.HpRate;
		gameObject.SetActive(_slider.value > 0);
	}
	 
	private void FixedUpdate()
	{
		MoveUI();
	}
	
	void MoveUI()
	{
		if (_hpUIRootPos != null)
		{
			Vector2 pos = Camera.main.WorldToScreenPoint(_hpUIRootPos.position);
			_rectTransform.localPosition = _rectTransform.parent.InverseTransformPoint(pos);
		}
	}
} 
