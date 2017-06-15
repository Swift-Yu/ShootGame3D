using UnityEngine;
using System.Collections;
using Umeng;

public class umengStart : MonoBehaviour {
    /*为了防止开发者忘了配置appkey 
     * 这里默认为空字符串，如果开发者不配置的话就会打印未配置appkey的提示日志(在开发时运行就能看到提示日志)
     * 切记在面板配置自己游戏的友盟统计的appkey！！！！！
     */

    /// <summary>
    ///Android appkey
    /// </summary>
    public string androidAppkey = "";

    /// <summary>
    /// iOS appkey
    /// </summary>
    public string iosAppkey = "";

    /// <summary>
    /// 当前关卡的名字 
    /// 若需统计关卡信息 请在后台配置关卡信息并在进入关卡和退出关卡的时候
    /// 分别调用StartLevel FinishLevel方法
    /// </summary>
    public static string CurLevel = "Level_1";
    // Use this for initialization
    void Start ()
    {
#if UNITY_ANDROID
        if (androidAppkey.Equals(""))
        {
            Debug.LogError("umeng androidAppkey 未配置 请在Start场景的umengStart预制件上配置Appkey");
            return;
        }
        GA.StartWithAppKeyAndChannelId(androidAppkey, "Android");
#elif UNITY_IOS
        if (iosAppkey.Equals(""))
        {
            Debug.LogError("umeng iosAppkey 未配置 请在Start场景的umengStart预制件上配置Appkey");
            return;
        }
        GA.StartWithAppKeyAndChannelId(iosAppkey, "Ios");
#endif
        Analytics.GetDeviceInfo();//统计设备信息
    }

    /// <summary>
    /// 进入关卡
    /// </summary>
    /// <param name="levelName"></param>
    public static void StartLevel(string levelName)
    {
        CurLevel = levelName;
        GA.StartLevel(CurLevel);
    }

    /// <summary>
    /// 退出关卡
    /// </summary>
    public static void FinishLevel()
    {
        GA.FinishLevel(CurLevel);
    }
}
