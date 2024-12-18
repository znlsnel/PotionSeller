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
        [SerializeField] List<GameObject> _loginButtons = new List<GameObject>();

	public bool isLoaded => _slider.value >= 1.0f;

	void Start()
        {
	        _slider.value = 0;

                 LoginManager.instance._onLogin.AddListener(OnLogin);
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

        void OnLogin()
        {
                _sliderObject.SetActive(true);

		foreach (var go in _loginButtons)
			go.SetActive(false);

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
