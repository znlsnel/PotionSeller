using System.Collections;
using UnityEngine;

public class MonsterHpUI : HpBar
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	[SerializeField] RectTransform _rectTransform;
	Transform _hpUIRootPos;

	public void SetParentTransform(Transform uipos) => _hpUIRootPos = uipos;

	public override void Awake()
	{
		base.Awake();
		gameObject.SetActive(false);
	}

	public virtual void FixedUpdate()
	{
		MoveUI();
	}

	public void SetActive(bool on)
	{
		if (_slider.value == 0.0f)
			return;

		MoveUI(); 
		gameObject.SetActive(on); 
	}

	public override void UpdateRate() 
	{
		base.UpdateRate();
		if (_slider.value == 0.0f)
			Utils.instance.SetTimer(() => { gameObject.SetActive(false); }, 1.0f);
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
