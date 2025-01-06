using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class AdmobManager : Singleton<AdmobManager>
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	bool isTestMode = true;
	const string rewardTestId = "ca-app-pub-3940256099942544/5224354917";
	const string rewardId = "ca-app-pub-5986236102536809/4328895340";
	RewardedAd _rewardedAd = null;
	 
	public override void Awake()
	{
		base.Awake();
		MobileAds.Initialize(initStatus => { });

#if UNITY_EDITOR
		isTestMode = true;
#else
	isTestMode = false;
#endif
	}

	bool _lock = false;
	public bool LoadRewardAd(Action rewardAction, Action close = null)
	{
		if (_lock)
			return false;

		_lock = true;
		UIHandler.instance.GetLoadingUI.StartLoading();

		if (_rewardedAd != null)
		{
			_rewardedAd.Destroy();
			_rewardedAd = null;
		}
		
		var adRequest = new AdRequest();
		 
		RewardedAd.Load(isTestMode ? rewardTestId : rewardId, adRequest,
		    (RewardedAd ad, LoadAdError error) =>
		    {
			    if (error != null || ad == null)
				    return;
			    			     
			    _rewardedAd = ad;
			    _rewardedAd.OnAdFullScreenContentClosed += () =>
			    {
				    _lock = false;
				    close?.Invoke();
				    UIHandler.instance.GetLoadingUI.EndLoading();
			    };

			    if (_rewardedAd != null && _rewardedAd.CanShowAd())
			    {
				    _rewardedAd.Show((Reward reward) =>
				    {

					    rewardAction?.Invoke();
					    if (rewardAction != null) 
						UIHandler.instance.GetLogUI.WriteLog("광고 보상 수령 완료!");
				    });
			    }
		    });
		return true;
	}
}
