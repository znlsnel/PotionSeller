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
using Firebase;
using Firebase.Auth;
using Google;
using System.Threading.Tasks;
using Firebase.Extensions;

public class LoginManager : Singleton<LoginManager>
{
	public UnityEvent _onLogin = new UnityEvent();
	public bool isLoginSuccess = false;

	private string webClientId = "103616091400-21tteuk42v83f81kk9g5dnttgcv2tr25.apps.googleusercontent.com";

	private FirebaseAuth _auth;
	private FirebaseUser _user;

	private GoogleSignInConfiguration _configuration;



	private void Start()
	{
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
		{
			Firebase.DependencyStatus status = task.Result;

			if (status == Firebase.DependencyStatus.Available)
			{
				Debug.Log("Firebase 초기화 성공!");

				// FirebaseAuth 초기화
				_auth = FirebaseAuth.DefaultInstance;
				Debug.Log("FirebaseAuth 인스턴스 생성 완료");
			}
			else
			{
				Debug.LogError($"Firebase 초기화 실패: {status}");
			}
		});
	//	_auth = FirebaseAuth.DefaultInstance; 
	}


	
	public void EmailRegister(string email, string password)
	{
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
		_auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
		{
			if (task.IsCanceled || task.IsFaulted)
			{ 
				UIHandler.instance.GetLogUI.WriteLog("로그인 실패"); 
				return;
			} 

			FirebaseUser user = task.Result.User;
			
			UIHandler.instance.GetLogUI.WriteLog($"로그인 성공!");
			CompleteLogin();
		});
	}
	  
	 
	public void LogOut()
	{
		_auth.SignOut();
	}


	public void GoogleSignInButton()
	{
		//GoogleSignIn.DefaultInstance.SignIn(); 
		// Google 로그인 시도
		GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthFinished);
		 
	}
	private void OnGoogleAuthFinished(Task<GoogleSignInUser> task)
	{
		if (task.IsFaulted)
		{
			Debug.LogError("Google Sign-In failed: " + task.Exception.Message);
		}
		else if (task.IsCanceled)
		{
			Debug.LogWarning("Google Sign-In was canceled.");
		}
		else
		{
			Debug.Log("Google Sign-In successful. ID Token: " + task.Result.IdToken);
			// Firebase 인증 처리
			SignInWithFirebase(task.Result.IdToken);
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


	void CompleteLogin() 
	{
		_onLogin?.Invoke();
		DataBase.instance.LoadData(_user.UserId);
	}
}
