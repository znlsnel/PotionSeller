using TMPro;
using UnityEngine;

public class CoinUI : Singleton<CoinUI>
{
       [SerializeField] TextMeshProUGUI _text;
        long _coin = 100000000;
	 
	private void Start()
	{
		UpdateCoinText(); 
	}
	public long GetCoint()
	{
		return _coin;
	}
	public void AddCoin(int coin)
        { 
                _coin += coin;

		UpdateCoinText();

	}

	void UpdateCoinText()
	{
		_text.text = Utils.instance.ConvertToCoin(_coin);
	}
}
