using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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


	Dictionary<string, float> _watch = new Dictionary<string, float>();
	
	public bool TimeCheck(MonoBehaviour _self, string functionName, float time)
	{
		string key = _self.name + functionName; 
		if (_watch.ContainsKey(key) == false)
		{
			_watch.Add(key, Time.time + time);
			return true;
		}
		else
		{
			if (_watch[key] <= Time.time)
			{
				_watch[key] = Time.time + time;
				return true;
			}
			else
				return false;
		}
	}
} 


public static class EncryptionHelper
{
	private static readonly int keySize = 256;
	private static readonly int IvSize = 16;
	private static readonly string _key = "@%V152B@B&*Osdas247ASNAL2D@@$SLK";
	public static string Encrypt(string plainText)
	{
		using (Aes aes = Aes.Create())
		{
			aes.Key = GenerateKey(_key);
			aes.IV = new byte[IvSize]; // 초기화 벡터 (0으로 초기화)

			using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
			{
				byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
				byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
				return Convert.ToBase64String(encryptedBytes);
			}
		}
	}

	public static string Decrypt(string encryptedText) 
	{
		using (Aes aes = Aes.Create())
		{
			aes.Key = GenerateKey(_key);
			aes.IV = new byte[IvSize];

			using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
			{
				byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
				byte[] plainBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
				return Encoding.UTF8.GetString(plainBytes);
			}
		}
	}

	// 키 생성 함수 (32바이트 길이로 고정)
	private static byte[] GenerateKey(string key)
	{
		return Encoding.UTF8.GetBytes(key.PadRight(32).Substring(0, 32));
	}
}