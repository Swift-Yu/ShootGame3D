using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;
using LitJson;
using UnityEngine.SceneManagement;
using Showbaby;
using Showbaby.UI;

namespace Showbaby.Bluetooth
{
    /// <summary>
    /// 蓝牙SDK
    /// </summary>
    public class BluetoothSDK : MonoBehaviour
    {
        #region 类的实例对象
        private static BluetoothSDK bluetoothSdk = null;

        public static BluetoothSDK BluetoothSdk
        {
            get
            {
                return bluetoothSdk;
            }
        }
        #endregion

        #region 初始化蓝牙参数
        [HideInInspector]
        public string DEVICENAME = "SHOWBABY";//过滤参数
        [HideInInspector]
        public string SERVICEUUID = "0000fff0-0000-1000-8000-00805f9b34fb";
        [HideInInspector]
        public string CHARUUID = "0000fff4-0000-1000-8000-00805f9b34fb";
        [HideInInspector]
        public string RECEIVERCHARUUID = "0000fff1-0000-1000-8000-00805f9b34fb";
#if UNITY_ANDROID
        [HideInInspector]
        public string DESCUUUID = "00002902-0000-1000-8000-00805f9b34fb";
#endif

        #region 以下字段为以前的参数，目前没什么用，当前版本未向android传这些参数，但是ios还是保留着了。 
        [HideInInspector]
        public int waitOpenTime = 500;
        [HideInInspector]
        public int scanningTime = 30000;
        [HideInInspector]
        public int findOtherDeviceTime = 3000;
        #endregion

#if UNITY_IPHONE
         [HideInInspector]
        public string ios_deviceInformationServiceUUID = "180A";
         [HideInInspector]
        public string ios_hardwareRevisionStringUUID = "2A27";
#endif
        #endregion

        /// <summary>
        /// 允许的最长连接时间 连接时间超过此值就停止连接并提示连接失败请稍后重试 并且在连接超时的状态下不接收任何蓝牙消息 
        /// </summary>
        [HideInInspector]
        public float MaxConnecttingTime = 10f;        
        

        #region 委托及事件定义
        [Serializable]
        public class BluetoothEvent : UnityEvent<JsonData> { }

        public UnityEvent OnShootDownEvent;//射击键按下
        public UnityEvent OnShootUpEvent;//射击键松开
        public UnityEvent OnSmoothDownEvent;//滑膛键按下
        public UnityEvent OnSmoothUpEvent;//滑膛键松开
        public UnityEvent OnActionDownEvent;// Action键按下
        public UnityEvent OnActionUpEvent;//  Action键松开
        /// <summary>
        /// 蓝牙连接成功
        /// </summary>
        [HideInInspector]
        public UnityEvent OnConnectedSuccessedEvent;

        /// <summary>
        /// 蓝牙连接失败
        /// </summary>
        [HideInInspector]
        public UnityEvent OnConnectedFailedEvent;

        /// <summary>
        /// 连接超时
        /// </summary>
        [HideInInspector]
        public UnityEvent OnConnectedTimeoutEvent;

        /// <summary>
        /// 蓝牙断开
        /// </summary>
        [HideInInspector]
        public UnityEvent OnDisconnectedEvent;
        /// <summary>
        /// unity主动断开
        /// </summary>
        [HideInInspector]
        public UnityEvent OnUnityDisconnectedEvent;
        /// <summary>
        /// 蓝牙重连
        /// </summary>
        [HideInInspector]
        public UnityEvent OnReconnectingEvent;
        /// <summary>
        /// 收到蓝牙设备信息
        /// </summary>
        [HideInInspector]
        public BluetoothEvent OnGetDeviceInfoEvent;

        /// <summary>
        /// 用户允许开启蓝牙
        /// </summary>
        [HideInInspector]
        public UnityEvent OnEnabledSuccessedEvent;

#if UNITY_IPHONE
        //需要用户手动开启蓝牙IOS
        [HideInInspector]
        public UnityEvent OnNeedOpenBluetoothEvent;
#endif
        /// <summary>
        /// 用户拒绝开启蓝牙
        /// </summary>
        [HideInInspector]
        public UnityEvent OnEnabledFailedEvent;

        /// <summary>
        /// 用户授权允许开启蓝牙
        /// </summary>
        [HideInInspector]
        public UnityEvent OnGrantedSuccessedEvent;

        /// <summary>
        /// 用户拒绝授权蓝牙
        /// </summary>
        [HideInInspector]
        public UnityEvent OnGrantedFailedEvent;

        /// <summary>
        /// 用户拒绝授权并不再提醒
        /// </summary>
        [HideInInspector]
        public UnityEvent OnGrantedNeverEvent;
#endregion

#region 变量定义
        /// <summary>
        /// android ios的蓝牙对象
        /// </summary>
        [HideInInspector]
        public BluetoothSDKImpl bluetoothSDKUtils = null;

        /// <summary>
        /// 当前是否已连接
        /// </summary>
        //[HideInInspector]
        public bool IsConnected = false;

        /// <summary>
        /// 蓝牙当前连接状态 0未连接 1连接中 2已连接 3连接超时 连接成功的时候置为已连接 连接失败或者断开的时候置为未连接
        /// </summary>
        [HideInInspector]
        private ConnectState ConnectedState = ConnectState.DisConnected;

        private enum ConnectState
        {
            /// <summary>
            /// 0未连接
            /// </summary>
            DisConnected = 0,
            /// <summary>
            /// 1连接中
            /// </summary>
            Connecting,
            /// <summary>
            /// 2已连接
            /// </summary>
            Connected,
            /// <summary>
            /// 3连接超时
            /// </summary>
            ConnectTimeout,
            /// <summary>
            /// 重连
            /// </summary>
            ReConnecting
        }

        /// <summary>
        /// 连接计时起始时间
        /// </summary>
        private float startTime = -1f;
       
        /// <summary>
        /// 是否已授权 ios没有 用不到
        /// </summary>
        [HideInInspector]
        public bool IsGranted = false;

#if UNITY_IPHONE
        /// <summary>
        /// ios蓝牙是否已开启
        /// </summary>
        private bool isOpenedIOS = false;

        /// <summary>
        /// 是否是第一次询问
        /// </summary>
        private bool isFrist = true;
#endif
        /// <summary>
        /// 权限的状态 -100允许 -101拒绝 -102拒绝且不再提醒
        /// </summary>
        [HideInInspector]
        public int GrantedState = -101;

        /// <summary>
        /// 是否正在扫描蓝牙
        /// </summary>
        [HideInInspector]
        public bool isScanning = false;
       
#endregion

#region 消息类型常量
        /// <summary>
        /// 不支持蓝牙
        /// </summary>
        private const int UNSUPPORT_BLUETOOTH = -1;

        /// <summary>
        /// 不支持低功耗
        /// </summary>
        private const int UNSUPPORT_BLE = -2;

        /// <summary>
        /// 没有启用蓝牙
        /// </summary>
        private const int BLUETOOTH_DISENABLED = -3;

        /// <summary>
        /// 开启了蓝牙
        /// </summary>
        private const int BLUETOOTH_ENABLED = -4;

        /// <summary>
        /// 扫描到设备
        /// </summary>
        private const int BLUETOOTH_SCAN_DEVICE = 0;

        /// <summary>
        /// 连接成功
        /// </summary>
        private const int BLUETOOTH_CONNECTED = 1;

        /// <summary>
        /// 断开连接
        /// </summary>
        private const int BLUETOOTH_DISCONNECTED = 2;

        /// <summary>
        /// 服务被发现
        /// </summary>
        private const int BLUETOOTH_SERVICE_DISCOVERED = 3;

        /// <summary>
        /// 通信值有改变
        /// </summary>
        private const int BLUETOOTH_VALUE_CHANGED = 4;

        /// <summary>
        /// 重连
        /// </summary>
        private const int BLUETOOTH_RECONNECT = 5;

        /// <summary>
        /// 权限允许
        /// </summary>
        private const int PERMISSION_GRANTED = -100;

        /// <summary>
        /// 权限拒绝
        /// </summary>
        private const int PERMISSION_DENIED = -101;

        /// <summary>
        /// 权限拒绝了，不再提醒
        /// </summary>
        private const int PERMISSION_DENIED_NEVER = -102;
#endregion

        void Awake()
        {
            if (null == bluetoothSdk)
            {
                bluetoothSdk = this;
                GameObject.DontDestroyOnLoad(bluetoothSdk.gameObject);              
            }
        }

        void Start()
        {
            Init();
        }

        void Update()
        {
            if (startTime != -1f)
            {
                //Debug.Log("Time.time " + Time.time + " startTime " + startTime);
                if (Time.time - startTime >= MaxConnecttingTime)
                {
                    startTime = -1f;
                    IsConnected = false;
                    ConnectedState = ConnectState.ConnectTimeout;
                    //主动断开连接并停止扫描
                    //Debug.Log("主动断开连接并停止扫描");
                    Disconnect();
                    StopScan();
                    if (null != OnConnectedTimeoutEvent)
                    {
                        //Debug.Log("Update 蓝牙连接超时 ");
                        OnConnectedTimeoutEvent.Invoke();
                    }
                }
            }           
        }
        //打开蓝牙
        public void OpenBluetoothPanel()
        {
           GetComponentInChildren<BluetoothWindow>().OpenBluetoothSelectPanel();
        }
        //判断蓝牙是否连接成功
        public bool IsBluetoothConnect()
        {
            return GetComponentInChildren<BluetoothWindow>().IsBluetoothConnect();
        }
        /// <summary>
        /// 查看当前蓝牙是否可用(检查蓝牙是否已开启以及是否已授权，并不考虑是否连接成功)
        /// </summary>
        /// <returns></returns>
        public bool IsBluetoothReady()
        {

#if UNITY_ANDROID && !UNITY_EDITOR
            //是否已开启
            var IsOpend = IsEnabled();
            if (IsOpend)
            {
                //是否已授权
                if (!IsGranted)
                {
                    if (GrantedState == PERMISSION_DENIED)
                    {
                        RequestPermission();
                    }
                    if (GrantedState == PERMISSION_DENIED_NEVER)
                    {
                        ArShowContext.Instance.OpenAppSetting();
                        GrantedState = PERMISSION_DENIED;//这里将状态置为拒绝，以便下次能够再次请求权限
                    }
                }
                return IsGranted;
            }
            else
            {
                OpenBluetooth();
            }
            return IsOpend;
#elif UNITY_IPHONE
            if (!isOpenedIOS)
            {
                if (isFrist)
                {
                    isFrist = false;
                    OpenBluetooth();
                }else
                {
                    OnNeedOpenBluetoothEvent.Invoke();
                }
            }
            return isOpenedIOS;
#elif UNITY_EDITOR
            return true;
#endif
        }

        public void Init()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            //初始化unityactivity（每个android插件初始化的时候都会加上这句保证unityactivity初始化，Init内部保证次方法只执行一次）
            ArShowContext.Instance.Init();
            bluetoothSDKUtils = new AndroidImpl(DEVICENAME, gameObject.name, "ReceiveBluetoothMessage",
            SERVICEUUID, CHARUUID, RECEIVERCHARUUID, DESCUUUID);
#elif UNITY_IPHONE && !UNITY_EDITOR
            bluetoothSDKUtils = new iOSImpl(gameObject.name, "ReceiveBluetoothMessage",
                DEVICENAME, SERVICEUUID, CHARUUID, RECEIVERCHARUUID, ios_deviceInformationServiceUUID,
                ios_hardwareRevisionStringUUID, findOtherDeviceTime);
#endif
        }

        public bool IsEnabled()
        {
#if !UNITY_EDITOR
            if (null != bluetoothSDKUtils)
            {
                return bluetoothSDKUtils.IsEnabled();
            }
            return false;
#else
            return true;//便于编辑器下测试
#endif
        }

        public void OpenBluetooth()
        {
            if (null != bluetoothSDKUtils)
            {
                bluetoothSDKUtils.OpenBluetooth();
            }
        }

        public void RequestPermission()
        {
            if (null != bluetoothSDKUtils)
            {
                bluetoothSDKUtils.RequestPermission();
            }
        }

        public void StartScan()
        {
            //Debug.Log("StartScan isScanning " + isScanning);
            if(!isScanning)
            {
                if (null != bluetoothSDKUtils)
                {
                    //Debug.Log("StartScan bluetoothSDKUtils.StartScan()");
                    bluetoothSDKUtils.StartScan();
                }
                isScanning = true;
            }                      
        }    
        public void StopScan()
        {            
            if (isScanning)
            {
                if (null != bluetoothSDKUtils)
                {
            //        Debug.Log("StopScan bluetoothSDKUtils.StopScan()");
                    bluetoothSDKUtils.StopScan();
                }
                isScanning = false;
            }
        }
        public void RestartScan()
        {
            if (null != bluetoothSDKUtils)
            {
                bluetoothSDKUtils.StopScan();
                bluetoothSDKUtils.StartScan();
                isScanning = true;
            }  
        }
        public void Connect(string address)
        {
            if (null != bluetoothSDKUtils)
            {
                bluetoothSDKUtils.Connect(address);
                ConnectedState = ConnectState.Connecting;
                startTime = Time.time;
            }            
        }

        public void Disconnect()
        {
            if (null != bluetoothSDKUtils)
            {
                bluetoothSDKUtils.Disconnect();
            }            
        }

        public void DestroyBluetooth()
        {
            if (null != bluetoothSdk.OnGetDeviceInfoEvent)
            {
                bluetoothSdk.OnGetDeviceInfoEvent.RemoveAllListeners();
            }
            if (null != bluetoothSdk.OnConnectedSuccessedEvent)
            {
                bluetoothSdk.OnConnectedSuccessedEvent.RemoveAllListeners();
            }
            if (null != bluetoothSdk.OnConnectedFailedEvent)
            {
                bluetoothSdk.OnConnectedFailedEvent.RemoveAllListeners();
            }
            if (null != bluetoothSdk.OnDisconnectedEvent)
            {
                bluetoothSdk.OnDisconnectedEvent.RemoveAllListeners();
            }
            if (null != bluetoothSdk.OnGrantedSuccessedEvent)
            {
                bluetoothSdk.OnGrantedSuccessedEvent.RemoveAllListeners();
            }
            if (null != bluetoothSdk.OnConnectedTimeoutEvent)
            {
                bluetoothSdk.OnConnectedTimeoutEvent.RemoveAllListeners();
            }
            if (null != bluetoothSdk.OnReconnectingEvent)
            {
                bluetoothSdk.OnReconnectingEvent.RemoveAllListeners();
            }
            if (null != bluetoothSdk.OnUnityDisconnectedEvent)
            {
                bluetoothSdk.OnUnityDisconnectedEvent.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 收到蓝牙消息
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveBluetoothMessage(string message)
        {
//            Debug.Log("ReceiveBluetoothMessage message " + message);
            var jsonData = JsonMapper.ToObject(message);
            if (jsonData.Keys.Contains("status"))
            {
                var status = (int)jsonData["status"];
                var data = jsonData["data"];
                switch (status)
                {
                    case BLUETOOTH_SCAN_DEVICE:
                        if (null != OnGetDeviceInfoEvent)
                        {
                            //Debug.Log("扫描到设备 " + data);
                            OnGetDeviceInfoEvent.Invoke(data);
                        }
                        break;
                    case BLUETOOTH_SERVICE_DISCOVERED://BLUETOOTH_CONNECTED只是连接成功 BLUETOOTH_SERVICE_DISCOVERED才是真正的可以正常通讯了
                        IsConnected = true;
                        startTime = -1f;//连接成功的时候才停止计时
                        ConnectedState = ConnectState.Connected;
                        if (null != OnConnectedSuccessedEvent)
                        {
                            //Debug.Log("蓝牙连接成功 ");                            
                            OnConnectedSuccessedEvent.Invoke();
                        }
                        break;
                    case BLUETOOTH_DISCONNECTED:
                        //避免无限断开的提示
                        if (ConnectedState != ConnectState.DisConnected)
                        {
                            ConnectedState = ConnectState.DisConnected;
                            if (!IsConnected)
                            {
                                startTime = -1f;//连接失败的时候才停止计时
                                                //当前未连接则为连接失败
                                if (null != OnConnectedFailedEvent)
                                {
                                    //Debug.Log("蓝牙连接失败 ");
                                    OnConnectedFailedEvent.Invoke();
                                }
                            }
                            else
                            {
                                //当前已连接则为断开连接
                                IsConnected = false;
                                if (null != OnDisconnectedEvent)
                                {
                                    //Debug.Log("蓝牙连接断开 ");
                                    OnDisconnectedEvent.Invoke();
                                }
                            }
                        }
                        break;
                    case BLUETOOTH_VALUE_CHANGED:
                        if (data[0].Keys.Contains("key"))
                        {
                            var key = data[0]["key"].ToString();
                            //Debug.Log("蓝牙消息 key " + key);
                            if (key.Equals("B2DOWN"))
                            {
                                if (null != OnShootDownEvent)
                                {
                                   OnShootDownEvent.Invoke();
                                }                                
                            }
                            else if (key.Equals("B2UP"))
                            {
                                if (null != OnShootUpEvent)
                                {
                                    OnShootUpEvent.Invoke();
                                }                                
                            }
                            else if (key.Equals("B3DOWN"))
                            {
                                //装子弹
                                if (null != OnSmoothDownEvent)
                                {
                                    OnSmoothDownEvent.Invoke();
                                }                               
                            }
                            else if (key.Equals("B3UP"))
                            {                              
                                if (null != OnSmoothUpEvent)
                                {
                                    OnSmoothUpEvent.Invoke();
                                }
                            }
                            else if (key.Equals("B4DOWN"))
                            {
                                if (null != OnActionDownEvent)
                                {
                                    OnActionDownEvent.Invoke();
                                }                                
                            }
                            else if (key.Equals("B4UP"))
                            {
                                if (null != OnActionUpEvent)
                                {
                                    OnActionUpEvent.Invoke();
                                }
                            }
                        }
                        break;
                    case BLUETOOTH_RECONNECT:
                        //重连
                        ConnectedState = ConnectState.ReConnecting;
                        startTime = Time.time;
                        if (null != OnReconnectingEvent)
                        {
                            //Debug.Log("蓝牙重连中 ");
                            OnReconnectingEvent.Invoke();
                        }
                        break;
                    case PERMISSION_GRANTED:
                        //用户授权蓝牙的时候点了允许
                        GrantedState = PERMISSION_GRANTED;
                        IsGranted = true;
                        if (null != OnGrantedSuccessedEvent)
                        {
                            //Debug.Log("用户授权蓝牙的时候点了允许");
                            OnGrantedSuccessedEvent.Invoke();
                        }
                        break;
                    case PERMISSION_DENIED:
                        //用户授权蓝牙的时候点了拒绝
                        GrantedState = PERMISSION_DENIED;
                        IsGranted = false;
                        if (null != OnGrantedFailedEvent)
                        {
                            //Debug.Log("用户授权蓝牙的时候点了拒绝 ");
                            OnGrantedFailedEvent.Invoke();
                        }
                        break;
                    case PERMISSION_DENIED_NEVER:
                        //用户授权蓝牙的时候点了拒绝不再提醒
                        GrantedState = PERMISSION_DENIED_NEVER;
                        IsGranted = false;
                        if (null != OnGrantedNeverEvent)
                        {
                            //Debug.Log("用户授权蓝牙的时候点了拒绝不再提醒 ");
                            OnGrantedNeverEvent.Invoke();
                        }
                        break;
                    case BLUETOOTH_DISENABLED:
                        //没有启用蓝牙
#if UNITY_IPHONE
                        isOpenedIOS = false;
#endif
                        if (null != OnEnabledFailedEvent)
                        {
                            //Debug.Log("没有启用蓝牙 ");
                            OnEnabledFailedEvent.Invoke();
                        }
                        break;
                    case BLUETOOTH_ENABLED:
                        //Debug.Log("开启了蓝牙 请求权限 ");
                        //开启了蓝牙
#if UNITY_ANDROID
                        RequestPermission();
                        if (null != OnEnabledSuccessedEvent)
                        {                            
                            OnEnabledSuccessedEvent.Invoke();
                        }
#elif UNITY_IPHONE
                        isOpenedIOS = true;
                        if (null != OnGrantedSuccessedEvent)
                        {
                            Debug.Log("ios 开启了蓝牙");
                            OnGrantedSuccessedEvent.Invoke();
                        }
#endif
                        break;
                    default:
                        //Debug.LogError("其它消息类型 status " + status);
                        break;
                }
            }
        }

        /// <summary>
        /// 重命名指定mac地址的蓝牙设备
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="name"></param>
        public void Rename()
        {
            string address = PlayerPrefs.GetString("preBluetoothAddress", "");
            if (!address.Equals(""))
            {
                if (TipWindow.Instance)
                {
                    TipWindow.Instance.ShowConfirmWindowInput(GetComponentInChildren<BluetoothWindow>().GetBluetoothDeviceInfo(address).alias , 
                        I2.Loc.ScriptLocalization.TipWindow.ConfirmText, 
                        I2.Loc.ScriptLocalization.TipWindow.CancelText,
                        (string name)=> 
                        {
                            if (GetComponentInChildren<BluetoothWindow>().SetBluetoothDeviceName(address, name))
                            {
                                 TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.RenameSuccess, 1.5f);                                
                            }
                            else
                            {                               
                                 TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.RenameFailed, 1.5f);                                
                            }
                        });
                }
                

            }else
            {
                if(TipWindow.Instance)
                {
                    TipWindow.Instance.ShowAlertWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.RenameNoDevice, I2.Loc.ScriptLocalization.TipWindow.ConfirmText,()=> { });
                }
            }
        }

        /// <summary>
        /// 从游戏切到桌面或者从桌面切回游戏
        /// </summary>
        /// <param name="pause"></param>
        void OnApplicationPause(bool pause)
        {
         //   Debug.LogError("OnApplicationPause pause " + pause);
            if (pause)
            {
                //Debug.LogError("1 OnApplicationPause pause " + pause);
                //暂停的时候断开
                ConnectedState = ConnectState.DisConnected;
                if (!IsConnected)
                {
                    //startTime = -1f;//连接失败的时候才停止计时
                    ////当前未连接则为连接失败
                    //if (null != OnConnectedFailedEvent)
                    //{
                    //    //Debug.Log("蓝牙连接失败 ");
                    //    OnConnectedFailedEvent.Invoke();
                    //}
                }
                else
                {
                    //当前已连接则为断开连接 unity主动断开
                    ConnectedState = ConnectState.DisConnected;
                    IsConnected = false;
                    if (null != OnUnityDisconnectedEvent)
                    {
                        //Debug.Log("unity主动断开蓝牙连接");
                        OnUnityDisconnectedEvent.Invoke();
                    }
                }
            }
            else
            {
                //Debug.LogError("2 OnApplicationPause pause " + pause);
            }
        }

        void OnDestroy()
        {
   //         Debug.LogError("OnDestroy 1 ");
            DestroyBluetooth();
            bluetoothSDKUtils = null;
            bluetoothSdk = null;
        }
    }
}