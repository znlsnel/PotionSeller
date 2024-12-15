using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartLoadingUI : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] UnityEngine.UI.Slider _slider;
        [SerializeField] AudioClip _finishAudio;

        
        bool successLoad = false;
        public bool isLoaded => _slider.value >= 1.0f;

        void Start()
        {
	        _slider.value = 0;      

	        StartCoroutine(UpdateLoadingBar());

                DataBase.instance._onLoadData.AddListener(() => { successLoad = true; });
        }
         
        IEnumerator UpdateLoadingBar()
        {
                while (_slider.value < 1.0f)
                {
#if UNITY_EDITOR
			_slider.value += (Time.deltaTime / 1.5f); 
#else 
                        _slider.value += successLoad  == false ? Time.deltaTime / 15 : Time.deltaTime / 1.5f;
#endif
			yield return null;
		}

                yield return new WaitForSeconds(0.5f);
                AudioManager.instance.PlayAudioClip(_finishAudio);
                gameObject.SetActive(false); 
	}
}
