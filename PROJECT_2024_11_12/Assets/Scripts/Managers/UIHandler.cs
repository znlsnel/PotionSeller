using UnityEngine;

public class UIHandler : Singleton<UIHandler>
{
     [SerializeField]  public JoystickController _joystick;
     [SerializeField]  public MainMenuUI _mainMenu;

	public bool _isOpenMainMenu => _mainMenu.isOpen();

}
