using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.Collections.Generic;
using UnityEngine.Events;
using Showbaby.Command;
using Showbaby.UI;
using System;

namespace Showbaby.Bluetooth
{
    /// <summary>
    /// 蓝牙界面
    /// </summary>
    public class BluetoothWindow : MonoBehaviour
    {
        #region 面板设置       

        /// <summary>
        /// 提示消息在tipAutoHideDelayTime后自动隐藏
        /// </summary>
        public float tipAutoHideDelayTime = 3f;

      

        /// <summary>
        /// 选择连接方式界面
        /// </summary>
        public GameObject selectConnectModePanel = null;

        /// <summary>
        /// 连接方式选择界面的提示
        /// </summary>
        public Text selectConnectModePanelTipText = null;    

        /// <summary>
        /// 扫描二维码界面
        /// </summary>
        public GameObject qrCodePanel = null;

        /// <summary>
        /// 请在光亮环境下的提示文字
        /// </summary>
        public Text QRCodePanelTipText = null;

        /// <summary>
        /// 蓝牙列表界面
        /// </summary>
        public GameObject bluetoothListPanel = null;

        /// <summary>
        /// 列表界面没有蓝牙设备的提示
        /// </summary>
        public Text NoneDeviceTipText = null;       

        /// <summary>
        /// 蓝牙列表界面ScrollView的Content
        /// </summary>
        public RectTransform content = null;

        /// <summary>
        /// 蓝牙item节点（用来克隆）
        /// </summary>
        public GameObject bluetoothItemObj = null;

        /// <summary>
        /// 用户是否已授权开启蓝牙
        /// </summary>
        [HideInInspector]
        public bool HasGranted = false;

        /// <summary>
        /// 蓝牙是否已开启
        /// </summary>
        [HideInInspector]
        public bool IsEnabled = false;

        /// <summary>
        /// 已连接的蓝牙mac地址
        /// </summary>
        [HideInInspector]
        public string connectedAddress = "";

        #endregion

        public delegate void QRCodeDelegate();
        public static event QRCodeDelegate OnQRCodeStartCamera;//二维码扫描开启摄像头
        public static event QRCodeDelegate OnQRCodeStopCamera;//二维码扫描关闭摄像头      

        public delegate void BluetoothDelegate();
        public static event BluetoothDelegate OnBluetoothConnectSuccess;//蓝牙连接成功
        public static event BluetoothDelegate OnBluetoothConnectFail;//蓝牙连接失败 

        #region 私有变量
        /// <summary>
        /// 当前请求连接的蓝牙mac地址
        /// </summary>
        private string requestAddress = "";

        /// <summary>
        /// 当前请求连接的蓝牙的名字
        /// </summary>
        private string requestName = "";

        /// <summary>
        /// IOS Home重连的id
        /// </summary>
        private string reconnectAddress = "";

        /// <summary>
        /// 上次连接成功的蓝牙manc地址
        /// </summary>
        private string preBluetoothAddress = "";

        /// <summary>
        /// 当前已连接的蓝牙的名字
        /// </summary>
        private string curConnectedName = "";

        /// <summary>
        /// 当前选择的蓝牙mac地址
        /// </summary>
        private string selectedBluetoothAddress = "";

        /// <summary>
        /// 上一次选择的蓝牙mac地址
        /// </summary>
        private string preSelectedBluetoothAddress = "";

        /// <summary>
        /// 当前请求的二维码code
        /// </summary>
        private string curRequestCode = "";

        /// <summary>
        /// 当前扫描到的蓝牙列表信息
        /// </summary>
        private static List<BluetoothSDKDevInfo> curBluetoothList = new List<BluetoothSDKDevInfo>();

        /// <summary>
        /// 蓝牙列表界面当前显示的蓝牙列表
        /// </summary>
        private static List<GameObject> curBluetoothGameObjectList = new List<GameObject>();

        /// <summary>
        /// 本地记录的二维码信息列表(之前扫描获取mac地址成功的都会保留)
        /// </summary>
        private List<QRCodeInfo> localQRCodeInfoList = new List<QRCodeInfo>();

        /// <summary>
        /// 是否从其他界面进入
        /// </summary>
        private bool isFromSetting = false;

        /// <summary>
        /// 是否从关卡进入
        /// </summary>
        private bool isFromChapter = false;

        /// <summary>
        /// 扫描识别的二维码信息
        /// </summary>
        private QRCodeInfo curQRCodeInfo = null;

        /// <summary>
        /// 是否识别二维码成功
        /// </summary>
        private bool isGetQRCodeSuccessed = false;

        private bool isQRCodeConnect = false;
        private bool isFirstShow = true;
        private enum QRCodeConnectState
        {          
            QRCodeNoneNetwork,
            QRCodeSuccessed,
            QRCodeError,
            QRCodeInvalid          
        }
        ///// <summary>
        ///// 是否成功收到枪的名字规则消息
        ///// </summary>
        //private bool isGetGunNameRuleSuccessed = false;

        /// <summary>
        /// 枪的名字规则列表
        /// </summary>
        private List<GunNameRule> ruleList = new List<GunNameRule>();

        /// <summary>
        /// ios切回来的时候重连
        /// </summary>
        private bool iosAutoConnect = true;

        /// <summary>
        /// 记录当前开启摄像机时隐藏的画布 便于在恢复时还原
        /// </summary>
        private List<GameObject> curHideCanvasList = new List<GameObject>();

        /// <summary>
        /// 扫描二维码结果的提示内容
        /// </summary>
        private string qrcoderesult = "";

        /// <summary>
        /// 断开的时候自动重连
        /// </summary>
        private bool autoconnect = true;
        #endregion

        #region 与语言相关的字符串属性
        //private string QRCodeError = "二维码有误";
        //private string QRCodeInvalid = "非法蓝牙设备";
        //private string QRCodeSuccessed = "获取成功";
        //private string BluetoothConnectting = "连接中...";
        //private string BluetoothConnectedSuccessed = "连接成功";
        //private string BluetoothConnectedFailed = "连接失败，请在设置中进行设置";
        //private string BluetoothDisconnected = "已断开";
        //private string BluetoothConnectTimeout = "连接超时，请重试";
        //private string NoneDeviceTip = "没有搜索到可以连接的AR魔力枪信号~";
        //private string ConnectDeviceTip = "连接AR魔力枪";
        //private string NoneSelectTip = "请选择要连接的蓝牙枪";
        //private string SelectConnectModePanelTipNoneDevice = "需要连接AR魔力枪才可以开始战斗！";//<color=#0004FFFF></color>
        //private string SelectConnectModePanelTipHasConnected = "已连接";
        //private string CheckARGunOn = "请检查AR魔力枪是否已开启";
        //private string ConnectString = "连接";
        //private string CloseString = "关闭";       
        //private string BluetoothReconnecting = "蓝牙重连中...";     
        //private string OpenBluetoothInSettingPlease = "请在设置中开启蓝牙";
        #endregion       

        #region 类的实例
        private static BluetoothWindow mInstance = null;

        public static BluetoothWindow Instance
        {
            get
            {
                return mInstance;
            }
        }
        #endregion

        #region Unity引擎方法
        void Awake()
        {
            mInstance = this;
            //GameObject.DontDestroyOnLoad(mInstance.gameObject);
            preBluetoothAddress = PlayerPrefs.GetString("preBluetoothAddress");
        }

        // Use this for initialization
        void Start()
        {
            if (null != BluetoothSDK.BluetoothSdk.OnGetDeviceInfoEvent)
            {
                BluetoothSDK.BluetoothSdk.OnGetDeviceInfoEvent.AddListener(OnGetDeviceInfo);
            }
            if (null != BluetoothSDK.BluetoothSdk.OnConnectedSuccessedEvent)
            {
                BluetoothSDK.BluetoothSdk.OnConnectedSuccessedEvent.AddListener(OnConnectedSuccessed);
            }
            if (null != BluetoothSDK.BluetoothSdk.OnConnectedFailedEvent)
            {
                BluetoothSDK.BluetoothSdk.OnConnectedFailedEvent.AddListener(OnConnectedFailed);
            }
            if (null != BluetoothSDK.BluetoothSdk.OnDisconnectedEvent)
            {
                BluetoothSDK.BluetoothSdk.OnDisconnectedEvent.AddListener(OnDisconnected);
            }
            if (null != BluetoothSDK.BluetoothSdk.OnGrantedSuccessedEvent)
            {
                BluetoothSDK.BluetoothSdk.OnGrantedSuccessedEvent.AddListener(OnGrantedSuccessed);
            }
            if (null != BluetoothSDK.BluetoothSdk.OnConnectedTimeoutEvent)
            {
                BluetoothSDK.BluetoothSdk.OnConnectedTimeoutEvent.AddListener(OnConnectTimeout);
            }
            if (null != BluetoothSDK.BluetoothSdk.OnReconnectingEvent)
            {
                BluetoothSDK.BluetoothSdk.OnReconnectingEvent.AddListener(OnReconnecting);
            }
            if (null != BluetoothSDK.BluetoothSdk.OnUnityDisconnectedEvent)
            {
                BluetoothSDK.BluetoothSdk.OnUnityDisconnectedEvent.AddListener(OnUnityDisconnected);
            }
#if UNITY_IPHONE
            if (null != BluetoothSDK.BluetoothSdk.OnNeedOpenBluetoothEvent)
            {
                BluetoothSDK.BluetoothSdk.OnNeedOpenBluetoothEvent.AddListener(OnNeedOpenBluetoothEvent);
            }
#endif
            //向服务器请求枪的名字规则
            CmdManager.Instance.SendGunNameRuleCmd();
            OnStartOpenBluetooth();
        }
     
        // Update is called once per frame
        void Update()
        {
            if (isGetQRCodeSuccessed)
            {                
                if (null != TipWindow.Instance)
                {
                    if (isQRCodeConnect)
                    {
                        if (qrcoderesult == I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeSuccessed)
                        {
                            OnGetAddressSuccessed(curQRCodeInfo);
                            OnRequestCodeResult(QRCodeConnectState.QRCodeSuccessed);
                        }
                        else if (qrcoderesult == I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeError)
                        {
                            OnRequestCodeResult(QRCodeConnectState.QRCodeError);
                        }
                        else if (qrcoderesult == I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeInvalid)
                        {
                            OnRequestCodeResult(QRCodeConnectState.QRCodeInvalid);
                        }
                    }

                    TipWindow.Instance.ShowToastWindow(qrcoderesult, tipAutoHideDelayTime);
                    qrcoderesult = "";
                }                              
                isGetQRCodeSuccessed = false;             
            }
        }

        void OnDestroy()
        {

        }

        /// <summary>
        /// 从游戏切到桌面或者从桌面切回游戏
        /// </summary>
        /// <param name="pause"></param>
        void OnApplicationPause(bool pause)
        {
#if UNITY_IOS
            if (pause)
            {
                //记录当前连接的id 断开连接
                reconnectAddress = connectedAddress;
                BluetoothSDK.BluetoothSdk.Disconnect();
            }
            else
            {
                //重连
                if (!reconnectAddress.Equals(""))
                {
                    iosAutoConnect = true;
                    BluetoothSDK.BluetoothSdk.StartScan();
                }
            }
#endif            
        }

        #endregion Unity引擎方法

        #region 蓝牙回调
        /// <summary>
        /// 收到蓝牙列表消息
        /// </summary>
        /// <param name="json"></param>
        public void OnGetDeviceInfo(JsonData data)
        {
       //     Debug.Log("OnGetDeviceInfo null != data " + (null != data) + "  data.Count " + data.Count);
            if (null != data && data.Count > 0)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    //Debug.Log("OnGetDeviceInfo " + data[i].ToJson());
                    //var deviceInfo = JsonMapper.ToObject<BluetoothSDKDevInfo>(data[i].ToJson());
                    var deviceInfo = new BluetoothSDKDevInfo();
                    deviceInfo.address = data[i]["address"].ToString();
                    deviceInfo.name = data[i]["name"].ToString();
                    //如果重命名功能开放后 这里先从重命名存储的里面去取 取不到的话说明没有重命名就根据规则显示
                    if(GetBluetoothDeviceInfo(deviceInfo.address)!=null)
                    {
                        deviceInfo.alias = GetBluetoothDeviceInfo(deviceInfo.address).alias;
                    }else
                    {
                        //根据名称获取别名
                        deviceInfo.alias = GetAliasByName(deviceInfo.name);
                    }                    
                    var isNew = true;
                    foreach (var info in curBluetoothList)
                    {
                        if (info.address.Equals(deviceInfo.address))
                        {
                            isNew = false;
                            break;
                        }
                    }
                    //断开的时候的自动重连
                    //if (autoconnect)
                    //{
                    //    if (deviceInfo.address.Equals(preBluetoothAddress))
                    //    {
                    //        autoconnect = false;
                    //        Connect(preBluetoothAddress);
                    //    }
                    //}
#if UNITY_IOS
                    //重连条件： ios环境 当前未连接 该设备为上次连接的设备 当前设备为断开时连接的设备
                    if (iosAutoConnect && !BluetoothSDK.BluetoothSdk.IsConnected
                        && deviceInfo.address.Equals(preBluetoothAddress)
                        && reconnectAddress.Equals(deviceInfo.address))
                    {
                        //Debug.LogError("iosAutoConnect " + iosAutoConnect + " reconnectAddress " + reconnectAddress + " deviceInfo.address " 
                        //    + deviceInfo.address + " preBluetoothAddress " + preBluetoothAddress);
                        Connect(reconnectAddress);
                        reconnectAddress = "";
                        iosAutoConnect = false;
                    }
#endif

                    if (isNew)
                    {
                        //isNew保证每个蓝牙信息只会进一次

                        /*从本地记录里找是否有记录过这个蓝牙枪信息
                         * 如果没有就将该蓝牙信息保存到本地 目的是为了保留住所有扫描到的蓝牙信息，这样重命名之后能生效
                         * 有就不处理
                         */
                        WriteBluetoothInfoToLocal(deviceInfo);

                        //Debug.Log("OnGetDeviceInfo isNew address " + deviceInfo.address + "  name " + deviceInfo.name);
                        curBluetoothList.Add(deviceInfo);                      

                        //在这里刷新蓝牙列表界面
                        RefreshBluetoothListPanel();
                    }
                }
            }
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        private void OnConnectedSuccessed()
        {               
            changeBluetoothBtnSprite(true);

            //将之前连接的蓝牙置为未连接状态
            RefreshConnectedState(connectedAddress, false);
            connectedAddress = requestAddress;
            var dev = GetBluetoothDeviceInfo(connectedAddress);
            if (null != dev)
            {
                curConnectedName = dev.alias;
            }
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.CloseWaitingWindow();
                TipWindow.Instance.ShowToastWindow(curConnectedName + " " + I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectedSuccessed, tipAutoHideDelayTime);
            }
            //Debug.Log("连接成功的地址 " + connectedAddress);
            savePreBluetoothDeviceInfo(connectedAddress);//保存当前连接的mac地址
            if(null != dev)
            {
                bool isExit = false;
                for (int i = 0; i < curBluetoothList.Count; i++)
                {
                    if (dev.address.Equals(curBluetoothList[i].address))
                    {
                        isExit = true;
                        break;
                    }
                }
                if(!isExit)
                {
                    curBluetoothList.Add(dev);
  
                }            
            }
            RefreshBluetoothListPanel();
            //则将该蓝牙置为已连接状态
            RefreshConnectedState(connectedAddress, true);
            RefreshSelectConnectModePanelTip();
            //如果蓝牙列表可见 将其隐藏
            OnClickBluetoothListPanelCancelBtn();
            OnClickSelectConnectPanelCloseBtn();
            if (isQRCodeConnect)
            {
                OnQRCodeConnectResult(true);//二维码扫描成功
            }
        }

        /// <summary>
        /// 连接失败
        /// </summary>
        private void OnConnectedFailed()
        {
            if (isQRCodeConnect)
            {
                OnQRCodeConnectResult(false);//二维码扫描失败
            }
            RefreshSelectConnectModePanelTip();
            var device = GetBluetoothDeviceInfo(requestAddress);
            var deviceName = "";
            if (null != device)
            {
                deviceName = device.alias;
            }
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.CloseWaitingWindow();
                TipWindow.Instance.ShowToastWindow(requestName +" "+ I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectedFailed, tipAutoHideDelayTime);
            }
            ////如果蓝牙界面可见 则将该蓝牙置为未连接状态
            //RefreshConnectedState(connectedAddress, false);
            connectedAddress = "";
            //清空当前扫描到的蓝牙列表信息 重新扫描
            DeleteConnectedAddress(requestAddress);
            //在这里刷新蓝牙列表界面
            RefreshBluetoothItems();
            BluetoothSDK.BluetoothSdk.RestartScan();
            if (!bluetoothListPanel.activeSelf && !qrCodePanel.activeSelf)
            {
                Debug.Log("连接失败 显示 选择面板");
                ShowSelectConnectModePanel();
            }            
        }

        /// <summary>
        /// 连接断开
        /// </summary>
        private void OnDisconnected()
        {
            //Debug.Log("OnDisconnected requestAddress " + requestAddress + " connectedAddress " + connectedAddress);

            //非切换导致的断开 显示断开的提示并刷新列表 切换连接导致的断开 不显示断开的提示且不刷新列表
            if (requestAddress.Equals(connectedAddress))
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowToastWindow(curConnectedName +" "+ I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothDisconnected, tipAutoHideDelayTime);
                }
                //清空当前扫描到的蓝牙列表信息 重新扫描
                DeleteConnectedAddress(connectedAddress);
                //在这里刷新蓝牙列表界面
                RefreshBluetoothItems();                           
                Connect(preBluetoothAddress);
                BluetoothSDK.BluetoothSdk.RestartScan();
            }
            RefreshSelectConnectModePanelTip();
            changeBluetoothBtnSprite(false);
            //如果蓝牙界面可见 则将该蓝牙置为未连接状态
            RefreshConnectedState(connectedAddress, false);
            connectedAddress = "";
        }

        /// <summary>
        /// unity主动断开
        /// </summary>
        private void OnUnityDisconnected()
        {
            //由于Android是Android端执行的重连 所以这里不清空列表 避免home返回后重连无法显示的问题
            RefreshSelectConnectModePanelTip();
            changeBluetoothBtnSprite(false);
            //如果蓝牙界面可见 则将该蓝牙置为未连接状态
            RefreshConnectedState(connectedAddress, false);
            connectedAddress = "";
        }

        public void OnReconnecting()
        {
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothReconnecting, -1);
            }
        }

        /// <summary>
        /// 连接超时
        /// </summary>
        public void OnConnectTimeout()
        {
            if (isQRCodeConnect)
            {
                OnQRCodeConnectResult(false);//二维码扫描超时
            }
            RefreshSelectConnectModePanelTip();
            //提示连接超时
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.CloseWaitingWindow();
                TipWindow.Instance.ShowToastWindow(requestName +" "+ I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectTimeout, tipAutoHideDelayTime);
            }

            DeleteConnectedAddress(connectedAddress);
            //在这里刷新蓝牙列表界面
            RefreshBluetoothItems();
            BluetoothSDK.BluetoothSdk.RestartScan();
            if (!bluetoothListPanel.activeSelf && !qrCodePanel.activeSelf)
            {
                ShowSelectConnectModePanel();
            }          
        }

        /// <summary>
        /// 用户授权允许开启蓝牙
        /// </summary>
        public void OnGrantedSuccessed()
        {
            //Debug.Log("android用户授权允许开启蓝牙/ios用户开启了蓝牙");
            ShowSelectConnectModePanel();
        }

        private void OnNeedOpenBluetoothEvent()
        {
            //提示连接超时
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.OpenBluetoothInSettingPlease, tipAutoHideDelayTime);
            }
        }

        #endregion 蓝牙回调

        #region 网络请求回调
        public void OnNoneNetwork()
        {
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Network.NoneNet, tipAutoHideDelayTime);
            }
            if(isQRCodeConnect)
            {
                OnRequestCodeResult(QRCodeConnectState.QRCodeNoneNetwork);
            }
        }

        public void GunNameRuleCmd_OnReceiveMessage(string url, string data)
        {
     //       Debug.Log("GunNameRuleCmd_OnReceiveMessage " + " data " + data);
            var json = JsonMapper.ToObject(data);
            int sts = 0;
            string rmk = "";
            if (null != json)
            {
                if (json.Keys.Contains("sts"))
                {
                    sts = (int)json["sts"];
                }
                if (json.Keys.Contains("rmk"))
                {
                    rmk = (string)json["rmk"];
                }
                //Debug.Log("rmk " + rmk);
                if (sts == 0)
                {
                    if (json.Keys.Contains("biz"))
                    {
                        var biz = json["biz"];
                        ruleList.Clear();
                        for (int i = 0; i < biz.Count; i++)
                        {
                            //Debug.Log("GunNameRuleCmd_OnReceiveMessage " + biz[i].ToJson());
                            var ruleInfo = JsonMapper.ToObject<GunNameRule>(biz[i].ToJson());
                            ruleList.Add(ruleInfo);
                        }
                        //isGetGunNameRuleSuccessed = true;
                    }
                }
                else
                {
                    //请求数据非法或者错误

                }
            }
        }

        /// <summary>
        /// 收到二维码请求mac地址消息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        public void BluetoothCodeCmd_OnReceiveMessage(string url, string data)
        {
           // Debug.Log("BluetoothCodeCmd_OnReceiveMessage " + " data " + data);
            var json = JsonMapper.ToObject(data);
            int sts = 0;
            string rmk = "";
            if (null != json)
            {
                if (json.Keys.Contains("sts"))
                {
                    sts = (int)json["sts"];
                }
                if (json.Keys.Contains("rmk"))
                {
                    rmk = (string)json["rmk"];
                }
                if (sts == 0)
                {
                    qrcoderesult = I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeSuccessed;                   
                }
                if (sts == 1)
                {
                    qrcoderesult = I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeError;                   
                }
                if (sts == 2)
                {
                    qrcoderesult = I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeInvalid;
                  
                }
                if (sts == 0)
                {
                    if (json.Keys.Contains("biz"))
                    {
                        var biz = json["biz"][0];
                        curQRCodeInfo = JsonMapper.ToObject<QRCodeInfo>(biz.ToJson());
#if UNITY_IOS
                        curQRCodeInfo.mac = curQRCodeInfo.name;//将ios的mac改为name的值
#endif
                        //isGetQRCodeSuccessed = true;
                    }
                }
                else
                {
                    //请求数据非法或者错误
                    curQRCodeInfo = null;
                }
                isGetQRCodeSuccessed = true;
            }
        }

        #endregion 网络请求回调

        #region 按钮事件
        /// <summary>
        /// 从其他界面打开蓝牙选择界面
        /// </summary>
        public void OpenBluetoothSelectPanel()
        {
            //Debug.Log("从其他界面打开蓝牙选择界面");
            var isAvailabled = BluetoothSDK.BluetoothSdk.IsBluetoothReady();//每次都要从底层获取是否已开启（避免用户中途关闭了蓝牙）
            if (isAvailabled)
            {
                //Debug.Log("蓝牙已开启 打开选择界面");
                ShowSelectConnectModePanel();
            }
        }
        private void OnStartOpenBluetooth()
        {
            var isAvailabled = BluetoothSDK.BluetoothSdk.IsBluetoothReady();//每次都要从底层获取是否已开启（避免用户中途关闭了蓝牙）  
                   
            if (isAvailabled)
            {
                ShowSelectConnectModePanel();              
            }
        }
        /// <summary>
        /// 展示蓝牙连接方式选择界面
        /// </summary>
        public void ShowSelectConnectModePanel()
        {
            if (null != selectConnectModePanel)
            {               
                selectConnectModePanel.SetActive(true);
                RefreshSelectConnectModePanelTip();
                if(isFirstShow)
                {
                    string address = PlayerPrefs.GetString("preBluetoothAddress", "");
                    if (!address.Equals(""))
                    {
						#if UNITY_IOS
							reconnectAddress = address;
							iosAutoConnect = true;
							BluetoothSDK.BluetoothSdk.StartScan();											
						#elif  UNITY_ANDROID
                        Connect(address);
						#endif
                    }
                    isFirstShow = false;
                }                     
            }
        }      

        /// <summary>
        /// 点击蓝牙连接方式选择界面关闭按钮
        /// </summary>
        public void OnClickSelectConnectPanelCloseBtn()
        {
            if (null != selectConnectModePanel)
            {
                selectConnectModePanel.SetActive(false);             
            }
        }

        /// <summary>
        /// 点击扫码连接按钮
        /// </summary>
        public void OnClickQRCodeConnectBtn()
        {
            
            //关闭连接方式选择界面
            if (null != selectConnectModePanel)
            {
                selectConnectModePanel.SetActive(false);
            }
            //开始扫描蓝牙设备
            BluetoothSDK.BluetoothSdk.StartScan();

            //显示扫描二维码界面
            if (null != qrCodePanel)
            {
                isQRCodeConnect = true;
                qrCodePanel.SetActive(true);
                qrCodePanel.GetComponent<QRScaner>().StartCamera(CameraType.Webcam);
                if(OnQRCodeStartCamera!=null)
                {
                    OnQRCodeStartCamera();
                }              
               
                RefreshSelectConnectModePanelTip();
            }

                    
        }

        /// <summary>
        /// 点击扫描二维码界面的关闭按钮
        /// </summary>
        public void OnClickQRCodePanelCloseBtn()
        {          
            if (null != qrCodePanel)
            {
                isQRCodeConnect = false;
                qrCodePanel.SetActive(false);
            }
            qrCodePanel.GetComponent<QRScaner>().StopCamera(CameraType.Webcam);
            if(OnQRCodeStopCamera!=null)
            {
                OnQRCodeStopCamera();
            }
           
            //关闭时的处理
            BluetoothSDK.BluetoothSdk.StopScan();
            //清除除了已连接的蓝牙设备的信息
            DeleteOtherAddress(connectedAddress);
        }
        private void DeleteConnectedAddress(string address)
        {
            for (int i = 0; i < curBluetoothList.Count; i++)
            {
                if (curBluetoothList[i].address.Equals(address))
                {
                    curBluetoothList.Remove(curBluetoothList[i]);
                }
            }
        }
        private void DeleteOtherAddress(string address)
        {
            for (int i = 0; i < curBluetoothList.Count; i++)
            {
                if (!curBluetoothList[i].address.Equals(address))
                {
                    curBluetoothList.Remove(curBluetoothList[i]);
                }
            }
        }
        /// <summary>
        /// 点击蓝牙列表界面关闭按钮
        /// </summary>
        public void OnClickBluetoothListPanelCancelBtn()
        {
            //隐藏蓝牙列表界面
            if (null != bluetoothListPanel)
            {
                bluetoothListPanel.SetActive(false);
            }
            //关闭时的处理
            //清除状态
            RefreshBluetoothItems();
            BluetoothSDK.BluetoothSdk.StopScan();
            //Debug.Log("停止扫描 并且 清除除了已连接的蓝牙设备的信息 connectedAddress " + connectedAddress);
            //清除除了已连接的蓝牙设备的信息
            DeleteOtherAddress(connectedAddress);
        }

        /// <summary>
        /// 点击蓝牙列表界面确定按钮
        /// </summary>
        public void OnClickBluetoothListPanelOKBtn()
        {
            //如果当前有选择的蓝牙设备且该设备当前未连接就连接该设备 若没有就不处理
            if (!selectedBluetoothAddress.Equals("") && !selectedBluetoothAddress.Equals(connectedAddress))
            {
                //Debug.Log("点击连接新的蓝牙");
                //如果当前有已连接状态的蓝牙 将已连接的状态置为未连接状态
                RefreshConnectedState(connectedAddress, false);
                Connect(selectedBluetoothAddress);
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowWaitingWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectting, false);
                }
            }
            else
            {
                if (selectedBluetoothAddress.Equals(""))
                {
                    if (null != TipWindow.Instance)
                    {
                        TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.NoneSelectTip, tipAutoHideDelayTime);
                    }
                }
            }
        }
        /// <summary>
        /// 点击列表选择连接按钮
        /// </summary>
        public void OnClickShowListBtn()
        {
            //Debug.Log("点击列表选择连接按钮");
            //关闭连接方式选择界面
            if (null != selectConnectModePanel)
            {
                isQRCodeConnect = false;
                selectConnectModePanel.SetActive(false);
            }
            //Debug.Log("开始扫描蓝牙设备");
            //开始扫描蓝牙设备
            BluetoothSDK.BluetoothSdk.StartScan();

            //显示蓝牙列表界面
            if (null != bluetoothListPanel)
            {
                bluetoothListPanel.SetActive(true);
                //测试打印所有扫描到的列表信息
                //Debug.Log("测试打印所有扫描到的列表信息");
                //foreach (var info in curBluetoothList)
                //{
                //    Debug.Log("OnClickShowListBtn info.address " + info.address);
                //}
                //打开的时候刷新一次列表内容 后续刷新是在收到扫描到新的蓝牙信息的时候触发
                RefreshBluetoothListPanel();
            }
        }


        #endregion 按钮事件

        #region 二维码扫描结果回调
        /// <summary>
        /// 扫描二维码得到code的回调
        /// </summary>
        /// <param name="code"></param>
        public void OnGetQRCodeResult(string code)
        {
            //Debug.Log("OnGetQRCodeResult code " + code);
            //           OnClickQRCodePanelCloseBtn();
            qrCodePanel.GetComponent<QRScaner>().StopScan();
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.ShowWaitingWindow(I2.Loc.ScriptLocalization.TipWindow.ProcessText, false);
            }
            //记录当前扫描到的二维码code
            curRequestCode = code;
            /*从本地记录里找是否有记录过这个二维码对应的蓝牙枪信息
             * 如果有就将该蓝牙地址保存为上次已连接的地址，
             * 这样当接收到蓝牙信息的时候如果发现有该蓝牙就会自动连接
             * 没有就去服务器请求
             */
            var localCode = GetLocalCode(code);
            //Debug.Log("null == localCode " + (null == localCode));
            if (null != localCode)
            {
                savePreBluetoothDeviceInfo(localCode.mac);               
                Connect(localCode.mac);
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowWaitingWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectting, false);
                }
                
            }
            else
            {
                //Debug.Log("OnGetQRCodeResult SendBluetoothCodeCmd code " + code);
                //向服务器发送code获取mac地址
                CmdManager.Instance.SendBluetoothCodeCmd(code);
            }
        }
        // 二维码扫描请求结果处理
        private void OnRequestCodeResult(QRCodeConnectState state)
        {
            if(state == QRCodeConnectState.QRCodeNoneNetwork)
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.CloseWaitingWindow();
                    TipWindow.Instance.ShowAlertWindow(I2.Loc.ScriptLocalization.Network.NoneNet,
                        I2.Loc.ScriptLocalization.TipWindow.ConfirmText,
                        () => { qrCodePanel.GetComponent<QRScaner>().StartScan(); });
                }
            }
            else if(state == QRCodeConnectState.QRCodeInvalid)
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.CloseWaitingWindow();
                    TipWindow.Instance.ShowAlertWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeInvalid,
                        I2.Loc.ScriptLocalization.TipWindow.ConfirmText,
                        () => { qrCodePanel.GetComponent<QRScaner>().StartScan(); });
                }
            }
            else if (state == QRCodeConnectState.QRCodeError)
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.CloseWaitingWindow();
                    TipWindow.Instance.ShowAlertWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.QRCodeError,
                        I2.Loc.ScriptLocalization.TipWindow.ConfirmText,
                        () => { qrCodePanel.GetComponent<QRScaner>().StartScan(); });
                }
            }
            else if (state == QRCodeConnectState.QRCodeSuccessed)
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowWaitingWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectting, false);
                }
            }
        }
        private void OnQRCodeConnectResult(bool success)
        {
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.CloseWaitingWindow();
            }
            if (success)
            {
                OnClickQRCodePanelCloseBtn();
            }
            else
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowAlertWindow(I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectedFailed,
                        I2.Loc.ScriptLocalization.TipWindow.TryAgainText,
                        ()=> { BluetoothSDK.BluetoothSdk.RestartScan();Invoke("StartScanQRCode",1.0f); });
                }
            }
        }
        private void StartScanQRCode()
        {
            qrCodePanel.GetComponent<QRScaner>().StartScan();
        }
        #endregion 二维码扫描结果回调

        #region 蓝牙界面显示相关方法
        /// <summary>
        /// 蓝牙是否已连接
        /// 当需要进入游戏场景的时候需要调用此方法判断蓝牙是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsBluetoothConnect()
        {
            if (BluetoothSDK.BluetoothSdk.IsConnected)
            {
                //提示连接成功
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowToastWindow(curConnectedName +" "+ I2.Loc.ScriptLocalization.Bluetooth_Tips.ConnectedString, tipAutoHideDelayTime);
                }
                return true;
            }
            else
            {
                var isAvailabled = BluetoothSDK.BluetoothSdk.IsBluetoothReady();//每次都要从底层获取是否已开启（避免用户中途关闭了蓝牙）
                if (isAvailabled)
                {
                    //自动重连 当前未连接 曾经连成功过 上次连接成功的地址为空或者等于上次连接成功的地址
                    if (!BluetoothSDK.BluetoothSdk.IsConnected
                        && !preBluetoothAddress.Equals("")
                        && (requestAddress.Equals("") || requestAddress.Equals(preBluetoothAddress)))
                    {
                        Connect(preBluetoothAddress);
                    }
                    else
                    {                 
                        ShowSelectConnectModePanel();
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 刷新选择界面的提示
        /// </summary>
        private void RefreshSelectConnectModePanelTip()
        {
            //Debug.Log("刷新选择界面的提示");       
            if (null != selectConnectModePanelTipText)
            {
                if (!BluetoothSDK.BluetoothSdk.IsConnected)
                {
                    //Debug.Log("选择界面的提示 未连接 ");
                    selectConnectModePanelTipText.text = I2.Loc.ScriptLocalization.Bluetooth_Tips.SelectConnectModePanelTipNoneDevice;
                    QRCodePanelTipText.text = I2.Loc.ScriptLocalization.Bluetooth.QRCodePanelTipText;

                }
                else
                {
                    //Debug.Log("选择界面的提示 已连接 " + curConnectedName);
                    //Debug.Log("RefreshSelectConnectModePanelTip curConnectedName " + curConnectedName);
                    selectConnectModePanelTipText.text = curConnectedName + " " +  I2.Loc.ScriptLocalization.Bluetooth_Tips.ConnectedString;
                    QRCodePanelTipText.text = "<color=#FE4931>" + curConnectedName + " "+ I2.Loc.ScriptLocalization.Bluetooth_Tips.ConnectedString + "</color>";
                }
            }
        }
  
        /// <summary>
        /// 当选择新的蓝牙设备时调用此方法
        /// </summary>
        /// <param name="address"></param>
        public void ChangeSelectedAddress(string address)
        {
            if (address.Equals(""))
            {
                return;
            }
            if (selectedBluetoothAddress.Equals(""))
            {
                //当前还没有选择过
                preSelectedBluetoothAddress = address;
                selectedBluetoothAddress = address;
                //这里省去对preSelectedBluetoothAddress的刷新，因为紧接着就要将其置为选择状态
                RefreshSelectedState(selectedBluetoothAddress, true);
            }
            else
            {
                if (!selectedBluetoothAddress.Equals(address))
                {
                    //与当前选择的不是同一个的时候才处理
                    preSelectedBluetoothAddress = selectedBluetoothAddress;
                    selectedBluetoothAddress = address;
                    RefreshSelectedState(preSelectedBluetoothAddress, false);
                    RefreshSelectedState(selectedBluetoothAddress, true);
                }
            }
        }

        /// <summary>
        /// 刷新连接成功失败状态
        /// </summary>
        /// <param name="address"></param>
        /// <param name="isConnected"></param>
        public void RefreshConnectedState(string address, bool isConnected)
        {
            if (address.Equals(""))
            {
                return;
            }
            //Debug.LogError("RefreshConnectedState address " + address + " isConnected " + isConnected);
            foreach (var item in curBluetoothGameObjectList)
            {
                if (null != item)
                {
                    var info = item.GetComponent<BluetoothItem>();
                    if (info.mInfo.address.Equals(address))
                    {
                        info.RefreshConnectedState(isConnected);
                        break;
                    }
                }                
            }
        }

        /// <summary>
        /// 刷新选中或者未选中状态
        /// </summary>
        /// <param name="address"></param>
        /// <param name="isSelected"></param>
        public void RefreshSelectedState(string address, bool isSelected)
        {
            if (address.Equals(""))
            {
                return;
            }
            foreach (var item in curBluetoothGameObjectList)
            {
                if (null != item)
                {
                    var info = item.GetComponent<BluetoothItem>();
                    if (info.mInfo.address.Equals(address))
                    {
                        var isonnected = false;
                        if (address.Equals(connectedAddress))
                        {
                            isonnected = true;
                        }                      
                        info.RefreshSelectedState(isSelected, isonnected);
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// 修改蓝牙按钮的状态
        /// </summary>
        /// <param name="enabled"></param>
        private void changeBluetoothBtnSprite(bool enabled)
        {
            if (enabled)
            {
                if (OnBluetoothConnectSuccess != null)
                {
                    OnBluetoothConnectSuccess.Invoke();
                }

            }
            else
            {
                if (OnBluetoothConnectFail != null)
                {
                    OnBluetoothConnectFail.Invoke();
                }
            }
        }


        /// <summary>
        /// 隐藏所有Items
        /// </summary>
        public void RefreshBluetoothItems()
        {
            //连接失败或者断开连接的时候需要重新扫描的时候隐藏所有items
            foreach (var obj in curBluetoothGameObjectList)
            {
                //将大小恢复为原始大小
                obj.GetComponent<RectTransform>().localScale = Vector3.one;
                //图片都重置为未选中状态
                obj.GetComponent<BluetoothItem>().RefreshSelectedState(false, false);
            }
            //清空之前的选择状态
            preSelectedBluetoothAddress = "";
            selectedBluetoothAddress = "";
            //刷新提示信息
            RefreshNoneDeviceTip();
        }

        /// <summary>
        /// 刷新NoneDeviceTip
        /// </summary>
        private void RefreshNoneDeviceTip()
        {
            //Debug.Log("RefreshNoneDeviceTip curBluetoothList.Count " + curBluetoothList.Count);
            var enabled = curBluetoothList.Count != 0 ? true : false;
            if (null != NoneDeviceTipText)
            {
                NoneDeviceTipText.text = enabled ? "" : I2.Loc.ScriptLocalization.Bluetooth_Tips.NoneDeviceTip;
            }           
        }

        /// <summary>
        /// 刷新蓝牙列表界面
        /// </summary>
        public void RefreshBluetoothListPanel()

        {
            //如果扫描到的列表比当前列表的数量大 则表示有新增 否则则可直接刷新原有的列表内容即可
            if (curBluetoothList.Count > curBluetoothGameObjectList.Count)
            {
                for (int i = 0; i < curBluetoothList.Count - curBluetoothGameObjectList.Count; i++)
                {
                    GameObject obj = GameObject.Instantiate(bluetoothItemObj, Vector3.zero, Quaternion.identity) as GameObject;
                    obj.GetComponent<RectTransform>().SetParent(content);
                    obj.GetComponent<RectTransform>().localScale = Vector3.one;
                    curBluetoothGameObjectList.Add(obj);
                }
            }
            foreach (var obj in curBluetoothGameObjectList)
            {
                obj.SetActive(false);
                ////将大小恢复为原始大小
                //obj.GetComponent<RectTransform>().localScale = Vector3.one;
                ////图片都重置为未选中状态
                //obj.GetComponent<BluetoothItem>().RefreshSelectedState(false);
            }
            //如果没有扫描到蓝牙信息则显示提示信息
            RefreshNoneDeviceTip();
            //Debug.LogError("RefreshBluetoothListPanel curBluetoothList.Count " + curBluetoothList.Count);
            //Debug.LogError("RefreshBluetoothListPanel curBluetoothGameObjectList.Count " + curBluetoothGameObjectList.Count);
            for (int i = 0; i < curBluetoothList.Count; i++)
            {
                var item = curBluetoothGameObjectList[i].GetComponent<BluetoothItem>();
                if (null != item)
                {
                    item.Init(curBluetoothList[i]);
   //                 Debug.Log("显示 item.mInfo.address " + item.mInfo.address);
                    curBluetoothGameObjectList[i].SetActive(true);                  
                }
            }
        }
        #endregion 蓝牙界面显示相关方法

        #region 蓝牙相关方法
        /// <summary>
        /// 传入一个后六位为XX:XX:XX的十六进制字符串 返回对应的十进制值
        /// needReverse控制是否需要反转
        /// </summary>
        /// <param name="name"></param>
        /// <param name="needReverse"></param>
        /// <returns></returns>
        private long ConvertToLong(string name,bool needReverse)
        {
            //例如传入SHOWBABY_6d:00:00 需要反转 返回109
            //例如传入80:EA:CA:00:00:6D 不需要反转 返回109
            //截取后八位
            var endString = name.Remove(0, name.Length - 8);
            var usefulString = "";
            if (needReverse)
            {
                var splitArrary = endString.Split(':');
                for (int i = splitArrary.Length - 1; i >= 0; i--)
                {
                    usefulString += splitArrary[i];
                    if (i != 0)
                    {
                        usefulString += ":";
                    }
                }
            }
            else
            {
                usefulString = endString;
            }
            string splitname = usefulString.Replace(":", "");
            var result = Convert.ToInt64(splitname, 16);
            return result;
        }

        /// <summary>
        /// 获取当前给定地址对应的蓝牙的名字
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public BluetoothSDKDevInfo GetBluetoothDeviceInfo(string address)
        {
            //Debug.Log("GetBluetoothDeviceName address " + address);
            foreach (var info in curBluetoothList)
            {
                //Debug.Log("GetBluetoothDeviceName info.address " + info.address);
                if (info.address.Equals(address))
                {
                    //Debug.Log("GetBluetoothDeviceName info.name " + info.alias);
                    return info;
                }
            }
            //Debug.Log("GetBluetoothDeviceName notfound address " + address);
            //列表里没有就去本地记录找
            var localBluetooth = GetLocalBluetoothInfo(address);
            if (null != localBluetooth)
            {
                return localBluetooth;
            }
            return null;
        }
        public bool SetBluetoothDeviceName(string address,string alias)
        {
            var localBluetooth = GetLocalBluetoothInfo(address);

            foreach (var info in curBluetoothList)
            {
                if (info.address.Equals(address))
                {
                    info.alias = alias;                 
                    break;
                }
            }

            if (null != localBluetooth)
            {
                localBluetooth.alias = alias;
                WriteBluetoothInfoToLocal(localBluetooth);
                if (BluetoothSDK.BluetoothSdk.IsConnected)
                {
                    if(connectedAddress.Equals(address))
                    {
                        curConnectedName = alias;
                    }
                }
                return true;
            }else
            {
                return false;
            }
        }
        /// <summary>
        /// 查询蓝牙名称对应的名字前缀
        /// </summary>
        /// <param name="mac"></param>
        /// <returns></returns>
        private string GetPreName(string name)
        {
            if (null != ruleList && ruleList.Count > 0)
            {
                var mac = ConvertToLong(name, true);
                foreach (var rule in ruleList)
                {
                    if (null != rule)
                    {
                        var min = ConvertToLong(rule.minValue, false);
                        var max = ConvertToLong(rule.maxValue, false);
                        if (mac >= min && mac <= max)
                        {
                            return rule.name + " ";//这里加了空格
                        }
                    }
                }
            }
            //如果没有找到规则就返回默认值
            return I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothDefaultPreName+" ";
        }

        /// <summary>
        /// 获取蓝牙名称后六位(除去两个冒号)对应的大写值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetUpperName(string name)
        {
            //例如传入SHOWBABY_6d:00:00 返回6D0000
            return name.Remove(0, name.Length - 8).Replace(":", "").ToUpper();
        }

        /// <summary>
        /// 根据名称获取别名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetAliasByName(string name)
        {
            return GetPreName(name) + GetUpperName(name);
        }    

        /// <summary>
        /// 连接指定mac地址的蓝牙设备
        /// </summary>
        /// <param name="address"></param>
        public void Connect(string address)
        {
            if (autoconnect)
            {
                //只要开始连接新的蓝牙了 就将自动重连禁掉 避免以后扫到之前的蓝牙的时候又自动重连了
                autoconnect = false;
            }
            //Debug.Log("Connect address " + address);            
            requestAddress = address;//记录当前请求的地址
            var device = GetBluetoothDeviceInfo(requestAddress);
            if (null != device)
            {
                requestName = device.alias;
            }
            if (null != TipWindow.Instance)
            {
                TipWindow.Instance.ShowToastWindow(requestName+" " + I2.Loc.ScriptLocalization.Bluetooth_Tips.BluetoothConnectting,-1);
            }
            //将当前连接的item置为选中状态
            ChangeSelectedAddress(requestAddress);
            BluetoothSDK.BluetoothSdk.Connect(address);
        }

        /// <summary>
        /// 存储当前连接的蓝牙枪的信息(供下次进入时访问以便自动重连)
        /// </summary>
        /// <param name="info"></param>
        private void savePreBluetoothDeviceInfo(string address)
        {
            preBluetoothAddress = address;
            PlayerPrefs.SetString("preBluetoothAddress", address);
        }
      
       
        /// <summary>
        /// 查找本地是否存储过该二维码对应的蓝牙枪信息 如果有返回其对应的mac地址 如果没有返回空
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public QRCodeInfo GetLocalCode(string code)
        {
            string localQRCodeList = PlayerPrefs.GetString("localQRCodeList");
            if (localQRCodeList.Equals(""))
            {
                //没记录就直接返回空 去服务器请求
                return null;
            }
            var json = JsonMapper.ToObject(localQRCodeList);
            for (int i = 0; i < json.Count; i++)
            {
                var qrcodeinfo = JsonMapper.ToObject<QRCodeInfo>(json[i].ToJson());
                if (qrcodeinfo.code.Equals(code))
                {
                    //返回记录中的二维码信息
                    return qrcodeinfo;
                }
            }
            //在记录中没找到这个二维码信息就说明这是一张新的二维码，返回空 去服务器请求
            return null;
        }

        /// <summary>
        /// 成功获取二维码对应的mac地址的处理
        /// </summary>
        /// <param name="qrcodeinfo"></param>
        private void OnGetAddressSuccessed(QRCodeInfo qrcodeinfo)
        {
            if (null == curQRCodeInfo)
            {
                //扫描结果失败的时候直接返回
                return;
            }
            //Debug.Log("OnGetAddressSuccessed qrcodeinfo " + qrcodeinfo);
            //将二维码信息存到本地 然后发起蓝牙连接请求
            string json = JsonMapper.ToJson(qrcodeinfo);

            JsonData jsondata = null;
            string localQRCodeList = PlayerPrefs.GetString("localQRCodeList","");
            if (localQRCodeList.Equals(""))
            {
                jsondata = new JsonData();
                jsondata.Add(JsonMapper.ToObject(json));
                PlayerPrefs.SetString("localQRCodeList", jsondata.ToJson());
            }
            else
            {
                var isExit = false;
                jsondata = JsonMapper.ToObject(localQRCodeList);
                for (int i = 0; i < jsondata.Count; i++)
                {
                    var qrinfo = JsonMapper.ToObject<QRCodeInfo>(jsondata[i].ToJson());
                    if (qrinfo.code.Equals(qrcodeinfo.code))
                    {
                        //说明已经存在
                        isExit = true;
                        break;
                    }
                }
                if (!isExit)
                {
                    jsondata.Add(JsonMapper.ToObject(json));
                    PlayerPrefs.SetString("localQRCodeList", jsondata.ToJson());
                }
            }
            //Debug.Log("Connect qrcodeinfo.mac " + qrcodeinfo.mac);
            Connect(qrcodeinfo.mac);
        }

        /// <summary>
        /// 查找本地是否存储过该蓝牙枪信息 如果有返回该蓝牙枪信息 如果没有返回空
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public BluetoothSDKDevInfo GetLocalBluetoothInfo(string address)
        {
            string localBluetoothList = PlayerPrefs.GetString("localBluetoothList");
            if (localBluetoothList.Equals(""))
            {
                //没记录就直接返回空
                return null;
            }
            var json = JsonMapper.ToObject(localBluetoothList);
            for (int i = 0; i < json.Count; i++)
            {
                var binfo = JsonMapper.ToObject<BluetoothSDKDevInfo>(json[i].ToJson());
                if (binfo.address.Equals(address))
                {
                    //返回记录中的蓝牙枪信息
                    return binfo;
                }
            }
            //在记录中没找到就说明这是新的，返回空
            return null;
        }

        /// <summary>
        /// 将蓝牙信息存到本地
        /// </summary>
        /// <param name="info"></param>
        private void WriteBluetoothInfoToLocal(BluetoothSDKDevInfo info)
        {
            string json = JsonMapper.ToJson(info);

            JsonData jsondata = null;
            string localBluetoothList = PlayerPrefs.GetString("localBluetoothList");
            if (localBluetoothList.Equals(""))
            {
                jsondata = new JsonData();
                jsondata.Add(JsonMapper.ToObject(json));
                PlayerPrefs.SetString("localBluetoothList", jsondata.ToJson());
            }
            else
            {
                var isExit = false;
                jsondata = JsonMapper.ToObject(localBluetoothList);
                for (int i = 0; i < jsondata.Count; i++)
                {
                    var binfo = JsonMapper.ToObject<BluetoothSDKDevInfo>(jsondata[i].ToJson());
                    if (binfo.address.Equals(info.address))
                    {
                        //说明已经存在
                        isExit = true;
                        jsondata[i] = JsonMapper.ToObject(json);
                        PlayerPrefs.SetString("localBluetoothList", jsondata.ToJson());
                        return;
                    }
                }
                if (!isExit)
                {
                    jsondata.Add(JsonMapper.ToObject(json));
                    PlayerPrefs.SetString("localBluetoothList", jsondata.ToJson());
                }
            }
        }
        #endregion 蓝牙相关方法
    }
}