using UnityEngine;
using System.Collections;
using Showbaby.Bluetooth;
using UnityEngine.SceneManagement;
using I2.Loc;

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
        BluetoothWindow.OnQRCodeStartCamera += QRCodeStartCamera;
        BluetoothWindow.OnQRCodeStopCamera += QRCodeStopCamera;
        BluetoothWindow.OnBluetoothConnectSuccess += OnBluetoothConnectSuccess;
        BluetoothWindow.OnBluetoothConnectFail += OnBluetoothConnectFail;

        if(BluetoothSDK.BluetoothSdk.IsConnected)
        {
            bluetoothBtnImage.sprite = bluetoothBtnEnabledSprite;
        }else
        {
            bluetoothBtnImage.sprite = bluetoothBtnDisabledSprite;
        }
    }
    void OnDestroy()
    {
        BluetoothWindow.OnQRCodeStartCamera -= QRCodeStartCamera;
        BluetoothWindow.OnQRCodeStopCamera -= QRCodeStopCamera;

        BluetoothWindow.OnBluetoothConnectSuccess -= OnBluetoothConnectSuccess;
        BluetoothWindow.OnBluetoothConnectFail -= OnBluetoothConnectFail;
    }

    private void QRCodeStartCamera()
    {
 //       CameraSettings.OnQRCodeStartCamera();
    }

    private void QRCodeStopCamera()
    {
//        CameraSettings.OnQRCodeStopCamera();
    }


    private void OnBluetoothConnectFail()
    {
        bluetoothBtnImage.sprite = bluetoothBtnDisabledSprite;
    }

    private void OnBluetoothConnectSuccess()
    {
        bluetoothBtnImage.sprite = bluetoothBtnEnabledSprite; ;
    }

    // Update is called once per frame
    public void OpenBluetooth()
    {
        if (BluetoothSDK.BluetoothSdk != null)
        {
            BluetoothSDK.BluetoothSdk.OpenBluetoothPanel();
        }
    }

    // Use this for initialization
    public void RenameBluetooth()
    {
        if (BluetoothSDK.BluetoothSdk != null)
        {
            BluetoothSDK.BluetoothSdk.Rename();
        }
    }

    public void GotoVuforia()
    {
        if (BluetoothSDK.BluetoothSdk != null && BluetoothSDK.BluetoothSdk.IsBluetoothConnect())
        {
            SceneManager.LoadScene("Vuforia");
        }

    }

    // Update is called once per frame
    public void GotoBack()
    {
        SceneManager.LoadScene("Start");
    }

    public void ChangeLanguage()
    {
        if (LocalizationManager.CurrentLanguage == "Chinese")
        {
            SetLanguage("English");
        }
        else
        {
            SetLanguage("Chinese");
        }
    }
    private void SetLanguage(string _Language)
    {
        if (LocalizationManager.HasLanguage(_Language))
        {
            LocalizationManager.CurrentLanguage = _Language;
        }
    }
}
