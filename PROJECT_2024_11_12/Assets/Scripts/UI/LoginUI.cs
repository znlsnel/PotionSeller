using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoginUI : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] UnityEngine.UI.Slider _slider;
        [SerializeField] AudioClip _finishAudio;

        [Space(10)]
        [SerializeField] GameObject _sliderObject;

        [Space(10)]
        [SerializeField] GameObject _googleLoginBt;
        [SerializeField] GameObject _emailLoginBt;
        [SerializeField] GameObject _gameStartBt;
        [SerializeField] GameObject _logoutBt;
        [SerializeField] TextMeshProUGUI _idInfoText;

	public bool isLoaded => _slider.value >= 1.0f;
        bool startGame = false;

	void Start()
        {
	        _slider.value = 0;

                 LoginManager.instance._onLogin.AddListener(OnGameStart);
		_sliderObject.SetActive(false); 
	}

	float lastClickTime = -3.0f;
        public void LoginGoogle()
        {
                if (Utils.instance.TimeCheck(this, "LoginGoogle", 3.0f) == false || LoginManager.instance.isLoginSuccess)
                        return; 

		lastClickTime = Time.time;
                LoginManager.instance.GoogleLogin();
	//	LoginManager.instance.GPGSLogin();
        }

	private void Update()
	{
                bool login = LoginManager.instance.IsLoginSuccessful; 
                
                _googleLoginBt.SetActive(!login && !startGame);
#if UNITY_EDITOR
		_emailLoginBt.SetActive(!login && !startGame);
#endif 
                 
                _idInfoText.text = "Email : " + LoginManager.instance.GetUserEmail;
		_idInfoText.gameObject.SetActive(login && !startGame);
		_gameStartBt.SetActive(login && !startGame);
		_logoutBt.SetActive(login && !startGame);  
		
	}

      

	void OnGameStart()
        {
                _sliderObject.SetActive(true);
                startGame = true;



		StartCoroutine(UpdateLoadingBar());
	} 

	IEnumerator UpdateLoadingBar()
        {
                while (_slider.value < 1.0f)
                {
			_slider.value += (Time.deltaTime / 1.5f); 
			yield return null;
		}

                yield return new WaitForSeconds(0.5f);
                AudioManager.instance.PlayAudioClip(_finishAudio);
                gameObject.SetActive(false);

                
	}
}
