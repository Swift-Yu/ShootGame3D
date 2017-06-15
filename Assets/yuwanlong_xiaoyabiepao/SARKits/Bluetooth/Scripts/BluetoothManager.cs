using Showbaby.Bluetooth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothManager : MonoBehaviour
{
    public GameObject Bluetooth;
    private GameObject clone;
    public static bool _IsClone = false;
    // Use this for initialization
    void Awake ()
    {
        if (!_IsClone)
        {
            clone = Instantiate(Bluetooth) as GameObject;
            DontDestroyOnLoad(clone);
            _IsClone = true;
            Invoke("ShowBluetooth", 1.0f);
        }
    }
  
    void ShowBluetooth()
    {
        BluetoothSDK.BluetoothSdk.OpenBluetoothPanel();
    }
}
