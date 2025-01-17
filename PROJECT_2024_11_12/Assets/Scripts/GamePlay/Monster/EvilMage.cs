using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EvilMage : MonsterController
{
	[SerializeField] GameObject _attackProjectile;
	[SerializeField] GameObject _skillProjectile;
	[SerializeField] Transform _spawnPosition;

	void AE_OnEvilMageAttack()
	{
		StartCoroutine(SpawnProjectile(3, 1.0f));
	}
	 
	void AE_OnEvilMageSkillI()
	{
		GameObject go = Instantiate<GameObject>(_skillProjectile);
		go.transform.position = _spawnPosition.position;
		go.GetComponent<Projectile>().FireProjectile(GetTargetPos());
	}

	IEnumerator SpawnProjectile(int cnt, float time)
	{
		int count = cnt;
		while (count > 0)
		{
			GameObject go = Instantiate<GameObject>(_attackProjectile);
			go.transform.position = _spawnPosition.position; 
			go.GetComponent<Projectile>().FireProjectile(GetTargetPos());
			count--;
			yield return time / cnt;
		}
		
	}
}
