using System;
using System.Collections;
using UnityEngine;

public class Utils : Singleton<Utils>	
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	long T = (long)1000 * 1000 * 1000 * 1000;
	long B = (long)1000 * 1000 * 1000;
	long M = (long)1000 * 1000;
	long K = 1000;

	public string ConvertToCoin(long n)
	{
		long A = 1;
		string Level = "";

		if (n / T > 0)
		{
			A = T;
			Level = "T";
		}
		else if (n / B > 0)
		{
			A = B;
			Level = "B";
		}
		else if (n / M > 0)
		{
			A = M;
			Level = "M";
		}
		else if (n / K > 0)
		{
			A = K; 
			Level = "K";
		}

		return  (n / A).ToString() + Level;
	}
	public void SetTimer(Action action, float time = 0.0f)
	{
		StartCoroutine(TIMER(action, time));
	}

	IEnumerator TIMER(Action action, float time)
	{
		if (time == 0.0f)
			yield return null;
		else
			yield return new WaitForSeconds(time);
		action.Invoke();
	}

} 
