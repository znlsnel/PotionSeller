using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoadingUI : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] UnityEngine.UI.Slider _slider;
    void Start()
    {
		_slider.value = 0;      

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
                gameObject.SetActive(false); 
	}
}
