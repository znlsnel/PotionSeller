using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] RectTransform _rectTransform;

	Slider _slider;
	HealthEntity _hpEntity;

	Transform _hpUIRootPos;

	void Start()
	{
		_slider = GetComponent<Slider>();
	} 

	public void InitHpBar(HealthEntity parent, Transform uipos)
	{
		_hpEntity = parent;
		_hpUIRootPos = uipos;
		parent._onChangedHp.AddListener(() => UpdateRate());
	}

	public void UpdateRate()
	{
		_slider.value = _hpEntity.HpRate;
	}
	 
	private void FixedUpdate()
	{
		if (_hpUIRootPos != null)
		{
			Vector2 pos = Camera.main.WorldToScreenPoint(_hpUIRootPos.position);
			_rectTransform.localPosition = _rectTransform.parent.InverseTransformPoint(pos);
		} 
	} 
} 
