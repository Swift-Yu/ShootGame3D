using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyPoolManager;

namespace HedgehogTeam.CoreShooterKit{
	
	[RequireComponent (typeof(DespawnObject))]
	public abstract class Ammunition : LethalEntity {

		#region Members
		public float lifeTime = 1;
		
		public float rayAdvance = 1f;
		public float rayRadius = 0.1f;

		public SubWeapon subWeapon;

		public GameObject impactEffect;
		public GameObject deathEffect;

		public GameObject timedEffect;
		public float time;

		public GameEntity target;

		protected RaycastHit hitInfo;
		protected bool isLifeEnd = true;
		protected bool isOnDespawn = false;
		#endregion

		#region PoolManager
		public virtual void OnSpawn(){
			isOnDespawn = false;
			isLifeEnd = true;

			if (timedEffect){
				Invoke("LaunchTimedEffect",time);
			}
		}

		public virtual void OnDespawn(){
			isOnDespawn = true;
			CancelInvoke();
			if ( isLifeEnd){

				if (subWeapon){
					
					SubWeapon sub = PoolManager.SpawnStart( subWeapon.gameObject, cachedTransform.position,cachedTransform.rotation).GetComponent<SubWeapon>();
					sub.Init( owner,faction,target);
					PoolManager.SpawnEnd( sub.gameObject);
				}

				if (deathEffect){
					PoolManager.Spawn( deathEffect, cachedTransform.position,cachedTransform.rotation);
				}
			}
		}
		#endregion

		#region Private & Protected methods
		protected bool RayCast(float distance){
			return Physics.SphereCast( cachedTransform.position,rayRadius, cachedTransform.forward,out hitInfo, distance, layerMask,QueryTriggerInteraction.Ignore);
		}

		private void LaunchTimedEffect(){
			PoolManager.Spawn( timedEffect, cachedTransform.position,cachedTransform.rotation);
		}
		#endregion
	
		public void Init(GameObject owner, Faction faction, GameEntity target){
			this.owner = owner;
			this.faction = faction;
			this.target = target;
		}
	}

}
