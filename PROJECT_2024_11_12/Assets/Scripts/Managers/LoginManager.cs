using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System;
using UnityEngine.SocialPlatforms;
using Unity.VisualScripting;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.Events;
using Firebase;
using Firebase.Auth;
using Google;
using System.Threading.Tasks;
using Firebase.Extensions;

public class LoginManager : Singleton<LoginManager>
{
	public UnityEvent _onLogin = new UnityEvent();
	public bool isLoginSuccess = false;

	public string GoogleAPI = "488591001695-bclacitl20t9g0vj8dd0hfaa7l2jm2jm.apps.googleusercontent.com";

	// af:86:fb:fd:4a:46:ef:c6:5d:2b:92:07:d5:0f:83:3f:97:47:8d:76
	// AF:86:FB:FD:4A:46:EF:C6:5D:2B:92:07:D5:0F:83:3F:97:47:8D:76
	private FirebaseAuth _auth;
	private FirebaseUser _user;

	private GoogleSignInConfiguration _configuration;

	private void Start()
	{
		StartCoroutine(InitPlugin());
	}

	IEnumerator InitPlugin()
	{
		yield return new WaitForSeconds(0.5f);

		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			Firebase.DependencyStatus status = task.Result;

			if (status == Firebase.DependencyStatus.Available)
			{
				UIHandler.instance.GetLogUI.WriteLog("Firebase 초기화 성공!");

				// FirebaseAuth 초기화
				_auth = FirebaseAuth.DefaultInstance;
				//_auth.StateChanged += OnChangedUserState;

			}
			else
			{
				UIHandler.instance.GetLogUI.WriteLog($"Firebase 초기화 실패: {status}");
			} 
		});

		_configuration = new GoogleSignInConfiguration
		{
			WebClientId = GoogleAPI,
			RequestIdToken = true,
		};
	}
	
	public void EmailRegister(string email, string password)
	{
		UIHandler.instance.GetLogUI.WriteLog($"Email : {email} \nPassword : {password}"); 

		_auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
			{
				UIHandler.instance.GetLogUI.WriteLog("회원가입 실패");
				return;
			}
			UIHandler.instance.GetLogUI.WriteLog("회원가입 완료!");
		});
	}

	public void EmailLogin(string email, string password) 
	{
		UIHandler.instance.GetLogUI.WriteLog($"Email : {email} \nPassword : {password}"); 
		_auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
			{ 
				UIHandler.instance.GetLogUI.WriteLog("로그인 실패"); 
				return;
			}  
			UIHandler.instance.GetLogUI.WriteLog("로그인 성공"); 
		});
	}
	  
	 
	public void LogOut()
	{
		_auth.SignOut();
	}


	public void GoogleLogin()
	{
	
		GoogleSignIn.Configuration = _configuration;
		GoogleSignIn.Configuration.UseGameSignIn = false;
		GoogleSignIn.Configuration.RequestIdToken = true; 
		GoogleSignIn.Configuration.RequestEmail = true;

		UIHandler.instance.GetLogUI.WriteLog("로그인이 눌리긴 했어..");
		GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnGoogleAuthFinished);
	} 
	private void OnGoogleAuthFinished(Task<GoogleSignInUser> task)
	{
		if (task.IsFaulted || task.IsCanceled)
			UIHandler.instance.GetLogUI.WriteLog("구글 로그인 실패 ");
		
		else
		{
			Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
			UIHandler.instance.GetLogUI.WriteLog("구글 로그인 완료. \nID Token: " + task.Result.IdToken);
			// Firebase 인증 처리
			_auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
			{
				if (task.IsCanceled || task.IsFaulted)
				{
					UIHandler.instance.GetLogUI.WriteLog("구글 로그인 실패 ");
					return;
				}
			});
			 
			//SignInWithFirebase(task.Result.IdToken);
			OnChangedUserState();
		}
	}
	private void SignInWithFirebase(string idToken)
	{
		Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
		_auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
		{
			if (task.IsFaulted)
			{
				Debug.LogError("Firebase Sign-In failed: " + task.Exception.Message);
			}
			else
			{
				FirebaseUser newUser = task.Result;
				Debug.Log($"Firebase Sign-In successful! User: {newUser.DisplayName}, Email: {newUser.Email}");
			}
		});
	}


	void OnChangedUserState(/*object obj, EventArgs arg*/) 
	{
		if (_auth.CurrentUser == _user) 
			return;

		bool signed =  _auth.CurrentUser != null;
		if (!signed && _user != null)
		{
			UIHandler.instance.GetLogUI.WriteLog("로그아웃");
		}

		_user = _auth.CurrentUser;
		if (signed)
		{
			UIHandler.instance.GetLogUI.WriteLog("로그인 성공");
			_onLogin?.Invoke();
			DataBase.instance.LoadData(_user.UserId);
		}
	} 
}
