using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class EmailLogin : MonoBehaviour
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        LoginManager _loginManager;
        [SerializeField] TMP_InputField _emailText;
	[SerializeField] TMP_InputField _passwordText;
	[SerializeField] TextMeshProUGUI _secretPassword;



        void Start()
        {
		_loginManager = LoginManager.instance;
                gameObject.SetActive(false);
        }

	private void Update()
	{
		if (LoginManager.instance.isLogined)
			gameObject.SetActive(false);

                string str = "";
                for (int i = 0; i < _passwordText.text.Length; i++)
                        str += "*";
                _secretPassword.text = str;  

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
                _emailText.text = "";
		_passwordText.text = "";
		_secretPassword.text = "";
	}
}
