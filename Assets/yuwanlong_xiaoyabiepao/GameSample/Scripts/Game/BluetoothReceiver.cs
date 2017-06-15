using Showbaby.Music;
using UnityDebug.Log;
using UnityEngine;

namespace Showbaby.Bluetooth
{
    /// <summary>
    /// 这个类用来继承，重写相应的接收方法实现自己的处理逻辑
    /// 此脚本不要用来挂载 只需写一个类来继承此类，然后挂载子类脚本即可
    /// 若有新的需求，请反馈后统一添加
    /// </summary>
    public class BluetoothReceiver : MonoBehaviour
    {
        /// <summary>
        /// 是否打印日志
        /// </summary>
        public bool ShowDebugInfo = false;

        /// <summary>
        /// 是否使用默认音效
        /// 由于武器种类多样 所以射击时的音效留给开发者在外部去根据不同的武器进行配置，然后自己外部写播放音效的代码
        /// 如果游戏中切换道具 换武器换子弹的音效只有一种的话，这里勾上后就只需要修改MusciManager上的音效内容即可（切记如果文件名改了要在面板进行修改!!!）
        /// 如果游戏中切换道具 换武器换子弹的音效不止一种的话，就把这里的勾去掉，然后自己外部适当的地方写播放音效的代码
        /// 这里默认的换子弹和换武器共用同一音效 换道具和换备用道具共用同一音效(可自行按需修改)
        /// </summary>
        public bool useDefaultAudio = false;

        /// <summary>
        /// 换子弹的音效文件名
        /// </summary>
        public string SwapBulletAudioName = "SwapWappen";

        /// <summary>
        /// 换武器的音效文件名
        /// </summary>
        public string SwapWappenAudioName = "SwapWappen";

        /// <summary>
        /// 换道具的音效文件名
        /// </summary>
        public string SwapItemAudioName = "SwapItem";

        /// <summary>
        /// 换备用道具的文件名
        /// </summary>
        public string LongSwapItemAudioName = "SwapItem";

        private void Start()
        {
            BluetoothListener.OnShootEvent += ReceiveShootInfo;
            BluetoothListener.OnSwapBulletEvent += ReceiveSwapBulletInfo;
            BluetoothListener.OnSwapWappenEvent += ReceiveSwapWappenInfo;
            BluetoothListener.OnSwapItemEvent += ReceiveSwapItemInfo;
            BluetoothListener.OnLongSwapItemEvent += ReceiveLongSwapItemInfo;
        }

        private void OnDestroy()
        {
            BluetoothListener.OnShootEvent -= ReceiveShootInfo;
            BluetoothListener.OnSwapBulletEvent -= ReceiveSwapBulletInfo;
            BluetoothListener.OnSwapWappenEvent -= ReceiveSwapWappenInfo;
            BluetoothListener.OnSwapItemEvent -= ReceiveSwapItemInfo;
            BluetoothListener.OnLongSwapItemEvent -= ReceiveLongSwapItemInfo;
        }

        /// <summary>
        /// 收到射击消息
        /// </summary>
        public virtual void ReceiveShootInfo()
        {
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("BluetoothReceiver 收到射击消息");
            }
            //射击音效自己外部添加播放代码

        }

        /// <summary>
        /// 收到更换子弹消息
        /// </summary>
        public virtual void ReceiveSwapBulletInfo()
        {
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("ReceiveSwapBulletInfo 收到更换子弹消息");
            }
            if (useDefaultAudio)
            {
                MusciManager.Instance.PlayEffectMusic(SwapBulletAudioName);
            }
        }

        /// <summary>
        /// 收到更换武器消息
        /// </summary>
        public virtual void ReceiveSwapWappenInfo()
        {
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("ReceiveSwapWappenInfo 收到更换武器消息");
            }
            if (useDefaultAudio)
            {
                MusciManager.Instance.PlayEffectMusic(SwapWappenAudioName);
            }
        }

        /// <summary>
        /// 收到更换道具消息
        /// </summary>
        public virtual void ReceiveSwapItemInfo()
        {
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("ReceiveSwapItemInfo 收到更换道具消息");
            }
            if (useDefaultAudio)
            {
                MusciManager.Instance.PlayEffectMusic(SwapItemAudioName);
            }
        }

        /// <summary>
        /// 收到更换备用道具消息
        /// </summary>
        public virtual void ReceiveLongSwapItemInfo()
        {
            if (ShowDebugInfo)
            {
                UnityLog.InfoPurple("ReceiveLongSwapItemInfo 收到更换备用道具消息");
            }
            if (useDefaultAudio)
            {
                MusciManager.Instance.PlayEffectMusic(LongSwapItemAudioName);
            }
        }
    }
}