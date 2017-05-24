using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyPoolManager;

public class ExplosionWithMesh : MonoBehaviour {

	public GameObject obj;
	public int amount;

	// Use this for initialization
	void OnSpawn() {
		for (int i=0;i<amount;i++){
			PoolManager.Spawn( obj,transform.position, Random.rotation);
		}
	}
	

}
