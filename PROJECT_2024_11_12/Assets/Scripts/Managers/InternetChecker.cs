using System.Collections;
using UnityEngine;

public class InternetChecker : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] GameObject _internetCheckUI;
        void Start()
        {
                StartCoroutine(Checker());
        }

         
        IEnumerator Checker()
        {
        while(true)
        {
                        _internetCheckUI.SetActive(Application.internetReachability == NetworkReachability.NotReachable);
	        yield return new WaitForSeconds(0.3f);
        }

        }
}
