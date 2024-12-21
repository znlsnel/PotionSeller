using UnityEngine;

public class CloudUI : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] AudioClip _open;
	[SerializeField] AudioClip _close;
	[SerializeField] AudioClip _success;
	[SerializeField] AudioClip _error;
	[Space(10)]
	[SerializeField] GameObject _mainPanel;
	[SerializeField] GameObject _cancelBt;

	private void Awake()
	{
		_mainPanel.SetActive(false);
		_cancelBt.SetActive(false);
	}
	public void OpenUI()
	{
		_mainPanel.SetActive(true);
		_cancelBt.SetActive(true);
		AudioManager.instance.PlayAudioClip(_open);

	}

	public void CloseUI()
	{
		_mainPanel.SetActive(false);
		_cancelBt.SetActive(false);
		AudioManager.instance.PlayAudioClip(_close);
	}

	public void SaveCloud()
	{
		if (UIHandler.instance._internetChecker.isConnectedInternet() == false)
		{
			AudioManager.instance.PlayAudioClip(_error);
			return;
		}
		AudioManager.instance.PlayAudioClip(_success);
		DataBase.instance.SaveDataToCloud();
	}

	public void LoadCloud()
	{
		if (UIHandler.instance._internetChecker.isConnectedInternet() == false)
		{
			AudioManager.instance.PlayAudioClip(_error);
			return;
		}
		AudioManager.instance.PlayAudioClip(_success);
		DataBase.instance.LoadGameData();
	}
}


