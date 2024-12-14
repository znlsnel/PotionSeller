using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHandler : Singleton<UIHandler>
{
     [SerializeField]  public JoystickController _joystick;
     [SerializeField]  public MainMenuUI _mainMenu;
	[SerializeField] public InternetChecker _internetChecker;
	[SerializeField] StartLoadingUI _startLoading;
	[SerializeField] LoadingUI _loading;
	[SerializeField] LogUI _logUI;

	public bool _isOpenUI => _mainMenu.isOpen() || _internetChecker.isConnectedInternet() || _loading.isLoading || _startLoading.isLoaded == false;
	public LoadingUI GetLoadingUI => _loading; 
	public LogUI GetLogUI => _logUI;

	public override void Awake()
	{
		base.Awake();
		_startLoading.gameObject.SetActive(true);
	}


}
