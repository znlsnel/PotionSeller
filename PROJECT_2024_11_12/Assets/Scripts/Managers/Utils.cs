using System;
using System.Collections;
using UnityEngine;

public class Utils : Singleton<Utils>	
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
