using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BluetoothManager : MonoBehaviour
{
    public GameObject Bluetooth;
    private GameObject clone;
    public static bool _IsClone = false;
    // Use this for initialization
    void Start ()
    {
        if (!_IsClone)
        {
            clone = Instantiate(Bluetooth, transform.position, transform.rotation) as GameObject;
            DontDestroyOnLoad(clone);
            _IsClone = true;
        }
    }

}
