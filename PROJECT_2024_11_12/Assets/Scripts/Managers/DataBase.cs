using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DataBase : Singleton<DataBase>
{
        [SerializeField] public SkillUpgradeSO _speed;
	[SerializeField] public SkillUpgradeSO _hp;
	[SerializeField] public SkillUpgradeSO _attack;
        [SerializeField] public SkillUpgradeSO _potionSpawnSpeed;
	[SerializeField] public SkillUpgradeSO _potionPrice;
	[SerializeField] public SkillUpgradeSO _customerPurchaseCnt;
	[SerializeField] public SkillUpgradeSO _itemDropRate;
	[SerializeField] public SkillUpgradeSO _maxCarryItemCnt;

	private string saveFilePath => Path.Combine(Application.persistentDataPath, "PotionSellerSaveData.json");

	[System.Serializable]
	public class SkillLevelEntry
	{
		public string key;
		public int value;
	}

	[System.Serializable]
	public class SkillLevels
	{
		public List<SkillLevelEntry> levels = new List<SkillLevelEntry>();
	}
	private void Start()
	{
		SaveData();
		LoadData();
	}
	public void SaveData()
        {
		var skillLevels = new SkillLevels();

		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_speed), value = _speed.GetLevel() });
		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_hp), value = _hp.GetLevel() });
		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_attack), value = _attack.GetLevel() });
		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_potionSpawnSpeed), value = _potionSpawnSpeed.GetLevel() });
		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_potionPrice), value = _potionPrice.GetLevel() });
		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_customerPurchaseCnt), value = _customerPurchaseCnt.GetLevel() });
		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_itemDropRate), value = _itemDropRate.GetLevel() });
		skillLevels.levels.Add(new SkillLevelEntry { key = nameof(_maxCarryItemCnt), value = _maxCarryItemCnt.GetLevel() });

		string json = JsonUtility.ToJson(skillLevels, true);
		File.WriteAllText(saveFilePath, json);
	}

        public void LoadData()
        {
		if (!File.Exists(saveFilePath))
			return;

		string json = File.ReadAllText(saveFilePath);
		var skills = JsonUtility.FromJson<SkillLevels>(json);
		Dictionary<string, int> skillLevels = new Dictionary<string, int>();
		foreach (var entry in skills.levels)
		{
			skillLevels.Add(entry.key, entry.value);
		}

		void LoadSkill(SkillUpgradeSO skillSO, string key)
		{
			if (skillLevels.ContainsKey(key))
				skillSO.SetLevel(skillLevels[key]);
		}

		LoadSkill(_speed, nameof(_speed));
		LoadSkill(_hp, nameof(_hp));
		LoadSkill(_attack, nameof(_attack));
		LoadSkill(_potionSpawnSpeed, nameof(_potionSpawnSpeed));
		LoadSkill(_potionPrice, nameof(_potionPrice));
		LoadSkill(_customerPurchaseCnt, nameof(_customerPurchaseCnt));
		LoadSkill(_itemDropRate, nameof(_itemDropRate));
		LoadSkill(_maxCarryItemCnt, nameof(_maxCarryItemCnt));
	}
}
