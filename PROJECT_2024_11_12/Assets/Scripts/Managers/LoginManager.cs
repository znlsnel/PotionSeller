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

	public string myKey = "488591001695-bclacitl20t9g0vj8dd0hfaa7l2jm2jm.apps.googleusercontent.com";

	private FirebaseAuth _auth;
	private FirebaseUser _user;

	private GoogleSignInConfiguration _configuration;

	public string GetUserId => _user == null ? "" : _user.UserId;
	public bool IsLoginSuccessful => _auth != null && _auth.CurrentUser != null;
	public string GetUserEmail => _user == null ? "" :  _user.Email;

	public override void Awake()
	{
		base.Awake();

		_configuration = new GoogleSignInConfiguration{
			WebClientId = myKey,
			RequestIdToken = true,
		};
	}

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
				_auth = FirebaseAuth.DefaultInstance;
				_auth.StateChanged += OnChangedUserState;
			}
		});
	}
	 
	public void EmailRegister(string email, string password)
	{
		//UIHandler.instance.GetLogUI.WriteLog($"Email : {email} \nPassword : {password}"); 
		UIHandler.instance.GetLoadingUI.StartLoading();
		_auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
				UIHandler.instance.GetLogUI.WriteLog("회원가입 실패");
			else
				UIHandler.instance.GetLogUI.WriteLog("회원가입 완료!");
			
			UIHandler.instance.GetLoadingUI.EndLoading();

		});
	}

	public void EmailLogin(string email, string password) 
	{
		//UIHandler.instance.GetLogUI.WriteLog($"Email : {email} \nPassword : {password}"); 
		UIHandler.instance.GetLoadingUI.StartLoading();
		_auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
				UIHandler.instance.GetLogUI.WriteLog("로그인 실패"); 
			else
				UIHandler.instance.GetLogUI.WriteLog("로그인 성공");
			UIHandler.instance.GetLoadingUI.EndLoading(); 
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

		GoogleSignIn.DefaultInstance.SignOut();
		UIHandler.instance.GetLoadingUI.StartLoading();

		Utils.instance.SetTimer(() =>
		{
			GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
		}, 0.1f);
		
		//GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnGoogleAuthenticatedFinished);
	}
	private void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task)
	{
		if (task.IsFaulted || task.IsCanceled)
		{
			if (task.IsFaulted)
				UIHandler.instance.GetLogUI.WriteLog("구글 로그인 실패 ");
			UIHandler.instance.GetLoadingUI.EndLoading();
		}
		else
			SignInWithFirebase(task.Result.IdToken);
		 
	}
	private void SignInWithFirebase(string idToken)
	{
		Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
		_auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
				UIHandler.instance.GetLogUI.WriteLog("구글 로그인 실패 ");
			else
				UIHandler.instance.GetLogUI.WriteLog("구글 로그인 완료");
			UIHandler.instance.GetLoadingUI.EndLoading();
		});

	}


	void OnChangedUserState(object obj, EventArgs arg) 
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
			UIHandler.instance.GetLogUI.WriteLog($"로그인 성공");
		}
	} 

	public void StartGame()
	{
		if (_auth.CurrentUser == null)
			return; 

		_onLogin?.Invoke(); 
		DataBase.instance.LoadGameData(); 
	}
}
