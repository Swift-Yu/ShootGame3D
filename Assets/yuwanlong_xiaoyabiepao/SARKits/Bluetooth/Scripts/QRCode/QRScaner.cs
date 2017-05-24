using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Showbaby.Bluetooth;
using Showbaby.UI;
public enum CameraType
{
    Webcam,
    Vuforia
}
public class QRScaner : MonoBehaviour {
    public GameObject videoBackImage;
    float timer = 0;
    string codeString = null;
    string currentCodeString = "";
  
    void Awake()
    {
        videoBackImage.SetActive(false);
        QRHelper.GetInst().OnQRScanned += OnQRScanned;
    }
    void OnDestroy()
    {
        QRHelper.GetInst().OnQRScanned -= OnQRScanned;
    }
    void OnDisable()
    {      
        videoBackImage.SetActive(false);
        codeString = null;
        currentCodeString = "";
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.3f) //0.5秒扫描一次
        {
            StartCoroutine(ScanCode());
            timer = 0;
        }
        if (codeString != null && !currentCodeString.Equals(codeString))
        {
            //扫码值改变          
            BluetoothWindow.Instance.OnGetQRCodeResult(codeString);
            currentCodeString = codeString;
        }
    }
    public void StartCamera(CameraType type)
    {
        if(type == CameraType.Webcam)
        {
            //打开相机，开始扫描
            StartCoroutine(InitCamera());
        }
       
    }
    public void StopCamera(CameraType type)
    {
        if (type == CameraType.Webcam)
        {
            //停止相机
            QRHelper.GetInst().StopCamera();
        }
    }
    public void StartScan()
    {
        QRHelper.GetInst().StartScan();
        codeString = null;
        currentCodeString = "";
    }
    public void StopScan()
    {
        QRHelper.GetInst().StopScan();
    }

    protected IEnumerator ScanCode()
    {
        yield return new WaitForEndOfFrame();
        QRHelper.GetInst().ScanCode();

    }
    protected IEnumerator InitCamera()
    {
        //获取授权
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))//在移动平台总是为真
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                QRHelper.GetInst().StartCamera(0, Screen.width / 2, Screen.height / 2, videoBackImage);
                Invoke("StartScan",0.3f);
            }
        }
    }

    //非主线程回调
    private void OnQRScanned(string obj)
    {
        codeString = obj;
    }

}
