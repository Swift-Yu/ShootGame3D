using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace Showbaby.Bluetooth
{
#if UNITY_IPHONE //&& !UNITY_EDITOR
    /// <summary>
    /// ios实现BluetoothSDKImpl
    /// </summary>
    public class iOSImpl : BluetoothSDKImpl
    {
        [DllImport("__Internal")]
        extern static private void bluetoothInitParams(
            string objName,
            string callBackName,
            string deviceName,
            string serviceUUID,
            string TXCAHRUUID,
            string RXCHARUUID,
            string DISUUID,
            string HRSUUID,
            int FindOthderDeviceTime);

        [DllImport("__Internal")]
        extern static private void bluetoothConnectionPeripheral(string address);//连接指定的蓝牙

        [DllImport("__Internal")]
        extern static private void openBluetooth();//开启蓝牙 会返回蓝牙是否已开启

        [DllImport("__Internal")]
        extern static private void bluetoothCancelConnectionPeripheral();//断开连接

        [DllImport("__Internal")]
        extern static private void bluetoothScanPeripheral();//开始扫描

        [DllImport("__Internal")]
        extern static private void bluetoothStopScanPeripheral();//停止扫描

        public iOSImpl(
            string gameObjectName,
            string callBackName,
            string deviceName,
            string serviceUUID,
            string charUUID,
            string receiverCharUUID,
            string deviceInformationServiceUUID,
            string hardwareRevisionStringUUID,
            int findOtherDeviceTime)
        {
            try
            {
                bluetoothInitParams(gameObjectName, callBackName, deviceName,
                    serviceUUID, charUUID, receiverCharUUID, deviceInformationServiceUUID,
                    hardwareRevisionStringUUID, findOtherDeviceTime);
            }
            catch (Exception e)
            {
                Debug.LogFormat("{0} Exception caught.", e);
            }
        }
        
        public override void Connect(string address)
        {
            bluetoothConnectionPeripheral(address);
        }

        public override void StartScan()
        {
            bluetoothScanPeripheral();
        }

        public override void StopScan()
        {
            bluetoothStopScanPeripheral();
        }

        public override void Disconnect()
        {
            bluetoothCancelConnectionPeripheral();
        }

        public override void RequestPermission()
        {
            //do nothing
        }

        public override bool IsEnabled()
        {
            return false;
        }

        public override void OpenBluetooth()
        {
            openBluetooth();
        }
    }
#endif
}