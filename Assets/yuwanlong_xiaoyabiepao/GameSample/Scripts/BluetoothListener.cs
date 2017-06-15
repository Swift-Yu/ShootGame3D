using UnityEngine;

namespace Showbaby.Bluetooth
{
    /// <summary>
    /// 此脚本在V1.2及以前的通用框架版本中未添加到Bluetooth预制件上
    /// 在V1.3版本中将该脚本添加到了Bluetooth预制件上
    /// 请在V1.3版本前开发的项目的开发者在更新通用框架时若之前在场景中挂载了该脚本请记得删除，保证场景中只有一份该脚本!!!
    /// V1.3版本之后开发的项目无需另外添加挂载该脚本
    /// 若有新的需求，请反馈后统一添加
    /// </summary>
    public class BluetoothListener : MonoBehaviour
    {
        public delegate void GunButtonDelegate();

        public static event GunButtonDelegate OnShootEvent;//射击事件
        public static event GunButtonDelegate OnShootUpEvent;
        public static event GunButtonDelegate OnSwapBulletEvent;//换子弹事件（短按滑膛）
        public static event GunButtonDelegate OnSwapWappenEvent;//换武器（换枪）长按滑膛2秒
        public static event GunButtonDelegate OnSwapItemEvent;// 短按功能键切换道具
        public static event GunButtonDelegate OnLongSwapItemEvent;//长按功能键2秒 切换备用道具

        public float tapLimitTime = 0.5f;
        public float longPressTime = 2.0f;
        private float smoothDownTime = -1;

        public float B4DownTapLimitTime = 0.5f;
        public float B4DownLongPressTime = 2f;
        private float B4DownStartTime = -1f;

        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {
            if (null != BluetoothSDK.BluetoothSdk)
            {
                if (null != BluetoothSDK.BluetoothSdk.OnShootDownEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnShootDownEvent.AddListener(ShootDownEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnShootUpEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnShootUpEvent.AddListener(ShootUpEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnSmoothDownEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnSmoothDownEvent.AddListener(SmoothDownEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnSmoothUpEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnSmoothUpEvent.AddListener(SmoothUpEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnActionDownEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnActionDownEvent.AddListener(ActionDownEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnActionUpEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnActionUpEvent.AddListener(ActionUpEvent);
                }
            }
        }

        void OnDestroy()
        {
            if (null != BluetoothSDK.BluetoothSdk)
            {
                if (null != BluetoothSDK.BluetoothSdk.OnShootDownEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnShootDownEvent.RemoveListener(ShootDownEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnShootUpEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnShootUpEvent.RemoveListener(ShootUpEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnSmoothDownEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnSmoothDownEvent.RemoveListener(SmoothDownEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnSmoothUpEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnSmoothUpEvent.RemoveListener(SmoothUpEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnActionDownEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnActionDownEvent.RemoveListener(ActionDownEvent);
                }
                if (null != BluetoothSDK.BluetoothSdk.OnActionUpEvent)
                {
                    BluetoothSDK.BluetoothSdk.OnActionUpEvent.RemoveListener(ActionUpEvent);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (smoothDownTime != -1f)
            {
                if (Time.time - smoothDownTime >= longPressTime)
                {
                    smoothDownTime = -1f;
                    if (null != OnSwapWappenEvent)
                    {
                        OnSwapWappenEvent.Invoke();
                    }
                }
            }

            if (B4DownStartTime != -1f)
            {
                if (Time.time - B4DownStartTime >= B4DownLongPressTime)
                {
                    B4DownStartTime = -1f;
                    if (null != OnLongSwapItemEvent)
                    {
                        OnLongSwapItemEvent.Invoke();
                    }
                }
            }
        }

        private void ShootDownEvent()
        {
            if (null != OnShootEvent)
            {
                OnShootEvent.Invoke();
            }
        }

        private void ShootUpEvent()
        {
            if (null != OnShootUpEvent)
            {
                OnShootUpEvent.Invoke();
            }
        }

        private void SmoothDownEvent()
        {
            smoothDownTime = Time.time;
        }

        private void SmoothUpEvent()
        {
            if (Time.time - smoothDownTime <= tapLimitTime)
            {
                if (null != OnSwapBulletEvent)
                {
                    OnSwapBulletEvent.Invoke();
                }
            }
            smoothDownTime = -1f;
        }

        private void ActionDownEvent()
        {
            B4DownStartTime = Time.time;
        }

        private void ActionUpEvent()
        {
            if (Time.time - B4DownStartTime <= B4DownTapLimitTime)
            {
                if (null != OnSwapItemEvent)
                {
                    OnSwapItemEvent.Invoke();
                }
            }
            B4DownStartTime = -1f;
        }

        /// <summary>
        /// 模拟射击(射击键)
        /// </summary>
        public static void VirtualShoot()
        {
            if (null != OnShootEvent)
            {
                OnShootEvent.Invoke();
            }
        }

        /// <summary>
        /// 模拟更换子弹(短按滑膛)
        /// </summary>
        public static void VirtualSwapBullet()
        {
            if (null != OnSwapBulletEvent)
            {
                OnSwapBulletEvent.Invoke();
            }
        }

        /// <summary>
        /// 模拟更换武器(长按滑膛2s)
        /// </summary>
        public static void VirtualSwapWappen()
        {
            if (null != OnSwapWappenEvent)
            {
                OnSwapWappenEvent.Invoke();
            }
        }

        /// <summary>
        /// 模拟更换道具(短按功能键)
        /// </summary>
        public static void VirtualSwapItem()
        {
            if (null != OnSwapItemEvent)
            {
                OnSwapItemEvent.Invoke();
            }
        }

        /// <summary>
        /// 模拟更换备用道具(长按功能键2s)
        /// </summary>
        public static void VirtualLongSwapItem()
        {
            if (null != OnLongSwapItemEvent)
            {
                OnLongSwapItemEvent.Invoke();
            }
        }
    }
}