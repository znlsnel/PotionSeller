using TMPro;
using UnityEngine;

public class CoinUI : Singleton<CoinUI>
{
       [SerializeField] TextMeshProUGUI _text;
        long _coin = 1000;
	  
	private void Start()
	{
		UpdateCoinText(); 
	}
	public long GetCoin()
	{
		return _coin;
	}
	public void AddCoin(long coin)
        { 
                _coin += coin;

		UpdateCoinText();

	}

	void UpdateCoinText()
	{
		_text.text = Utils.instance.ConvertToCoin(_coin);
	}
}
