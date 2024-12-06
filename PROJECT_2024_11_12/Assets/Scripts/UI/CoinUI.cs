using TMPro;
using UnityEngine;

public class CoinUI : Singleton<CoinUI>
{
       [SerializeField] TextMeshProUGUI _text;
        long _coin = 0;
	 
	long T = (long)1000 * 1000 * 1000 * 1000;
	long B = (long)1000 * 1000 * 1000;
	long M = (long)1000 * 1000;
	long K = 1000;
	private void Start()
	{
		UpdateCoinText(); 
	}

	public void AddCoin(int coin)
        {
                _coin += coin;

		UpdateCoinText();

	}

	void UpdateCoinText()
	{
		long A = 1;
		string Level = "";

		if (_coin / T > 0)
		{
			A = T;
			Level = "T";
		}
		else if (_coin / B > 0)
		{
			A = B;
			Level = "B";
		}
		else if (_coin / M > 0)
		{
			A = M;
			Level = "M";
		}
		else if (_coin / K > 0)
		{ 
			A = K; 
			Level = "K";
		}

		_text.text = (_coin / A).ToString() + Level;
	}
}
