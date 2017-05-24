/// <summary>
/// Destructible entity.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using HedgehogTeam.EasyPoolManager;
using System;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Game System/Game Entity")]
	public class GameEntity : LethalEntity {

		#region Unity events
		[System.Serializable] public class OnEntitySpawn : UnityEvent{};
		[System.Serializable] public class OnHit : UnityEvent<Impact>{};
		[System.Serializable] public class OnHeal : UnityEvent<float>{};
		[System.Serializable] public class OnEntityDestroy : UnityEvent{};

		[SerializeField] public OnEntitySpawn onEntitySpawn;
		[SerializeField] public OnHit onHit;
		[SerializeField] public OnHeal onHeal;
		[SerializeField] public OnEntityDestroy onEntityDestroy;
		#endregion

		#region Members
		public EntityType entityType;
		public bool subEntity = false;
		public GameEntity parentEntity;

		public bool isInvulnerable = false;
		public float invulnerableSpawn = 0;
		public bool randomLife;
		public int rndMinLife;
		public int rndMaxLife;
		public int maxLife = 1000;
		public int scorePoint = 1;
		public bool isExternalEndLife = false;
		public bool isPooled = false;
		public bool addForceFromImpact = false;
		public float[] damagesSensitivity;
		public float damageThreshold = 10;

		public bool useTrigger = false;

		public float life{  get; private set; }

		public Vector3 spawnEffectOffset;
		public GameObject[] spawnEffects;

		public Vector3 hitEffectOffset;
		public GameObject[] hitEffects;

		public Vector3 healEffectOffset;
		public GameObject[] healEffects;

		public Vector3 deathEffectOffset;
		public GameObject[] deathEffects;

		public bool showPropertie = true;
		public bool showDamage = false;
		public bool showSensitivity = false;
		public bool showEffect = false;
		public bool showEvent = false;

		private bool isOnDespawn = false;
		private Rigidbody cachedRigidbody;
		private float damageStack = 0;
		#endregion

		#region Monobehaviour Callback
		public override void Awake (){
			base.Awake ();
			cachedRigidbody = GetComponent<Rigidbody>();
		}


		void OnEnable(){
			if (!isPooled){
				SpawnEntity();
			}
		}
			
		void OnCSKCollisionEnter(Collision collision){
			OnCollisionEnter(collision);
		}

		void OnCSKTriggerEnter(Collider other){
			OnTriggerEnter(other);
		}

		void OnCollisionEnter(Collision collision){

			if (doDamageOnCollision && !isOnDespawn && !useTrigger){
				DoDamage(collision.collider.gameObject,collision.contacts[0].point,collision.contacts[0].normal );
			}
		}

		void OnTriggerEnter(Collider other){
			if (doDamageOnCollision && !isOnDespawn && useTrigger){
				DoDamage( other.gameObject, cachedTransform.position, Vector3.zero);
			}
		}

		void OnDestroy(){
			if (GameManager.instance){
				if (entityType!= EntityType.NotTraceable){
					GameManager.Instance.UnregisterEntity( this);
				}
			}
		}

		#endregion

		#region PoolManager
		void OnSpawn(){
			SpawnEntity();
		}

		void OnDespawn(){
			isOnDespawn = true;
		}

		private void SpawnEntity(){

			owner = gameObject;
			isOnDespawn = false;

			if (invulnerableSpawn >0 && !isInvulnerable){
				isInvulnerable = true;
				Invoke( "StopInvulnerable",invulnerableSpawn);
			}

			if (randomLife){
				life = UnityEngine.Random.Range( rndMinLife, rndMaxLife);
				maxLife=(int)life;
			}
			else{
				life = maxLife;
			}

			if (subEntity && parentEntity){
				faction = parentEntity.faction;
				entityType = parentEntity.entityType;
			}

			if (entityType!= EntityType.NotTraceable){
				GameManager.Instance.RegisterEntity( this);
			}

			onEntitySpawn.Invoke();
			if (spawnEffects.Length>0){
				for (int i=0;i<spawnEffects.Length;i++){
					PoolManager.Spawn( spawnEffects[i],transform.position + spawnEffectOffset,transform.rotation);
				}
			}
				
			#if UNITY_EDITOR
			if (faction == null && !subEntity){
				//Debug.LogWarning("Faction not assigned on " + gameObject.name,gameObject);
			}

			if (subEntity && parentEntity==null){
				Debug.LogWarning("Parent entity not assigned on " + gameObject.name,gameObject);
			}
			#endif
		}

		private void DespawnEntity(){
			
			if (GameManager.instance){
				if (entityType!= EntityType.NotTraceable){
					GameManager.Instance.UnregisterEntity( this);
				}
			}

			if (isPooled){
				PoolManager.Despawn( gameObject);
			}
			else{
				Destroy(gameObject);
			}
		}

		private void StopInvulnerable(){
			isInvulnerable = false;
		}
		#endregion

		#region Public methods
		public void ReceiveDamage(Impact impact){


			if (!isInvulnerable && !isOnDespawn){

				impact.damagePoint = impact.damagePoint * damagesSensitivity[impact.damageNature];


				life -= impact.damagePoint;
				damageStack +=impact.damagePoint;

				onHit.Invoke(impact);

				// Life is over
				if (life <=0 && !isOnDespawn){

					// Hit
					if (hitEffects.Length>0 ){
						for (int i=0;i<hitEffects.Length;i++){
							GameObject tmpObj = PoolManager.Spawn( hitEffects[i], impact.hitPoint + hitEffectOffset,Quaternion.identity);
							IHitEffect effect = tmpObj.GetComponent<IHitEffect>();
							if (effect!=null){
								effect.InitHitEffect( impact);
							}	
						}
						onHit.Invoke(impact);
					}

					Kill();

					// Score management
					if (impact.owner && impact.owner.CompareTag("Player")){
						GameEntity playerEntity = impact.owner.GetComponent<GameEntity>();
						if( (playerEntity && playerEntity.life>0) || playerEntity == null){
							GameManager.instance.Add2Score( scorePoint);
						}
					}
				}
				else{
					// Force
					if (impact.isPhysic && cachedRigidbody && addForceFromImpact){
						if (impact.damageType == DamageType.Simple){
							cachedRigidbody.AddForceAtPosition( impact.hitNormal*impact.force *-1 * damagesSensitivity[impact.damageNature], impact.hitPoint, impact.forceMode);
						}
						else{
							cachedRigidbody.AddExplosionForce( impact.force, impact.hitPoint,impact.damageRadius, 2f, impact.forceMode);
						}
					}

					// Hit Effect
					if (hitEffects.Length>0 && damageStack>= damageThreshold ){
						for (int i=0;i<hitEffects.Length;i++){
							impact.damagePoint = damageStack;
							GameObject tmpObj = PoolManager.Spawn( hitEffects[i], impact.hitPoint + hitEffectOffset,Quaternion.identity);
							IHitEffect effect = tmpObj.GetComponent<IHitEffect>();
							if (effect!=null){
								effect.InitHitEffect( impact);
							}	
						}

						damageStack =0;
					}

				}
			}
		}
			
		public void LifeRecovery(float point){
							
			life +=point;
			life = Mathf.Clamp(life,0,maxLife);

			if (healEffects.Length>0){
				for (int i=0;i<healEffects.Length;i++){
					GameObject tmpObj = PoolManager.Spawn( healEffects[i], cachedTransform.position + healEffectOffset,Quaternion.identity);
					IHealEffect effect = tmpObj.GetComponent<IHealEffect>();
					if (effect!=null){
						effect.InitHealEffect( point);
					}
				}
				onHeal.Invoke( point);
			}
		}

		public void SetLife(float point){
			life = point;
			life = Mathf.Clamp(life,0,maxLife);
		}

		public void Kill(){
			// Death Effect
			if (deathEffects.Length>0){
				for (int i=0;i<deathEffects.Length;i++){
					PoolManager.Spawn( deathEffects[i],cachedTransform.position + deathEffectOffset,Quaternion.identity);
				}
			}

			// Destruction management
			onEntityDestroy.Invoke();
			life=0;
			damageStack = 0;
			if (!isExternalEndLife){
				if (isPooled){
					PoolManager.Despawn( gameObject);
				}
				else{
					Destroy(gameObject);
				}
			}
				
		}

		#endregion 
	}

}