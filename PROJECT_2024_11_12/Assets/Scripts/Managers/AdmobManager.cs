using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdmobManager : Singleton<AdmobManager>
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	bool isTestMode = true;
	const string rewardTestId = "ca-app-pub-3940256099942544/5224354917";
	const string rewardId = "123";
	RewardedAd _rewardedAd = null;
	public override void Awake()
	{
		base.Awake();
		MobileAds.Initialize(initStatus => { });
	}
	public void LoadRewardAd(Action action)
	{
		if (_rewardedAd != null)
		{
			_rewardedAd.Destroy();
			_rewardedAd = null;
		}

		var adRequest = new AdRequest();
		 
		// send the request to load the ad.
		RewardedAd.Load(isTestMode ? rewardTestId : rewardId, adRequest,
		    (RewardedAd ad, LoadAdError error) =>
		    {
			    // if error is not null, the load request failed.
			    if (error != null || ad == null)
				    return;
			    
			    Debug.Log("Rewarded ad loaded with response : "
			    + ad.GetResponseInfo());

			    _rewardedAd = ad;

			    if (_rewardedAd != null && _rewardedAd.CanShowAd())
			    {
				    _rewardedAd.Show((Reward reward) =>
				    {
					    action?.Invoke();
					    // TODO: Reward the user.
					    Debug.Log("리워드 보상!");
				    });
			    }
		    });


	}
}
