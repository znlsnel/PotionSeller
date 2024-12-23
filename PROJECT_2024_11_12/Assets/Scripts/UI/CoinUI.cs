using System.Collections;
using TMPro;
using UnityEngine;

public class CoinUI : Singleton<CoinUI>
{
       [SerializeField] TextMeshProUGUI _text;
        long _coin = 1000;

	public override void Awake() 
	{
		base.Awake();

		_coin = 0;


		UpdateCoinText(); 

	}
	public long GetCoin()
	{
		return _coin;
	}

	public void AddCoin(long coin, bool isLoad = false)
        { 
                _coin += coin;

		if (isLoad == false)
			DataBase.instance.RegisterSave();

		UpdateCoinText();  
	}



	void UpdateCoinText()
	{
		_text.text = Utils.instance.ConvertToCoin(_coin);
	}


}
