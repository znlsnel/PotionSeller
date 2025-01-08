using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Runtime.Serialization.Json;
using System.Text;


using System.Collections;
using UnityEngine.Events;
using static DataBase;
using Newtonsoft.Json;
using Firebase.Firestore;
using System;

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
	
	
	private string _userId => LoginManager.instance.GetUserId;
	private string saveFilePath => Path.Combine(Application.persistentDataPath, _userId+"SaveData.json");
	private Timestamp lastSaveTime = new Timestamp();
	public UnityEvent _onLoadData = new UnityEvent();

	public int GetHighestSkillLevel()
	{
		int ret = 99999;

		ret = Mathf.Min(_speed.GetLevel(), ret);
		ret = Mathf.Min(_hp.GetLevel(), ret);
		ret = Mathf.Min(_attack.GetLevel(), ret);
		ret = Mathf.Min(_potionSpawnSpeed.GetLevel(), ret);
		ret = Mathf.Min(_potionPrice.GetLevel(), ret);
		ret = Mathf.Min(_customerPurchaseCnt.GetLevel(), ret);
		ret = Mathf.Min(_itemDropRate.GetLevel(), ret);
		ret = Mathf.Min(_maxCarryItemCnt.GetLevel(), ret); 

		return ret;
	}
	[FirestoreData]
	public class SaveDatas
	{
		[FirestoreProperty]
		public string userId { get; set; } = "";

		[FirestoreProperty]
		[JsonConverter(typeof(FirestoreTimestampConverter))]
		public Timestamp date { get; set; }
		[FirestoreProperty]
		public long coin { get; set; } = 0;

		[FirestoreProperty]
		public List<SkillLevelEntry> levels { get; set; } = new List<SkillLevelEntry>();
	}

	[FirestoreData]
	public class SkillLevelEntry
	{
		[FirestoreProperty]
		public string key { get; set; }

		[FirestoreProperty]
		public int value { get; set; }
	}

	Coroutine saveJson;
	Coroutine saveCloud;

	public void RegisterSave()
	{
		if (saveJson != null)
			StopCoroutine(saveJson);

		if (saveCloud != null)
			StopCoroutine(saveCloud);

		saveJson = StartCoroutine(CallSaveToJson());
		saveCloud = StartCoroutine(CallSaveToCloud());
	}
	 
	IEnumerator CallSaveToJson() 
	{
		// 1�ʵ��� ���� ������ Local ����
		yield return new WaitForSeconds(1f);
		saveJson = null;
		SaveDataToLocal();
	}

	
	IEnumerator CallSaveToCloud()
	{
		// 5�ʿ� �ѹ��� cloud ���� - ���� ���ῡ ���� ���� ������ ���� ���࿡ ������Ʈ��
		yield return new WaitForSeconds(5.0f);
		saveCloud = null;
		SaveDataToCloud(); 
	}

	SaveDatas MakeSaveDatas()
        {
		SaveDatas saveDatas = new SaveDatas();
		saveDatas.userId = _userId;
		saveDatas.coin = CoinUI.instance.GetCoin();
		saveDatas.date = Timestamp.GetCurrentTimestamp();
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_speed), value = _speed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_hp), value = _hp.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_attack), value = _attack.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionSpawnSpeed), value = _potionSpawnSpeed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionPrice), value = _potionPrice.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_customerPurchaseCnt), value = _customerPurchaseCnt.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_itemDropRate), value = _itemDropRate.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_maxCarryItemCnt), value = _maxCarryItemCnt.GetLevel() });

		lastSaveTime = Timestamp.GetCurrentTimestamp();

		return saveDatas;

	}
	public void SaveDataToLocal() 
	{
		SaveDatas saveDatas = MakeSaveDatas();
		string json = JsonConvert.SerializeObject(saveDatas, Formatting.Indented);
		string encryptedJson = EncryptionHelper.Encrypt(json);
		File.WriteAllText(saveFilePath, encryptedJson);
		UIHandler.instance.logUI.WriteLog("���� ����..."); 
	}

	public void SaveDataToCloud()
	{
		SaveDatas saveDatas = MakeSaveDatas();
		FirebaseFirestore db = FirebaseFirestore.GetInstance("potionsellerkorea");

		db.Collection("userData").Document(_userId).SetAsync(new
		{
			date = saveDatas.date, 
			coin = saveDatas.coin, // ���� �ʵ�
			skillLevels = saveDatas.levels // ��ų �����͸� �迭�� ���� 
		}).ContinueWith(task => 
		{
			if (task.IsCompleted)
			{
				UIHandler.instance.logUI.WriteLog("Ŭ���� ����.."); 
			}
			else
			{
				//task.Exception
				UIHandler.instance.logUI.WriteLog("Ŭ���� ���� ����");
			}
		});
	}

	public void LoadGameData()
	{
		UIHandler.instance.logUI.WriteLog("���� �ҷ�����...");
		LoadDataFromLocal();
		LoadDataFromCloud();
	} 

	void LoadDataFromLocal()
	{
		if (File.Exists(saveFilePath) == false) 
			return;

#if UNITY_EDITOR == false 
	if (_userId == "")
		return; 
#endif

		string json = File.ReadAllText(saveFilePath); // ���� ������ �о��
		string decryptedJson = EncryptionHelper.Decrypt(json);
		//saveDatas = JsonUtility.FromJson<SaveDatas>(decryptedJson); // JSON �����͸� ��ü�� ������ȭ 
		SaveDatas saveDatas = JsonConvert.DeserializeObject<SaveDatas>(decryptedJson);

		ApplyLoadData(saveDatas);
	}

	void LoadDataFromCloud() 
	{
		FirebaseFirestore db = FirebaseFirestore.GetInstance("potionsellerkorea");
		SaveDatas saveDatas = null; 
		db.Collection("userData").Document(_userId).GetSnapshotAsync().ContinueWith(task =>
		{
			if (task.IsCompleted && task.Result.Exists)
			{
				DocumentSnapshot snapshot = task.Result; 
				Timestamp ts = snapshot.GetValue<Timestamp>("date");

				// �ֽ� �����Ͱ� �ƴ϶�� �ҷ����� ĵ�� �� ���� ����
				if (lastSaveTime.CompareTo(ts) > 0) 
				{
					SaveDataToCloud();
					return;
				}

				saveDatas = new SaveDatas();
				saveDatas.userId = _userId;
				saveDatas.date = ts;
				saveDatas.coin = snapshot.GetValue<long>("coin"); 
				saveDatas.levels = snapshot.GetValue<List<SkillLevelEntry>>("skillLevels");

				ApplyLoadData(saveDatas);
				UIHandler.instance.logUI.WriteLog("Ŭ���� ������ �ҷ����� ����");
			}
			else
			{
				//UIHandler.instance.GetLogUI.WriteLog("Ŭ���� ������ �ҷ����� ����"); 
			}
		});

		return;
	}
	 void ApplyLoadData(SaveDatas saveDatas)
	{
		if (saveDatas == null || saveDatas.userId != _userId) 
			return;

		Dictionary<string, int> datas = new Dictionary<string, int>();

		foreach (var entry in saveDatas.levels)
		{
			datas.Add(entry.key, entry.value);
		}

		CoinUI.instance.AddCoin(-CoinUI.instance.GetCoin() + saveDatas.coin, true);

		void LoadSkill(SkillUpgradeSO skillSO, string key)
		{
			if (datas.ContainsKey(key))
				skillSO.SetLevel(datas[key], true);
		}

		LoadSkill(_speed, nameof(_speed));
		LoadSkill(_hp, nameof(_hp));
		LoadSkill(_attack, nameof(_attack));
		LoadSkill(_potionSpawnSpeed, nameof(_potionSpawnSpeed));
		LoadSkill(_potionPrice, nameof(_potionPrice));
		LoadSkill(_customerPurchaseCnt, nameof(_customerPurchaseCnt));
		LoadSkill(_itemDropRate, nameof(_itemDropRate));
		LoadSkill(_maxCarryItemCnt, nameof(_maxCarryItemCnt));

		lastSaveTime = saveDatas.date;

		_onLoadData?.Invoke();
	}

}
 

public class FirestoreTimestampConverter : JsonConverter<Timestamp>
{
	public override void WriteJson(JsonWriter writer, Timestamp value, JsonSerializer serializer)
	{
		writer.WriteValue(value.ToDateTime().ToString("o"));
	}

	public override Timestamp ReadJson(JsonReader reader, Type objectType, Timestamp existingValue, bool hasExistingValue, JsonSerializer serializer)
	{
		DateTime dateTime = DateTime.Parse(reader.Value.ToString());
		return Timestamp.FromDateTime(dateTime);
	}

}