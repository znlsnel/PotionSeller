using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.Events;

public class GPGSManager : Singleton<GPGSManager>
{
	[SerializeField] TextMeshProUGUI _logText;
	public UnityEvent<bool> _onLogin = new UnityEvent<bool>();
	public bool isLoginSuccess = false;

	public override void Awake()
	{
		//Social.localUser.Authenticate(ProcessAuthentication); 
		//GPGSLogin(); 
	} 

	public void GPGSLogin()
	{
		PlayGamesPlatform.Activate();
		PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
	}   

	internal void ProcessAuthentication(SignInStatus status)
	{
		if (status == SignInStatus.Success)
		{
			string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
			string userID = PlayGamesPlatform.Instance.GetUserId();
			isLoginSuccess = true;
			UIHandler.instance.GetLogUI.WriteLog($"google login : {userID}");
			DataBase.instance.LoadData(userID);  
		}

		_onLogin?.Invoke(status == SignInStatus.Success);
		_onLogin.RemoveAllListeners();

#if UNITY_EDITOR
		DataBase.instance.LoadData("");
#endif
	}
}
