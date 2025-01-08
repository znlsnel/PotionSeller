using GoogleMobileAds;
using GoogleMobileAds.Api;

using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AdmobManager : Singleton<AdmobManager>
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
//	bool isTestMode = true;
	//const string rewardTestId = "ca-app-pub-3940256099942544/5224354917";
	const string rewardId_1 = "ca-app-pub-5986236102536809/4328895340";
	const string rewardId_2 = "ca-app-pub-5986236102536809/8492539044";
	const string rewardId_3 = "ca-app-pub-5986236102536809/5866375708"; 

	string[] rewardIds = {rewardId_1,  rewardId_2, rewardId_3};
	RewardedAd[] _rewardedAds = { null, null, null }; 
	 
	public override void Awake()
	{
		base.Awake();
		MobileAds.Initialize(initStatus => { });

//#if UNITY_EDITOR
//		isTestMode = false;
//#else
//	isTestMode = false;
//#endif
	}

	private void Start()
	{
		for (int i = 0; i < 3; i++)
			LoadRewardAd(i);
	}

	bool _lock = false;

	public bool PlayRewardAd(Action rewardAction, Action close = null)
	{
		int idx = GetRewardAd();
		RewardedAd ad = idx >= 0 ?_rewardedAds[idx] : null;

		if (_lock || ad == null || ad.CanShowAd() == false)
		{
			UIHandler.instance.logUI.WriteLog("��� �� �ٽ� �õ����ּ���."); 
			return false; 
		} 

		UIHandler.instance.loadingUI.StartLoading();
		_lock = true;

		ad.OnAdFullScreenContentClosed += () =>
		{
			_lock = false;
			close?.Invoke();
			UIHandler.instance.loadingUI.EndLoading();
			Utils.instance.SetTimer(() => { LoadRewardAd(idx); }, 1.0f);
		};

		ad.Show((Reward reward) =>
		{
			rewardAction?.Invoke(); 
			if (rewardAction != null) 
				UIHandler.instance.logUI.WriteLog("���� ���� ���� �Ϸ�!");
		});

		
		return true;
	}

	void LoadRewardAd(int idx)
	{
		if (_rewardedAds[idx] != null && _rewardedAds[idx].CanShowAd())
		{
			//UIHandler.instance.logUI.WriteLog("�̹� �ε�� ���� �ֽ��ϴ�.");
			return;
		}

		var adRequest = new AdRequest(); 

		RewardedAd.Load(rewardIds[0], adRequest,
		(RewardedAd ad, LoadAdError error) =>
		{
			if (error != null || ad == null)
			{
			//	UIHandler.instance.logUI.WriteLog($"���� �ε� ����: {error.GetMessage()}");
				return;
			}

		//UIHandler.instance.logUI.WriteLog("���� �ε� �Ϸ�");
			_rewardedAds[idx] = ad;
		}); 
		return;
	}

	int GetRewardAd()
	{
		for (int i = 0; i < 3; i++)
		{
			if (_rewardedAds[i] != null && _rewardedAds[i].CanShowAd())
				return i;
		}

		return -1;
	}
}
