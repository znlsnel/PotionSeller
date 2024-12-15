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

public class GPGSManager : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI _logText;
	private void Awake()
	{
		PlayGamesPlatform.Activate();
		//Social.localUser.Authenticate(ProcessAuthentication); 
		GPGSLogin();
	} 

	public void GPGSLogin()
	{
		PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
	}

	internal void ProcessAuthentication(SignInStatus status)
	{
		if (status == SignInStatus.Success)
		{
			string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
			string userID = PlayGamesPlatform.Instance.GetUserId();

			UIHandler.instance.GetLogUI.WriteLog($"google login : {userID}");
			DataBase.instance.LoadData(userID);  
		} 
	}
}
