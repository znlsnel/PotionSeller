using System.Collections;
using UnityEngine;

public class MonsterHpUI : HpBar
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	[SerializeField] RectTransform _rectTransform;
	bool _uiActive = true;
	Transform _hpUIRootPos;

	public override void Awake()
	{
		base.Awake();
		gameObject.SetActive(false);
		_rectTransform.localPosition = new Vector3(-100000, -100000, -100000);
	}
	public virtual void FixedUpdate()
	{
		MoveUI();
	}
	private void Start()
	{
		StartCoroutine(UpdateActivation());

	}

	// Update is called once per frame
	public void SetParentTransform(Transform uipos)
	{
		_hpUIRootPos = uipos; 
	}
	public void SetHpBar(bool on)
	{
		MoveUI();
		_uiActive = on;
		gameObject.SetActive(_slider.value > 0 && on);
	}

	void MoveUI()
	{
		if (_hpUIRootPos != null)
		{
			Vector2 pos = Camera.main.WorldToScreenPoint(_hpUIRootPos.position);
			_rectTransform.localPosition = _rectTransform.parent.InverseTransformPoint(pos);
		}
	}
	IEnumerator UpdateActivation()
	{
		yield return new WaitForSeconds(0.3f);
		gameObject.SetActive(_slider.value > 0 && _uiActive);
	}
}
