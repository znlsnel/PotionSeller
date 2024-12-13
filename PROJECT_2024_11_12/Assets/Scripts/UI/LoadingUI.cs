using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
	[SerializeField] GameObject _loadingUI;

	int _calledLoad = 0;
	private void Awake()
	{
		_loadingUI.SetActive(false);
	}

	public void StartLoading()
	{
		_loadingUI.SetActive(true);
		_calledLoad++;
	} 

	public void EndLoading()
	{
		if (--_calledLoad == 0)
			_loadingUI.SetActive(false);
	}

	public bool isLoading => _loadingUI.activeSelf;

}
