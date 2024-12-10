using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class DamageUI : MonoBehaviour
{
        [SerializeField] Transform _uiPos;
        [SerializeField] RectTransform _rect;
	[SerializeField] List<TextMeshProUGUI> _texts = new List<TextMeshProUGUI>();
	int idx = 0;
	private void Start()
	{
		CloseUI();
		//gameObject.SetActive(false); 
	}
	private void OnEnable()
	{

		//foreach (var text in _texts)
		//	text.gameObject.SetActive(false);
	}
	
	public void SetDamage(int damage)
	{
		UpdatePos(); 
	//	gameObject.SetActive(true);
		String str =  damage.ToString();
		if (decimal.TryParse(str, out decimal number))
		{
			// 천 단위 구분 쉼표 추가
			string formattedNumber = number.ToString("N0"); 
			_texts[idx].text = formattedNumber;
		}
		_texts[idx].gameObject.SetActive(true);
		_texts[idx].GetComponent<Animator>().Rebind();
		idx = (idx + 1) % _texts.Count; 
	}

	public void CloseUI()
	{
		foreach (var text in _texts)
			text.gameObject.SetActive(false); 
	//	gameObject.SetActive(false);
	}
	// Update is called once per frame
	void FixedUpdate() 
	{
		UpdatePos();
	} 

	void UpdatePos()
	{
		Vector2 targetPos = Camera.main.WorldToScreenPoint(_uiPos.position);
		_rect.localPosition = _rect.parent.InverseTransformPoint(targetPos);
	}
}
 