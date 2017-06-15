using UnityEngine;
using Showbaby.Bluetooth;
using Showbaby.Music;

public class Bluetooth : MonoBehaviour
{
    /// <summary>
    /// 蓝牙按钮图片
    /// </summary>
    public UnityEngine.UI.Image bluetoothBtnImage = null;

    /// <summary>
    /// 蓝牙按钮已连接状态图片
    /// </summary>
    public Sprite bluetoothBtnEnabledSprite = null;

    /// <summary>
    /// 蓝牙按钮未连接状态图片
    /// </summary>
    public Sprite bluetoothBtnDisabledSprite = null;
    // Use this for initialization
    void Start()
    {       
        BluetoothWindow.OnBluetoothConnectSuccess += OnBluetoothConnectSuccess;
        BluetoothWindow.OnBluetoothConnectFail += OnBluetoothConnectFail;

        if (null != BluetoothSDK.BluetoothSdk)
        {
            if (BluetoothSDK.BluetoothSdk.IsConnected)
            {
                bluetoothBtnImage.sprite = bluetoothBtnEnabledSprite;
            }
            else
            {
                bluetoothBtnImage.sprite = bluetoothBtnDisabledSprite;
            }
        }        
    }
    void OnDestroy()
    {     
        BluetoothWindow.OnBluetoothConnectSuccess -= OnBluetoothConnectSuccess;
        BluetoothWindow.OnBluetoothConnectFail -= OnBluetoothConnectFail;
    }  

    private void OnBluetoothConnectFail()
    {
        bluetoothBtnImage.sprite = bluetoothBtnDisabledSprite;
    }

    private void OnBluetoothConnectSuccess()
    {
        bluetoothBtnImage.sprite = bluetoothBtnEnabledSprite;
    }

    // Update is called once per frame
    public void OpenBluetooth()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
        if (BluetoothSDK.BluetoothSdk != null)
        {
            BluetoothSDK.BluetoothSdk.OpenBluetoothPanel();
        }
    }

    // Use this for initialization
    public void RenameBluetooth()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
        if (BluetoothSDK.BluetoothSdk != null)
        {
            BluetoothSDK.BluetoothSdk.Rename();
        }
    }
}
