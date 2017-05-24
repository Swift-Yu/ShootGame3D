using UnityEngine;
using System.Collections;

namespace Showbaby
{
    /// <summary>
    /// 此类负责Context的初始化
    /// 所有需要调用封装的android底层工具的方法使用之前都需要先执行本类的初始化方法，并且此方有且只执行一次
    /// </summary>
    public class ArShowContext
    {
        private static ArShowContext mInstance = null;
        public static ArShowContext Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = new ArShowContext();
                }
                return mInstance;
            }
        }

        private bool hasInit = false;//是否已初始化 为了保证有且只执行一次

#if UNITY_ANDROID && !UNITY_EDITOR
        public AndroidJavaClass ArContext = null;

        public AndroidJavaObject CurrentActivity = null;
#endif

        /// <summary>
        /// 初始化Context
        /// </summary>
        public void Init()
        {
            if (!hasInit)
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                //初始化android ArShowContext 有且只执行一次
                var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                CurrentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                ArContext = new AndroidJavaClass("com.arshowbaby.unitylibrary.ArShowContext");
                ArContext.CallStatic("init", CurrentActivity);
#endif
                hasInit = true;
            }
        }

        /// <summary>
        /// 添加插件
        /// 便于android端控制生命周期
        /// </summary>
        /// <param name="calssName"></param>
        /// <param name="obj"></param>
        public void PutPlugin(string calssName, AndroidJavaObject obj)
        {
            //Debug.Log("PutPlugin calssName " + calssName);
#if UNITY_ANDROID && !UNITY_EDITOR
            ArContext.CallStatic<AndroidJavaObject>("getPluginManager").Call("putPlugin", calssName, obj);
#endif
        }

        /// <summary>
        /// 退到后台
        /// </summary>
        public void MoveTaskToBack()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            CurrentActivity.Call<bool>("moveTaskToBack", false);
#endif
        }

        public void Finish()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            CurrentActivity.Call("finish");
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 跳转到设置权限的界面
        /// </summary>
        public void OpenAppSetting()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ArContext.CallStatic("openAppSetting");
#endif
        }
    }
//#endif
}