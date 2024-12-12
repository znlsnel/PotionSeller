using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
	[SerializeField] GameObject _loadingUI;

	private void Awake()
	{
		_loadingUI.SetActive(false);
	}

	public void StartLoading()
	{
		_loadingUI.SetActive(true);
	} 

	public void EndLoading()
	{
		_loadingUI.SetActive(false);

	}

	public bool isLoading => _loadingUI.activeSelf;

}
