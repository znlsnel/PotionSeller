using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartLoadingUI : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] UnityEngine.UI.Slider _slider;
        [SerializeField] AudioClip _finishAudio;

        [SerializeField] TextMeshProUGUI _log;
        [SerializeField] GameObject _sliderObject;

        public bool isLoaded => _slider.value >= 1.0f;
        
        void Start()
        {
	        _slider.value = 0;
		_log.text = "";

		_sliderObject.SetActive(false);
	}

	float lastClickTime = -3.0f;
        public void OnButton_Login()
        {
                if (Time.time - lastClickTime < 3.0f || LoginManager.instance.isLoginSuccess)
                        return; 

		lastClickTime = Time.time;
                LoginManager.instance._onLogin.AddListener(OnLogin);
		LoginManager.instance.GPGSLogin();
        }

        void OnLogin(bool login)
        {
                if (login)
                {
                        _sliderObject.SetActive(true);
			_log.text = "Login Success!";

			StartCoroutine(UpdateLoadingBar());
		}
                else
                {
                        _log.text = "Login Failure";
                }

#if UNITY_EDITOR
                _sliderObject.SetActive(true);
		StartCoroutine(UpdateLoadingBar());
#endif
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
