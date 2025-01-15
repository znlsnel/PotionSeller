using UnityEngine;

public class BossHandler : MonoBehaviour
{
     [SerializeField] BossMonster _boss;
	[SerializeField] int respawnTime;
	Transform _spawnPos;
	

	private void Start()
	{
		_spawnPos = _boss.gameObject.transform;
		_boss.InitMonster(_spawnPos.position, OffBoss);

	}

	void OffBoss() 
	{
		_boss.gameObject.SetActive(false);

		Utils.instance.SetTimer(() => { OnBoss(); }, respawnTime);
	}

	void OnBoss()
	{
		_boss.gameObject.transform.position = _spawnPos.position;
		_boss.gameObject.transform.rotation = _spawnPos.rotation;
		_boss.InitMonster(_spawnPos.position, OffBoss);

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<PlayerController>() != null)
			_boss.SetPlayerInThisStage(true);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<PlayerController>() != null)
		{
			_boss.SetPlayerInThisStage(false);
			_boss.InitHp();
		}
	}

} 
