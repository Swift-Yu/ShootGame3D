/// <summary>
/// Sub launcher.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyPoolManager;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Weapon System/Sub Weapon")]
	public class SubWeapon : MonoBehaviour {

		public Ammunition ammunition;
		public SubEmitter subEmitter;
		public float radius;
		public int amount;
		public GameObject spawnEffect;

		private GameObject owner;
		private Faction faction;
		private GameEntity target;

		public void Init(GameObject owner,Faction faction, GameEntity target){
			this.owner = owner;
			this.faction = faction;
			this.target = target;
		}

		void OnSpawn(){
			float angle =0;
			float step = 360f/amount;
			GameObject tmp =null;

			for (int i=0; i<amount; i++){

				switch(subEmitter){
				case SubEmitter.Sphere:
					Vector3 position = transform.position + Random.onUnitSphere * radius;
					tmp = PoolManager.SpawnStart(ammunition.gameObject,position, Random.rotation );
					InitAmmunition( tmp);
					PoolManager.SpawnEnd( tmp);
					break;
				case SubEmitter.Cylinder:
					angle+= step;
					tmp = PoolManager.SpawnStart(ammunition.gameObject,transform.position, transform.rotation);
					tmp.transform.Translate( Vector3.up * radius, Space.Self);
					tmp.transform.RotateAround( transform.position, transform.forward, angle );
					InitAmmunition( tmp);
					PoolManager.SpawnEnd( tmp);
					break;
				}
			}

			if (spawnEffect){
				PoolManager.Spawn( spawnEffect,transform.position,Quaternion.identity);
			}
		}

		private void InitAmmunition( GameObject obj){

			if (obj){
				Ammunition[] tmpProjectile = obj.transform.GetComponentsInChildren<Ammunition>(true);
				if (tmpProjectile.Length>0){
					for (int z=0;z<tmpProjectile.Length;z++){
						tmpProjectile[z].Init( owner,faction,target);
					}
				}
			}

		}

	}

}