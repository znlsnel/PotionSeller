using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class DungeonDoorway : Singleton<DungeonDoorway>
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] PlayerController _player;
        [SerializeField] GameObject _enter;
        [SerializeField] GameObject _exit;

	PlayerCombatController _playerCombat;
        void Awake()
        {
                _enter.GetComponent<DelegateColliderBinder>()._triggerEnter.AddListener((GameObject go) => EnterDungeon(go));
	        _exit.GetComponent<DelegateColliderBinder>()._triggerEnter.AddListener((GameObject go) => ExitDungeon(go));
                _player._onDead.AddListener(()=>ExitDungeon(_player.gameObject));
		_playerCombat = _player.GetComponent<PlayerCombatController>();
		 
	}
          
        void EnterDungeon(GameObject go)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player");
		if ((findLayerMask.value & (1 << go.gameObject.layer)) == 0)
			return;
		_playerCombat.isInHuntZone = true;

		
	}

	void ExitDungeon(GameObject go)
	{
		LayerMask findLayerMask = LayerMask.GetMask("Player");
		if ((findLayerMask.value & (1 << go.gameObject.layer)) == 0)
			return;

		var pc = go.GetComponent<PlayerController>();
		pc.HP = pc.MaxHP();

		_playerCombat.isInHuntZone = false;
		MonsterSpawner[] mss = FindObjectsByType<MonsterSpawner>(FindObjectsSortMode.None);
		foreach (MonsterSpawner monster in mss)
			monster.InitMonsters();

		ItemSpawner.instance.InitDungeon(); 
	}
	public bool isPlayerInDungeon() { return _playerCombat.isInHuntZone; }
}
