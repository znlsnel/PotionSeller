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

        public bool isOpen()
        {
                return active; 
	}
         
        IEnumerator Checker()
        {
        while(true)
        {
                _internetCheckUI.SetActive(Application.internetReachability == NetworkReachability.NotReachable);
                if (active == false && _internetCheckUI.activeSelf)
                        DataBase.instance.LoadData();
                 
                active = _internetCheckUI.activeSelf;

		yield return new WaitForSeconds(0.3f);
        }

        }
}
