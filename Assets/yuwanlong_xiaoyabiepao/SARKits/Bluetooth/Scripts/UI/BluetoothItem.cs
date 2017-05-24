using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Showbaby.Bluetooth
{
    /// <summary>
    /// 蓝牙Item
    /// </summary>
    public class BluetoothItem : MonoBehaviour
    {
        #region 面板设置
        /// <summary>
        /// 背景图片
        /// </summary>
        public Image background = null;

        /// <summary>
        /// 名字
        /// </summary>
        public Text nameText = null;

        public Text connectText = null;//连接按钮文字
        /// <summary>
        /// 已选中状态的图片
        /// </summary>
        public Sprite selectedSprite = null;

        /// <summary>
        /// 未选中状态的图片
        /// </summary>
        public Sprite disselectedSprite = null;

        /// <summary>
        /// 已连接状态的图片
        /// </summary>
        public Sprite connectedSprite = null;
        
        /// <summary>
        /// 选中时放大的倍数
        /// </summary>
        public Vector3 maxScale = new Vector3(1.05f,1.05f,1.05f);
        #endregion

        /// <summary>
        /// 绑定的蓝牙信息
        /// </summary>
        public BluetoothSDKDevInfo mInfo = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 点击选中按钮
        /// </summary>
        public void OnClickSelectBtn()
        {            
            if (null != BluetoothWindow.Instance)
            {
                BluetoothWindow.Instance.ChangeSelectedAddress(mInfo.address);
            }
        }

        /// <summary>
        /// 刷新连接成功或者连接断开的状态
        /// </summary>
        /// <param name="isConnected"></param>
        public void RefreshConnectedState(bool isConnected)
        {
            //Debug.LogError("RefreshConnectedState isConnected " + isConnected);
            if (isConnected)
            {
                //置为已连接状态
                background.sprite = connectedSprite;
                connectText.text = I2.Loc.ScriptLocalization.Bluetooth_Tips.ConnectedString;
            }
            else
            {
                //置为未连接状态
                background.sprite = disselectedSprite;
                connectText.text = I2.Loc.ScriptLocalization.Bluetooth_Tips.ConnectString;
            }
        }

        /// <summary>
        /// 刷新当前item的选中状态
        /// </summary>
        /// <param name="isSelected">是否是选中状态</param>
        /// <param name="isConnected">是否是已连接状态</param>
        public void RefreshSelectedState(bool isSelected, bool isConnected)
        {           
            if (!isConnected)
            {
                gameObject.GetComponent<RectTransform>().localScale = isSelected ? maxScale : Vector3.one;
                background.sprite = isSelected ? selectedSprite : disselectedSprite;
            }
        }

        /// <summary>
        /// 初始化
        /// info 蓝牙信息（mac地址和名字）
        /// </summary>
        /// <param name="info"></param>
        public void Init(BluetoothSDKDevInfo info)
        {
            if (null == info)
            {
                return;
            }
            mInfo = info;
            nameText.text = mInfo.alias;
            var isconnected = mInfo.address == BluetoothWindow.Instance.connectedAddress ? true : false;
            RefreshConnectedState(isconnected);
        }
    }
}