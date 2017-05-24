using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using HedgehogTeam.CoreShooterKit;

public class HudManager : MonoBehaviour {

	public Text ammunition;
	public Text weaponName;
	public Text WeaponMode;
	public Text reloadWarning;
	public Text autoReloadWarning;
	public Text lowEnergy;

	private Weapon currentWeapon;

	void Start(){
		reloadWarning.enabled = false;
		autoReloadWarning.enabled = false;
	}

	// Update is called once per frame
	void Update () {
	

		if (currentWeapon){

			if (currentWeapon.enableMagazine){
				if (currentWeapon.magazineType == MagazineType.Unit){
					ammunition.text = currentWeapon.magazineValue.ToString("000");
				}
				else{
					ammunition.text = (currentWeapon.magazineValue / currentWeapon.magazineCapacity *100).ToString("000")  + "%";
				}
			}
			else{
				ammunition.text="Infinity";
			}

			// Reload
			if (!currentWeapon.magazineAutoReload && currentWeapon.magazineValue == 0 ){
				reloadWarning.enabled = true;
			}
			else{
				reloadWarning.enabled = false;
			}

			// auto reload
			if (currentWeapon.magazineAutoReload && currentWeapon.inReload){
				autoReloadWarning.text= "AUTO RELOAD " + (currentWeapon.reloadValue - (Time.time- currentWeapon.autoReloadStartTime )).ToString("00");
				autoReloadWarning.enabled = true;
			}
			else{
				autoReloadWarning.enabled = false;
			}

			// Low energy
			if (currentWeapon.magazineAutoReload && (currentWeapon.magazineValue / currentWeapon.magazineCapacity *100)<7 && currentWeapon.magazineType == MagazineType.Time){
				lowEnergy.enabled = true;
			}
			else{
				lowEnergy.enabled = false;
			}

			// Mode
			WeaponMode.text = currentWeapon.currentGunMode.ToString();
		}
	}

	public void OnWeaponChange(Weapon weapon,int armoryIndex){
		currentWeapon = weapon;
		weaponName.text = weapon.name;
	}


}
