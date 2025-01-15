using System.Collections.Generic;
using UnityEngine;

public class ObjectSensor : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created

	HashSet<GameObject> overlapObjs = new HashSet<GameObject>();

	private void OnTriggerEnter(Collider other)
	{
		overlapObjs.Add(other.gameObject);
	}

	private void OnTriggerExit(Collider other)
	{ 
		overlapObjs.Remove(other.gameObject);
	}

	public List<GameObject> GetOvelapObjects()
	{
		List<GameObject> ret = new List<GameObject>();
		foreach (GameObject obj in overlapObjs)
			ret.Add(obj);

		return ret;
	}

	public GameObject FindTargetByLayer(string layer)
	{
		foreach (GameObject obj in overlapObjs)
		{
			LayerMask findLayerMask = LayerMask.GetMask("Player");
			if ((findLayerMask.value & (1 << obj.layer)) == findLayerMask.value)
				return obj;
		}
		return null;
	}
}
