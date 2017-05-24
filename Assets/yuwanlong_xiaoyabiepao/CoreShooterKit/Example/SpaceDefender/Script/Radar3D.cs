using UnityEngine;
using System.Collections;
using HedgehogTeam.CoreShooterKit;
using HedgehogTeam.EasyPoolManager;
using System.Collections.Generic;

public class Radar3D : MonoBehaviour {

	public Faction faction;
	public Faction missileFaction;
	public GameObject plot;
	public GameObject plotMissile;
	public float range = 1000;

	public Transform radarTransform;
	public float radius;

	private GameEntity[] gameEntities;


	void Update () {

		// Get all enemies from game manger
		PoolManager.DespawnPool(plot); 
		gameEntities = GameManager.Instance.GetEnemiesByPriority( faction, TargetPriority.Closest, false);

		for (int i=0;i<gameEntities.Length;i++){
			DrawPlot( gameEntities[i],plot);
		}


		PoolManager.DespawnPool(plotMissile); 
		gameEntities = GameManager.Instance.GetEntityByFaction( missileFaction);
		for (int i=0;i<gameEntities.Length;i++){
			DrawPlot( gameEntities[i],plotMissile);
		}
	}
		
	private void DrawPlot(GameEntity entity,GameObject radarPlot){

		if (Vector3.Distance(radarTransform.position,entity.transform.position)<= range){

			Vector3 position = radarTransform.position + (entity.transform.position / (range/radius));
			position = radarTransform.InverseTransformPoint( position);
			position.y = 0;
			PoolManager.Spawn( radarPlot,radarTransform.TransformPoint( position),Quaternion.identity);
		}
	}
}
