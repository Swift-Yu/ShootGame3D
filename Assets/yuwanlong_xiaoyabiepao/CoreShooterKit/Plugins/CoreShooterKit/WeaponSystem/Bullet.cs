/// <summary>
/// Bullet.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyPoolManager;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Weapon System/Bullet")]
	public class Bullet : Ammunition {

		#region Members
		public ProjectileType projectileType = ProjectileType.Velocity;
		public DamageAt projectileDamage = DamageAt.Contact;
		public float velocity = 200;
		public ForceMode projectileForceMode;

		private float _velocity;
		private Rigidbody cachedRigidbody;

		public bool showProperties = true;
		public bool showDamage = false;
		public bool showPhysic = false;
		public bool showEffect = false;

		#endregion

		#region Monobehaviour Callback
		public override void Awake (){
			base.Awake();

			cachedRigidbody = GetComponent<Rigidbody>();
			if (!cachedRigidbody && projectileType== ProjectileType.Physic){
				Debug.LogWarning("No rigidbody on projectile : " + gameObject.name);
			}

			if (projectileType == ProjectileType.Velocity){
				_velocity = velocity/3.6f;
			}
			else{
				_velocity = velocity;
			}

			if (projectileType != ProjectileType.Instant){
				GetComponent<DespawnObject>().lifeTime = lifeTime;
			}
			else{
				GetComponent<DespawnObject>().lifeTime =-1;
			}
		}

		void Update(){

			if (!isOnDespawn){
				if (projectileType == ProjectileType.Velocity){
					UpdateVelocityProjectile();
				}
			}
		}
		
		void OnCollisionEnter(Collision collision){

			if (!isOnDespawn && collision.gameObject != owner){
				if (projectileType == ProjectileType.Physic && projectileDamage == DamageAt.Contact){
					isLifeEnd = false;

					DoDamage( collision.gameObject, collision.contacts[0].point,collision.contacts[0].normal);

					if (impactEffect){
						PoolManager.Spawn( impactEffect,collision.contacts[0].point,cachedTransform.rotation);
					}
					PoolManager.Despawn( gameObject);
				}
			}
		}
		#endregion

		#region Pool Manager
		public override void OnSpawn (){

			base.OnSpawn ();

			switch (projectileType){
			case ProjectileType.Instant:
				if (RayCast(rayAdvance)){
					isLifeEnd = false;
					if (impactEffect){
						PoolManager.Spawn( impactEffect,hitInfo.point,transform.rotation);
					}
					DoDamage(hitInfo.collider.gameObject,hitInfo.point, hitInfo.normal);
				}
				PoolManager.Despawn( gameObject);
				break;
			case ProjectileType.Physic:
				cachedRigidbody.AddRelativeForce( new Vector3(0,0,_velocity),projectileForceMode );
				break;
			}
		}

		public override void OnDespawn(){

			base.OnDespawn ();

			if (isLifeEnd){
				if ( projectileDamage == DamageAt.EndOfLife && damageType == DamageType.Area){
					DoAreaDamage( cachedTransform.position);
				}
			}
		}
		#endregion

		#region Private methods
		private void UpdateVelocityProjectile(){
			
			if (!isOnDespawn){
				Vector3 step = transform.forward * Time.deltaTime * _velocity;

				if (RayCast(step.magnitude * rayAdvance )){
					isLifeEnd = false;

					DoDamage(hitInfo.collider.gameObject, hitInfo.point,hitInfo.normal);
					if (impactEffect){
						PoolManager.Spawn( impactEffect,hitInfo.point,transform.rotation);
					}
					PoolManager.Despawn( gameObject);
				}
				else{
					transform.position += step;
				}
			}
		}
		#endregion
	}

}
