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

	private string _userId => LoginManager.instance.UserId;
	private string saveFilePath => Path.Combine(Application.persistentDataPath, _userId+"SaveData.json");

	public UnityEvent _onLoadData = new UnityEvent();


	SaveDatas saveDatas = new SaveDatas();

	[FirestoreData]
	public class SaveDatas
	{
		[FirestoreProperty]
		public string userId { get; set; } = "";

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

	Coroutine save;

	public void RegisterSave()
	{
		if (save == null)
			save = StartCoroutine(Save());
	}
	 
	IEnumerator Save() 
	{
		yield return 1.0f;  
		SaveData();
		save = null;
	}

	public void SaveCloud()
	{
		SaveData(true);
	}

	void SaveData(bool cloud = false)
        {
		saveDatas = new SaveDatas();
		saveDatas.userId = _userId;
		saveDatas.coin = CoinUI.instance.GetCoin();
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_speed), value = _speed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_hp), value = _hp.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_attack), value = _attack.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionSpawnSpeed), value = _potionSpawnSpeed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionPrice), value = _potionPrice.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_customerPurchaseCnt), value = _customerPurchaseCnt.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_itemDropRate), value = _itemDropRate.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_maxCarryItemCnt), value = _maxCarryItemCnt.GetLevel() });

		if (cloud)
			SaveCloudData(); 
		else
			SaveJsonData();

		UIHandler.instance.GetLogUI.WriteLog("게임 저장...");
	}
	void SaveJsonData() 
	{
		string json = JsonConvert.SerializeObject(saveDatas, Formatting.Indented);
		string encryptedJson = EncryptionHelper.Encrypt(json);
		File.WriteAllText(saveFilePath, encryptedJson);
	}

	void SaveCloudData()
	{
		FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

		// SaveDatas 객체를 Firestore에 저장
		db.Collection("users").Document(_userId).SetAsync(new
		{
			coin = saveDatas.coin, // 코인 필드
			skillLevels = saveDatas.levels // 스킬 데이터를 배열로 저장
		}).ContinueWith(task =>
		{
			if (task.IsCompleted)
			{
				UIHandler.instance.GetLogUI.WriteLog("클라우드 저장 성공");
			}
			else
			{
				//task.Exception
				UIHandler.instance.GetLogUI.WriteLog("클라우드 저장 실패");
			}
		});

	}

	public void LoadData()
	{
		if (File.Exists(saveFilePath) == false) 
			return;

#if UNITY_EDITOR == false
	if (_userId == "")
		return; 
#endif

		string json = File.ReadAllText(saveFilePath); // 파일 내용을 읽어옴
		string decryptedJson = EncryptionHelper.Decrypt(json);
		//saveDatas = JsonUtility.FromJson<SaveDatas>(decryptedJson); // JSON 데이터를 객체로 역직렬화 
		saveDatas = JsonConvert.DeserializeObject<SaveDatas>(decryptedJson);
		OpenLoadGame();

		UIHandler.instance.GetLogUI.WriteLog("게임 불러오기...");
	}

	public void LoadCloudData()
	{
		FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

		db.Collection("users").Document(_userId).GetSnapshotAsync().ContinueWith(task =>
		{
			if (task.IsCompleted && task.Result.Exists)
			{
				DocumentSnapshot snapshot = task.Result;

				saveDatas = new SaveDatas();
				saveDatas.userId = _userId; 
				saveDatas.coin = snapshot.GetValue<long>("coin"); 
				saveDatas.levels = snapshot.GetValue<List<SkillLevelEntry>>("skillLevels");
				OpenLoadGame();

				Debug.Log("클라우드 데이터 불러오기 성공");
				UIHandler.instance.GetLogUI.WriteLog("클라우드 데이터 불러오기 성공");
			}
			else
			{
				UIHandler.instance.GetLogUI.WriteLog("클라우드 데이터 불러오기 실패");
			}
		});

		return;
	}


	 void OpenLoadGame()
	{
		if (saveDatas.userId != _userId)
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

		_onLoadData?.Invoke();
	}

}
