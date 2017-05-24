using UnityEngine;
using System.Collections;
using System;

public class GunCallBack : MonoBehaviour
{
    public GameObject cube;
	// Use this for initialization
	void Start ()
    {
        BluetoothListener.OnShootEvent+=OnShootEvent;
      
    }
	void OnDestroy()
    {
        BluetoothListener.OnShootEvent-=OnShootEvent;
    }

    private void OnShootEvent()
    {
        cube.transform.Translate(0.1f, 0.225f,0);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
