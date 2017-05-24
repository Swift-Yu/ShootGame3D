using LitJson;
using Showbaby.Bluetooth;
using System;
using UnityEngine;
using Showbaby.UI;

namespace Showbaby.Command
{
    /// <summary>
    /// 指令管理类
    /// 所有的指令在此进行设置(方便统一配置以及便于正式与测试的切换调试)
    /// 如果没有将预制件挂到场景中，实例化的时候会自动挂载
    /// </summary>
    public class CmdManager : MonoBehaviour
    {
        #region 界面设置
        /// <summary>
        /// 是否是测试服务器
        ///!!!!!!!正式打包的时候记得这里不要置为true!!!!!!!
        /// </summary>
        public bool IsDebug = false;

        /// <summary>
        /// 测试服务器地址
        /// </summary>
        public string testPreUrl = "http://arshow.showbabybox.com/ARShowUnity/";

        /// <summary>
        /// 正式服务器地址
        /// </summary>
        public string formalPreUrl = "https://www.showbabybox.com/BTARShowUnity/";
        #endregion

        #region 私有变量
        /// <summary>
        /// 服务器地址（便于测试与正式的转换）
        /// </summary>
        private string PreUrl
        {
            get
            {
                if (IsDebug)
                {
                    return testPreUrl;
                }
                else
                {
                    return formalPreUrl;
                }
            }
        }

        /// <summary>
        /// 获取二维码对应的mac地址的接口
        /// </summary>
        public const string bluetoothCodeUrl = "bluetoothCode_getCode.do";

        /// <summary>
        /// 随机昵称的接口
        /// </summary>
        public const string randomNickUrl = "randomNickUrl";

        /// <summary>
        /// 创建昵称的接口
        /// </summary>
        public const string createNickUrl = "createNickUrl";

        /// <summary>
        /// 从服务器获取枪的名字前缀规则
        /// </summary>
        public const string gunNameRuleUrl = "resourceUnity_findUnityBluetoothRangeList.do";
        #endregion

        #region 获取类的实例
        private static CmdManager mInstance = null;
        /// <summary>
        /// 获取类的实例
        /// </summary>
        /// <returns></returns>
        public static CmdManager Instance
        {
            get
            {
                if (null == mInstance)
                    mInstance = new GameObject("CmdManager").AddComponent<CmdManager>();
                return mInstance;
            }
        }
        #endregion

        void Awake()
        {
            mInstance = this;
            Cmd.OnReceiveMessage += Cmd_OnReceiveMessage;
            Cmd.OnNoneNetwork += OnNoneNetwork;
            //            GameObject.DontDestroyOnLoad(gameObject);
        }
        void OnDestroy()
        {
            Cmd.OnNoneNetwork -= OnNoneNetwork;
        }
        // Use this for initialization
        void Start()
        {
            
        }
      
        /// <summary>
        /// 发送扫描到的二维码到服务器请求对应的蓝牙mac地址
        /// </summary>
        /// <param name="code"></param>
        public void SendBluetoothCodeCmd(string code)
        {            
            string biz = "[{\"code\":\"" + code + "\"}]";
            string pdata = Cmd.Instance.GetPdataByBiz(biz);
            var requestUrl = PreUrl + bluetoothCodeUrl + "?pdata=" + pdata;
            Debug.Log("SendBluetoothCodeCmd code " + code);        
         
            Cmd.Instance.HttpPost(requestUrl);
        }
      
        /// <summary>
        /// 发送获取枪的名字前缀规则的指令
        /// </summary>
        public void SendGunNameRuleCmd()
        {
            string biz = "[{}]";
            string pdata = Cmd.Instance.GetPdataByBiz(biz);
            var requestUrl = PreUrl + gunNameRuleUrl + "?pdata=" + pdata;          
            Cmd.Instance.HttpPost(requestUrl);
        }

        //收到无网络提示
        private void OnNoneNetwork()
        {
            if (null != BluetoothWindow.Instance)
            {
                BluetoothWindow.Instance.OnNoneNetwork();
            }     
        }

        /// <summary>
        /// 收到二维码请求mac地址消息
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        private void Cmd_OnReceiveMessage(string url, string data)
        {
            //Debug.Log("Cmd_OnReceiveMessage url " + url + " data " + data);
            switch (url)
            {
                case bluetoothCodeUrl:
                    if (null != BluetoothWindow.Instance)
                    {
                        BluetoothWindow.Instance.BluetoothCodeCmd_OnReceiveMessage(url,data);
                    }
                  
                    break;
                case gunNameRuleUrl:
                    if (null != BluetoothWindow.Instance)
                    {
                        BluetoothWindow.Instance.GunNameRuleCmd_OnReceiveMessage(url, data);
                    }
                  
                    break;
            }
        }
    }
}