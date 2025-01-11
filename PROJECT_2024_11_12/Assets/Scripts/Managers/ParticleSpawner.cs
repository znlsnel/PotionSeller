using System;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using static UnityEngine.ParticleSystem;


[Serializable]
public struct ParticleInfo
{
	public GameObject prefab;
	public Transform tp;
	public float len;
}
public class ParticleSpawner : MonoBehaviour
{
        [SerializeField] List<ParticleInfo> _particles = new List<ParticleInfo>();
	public void SpawnParticle()
        {
                foreach (var particle in _particles)
                {
			GameObject go = Instantiate<GameObject>(particle.prefab);
			go.transform.position = particle.tp.position;
			go.transform.rotation = particle.tp.rotation;

			Utils.instance.SetTimer(() =>
			{
				Destroy(go);
			}, particle.len);
		} 
               
        }
}
