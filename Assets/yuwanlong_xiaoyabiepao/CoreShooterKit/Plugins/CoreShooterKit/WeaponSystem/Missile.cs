/// <summary>
/// Missile.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyPoolManager;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Weapon System/Missile")]
	public class Missile : Ammunition {

		#region Members
		public DamageAt projectileDamage = DamageAt.Contact;

		public bool useAdvance;
		public float velocity = 300;
		public float straightTime = 2f;
		public float angularSpeed = 75;
		[Range(0f,1f)]
		public float chaos =0;
		[Range(0f,0.1f)]
		public float chaosFrenquency = 0.5f;

		public bool autonomous = false;
		public bool autoTarget = true;
		public bool autoReTarget = false;
		public int maxMissileOnTarget = 1;

		private Vector3 moveVector;

		private float chaosTime = 0;
		private Vector3 startDirection;
		private float launchTime =0;

		private float _angularSpeed;
		private float _chaos;
		private float _chaosFrenquency;
		private Vector3 targetVelocity;
		private Vector3 targetOldPosition;
		private bool hasTarget = false;
		private bool activated = true;
		private Radar cachedRadar;

		public bool showProperties = true;
		public bool showDamage = false;
		public bool showPhysic = false;
		public bool showEffect = false;

		#endregion

		#region Monobehaviour callback
		public override void Awake (){
			base.Awake ();
			GetComponent<DespawnObject>().lifeTime = lifeTime;
			cachedRadar = GetComponent<Radar>();
		}

		public void Start(){
			if (autonomous) activated = false;
		}

		public void Update(){

			if (!isOnDespawn && activated){
				Vector3 direction =cachedTransform.forward;

				if (target && target.gameObject.activeInHierarchy){
					float advance = 0;
					if (useAdvance){
						float distance = (target.transform.position - cachedTransform.position).magnitude;
						advance=(distance/ velocity*0.75f);
					}
					
					direction = (target.transform.position - cachedTransform.position) + targetVelocity*advance;
				}
				else{
					if (hasTarget){
						target= null;
						if (autoReTarget){
							GetTarget();  
							if (target){
								direction = target.transform.position - cachedTransform.position;
							}
						}							
					}
					else{
						direction = startDirection;
					}

				}
					

				float life = Time.realtimeSinceStartup-launchTime;
				if(life < straightTime){
					direction = direction * life/straightTime + startDirection * (1.5f - ( life/straightTime) ) ;
				}

				moveVector = (moveVector + direction.normalized * _angularSpeed * Time.deltaTime).normalized;
				chaosTime += Time.deltaTime;
				if (chaos>0 && chaosTime > _chaosFrenquency){
					chaosTime = 0;
					moveVector += Random.insideUnitSphere * _chaos;
				}

				Vector3 step = cachedTransform.forward.normalized * Time.deltaTime * velocity;

				if ( RayCast(step.magnitude * rayAdvance )){
					
					isLifeEnd = false;
					DoDamage(hitInfo.collider.gameObject, hitInfo.point,hitInfo.normal);
					if (impactEffect){
						PoolManager.Spawn( impactEffect,hitInfo.point,transform.rotation);
					}

					PoolManager.Despawn( gameObject);
				}
				else{
					if (moveVector != Vector3.zero){
						cachedTransform.forward = moveVector.normalized;
						cachedTransform.position += step;
					}
				}

			}
		}

		void LateUpdate(){

			targetVelocity = Vector3.zero;
			if (target && target.gameObject.activeInHierarchy){
				targetVelocity = (target.transform.position-targetOldPosition) / Time.deltaTime;
				targetOldPosition = target.transform.position;
			}

		}
		#endregion

		private void GetTarget(){

			target = cachedRadar.GetTargetRestricted(cachedTransform, faction ,maxMissileOnTarget);
			if (target){
				hasTarget = true;
				targetOldPosition = target.transform.position;
			}
		}

		#region Pool Manager
		public override void OnSpawn(){

			base.OnSpawn ();

			moveVector = cachedTransform.forward;
			startDirection = cachedTransform.forward *1000;

			launchTime = Time.realtimeSinceStartup;

			_angularSpeed = MathH.Remap(angularSpeed,0f,1f,0f,0.1f);
			_chaos = MathH.Remap( chaos,0f,1f,0f,0.5f);
			_chaosFrenquency = chaosFrenquency;
			chaosTime = Mathf.Infinity;

			if (autoTarget && cachedRadar){
				GetTarget();
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

		#region Public methods
		public void Launch(GameEntity target,GameObject owner, Faction faction){
			if (!activated){
				activated = true;
				this.target = target;
				this.faction = faction;
				this.owner = owner;
				OnSpawn();
			}
		}
		#endregion
	}

}
