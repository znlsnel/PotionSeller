using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
        [SerializeField] SkillUpgradeSO _skill;
        [SerializeField] TextMeshProUGUI _priceText;
	[SerializeField] List<Image> _state = new List<Image>();
	[SerializeField] AudioClip _successAudio;
	[SerializeField] AudioClip _failedAudio;

	private void Start()
	{
		UpdatePrice();
		UpdateColor();

		_skill._onChangedLevel.AddListener(() =>
		{
			UpdatePrice();
			UpdateColor();
		});
	}

	public void UpgradeSkill()
        {
		int price = (int)Mathf.Pow(4, _skill.GetLevel()+1); 
		if (_skill.GetLevel() == 10 || price > CoinUI.instance.GetCoin())
		{
			AudioManager.instance.PlayAudioClip(_failedAudio);
			return;
		}
		AudioManager.instance.PlayAudioClip(_successAudio);

		CoinUI.instance.AddCoin(-price);
		_skill.SetLevel(_skill.GetLevel() + 1);  
	}

	void UpdatePrice()
	{
		_priceText.text = _skill.GetLevel() == 10 ? "-" : Utils.instance.FormatGameNumber((long)Mathf.Pow(4, _skill.GetLevel()+1));
	} 

	void UpdateColor()
	{
		int level = _skill.GetLevel();
		
		for (int i = 1; i <= 5; i++)
		{
			if (level - 5 >= i)
				_state[i - 1].color = Color.red;
			else if (level >= i)
				_state[i - 1].color = Color.green;
			else
				_state[i - 1].color = Color.white;
		}
	}

	
	public void LoadAds()
	{
		if (!AdmobManager.instance.LoadRewardAd(() => { AudioManager.instance.PlayAudioClip(_successAudio); _skill.SetLevel(_skill.GetLevel() + 1); }))
			AudioManager.instance.PlayAudioClip(_failedAudio);

	}

}
