/// <summary>
/// Weapon 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using HedgehogTeam.EasyPoolManager;
using System;
using Showbaby.Bluetooth;

namespace HedgehogTeam.CoreShooterKit{

	[System.Serializable]
	[RequireComponent(typeof (AudioSource))] 
	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Weapon System/Weapon")]
	public class Weapon : MonoBehaviour {

		#region Events
		[System.Serializable] public class OnStartShooting : UnityEvent<Weapon>{}
		[System.Serializable] public class OnShooting : UnityEvent<Weapon>{}
		[System.Serializable] public class OnStopShooting : UnityEvent<Weapon>{}
		[System.Serializable] public class OnReload : UnityEvent<Weapon>{}
		[System.Serializable] public class OnReloadEnd : UnityEvent<Weapon>{}
		[System.Serializable] public class OnAutoReload : UnityEvent<Weapon>{}
		[System.Serializable] public class OnNeedReload : UnityEvent<Weapon>{}
		
		[SerializeField] public OnStartShooting onStartShooting;
		[SerializeField] public OnShooting onShooting;
		[SerializeField] public OnStopShooting onStopShooting;
		[SerializeField] public OnReload onReload;
		[SerializeField] public OnReloadEnd onReloadEnd;
		[SerializeField] public OnAutoReload onAutoReload;
		[SerializeField] public OnNeedReload onNeedReload;
		#endregion

		#region Members
		public Ammunition ammunition; //The ammunition fired

		public bool isAutonomous = false; // The weapon is autonomous for shooting and required a Radar component on GameObject
		public Faction faction; // If the weapon is autonomous, it's requiring a faction

		public bool fpsAlignment = false; // For First Person View to align the line of slight to the center of the screen
		public bool fixedAlignment = false;
		public Transform alignmentPoint;
		// Barrel
		public Transform[] barrels; // barrels 
		public bool isSynchronizedBarrel = false; // All barrels will shooting at the same time
		
		// Mode
		public bool isAutmotic = true;  // continuous fired
		public bool isBurst = true; // Fired by amount
		public bool isOneShot = true; // shot per shot

		// Magazine
		public bool enableMagazine;
		public bool magazineAutoReload;
		public MagazineType magazineType;
		public float magazineCapacity=30;
		public float shotValue=1;
		public float reloadValue=1;
		public bool inReload=false;
		public float autoReloadStartTime;

	
		// Amunition
		public bool enableAmmunitionStock = false;
		public float ammunitionMaxStock = 100;
		public float ammunitionStock= 30;
		private float ammunitionCurrentStock;

		// Rate
		public float fireRate=1f;
		public int burstShotsAmount = 3;

		// Laserbeam
		private bool isBeamStart = false;
		private Transform[] laserBeamTransforms;
		private GameObject[] laserBeamMuzzle;

		// Effect;
		public GameObject muzzleEffect;
		public GameObject startShootingEffect;
		public GameObject StopShootingEffect;

		private GameObject owner;

		#region private
		public WeaponType weaponType;
		public WeaponMode currentGunMode;
		public float magazineValue {get; private set;}
	
		public bool isShooting = false;

		private int gunIndex;
		private float shotTime;
		private int shotsCount;

		private bool isNeed2Reload = false;

		private RaycastHit hitInfo;
		private GameEntity target;
		private GameObject tmpObj;
		private Ammunition[] tmpProjectile;
		private AudioSource cachedAudioSource;
		private float lasShot=0;
		private Radar radar;
		private Transform cachedTransform;
		#endregion

		#region Inspector
		public bool showProperties=true;
		public bool showMode=false;
		public bool showMagazine=false;
		public bool showStock = false;
		public bool showEffect=true;
		public bool showEvents=false;
		#endregion

		#endregion

		#region MonoBehaviour Callback
		void Awake(){
			
			currentGunMode = WeaponMode.None;
			if (isOneShot) currentGunMode = WeaponMode.OneShot;
			if (isBurst) currentGunMode = WeaponMode.Burst;
			if (isAutmotic) currentGunMode = WeaponMode.FullyAutomatic;

			cachedAudioSource = GetComponent<AudioSource>();
			magazineValue = magazineCapacity;

			if (weaponType == WeaponType.LaserBeam){
				laserBeamTransforms = new Transform[barrels.Length];
				if (muzzleEffect){
					laserBeamMuzzle = new GameObject[barrels.Length];
				}
				isSynchronizedBarrel = true;
			}


			if (isAutonomous){
				owner = gameObject;
			}

			radar = GetComponent<Radar>();
			cachedTransform = transform;

			#if UNITY_EDITOR
			if (isAutonomous){
				if (faction == null){
					Debug.LogWarning("Faction not assigned on " + gameObject.name,gameObject);
				}

				if (radar == null){
					Debug.LogWarning("Radar component doesn't exit on " + gameObject.name,gameObject);
				}
			}
			#endif

			//InitBarrelOrigin();
		}

		void Start(){
			ammunitionCurrentStock = ammunitionStock;
		}

		void Update(){

			if (isShooting && !isNeed2Reload){
				shotTime += Time.deltaTime;

				switch (weaponType){
				case WeaponType.Projectile:
					if (shotTime>= 1f/fireRate){
						shotTime =0;

						LaunchShot();
					}
					break;
				case WeaponType.LaserBeam:
					LaunchLaserBeam();
					break;
				}
			}
				
			if (enableMagazine && magazineType  == MagazineType.Time && !isShooting && magazineAutoReload){
				magazineValue += reloadValue * Time.deltaTime;
				magazineValue = Mathf.Clamp(magazineValue,0,magazineCapacity);
			}
	
			if (isAutonomous && radar){
				if ( !isShooting ){

					target = radar.GetTarget( cachedTransform, faction,radar.visible);
					if (target){
						Shoot(target);
					}
					else{
						StopShoot();
					}
				}
				else{
					if (target && target.gameObject.activeInHierarchy){
						if ( !radar.fov.IsVisble( cachedTransform, target)){
							StopShoot();
						}
					}
					else{
						StopShoot();
						target = null;
					}
				}
			}
		}
			
		void OnDisable(){
			StopShoot();
		}
		#endregion

		#region Private method
		private void LaunchShot(){

			// Can we launch shoot
			float shot = shotValue;
			if (magazineType == MagazineType.Time) shot = shotValue * Time.deltaTime;


			if (!enableMagazine || (enableMagazine && magazineValue>=shot)){
				
				shotsCount++;

				if ((currentGunMode == WeaponMode.OneShot && shotsCount==1)
					|| (currentGunMode == WeaponMode.Burst && shotsCount<= burstShotsAmount)
				    || currentGunMode == WeaponMode.FullyAutomatic){

					if (cachedAudioSource.loop){
						if (!cachedAudioSource.isPlaying){
							cachedAudioSource.Play();
						}
							
					}
					else{
						cachedAudioSource.PlayOneShot( cachedAudioSource.clip);
					}

					// Align option
					if (fpsAlignment) FPSAlignment();
					if (fixedAlignment) FixedAlignment();

					// Spawn effect
					SpawnProjectile();

					// Event
					onShooting.Invoke(this);

					// Gun index
					if (!isSynchronizedBarrel){
						gunIndex++;

						if (gunIndex==barrels.Length){
							gunIndex=0;
						}

					}
				}
				else{
					if (cachedAudioSource.isPlaying && cachedAudioSource.loop){
						cachedAudioSource.Stop();
					}
					
				}
			}

			// Need to reload
			if (enableMagazine && magazineValue<shot){
				if (!inReload){

					if (!magazineAutoReload && magazineType != MagazineType.Time){
						isNeed2Reload = true;
					}

					StopShoot();
					if (magazineAutoReload  && magazineType == MagazineType.Unit){
						onAutoReload.Invoke(this);
						autoReloadStartTime = Time.time;
						ReloadWeapon();
					}
				}
					
			}
		}
		
		private void SpawnProjectile(){

			if (ammunition){
				if (!isSynchronizedBarrel){

					#if UNITY_EDITOR
					if (barrels[gunIndex] == null){
						Debug.LogWarning("Barrels not assigned on " + gameObject.name,gameObject);
						return;
					}
					#endif

					MagazineManagement();

					tmpObj = PoolManager.SpawnStart(ammunition.gameObject, barrels[gunIndex].position,barrels[gunIndex].rotation,1 );
					if (tmpObj){
						tmpProjectile = tmpObj.transform.GetComponentsInChildren<Ammunition>(true);
						if (tmpProjectile.Length>0){
							for (int z=0;z<tmpProjectile.Length;z++){
								tmpProjectile[z].Init( owner,faction,target);
							}
						}
						PoolManager.SpawnEnd( tmpObj);
						SpawnProjectileMuzzle(barrels[gunIndex]);
					}
				}
				else{
					for (int i=0;i<barrels.Length;i++){
						#if UNITY_EDITOR
						if (barrels[gunIndex] == null){
							Debug.LogWarning("Barrels not assigned on " + gameObject.name,gameObject);
							return;
						}
						#endif

						if (MagazineManagement()){
							tmpObj = PoolManager.SpawnStart(ammunition.gameObject, barrels[i].position,barrels[i].rotation,1 );
							
							tmpProjectile = tmpObj.GetComponentsInChildren<Ammunition>(true);

							if (tmpProjectile.Length>0){
								for (int x=0;x<tmpProjectile.Length;x++){
									tmpProjectile[x].Init( owner,faction,target);
								}
							}
							PoolManager.SpawnEnd( tmpObj);

							SpawnProjectileMuzzle(barrels[i]);
						}
					}
				}
			}
		}

		private void LaunchLaserBeam(){

			if (!enableMagazine || (enableMagazine && MagazineManagement())){
				
				if (fpsAlignment) FPSAlignment();

				if (!isBeamStart){
					cachedAudioSource.Play();
					isBeamStart = true;
					SpawnLaserBeam();

					onShooting.Invoke(this);
				}
				else{

					for (int i=0;i<laserBeamTransforms.Length;i++){
						laserBeamTransforms[i].position  =  barrels[i].position;
						laserBeamTransforms[i].rotation  = barrels[i].rotation;
						if (muzzleEffect){
							laserBeamMuzzle[i].transform.position =  barrels[i].position;
						}
					}
				}
			}
			else{
				if (!inReload){

					if (!magazineAutoReload && magazineType != MagazineType.Time){
						isNeed2Reload = true;
					}
					StopShoot();

					if (magazineAutoReload  && magazineType == MagazineType.Unit){
						onAutoReload.Invoke(this);
						ReloadWeapon();
					}
				}
			}
		}

		private void SpawnLaserBeam(){

			for (int i=0;i<laserBeamTransforms.Length;i++){

				// Laser;
				laserBeamTransforms[i] = PoolManager.Spawn( ammunition.gameObject, barrels[i].position, barrels[i].rotation).transform;
				LaserBeam[] laser = laserBeamTransforms[i].GetComponentsInChildren<LaserBeam>(true);
				for(int l=0;l<laser.Length;l++){
					laser[l].Init( owner,faction,target);
				}

				// Muzzle
				if (muzzleEffect){
					laserBeamMuzzle[i] = PoolManager.Spawn( muzzleEffect,  barrels[i].position, barrels[i].rotation);
				}
			}
		}
			
		private void DespawnLaserBeam(){

			cachedAudioSource.Stop();

			for (int i=0;i<laserBeamTransforms.Length;i++){
				if (laserBeamTransforms[i]){
					PoolManager.Despawn( laserBeamTransforms[i].gameObject) ;
					laserBeamTransforms[i] = null;

					if (muzzleEffect){
						PoolManager.Despawn( laserBeamMuzzle[i]);
						laserBeamMuzzle[i] = null;
					}
				}
			}
		}

		private bool MagazineManagement(){

			// Managzine management
			if (enableMagazine){
				float currentMagazine = magazineValue;
				switch (magazineType){
				case MagazineType.Unit:
					magazineValue -= shotValue;
					break;
				case MagazineType.Time:
					magazineValue -= shotValue * Time.deltaTime;
					break;
				}
				if (magazineValue<=0){ 
					magazineValue=0 ;
				}
				float shot = shotValue;
				if (magazineType == MagazineType.Time) shot = shotValue * Time.deltaTime;
				if (currentMagazine>=shot){
					return true;
				}
				else{
					return false;
				}
			}
			else{
				return true;
			}
		}

		private void SpawnProjectileMuzzle(Transform transform){

			if (muzzleEffect){

				DespawnObject despawnObj = PoolManager.SpawnStart( muzzleEffect, transform.position,transform.rotation).GetComponent<DespawnObject>();
				if (despawnObj){
					despawnObj.dynamicParent = transform;
				}
				PoolManager.SpawnEnd( despawnObj.gameObject);
			}
		}

		IEnumerator  WaitReload(float  amount){

			yield return new WaitForSeconds(reloadValue);
			inReload = false;
			isNeed2Reload = false;

			onReloadEnd.Invoke( this);
			magazineValue =  Mathf.Clamp(amount,0,magazineCapacity);


		}

		private void FPSAlignment(){

			Ray ray = Camera.main.ScreenPointToRay( new Vector3(Screen.width/2,Screen.height/2,0) );

			if (Physics.Raycast( cachedTransform.position, ray.direction,out hitInfo,Mathf.Infinity,~(1<<-1), QueryTriggerInteraction.Ignore)){				
				for (int i=0;i<barrels.Length;i++){
					barrels[i].LookAt(hitInfo.point );
				}
			}
			else{
				for (int i=0;i<barrels.Length;i++){
					barrels[i].forward = ray.direction;
				}
			}
		}

		private void FixedAlignment(){

			if (fixedAlignment && alignmentPoint){
				for (int i=0;i<barrels.Length;i++){
					barrels[i].LookAt(alignmentPoint.position );
				}
			}
		}

		/*
		private void InitBarrelOrigin(){
			barrelOriginRotations = new Quaternion[barrels.Length];
			for (int i=0;i<barrels.Length;i++){
				barrelOriginRotations[i] = barrels[i].rotation;
			}
		}*/

		#endregion
		
		#region Public method
		public void Init(GameObject owner, Faction faction){
			this.owner = owner;
			this.faction = faction;
		}
			
		public void Shoot(GameEntity target = null){

            //Debug.Log("WeaponShoot>>>>>>11111111");
            //Debug.Log("bluetoothsdk is null?"+(BluetoothSDK.BluetoothSdk == null));
			#if UNITY_EDITOR
			if (barrels.Length == 0){
				//Debug.LogWarning("Barrels not assigned on " + gameObject.name,gameObject);
				barrels = new Transform[1];
				barrels[0] = cachedTransform;
			}
			#endif

			if (!isNeed2Reload){
				if (!isShooting && barrels.Length>0 && currentGunMode!= WeaponMode.None && !inReload && ((weaponType == WeaponType.Projectile && Time.realtimeSinceStartup-lasShot > 1f/fireRate) || weaponType == WeaponType.LaserBeam) ){
                    //Debug.Log("0000000000000");
                    //Debug.Log("target == null? "+(target == null));
                    this.target = target;
                    //Debug.Log("111111111111111");
					isShooting = true;
					shotTime = 1f/fireRate;
					isBeamStart = false;
					shotsCount = 0;
					onStartShooting.Invoke(this);
                    //Debug.Log("effect is null"+(startShootingEffect == null));
					//if (startShootingEffect){
					//	PoolManager.Spawn(startShootingEffect, transform.position,transform.rotation);
					//}


				}
			}
			else{
				if (!isShooting){
					isShooting = true;
					onNeedReload.Invoke( this);
				}
			}
		}

		public void StopShoot(){
            //Debug.Log("WeaponStopShoot>>>>>>>>>>>");
            //Debug.Log("cachedAudioSource is "+ cachedAudioSource);
            //Debug.Log("Audio is null"+ (cachedAudioSource == null));
			if (isShooting){
				//if (cachedAudioSource.loop){
				//	cachedAudioSource.Stop();
				//}
				onStopShooting.Invoke(this);
				isShooting = false;
				lasShot = Time.realtimeSinceStartup;


				//if (StopShootingEffect){
				//	PoolManager.Spawn(StopShootingEffect, transform.position,transform.rotation);
				//}
					
				if (weaponType == WeaponType.LaserBeam){
					DespawnLaserBeam();
				}
			}
		}

		public void ReloadWeapon(){
			if (!inReload){
				inReload = true;
				onReload.Invoke(this);

				if ( enableAmmunitionStock && enableMagazine && magazineType== MagazineType.Unit){
					float reloadValue = magazineCapacity - magazineValue;

					if ( ammunitionCurrentStock>= reloadValue){
						ammunitionCurrentStock -= reloadValue;
						StartCoroutine( WaitReload (magazineCapacity));
					}
					else{
						reloadValue = ammunitionCurrentStock;
						ammunitionCurrentStock = 0;
						StartCoroutine( WaitReload (reloadValue  +magazineValue ));
					}
				}
				else{
					StartCoroutine( WaitReload (magazineCapacity));
				}
			};
		}

		public bool SetWeaponMode( WeaponMode mode){
			if ((isBurst && mode == WeaponMode.Burst) || (isAutmotic && mode == WeaponMode.FullyAutomatic) || (isOneShot && mode == WeaponMode.OneShot)){
				currentGunMode = mode;
				return true;
			}
			else{
				return false;
			}
		}

		public bool SetWeaponMode( int mode){
			if ((isBurst && mode == 1) || (isAutmotic && mode == 2) || (isOneShot && mode == 0)){
				currentGunMode = (WeaponMode)mode;
				return true;
			}
			else{
				return false;
			}
		}

		public void CycleWeaponMode(){
			int current = (int)currentGunMode;
			current++;
			if (current>2) current=0;

			if (!SetWeaponMode(current)){
				current++;
				if (!SetWeaponMode(current)){
					current++;
					if (!SetWeaponMode(current)){
						current=0;
					}
				}
			}
				
		}

		public void SetBarrels(Transform[] barrelTransform){
			barrels = barrelTransform;
			laserBeamTransforms = new Transform[barrels.Length];
			//InitBarrelOrigin();
		}

		public float GetMaxDistance(){

			float distance = 0;
			switch (weaponType){
			case WeaponType.LaserBeam:
				distance = ((LaserBeam)ammunition).rayAdvance;
				break;
			case WeaponType.Projectile:
				
				switch (((Bullet)ammunition).projectileType){
				case ProjectileType.Instant:
					distance = ((Bullet)ammunition).rayAdvance;
					break;
				case ProjectileType.Velocity:
					distance = ((Bullet)ammunition).velocity/3.6f * ((Bullet)ammunition).lifeTime;
					break;
				}
				break;
			}

			return distance;
		}
		#endregion
		
	}

}