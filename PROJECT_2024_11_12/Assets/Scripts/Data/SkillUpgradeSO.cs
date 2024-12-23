using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewSkillUpgrade", menuName = "Skill/Skill Upgrade")]
public class SkillUpgradeSO : ScriptableObject
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	[SerializeField] List<int> effect = new List<int>();

	[SerializeField, UnityEngine.Range(0, 10)] int level = 0;

	public UnityEvent _onChangedLevel = new UnityEvent();
	public int GetLevel() => level;

	private void Awake()
	{

	//	level = 0;

	}

	private void OnEnable()
	{
		level = 0;

	}

	public int GetValue()
	{
		return effect[level]; 
	}

	public void SetLevel(int value, bool isLoad = false)
	{
		level = value; 
		_onChangedLevel?.Invoke();

		if (isLoad == false ) 
			DataBase.instance.RegisterSave();
	}


}
