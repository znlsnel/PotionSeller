using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : HpBar
{
	[SerializeField] TextMeshProUGUI _text;

	// Update is called once per frame

	public override void UpdateRate()
	{
		base.UpdateRate();
		_text.text = $"{_hpEntity.HP} / {_hpEntity.MaxHP}";
	}
}
