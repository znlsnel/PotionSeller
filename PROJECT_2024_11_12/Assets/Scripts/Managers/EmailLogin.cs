using System.Collections;
using TMPro;
using UnityEngine;

public class EmailLogin : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        LoginManager _loginManager;
        [SerializeField] TextMeshProUGUI _emailText;
	[SerializeField] TextMeshProUGUI _passwordText;

        void Start()
        {
                _loginManager = LoginManager.instance;
                gameObject.SetActive(false);
        }
         
        public void LoginEmail() 
        {
                if (Utils.instance.TimeCheck(this, "LoginEmail", 3.0f))
                        _loginManager.EmailLogin(_emailText.text, _passwordText.text);
                else
                        UIHandler.instance.GetLogUI.WriteLog("잠시 뒤에 시도해주세요."); 
        }

        public void RegisterEmail()
        {
		if (Utils.instance.TimeCheck(this, "RegisterEmail", 3.0f))
			_loginManager.EmailRegister(_emailText.text, _passwordText.text);
                else
			UIHandler.instance.GetLogUI.WriteLog("잠시 뒤에 시도해주세요.");
	}

      

        public void CloseUI()
        {
                gameObject.SetActive(false);
        }

        public void OpenUI()
        {
		gameObject.SetActive(true);

	}
}
