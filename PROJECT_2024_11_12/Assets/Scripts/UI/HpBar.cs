using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;


public class HpBar : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] RectTransform _rectTransform;

	protected Slider _slider;
	HealthEntity _hpEntity;

	Transform _hpUIRootPos;

	bool isMonster = false;
	bool _uiActive = true;


	void Awake()
	{
		_slider = GetComponentInChildren<Slider>();
		gameObject.SetActive(false);
	}

	public void InitHpBar(HealthEntity parent, Transform uipos)
	{
		isMonster = (parent as MonsterController) != null;
		_hpEntity = parent;
		_hpUIRootPos = uipos;

		parent._onChangedHp.AddListener(() => UpdateRate());
		MoveUI();
		if (isMonster == false)
			gameObject.SetActive(true); 
	}

	public void UpdateRate()
	{ 
		_slider.value = _hpEntity.HpRate;
	}

	private void FixedUpdate()
	{
		MoveUI();
		gameObject.SetActive(_uiActive && _slider.value > 0); 
	}

	void MoveUI()
	{
		if (_hpUIRootPos != null)
		{
			Vector2 pos = Camera.main.WorldToScreenPoint(_hpUIRootPos.position);
			_rectTransform.localPosition = _rectTransform.parent.InverseTransformPoint(pos);
		}
	}

	public void SetHpBar(bool on)
	{
		_uiActive = on;
		gameObject.SetActive(_slider.value > 0 && on); 
	}



} 
