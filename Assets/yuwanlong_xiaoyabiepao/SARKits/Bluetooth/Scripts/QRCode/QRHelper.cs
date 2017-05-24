using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Timers;
using System.Threading;
/// <summary>  
/// 二维码解析工具  
/// 关键函数:  
///     public static QRHelper GetInst()                                      --得到单例  
///     public event Action<string> OnQRScanned;                              --扫描回调  
///     public void StartCamera(int index)                                    --启动摄像头  
///     public void StopCamera()                                              --停止摄像头  
/// </summary>  
public class QRHelper
{

    public event Action<string> OnQRScanned;

	private int m_reqW, m_reqH;
	private GameObject m_videoBackImage;
    private static QRHelper _inst;
    public static QRHelper GetInst()
    {
        if (_inst == null)
        {
            _inst = new QRHelper();
        }
        return _inst;
    }
    private WebCamTexture webcam;
    private bool isScanning = false;
	private bool startCameraFirstFrame = false;
    System.Timers.Timer timer_out;
    /// <summary>  
    /// 启动摄像头  
    /// </summary>  
    /// <param name="index">手机后置为0，前置为1</param>  
    public void StartCamera(int index,int reqW, int reqH,  GameObject videoBackImage)
    {
		m_videoBackImage = videoBackImage;
		startCameraFirstFrame = true;
		m_reqW = reqW;
		m_reqH = reqH;
        StopCamera();
        lock (mutex)
        {
            buffer = null;
            tbuffer = null;
        }
        var dev = WebCamTexture.devices;
        webcam = new WebCamTexture(dev[index].name, reqW, reqH,30);        
        webcam.Play();
									       
        InitTimer();
        timer_out.Start();
    }
   
    /// <summary>  
    /// 停止  
    /// </summary>  
    public void StopCamera()
    {
        if (webcam != null)
        {
            webcam.Stop();
            UnityEngine.Object.Destroy(webcam);
            Resources.UnloadUnusedAssets();
            webcam = null;
            StopScan();

      
            timer_out.Start();
            timer_out = null;
        }
    }
   
   public void StartScan()
    {     
        isScanning = true;      
    }
    public void StopScan()
    {
        isScanning = false;
    }
    public void ScanCode()
    {
        if (isScanning)
        {
            WriteDataBuffer();
        }
    }
    private void InitTimer()
    {
       
        timer_out = new System.Timers.Timer(400);
        timer_out.AutoReset = true;
        timer_out.Elapsed += (a, b) => {
            Analysis();
        };
    }
	private void ResetVideoTexture()
	{
		if (webcam.height * m_reqW > webcam.width * m_reqH)
		{
			float scale = 1.0f * (webcam.height * m_reqW) / (webcam.width * m_reqH);
			#if UNITY_IOS
			m_videoBackImage.transform.localScale = new Vector3(1, scale * -1, 0.0f);
			#elif UNITY_ANDROID
			m_videoBackImage.transform.localScale = new Vector3(1, scale * 1, 0.0f);
			#endif

		}
		else
		{
			float scale = 1.0f * (webcam.width * m_reqH) / (webcam.height * m_reqW);
			#if UNITY_IOS
			m_videoBackImage.transform.localScale = new Vector3(scale * 1, -1, 0.0f);
			#elif UNITY_ANDROID
			m_videoBackImage.transform.localScale = new Vector3(scale * 1, 1, 0.0f);
			#endif

		}
		m_videoBackImage.GetComponent<RawImage>().texture = webcam;
        m_videoBackImage.SetActive(true);

    }

    private Color32[] buffer = null;
    private Color32[] tbuffer = null;
    private object mutex = new object();

    int dw, dh;
    private void WriteDataBuffer()
    {
        lock (mutex)
        {
            if (buffer == null && webcam != null)
            {
                buffer = webcam.GetPixels32();
				if (startCameraFirstFrame) 
				{
					startCameraFirstFrame = false;
					ResetVideoTexture ();
				}
                dw = webcam.width;
                dh = webcam.height;
            }
        }
    }
    //解析二维码  
    private void Analysis()
    {
        if (isScanning)
        {
            lock (mutex)
            {
                tbuffer = buffer;
                buffer = null;
            }
            if (tbuffer == null)
            {
                ;
            }
            else
            {
                string str = QR.DecodeColData(tbuffer, dw, dh);
                tbuffer = null;
                if (!string.IsNullOrEmpty(str) && OnQRScanned != null)
                {
                    ThreadPool.QueueUserWorkItem((obj) => {
                        if (OnQRScanned != null)
                            OnQRScanned(str);
                    });
                }
            }
        }
        tbuffer = null;
    }
}