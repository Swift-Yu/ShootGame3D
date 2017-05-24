using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using ShootingGallery;

namespace HedgehogTeam.CoreShooterKit{

	[System.Serializable]
	public class WeaponSlot {

		public bool enabled = true;
		public float startTime = 0;
		public Weapon primary;
		public Weapon secondary;
		public GameObject model3d;
	}

	[System.Serializable]
	public class ArmoryBinding{

		public bool onDown = false;
		public KeyCode key;
		public string button;
		public bool inverseAxis = false;
		public string axis;

		public bool Triggered(){

			// KeyCode
			if ((Input.GetKeyDown( key) && onDown) || (Input.GetKey( key) && !onDown ) ){
				return true;
			}

			// Button
			if (!string.IsNullOrEmpty(button) && ( (Input.GetButtonDown(button) && onDown)  || (Input.GetButton(button) && !onDown )  )){
				return true;
			}

			// Axis
			if (!string.IsNullOrEmpty(axis) && ((Input.GetAxis(axis)>0 && !inverseAxis) ||  (Input.GetAxis(axis)<0 && inverseAxis))) {
				return true;
			}

			return false;
		}

		public bool UnTriggered(){

			// Key code
			if (Input.GetKeyUp( key)){
				return true;
			}

			// Button
			if (!string.IsNullOrEmpty(button) && Input.GetButtonUp(button)){
				return true;
			}

			return false;
		}

	}

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Weapon System/Armory")]
	public class Armory : MonoBehaviour {

		[System.Serializable] public class OnWeaponChange : UnityEvent<Weapon,int>{};
		[System.Serializable] public class OnWeaponReload : UnityEvent<Weapon,int>{};

		#region Members
		[Header("Slots")]
		[SerializeField]
		public WeaponSlot[] slots;

		[Header("Input")]
		public bool enableBinding = false;
		public ArmoryBinding primaryFire;
		public ArmoryBinding secondaryFire;
		public ArmoryBinding nextSlot;
		public ArmoryBinding prevSlot;
		public ArmoryBinding reload;
		public ArmoryBinding mode;
		public ArmoryBinding drop;
	    public SGTGameController Controller;

		private GameEntity gameEntity;
		private int launcherIndex = -1;
		private Weapon primaryWeapon;
		private Weapon secondaryWeapon;

		private bool allowShooting = true;


		[Header("Events")]
		[SerializeField] public OnWeaponChange onWeaponChange;
		[SerializeField] public OnWeaponReload onWeaponReload;
		#endregion

		#region Monobehaviours Callback
		void Awake(){
			launcherIndex = -1;
			gameEntity = GetComponentInParent<GameEntity>();

			// init
			if (gameEntity != null){
				if (slots.Length>0){
					
					for (int i=0;i<slots.Length;i++){
						if (slots[i].primary){
							slots[i].primary.Init( gameEntity.gameObject,gameEntity.faction );
							if (slots[i].secondary){
								slots[i].secondary.Init( gameEntity.gameObject,gameEntity.faction );
							}
							if (slots[i].enabled && launcherIndex == -1){
								launcherIndex = i;
								primaryWeapon = slots[i].primary;


								secondaryWeapon = slots[i].secondary;
		
								onWeaponChange.Invoke( primaryWeapon,i);
								if (slots[i].model3d){
									slots[i].model3d.SetActive(true);
								}
							}

							if (launcherIndex!=-1 && launcherIndex!=i){
								if (slots[i].model3d){
									slots[i].model3d.SetActive(false);
								}
							}
						}
					}
				}

			}
			else{
				Debug.LogWarning("No Game Entity on GameObject or on parent on " + gameObject.name,gameObject);
			}
		
		}
			
		void Update(){

			if (enableBinding){

				// Shooting
				if (primaryFire.Triggered()){
					////PrimaryShoot();
				 //   if (Controller.isPaused)
				 //   {
				 //       Controller.UnpauseGame();
				 //   }
				 //   else
				 //   {
				 //       Controller.PauseGame();   
				 //   }
				}

				if (primaryWeapon.isShooting){
					if (primaryFire.UnTriggered()){
						PrimaryStopShoot();
					}
				}


				if (secondaryFire.Triggered()){
					SecondaryShoot();
				}

				if (secondaryWeapon && secondaryWeapon.isShooting){
					if (secondaryFire.UnTriggered()){
						SecondaryStopShoot();
					}
				}

				// Next
				if (nextSlot.Triggered()){
					NextSlot();
				}

				// Prev
				if (prevSlot.Triggered()){
					PreviousSlot();
				}

				// Reload
				if (reload.Triggered()){
					Reload();
				}

				// Weapon Mode
				if (mode.Triggered()){
					CycleWeaponMode();
				}
			}
		}
		#endregion

		#region Public Methods
		public void PrimaryShoot(){
			if (allowShooting){
				primaryWeapon.Shoot();
			}
		}

		public void SecondaryShoot(){
			if (allowShooting){
				secondaryWeapon.Shoot();
			}
		}

		public void PrimaryStopShoot(){
			primaryWeapon.StopShoot();
		}

		public void SecondaryStopShoot(){
			if (secondaryWeapon) secondaryWeapon.StopShoot();
		}

		public void Reload(){

			if (primaryWeapon.enableMagazine && !primaryWeapon.magazineAutoReload){
				primaryWeapon.ReloadWeapon();
			}
			if (secondaryWeapon){
				if (secondaryWeapon.enableMagazine && !secondaryWeapon.magazineAutoReload){
					secondaryWeapon.ReloadWeapon();
				}
			}
		}

		public void NextSlot(){

			allowShooting = false;

			if (slots[launcherIndex].model3d){
				slots[launcherIndex].model3d.SetActive(false);
			}

			PrimaryStopShoot();
			SecondaryStopShoot();

			if (slots.Length>0){
				bool find = false;
				int backIndex = launcherIndex;
				while (!find){
					launcherIndex++;
					if (launcherIndex >= slots.Length){
						launcherIndex =0;
					}

					if (launcherIndex == backIndex || slots[launcherIndex].enabled){
						find = true;
						primaryWeapon = slots[launcherIndex].primary;
						secondaryWeapon = slots[launcherIndex].secondary;
						if (slots[launcherIndex].model3d){
							slots[launcherIndex].model3d.SetActive(true);
						}
						onWeaponChange.Invoke( primaryWeapon,launcherIndex);
						Invoke("EnabledShooting",slots[launcherIndex].startTime);
					}
				}
			}
		}

		public void PreviousSlot(){
			allowShooting = false;

			if (slots[launcherIndex].model3d){
				slots[launcherIndex].model3d.SetActive(false);
			}

			PrimaryStopShoot();
			SecondaryStopShoot();
			if (slots.Length>0){
				bool find = false;
				int backIndex = launcherIndex;
				while (!find){
					launcherIndex--;
					if (launcherIndex < 0 ){
						launcherIndex = slots.Length-1;
					}

					if (launcherIndex == backIndex || slots[launcherIndex].enabled){
						find = true;
						primaryWeapon = slots[launcherIndex].primary;
						secondaryWeapon = slots[launcherIndex].secondary;
						if (slots[launcherIndex].model3d){
							slots[launcherIndex].model3d.SetActive(true);
						}
						onWeaponChange.Invoke( primaryWeapon,launcherIndex);
						Invoke("EnabledShooting",slots[launcherIndex].startTime);
					}
				}
			}
				
		}

		public void CycleWeaponMode(){
			primaryWeapon.CycleWeaponMode();
		}
		#endregion
	
		private void EnabledShooting(){
			allowShooting = true;
		}

	}

}