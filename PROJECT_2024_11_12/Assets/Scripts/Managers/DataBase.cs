using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Runtime.Serialization.Json;
using System.Text;
using static UnityEngine.EventSystems.EventTrigger;
using static DataBase;
using System.Collections;

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
	private string fileName = "SaveData.dat";
	SaveDatas saveDatas = new SaveDatas();
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
		ScreenDebug.instance.DebugText("Call Load Data Function"); 

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

	void SaveData()
        {
		saveDatas = new SaveDatas();
		saveDatas.coin = CoinUI.instance.GetCoin();
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_speed), value = _speed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_hp), value = _hp.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_attack), value = _attack.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionSpawnSpeed), value = _potionSpawnSpeed.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_potionPrice), value = _potionPrice.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_customerPurchaseCnt), value = _customerPurchaseCnt.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_itemDropRate), value = _itemDropRate.GetLevel() });
		saveDatas.levels.Add(new SkillLevelEntry { key = nameof(_maxCarryItemCnt), value = _maxCarryItemCnt.GetLevel() });

		ScreenDebug.instance.DebugText($"COIN : {saveDatas.coin} "); 

		foreach (var data in saveDatas.levels)
		{
			ScreenDebug.instance.DebugText($"KEY : {data.key}, Value : {data.value} "); 

		}

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLastKnownGood,
			OnSavedGameOpened); 

		//string json = JsonUtility.ToJson(saveDatas, true);
		//File.WriteAllText(saveFilePath, json);
	}

	void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

		if (status == SavedGameRequestStatus.Success)
		{
			ScreenDebug.instance.DebugText("Save Success");
		//	Debug.Log("저장 성공");
			var update = new SavedGameMetadataUpdate.Builder().Build();
			UIHandler.instance.GetLogUI.WriteLog("save game...");
			var json = JsonUtility.ToJson(saveDatas);
			byte[] bytes = Encoding.UTF8.GetBytes(json);

		//	Debug.Log("저장 데이터 : " + bytes);
			savedGameClient.CommitUpdate(game, update, bytes, OnSavedGameWritten);
		}
		else
			ScreenDebug.instance.DebugText("Save Failed");
			//Debug.Log("저장 실패");

	}

	void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
	{
		if (status == SavedGameRequestStatus.Success)
		{
			ScreenDebug.instance.DebugText("Data Save Success");
	//		Debug.Log("저장 성공");
		}
		else
			ScreenDebug.instance.DebugText("Data Save Failed");
		//	Debug.Log("저장 실패");
	}

	 
	
	public bool LoadData()
	{

		var savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		if (savedGameClient == null)
			return false;
		
		savedGameClient.OpenWithAutomaticConflictResolution(fileName, DataSource.ReadCacheOrNetwork,
			ConflictResolutionStrategy.UseLastKnownGood, LoadGameData);
		return true;
	}

	void LoadGameData(SavedGameRequestStatus status, ISavedGameMetadata data)
	{
		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

		if (status == SavedGameRequestStatus.Success)
		{
			ScreenDebug.instance.DebugText("Load Success");
		//	Debug.Log("로드 성공");
			savedGameClient.ReadBinaryData(data, OnSavedGameDataRead);
		}
		else
		{
			ScreenDebug.instance.DebugText("Load Failed");
			UIHandler.instance.GetLogUI.WriteLog("Load Failed");
		}
		//	Debug.Log("로드 실패");
	}

	void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] loadedData)
	{
		string data = System.Text.Encoding.UTF8.GetString(loadedData);
		if (data == "")
		{
			ScreenDebug.instance.DebugText("There's No Data - Failed Load Game - Save Current Datas");
			UIHandler.instance.GetLogUI.WriteLog("Load Failed");
			SaveData();
			OpenLoadGame(); 
		}
		else
		{
			ScreenDebug.instance.DebugText("Game Data Load Success ");
			saveDatas = new SaveDatas(); 
			saveDatas = JsonUtility.FromJson<SaveDatas>(data);
			UIHandler.instance.GetLogUI.WriteLog("load game...");
			OpenLoadGame();
		}
	}
	 void OpenLoadGame()
	{
		Dictionary<string, int> datas = new Dictionary<string, int>();
		UIHandler.instance.GetLoadingUI.EndLoading();

		ScreenDebug.instance.DebugText($"COIN : {saveDatas.coin} "); 
		foreach (var entry in saveDatas.levels)
		{
			ScreenDebug.instance.DebugText($"KEY : {entry.key}, Value : {entry.value} ");
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
	}

}
