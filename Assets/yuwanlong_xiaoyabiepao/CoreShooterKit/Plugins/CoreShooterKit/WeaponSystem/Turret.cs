/// <summary>
/// Turret.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using System;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Weapon System/Turret")]
	[RequireComponent( typeof(Radar))]
	[RequireComponent( typeof(Weapon))]
	public class Turret : MonoBehaviour {

		#region Members
		public bool isAutonomous = false;
		public Faction faction;
		public bool enableAtStart = true;

		public Transform hub;
		public Transform axis;
		public bool isPooled = false;

		public float speed = 50;

		public bool hubRotation = true;
		public bool axisRotation = true;

		private Quaternion hubDestination;
		private Quaternion axisDestination;

		private TurretState state;
		private Radar radar;
		private GameEntity target;
		private Transform cachedTransform;
		private Weapon launcher;

		private Vector3 targetVelocity;
		private Vector3 targetOldPosition;
		private float weaponSpeed;
		private bool enableSeekTarget;
		int frameCount = 0;
		Vector3 velocitySum;
		#endregion

		#region Monobehavior Callback
		void Awake(){
			cachedTransform = transform;
			radar = GetComponent<Radar>();
			launcher = GetComponent<Weapon>();

			weaponSpeed = 0;
			if (launcher.ammunition){
				if (launcher.ammunition.GetType() == typeof( Bullet)){
			
					Bullet bullet = (Bullet)launcher.ammunition;
					if (bullet && bullet.projectileType == ProjectileType.Velocity){
						
						weaponSpeed = bullet.velocity/3.6f;

					}
				}
			}
				
			if (isAutonomous){
				launcher.Init( gameObject, faction);
			}
		}

		void Start(){
			if (!isPooled){
				SpawnEntity();
			}
				
		}
						
		void LateUpdate(){

			if (state == TurretState.InPursue || state == TurretState.InAcquisition ){
				if (target == null || !target.gameObject.activeInHierarchy ){
					state = TurretState.InRadarSeek;
					launcher.StopShoot();
				}
				else{
					frameCount++;
					velocitySum += (target.transform.position-targetOldPosition) / Time.deltaTime;
					targetVelocity = velocitySum/frameCount;

					targetOldPosition = target.transform.position;
				}
			}
		}


		void Update(){

			// Seek for detection
			if ( state == TurretState.InRadarSeek && enableSeekTarget){
				target = radar.GetTarget( cachedTransform, faction,true);

				// Target is found
				if (target!=null){
					targetOldPosition = target.transform.position;
					state = TurretState.InAcquisition;
					velocitySum = Vector3.zero;
					frameCount = 1;
				}
				// Reset target position
				else{
					
					float localHubYawAngle = hub.localEulerAngles.y>180?hub.localEulerAngles.y-360:hub.localEulerAngles.y;
					float localAxisAngle =  axis.localEulerAngles.x>180?axis.localEulerAngles.x-360:axis.localEulerAngles.x;

					// Yaw
					if (hubRotation){
						if ( Mathf.Abs( localHubYawAngle)> speed*Time.deltaTime){
							if ( localHubYawAngle < 0){
								hub.Rotate( Vector3.up * speed * Time.deltaTime , Space.Self);
							}
							else{
								hub.Rotate( Vector3.up * -speed * Time.deltaTime , Space.Self);
							}
						}
					}

					// Ptich
					if (axisRotation){
						if ( Mathf.Abs(localAxisAngle)> speed*Time.deltaTime){
							if (localAxisAngle < 0){
								axis.Rotate( Vector3.left * -speed * Time.deltaTime , Space.Self);
							}
							else{
								axis.Rotate( Vector3.left * speed * Time.deltaTime , Space.Self);
							}
						}
					}
				}
			}

			// Reach & track
			if ( state !=  TurretState.InRadarSeek){

				float distance=0;
				// Target visible
				if (radar.fov.IsVisble( cachedTransform,  target,out distance)){

					float advance = 0;
					if (weaponSpeed>0) advance=(distance / weaponSpeed);

					// Target + velocity in range
					Vector3 targetLookAt = target.transform.position + targetVelocity * advance;
					if (radar.fov.InFov( cachedTransform,  targetLookAt,out distance)){

						// In reach
						if (state == TurretState.InAcquisition){
							Vector2 globalAngle = MathH.GetDeltaAngle( cachedTransform,targetLookAt,axis.localPosition);

							float localHubYawAngle = hub.localEulerAngles.y>180?hub.localEulerAngles.y-360:hub.localEulerAngles.y;
							float localAxisAngle =  axis.localEulerAngles.x>180?axis.localEulerAngles.x-360:axis.localEulerAngles.x;

							// Yaw
							bool lockYaw = false;
							if (hubRotation){
								if ( Mathf.Abs( globalAngle.y-localHubYawAngle)> speed*Time.deltaTime){
									if ( localHubYawAngle < globalAngle.y){
										hub.Rotate( Vector3.up * speed * Time.deltaTime , Space.Self);
									}
									else{
										hub.Rotate( Vector3.up * -speed * Time.deltaTime , Space.Self);
									}
								}
								else{ lockYaw = true;}
							}
							else{
								lockYaw = true;
							}

							// Ptich
							bool lockPitch = false;
							if (axisRotation){
								if ( Mathf.Abs( globalAngle.x-localAxisAngle)> speed*Time.deltaTime){
									if (localAxisAngle < globalAngle.x){
										axis.Rotate( Vector3.left * -speed * Time.deltaTime , Space.Self);
									}
									else{
										axis.Rotate( Vector3.left * speed * Time.deltaTime , Space.Self);
									}
								}
								else{
									lockPitch = true;
								}
							}
							else{
								lockPitch = true;
							}

							if (lockPitch && lockYaw){
								state = TurretState.InPursue;
							}
						}
						// In track
						else{
							if (hubRotation){
								hub.LookAt(targetLookAt );
								hub.localRotation = Quaternion.Euler(new Vector3(0,hub.localRotation.eulerAngles.y,0) );
							}

							if (axisRotation){
								axis.LookAt(targetLookAt );
								axis.localRotation = Quaternion.Euler(new Vector3(axis.localRotation.eulerAngles.x,0,0) );
							}

							launcher.Shoot(target);
						}
					}
					else{
						launcher.StopShoot();
						state = TurretState.InRadarSeek;
						//targetOldPosition = Vector3.zero;
					}
				}
				else{
					launcher.StopShoot();
					state = TurretState.InRadarSeek;
					//targetOldPosition = Vector3.zero;
				}
			}
				
		}
			
		#endregion

		#region PoolManager
		void OnSpawn(){
			SpawnEntity();
		}

		private void SpawnEntity(){
			state = TurretState.InRadarSeek;
			enableSeekTarget = false;

			if (isAutonomous){
				enableSeekTarget = enableAtStart;
			}
		}
		#endregion
	
		#region Public methods
		public void EnableTurret(bool enable){
			enableSeekTarget = enable;
		}
		#endregion
	}

}

