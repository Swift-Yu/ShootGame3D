using UnityEngine;
using System.Collections;
using HedgehogTeam.CoreShooterKit;
using ShootingGallery;
using Showbaby.Bluetooth;

public class BlueToothEvent : MonoBehaviour
{

    [SerializeField] private SGTGameController controller;
    [SerializeField] private Weapon weapon;
	// Use this for initialization
	void Start ()
	{
        //Debug.LogError("BlueToothEvent Start");
	    //BluetoothSDK.BluetoothSdk.OnShootDownEvent.AddListener(GunShoot);
     //   BluetoothSDK.BluetoothSdk.OnSmoothDownEvent.AddListener(GunReload);
        BluetoothListener.OnShootEvent += GunShoot;
	    BluetoothListener.OnSwapBulletEvent += GunReload;
	    BluetoothListener.OnShootUpEvent += StopShoot;
	}

    void GunShoot()
    {
        //Debug.Log("Gun is shooting>>>>>>>>>>>>>");
        //weapon.Shoot();
        controller.Shoot();
    }

    void GunReload()
    {
        //Debug.Log("Gun is Reloading>>>>>>>>>>>>>");
        //weapon.ReloadWeapon();
        controller.Reload();
    }

    void StopShoot()
    {
        weapon.StopShoot();
    }

}
