using System.Collections;
using TMPro;
using UnityEngine;

public class LogUI : MonoBehaviour
{
        [SerializeField] TextMeshProUGUI _text;

        Coroutine _crt;
	private void Awake()
	{
                _text.text = "";
	}

	public void WriteLog(string text)
        {
                _text.text = text;
                if (_crt != null)
                        StopCoroutine(_crt);

                _crt = StartCoroutine(EraseLog());
	}
         
        IEnumerator EraseLog() 
        {
                yield return new WaitForSeconds(2.0f);
                _text.text = "";
                _crt = null;
	}
}
