using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

using Button = UnityEngine.UI.Button;

public class MainMenuUI : MonoBehaviour
{ 
	[SerializeField] GameObject _closeBt; 
	[SerializeField] GameObject _menu;
	[SerializeField] AudioClip _openSound;
	[SerializeField] AudioClip _closeSound;
	private void Awake()
	{
		_closeBt.SetActive(false);
		_menu.SetActive(false);

	}

	public void BT_OpenUI()
	{
		AudioManager.instance.PlayAudioClip(_openSound);
		_closeBt.SetActive(true); 
		_menu.SetActive(true);
	}

	public void BT_CloseUI()
	{
		AudioManager.instance.PlayAudioClip(_closeSound);
		_closeBt.SetActive(false);
		_menu.SetActive(false);

	}
	public bool isOpen()
	{
		return _menu.activeSelf;
	}
}
