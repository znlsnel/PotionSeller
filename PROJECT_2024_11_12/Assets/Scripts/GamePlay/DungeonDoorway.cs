using UnityEngine;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class DungeonDoorway : Singleton<DungeonDoorway>
{
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] PlayerController _player;
        [SerializeField] GameObject _enter;
        [SerializeField] GameObject _exit;

	PlayerCombatController _playerCombat;
	MonsterPool[] monsterSpawners;

	public override void Awake()
        {
		base.Awake();
                _enter.GetComponent<DelegateColliderBinder>()._triggerEnter.AddListener((GameObject go) => EnterDungeon(go));
	        _exit.GetComponent<DelegateColliderBinder>()._triggerEnter.AddListener((GameObject go) => ExitDungeon(go));
                _player._onDead.AddListener(()=>ExitDungeon(_player.gameObject));
		_playerCombat = _player.GetComponent<PlayerCombatController>();

		monsterSpawners = FindObjectsByType<MonsterPool>(FindObjectsSortMode.None);
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

		if (pc.isDead == false)
			pc.HP = pc.MaxHP(); 

		_playerCombat.isInHuntZone = false;
		foreach (MonsterPool monster in monsterSpawners)
			monster.InitPool();

		ItemSpawner.instance.InitDungeon(); 
	}
	public bool isPlayerInDungeon() { return _playerCombat.isInHuntZone; }
}
