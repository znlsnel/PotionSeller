using System.Collections;
using UnityEngine;

public class InternetChecker : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] GameObject _internetCheckUI;
        bool active = true;
        void Start()
        {
                StartCoroutine(Checker());
        }

        public bool isConnectedInternet()
        {
                return !(Application.internetReachability == NetworkReachability.NotReachable);   
	}
         
        IEnumerator Checker()
        {
                while(true)
                { 
                        _internetCheckUI.SetActive(!isConnectedInternet());                 
                        active = _internetCheckUI.activeSelf;

		        yield return new WaitForSeconds(0.3f);
                }

        }
}
