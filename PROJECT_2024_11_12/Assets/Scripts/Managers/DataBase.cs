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
	public class SaveDatas
	{
		public long coin = 0;
		public List<SkillLevelEntry> levels = new List<SkillLevelEntry>();
	}
	private void Start()
	{
		SaveData();
		LoadData();
	}
	public void SaveData()
        {
		var saveDatas = new SaveDatas();

		saveDatas.coin = CoinUI.instance.GetCoin();
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_speed), value = _speed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_hp), value = _hp.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_attack), value = _attack.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionSpawnSpeed), value = _potionSpawnSpeed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionPrice), value = _potionPrice.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_customerPurchaseCnt), value = _customerPurchaseCnt.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_itemDropRate), value = _itemDropRate.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_maxCarryItemCnt), value = _maxCarryItemCnt.GetLevel() });

		string json = JsonUtility.ToJson(saveDatas, true);
		File.WriteAllText(saveFilePath, json);
	}

        public void LoadData() 
        {
		if (!File.Exists(saveFilePath))
			return;

		string json = File.ReadAllText(saveFilePath);
		var saves = JsonUtility.FromJson<SaveDatas>(json);

		Dictionary<string, int> datas = new Dictionary<string, int>();
		foreach (var entry in saves.levels)
		{
			datas.Add(entry.key, entry.value);
		}

		CoinUI.instance.AddCoin(-CoinUI.instance.GetCoin() + saves.coin);

		void LoadSkill(SkillUpgradeSO skillSO, string key)
		{
			if (datas.ContainsKey(key))
				skillSO.SetLevel(datas[key]);
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
