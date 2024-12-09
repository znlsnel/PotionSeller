using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Button = UnityEngine.UI.Button;

public class MainMenuUI : MonoBehaviour
{ 
	[SerializeField] GameObject _closeBt; 
	[SerializeField] GameObject _menu;

	private void Start()
	{
		_closeBt.SetActive(false);
		_menu.SetActive(false); 

	}
	public void BT_OpenUI()
	{
		_closeBt.SetActive(true); 
		_menu.SetActive(true);
	}

	public void BT_CloseUI()
	{
		_closeBt.SetActive(false);
		_menu.SetActive(false);

	}

}
