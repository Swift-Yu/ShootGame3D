using UnityEngine;
using System.Collections;
using System;
using Showbaby;

namespace Showbaby.Bluetooth
{
#if UNITY_ANDROID //&& !UNITY_EDITOR
    /// <summary>
    /// android实现BluetoothSDKImpl
    /// </summary>
    public class AndroidImpl : BluetoothSDKImpl
    {
        private AndroidJavaObject bsdk;

        /// <summary>
        /// 初始化蓝牙
        /// </summary>
        /// <param name="filterName"></param>
        /// <param name="gameObjectName"></param>
        /// <param name="callBackName"></param>
        /// <param name="serviceUUID"></param>
        /// <param name="charUUID"></param>
        /// <param name="receiverCharUUID"></param>
        /// <param name="descUUUID"></param>
        public AndroidImpl(string filterName, string gameObjectName, string callBackName,
            string serviceUUID, string charUUID, string receiverCharUUID, string descUUUID)
        {
            try
            {
                //Debug.LogError("AndroidImpl filterName " + filterName + " gameObjectName " + gameObjectName + " callBackName " + callBackName);    
                //初始化蓝牙
                bsdk = new AndroidJavaObject("com.arshowbaby.unitylibrary.unity.BluetoothUnity",
                    filterName, gameObjectName, callBackName,serviceUUID,charUUID,receiverCharUUID,descUUUID);
                ArShowContext.Instance.PutPlugin("com.arshowbaby.unitylibrary.unity.BluetoothUnity", bsdk);
            } catch (Exception e)
            {
                Debug.LogFormat("{0} Exception caught.", e);
            }
        }

        public override void Connect(string address)
        {
            bsdk.Call("connectDevice", address);
        }

        public override void StartScan()
        {
            bsdk.Call("startScan");
        }

        public override void StopScan()
        {
            bsdk.Call("stopScan");
        }

        public override void Disconnect()
        {
            bsdk.Call("disconnectDevice");
        }

        public override void RequestPermission()
        {
            //Debug.Log("RequestPermission");
            bsdk.Call("requestPermission");
        }

        public override bool IsEnabled()
        {
            return bsdk.Call<bool>("isEnabled");
        }

        public override void OpenBluetooth()
        {
            bsdk.Call("openBluetooth");
        }
    }
#endif
}
