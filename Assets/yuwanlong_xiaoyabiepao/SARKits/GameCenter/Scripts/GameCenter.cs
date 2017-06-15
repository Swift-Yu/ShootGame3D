using UnityEngine;
using System.Collections;
using com.showbaby.sar.uniwebview;
using Showbaby.Command;
using LitJson;
using Showbaby.UI;
using I2.Loc;
using System;
using UnityEngine.UI;
using Showbaby.Music;

public class GameCenter : MonoBehaviour
{
    public delegate void SwitchDelegate(bool state);

    /// <summary>
    /// 开关声音 True声音开 false声音关
    /// </summary>
    public static event SwitchDelegate OnSwitchVoice;

    public string MoreGameURL = "http://src.showbabybox.com/ARGun/ARGunHome/index.html";
    public string QuestionAndAnswerURL = "http://src.showbabybox.com/ARGun/ARGunHome/index.html";

    private static GameCenter mInstance = null;
    private bool receiveFeedBackMessage = false;
    private int FeedBackSTS = -1;

    private bool isRequestFeedBack = false;
    private bool resultErrorNetwork = false;

    public Image LanguageChinese;
    public Image LanguageEnglish;
    public Image AudioOpen;
    public Image AudioClose;
    public Text VersionText;
    public GameObject BluetoothBtn;
    public GameObject MoreGameBtn;
    public Vector2 Pos1;
    public Vector2 Pos2;
    public String Version;//游戏版本号
    public bool IsGooglePlay = false;//是否是Google Play版本
    public static GameCenter Instance
    {
        get
        {
            return mInstance;
        }
    }
    void Awake()
    {       
        mInstance = this;
        showVersion();
        showMainButtons();
        if (PlayerPrefs.GetString("Language","").Equals("Chinese"))
        {
            changeChinese();
        }
        else
        {
            changeEnglish();
        }

        if (PlayerPrefs.GetInt("CloseAudio", 0) == 0)
        {
            switchAudio(true);
        }
        else
        {
            switchAudio(false);
        }
    }

    /// <summary>
    /// 根据是否是Google Play版本决定是否显示更多游戏按钮
    /// </summary>
    private void showMainButtons()
    {
        if (IsGooglePlay)
        {
            BluetoothBtn.GetComponent<RectTransform>().anchoredPosition = Pos2;
            MoreGameBtn.SetActive(false);
        }
        else
        {
            BluetoothBtn.GetComponent<RectTransform>().anchoredPosition = Pos1;
            MoreGameBtn.GetComponent<RectTransform>().anchoredPosition = Pos2;
            MoreGameBtn.SetActive(true);
        }
    }

    /// <summary>
    /// 显示游戏版本信息
    /// </summary>
    private void showVersion()
    {
        VersionText.text = Version;
    }

	void Update()
    {
        if(receiveFeedBackMessage)
        {
            if (FeedBackSTS == 0)
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.FeedBack.FeedBackSubmitSuccess, 2.0f);
                   
                }
            }
            else
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.FeedBack.FeedBackSubmitFail, 2.0f);
                }

            }
            FeedBack.Instance.CloseFeedBackWindow();
            receiveFeedBackMessage = false;
        }
    }

    public void OnErrorNetwork()
    {
        if (isRequestFeedBack && null != TipWindow.Instance)
        {
            TipWindow.Instance.CloseWaitingWindow();
            TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Network.NetError, 2.0f);
            isRequestFeedBack = false;
        }
    }

    public void OnNoneNetwork()
    {
        if (isRequestFeedBack && null != TipWindow.Instance )
        {
            TipWindow.Instance.CloseWaitingWindow();
            TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.Network.NoneNet, 2.0f);
            isRequestFeedBack = false;
        }

    }

    public void FeedBackCmd_OnReceiveMessage(string url,string data)
    {
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
                FeedBackSTS = 0;
            }
            else
            {
                FeedBackSTS = 1;

            }
        }
        isRequestFeedBack = false;
        receiveFeedBackMessage = true;
    }

    //按钮事件
    public void OpenFeedBack()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
        if (FeedBack.Instance != null)
        {
            FeedBack.Instance.ShowFeedBackWindow((content,contact)=> 
            {
                if(CmdManager.Instance != null)
                {
                    isRequestFeedBack = true;
                    CmdManager.Instance.SendFeedBackCmd(content, contact);

                }
            });
        }
    }

    public void QuestionAndAnswer()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
#if UNITY_ANDROID || UNITY_IOS
        WebView.Instance.OpenURL(QuestionAndAnswerURL);
        #endif
    }

    public void MoreGame()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
#if UNITY_ANDROID || UNITY_IOS
        WebView.Instance.OpenURL(MoreGameURL);
        #endif
    }

    /// <summary>
    /// 点击设置按钮
    /// </summary>
    public void OnClickSettingBtn()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
    }

    public void ExitGame()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
        if (null != TipWindow.Instance)
        {
            TipWindow.Instance.ShowConfirmWindow(I2.Loc.ScriptLocalization.TipWindow.ExitGame,I2.Loc.ScriptLocalization.TipWindow.ExitText,I2.Loc.ScriptLocalization.TipWindow.CancelText,
                ()=> { Application.Quit(); });
        }
    }

    /// <summary>
    /// 点击设置面板的关闭按钮
    /// </summary>
    public void OnClickCloseSettingWindowBtn()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
    }

    public void openAudio()
    {        
        switchAudio(true);
        MusciManager.Instance.PlayEffectMusic("CommonButton");
    }

    private void switchAudio(bool f)
    {
        if (f)
        {
            PlayerPrefs.SetInt("CloseAudio", 0);
            var AudioOpenLocalize = AudioOpen.GetComponent<Localize>();
            AudioOpenLocalize.SetTerm("Settings/AudioOpen");
            var AudioCloseLocalize = AudioClose.GetComponent<Localize>();
            AudioCloseLocalize.SetTerm("Settings/AudioCloseOff");
            
        } else
        {
            PlayerPrefs.SetInt("CloseAudio", 1);
            var AudioOpenLocalize = AudioOpen.GetComponent<Localize>();
            AudioOpenLocalize.SetTerm("Settings/AudioOpenOff");
            var AudioCloseLocalize = AudioClose.GetComponent<Localize>();
            AudioCloseLocalize.SetTerm("Settings/AudioClose");
        }
        if (null != OnSwitchVoice)
        {
            OnSwitchVoice.Invoke(f);
        }
    }

    public void CloseAudio()
    {
        //MusciManager.Instance.PlayEffectMusic("CommonButton");
        switchAudio(false);        
    }

    public void ChangeLanguageChinese()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
        changeChinese();
    }

    private void changeChinese()
    {
        SetLanguage("Chinese");
        var LanguageChineseOnLocalize = LanguageChinese.GetComponent<Localize>();
        LanguageChineseOnLocalize.SetTerm("Settings/LanguageChinese");
        var LanguageChineseOffLocalize = LanguageEnglish.GetComponent<Localize>();
        LanguageChineseOffLocalize.SetTerm("Settings/LanguageEnglishOff");
    }

    public void ChangeLanguageEnglish()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
        changeEnglish();
    }

    private void changeEnglish()
    {
        SetLanguage("English");
        var LanguageChineseOnLocalize = LanguageChinese.GetComponent<Localize>();
        LanguageChineseOnLocalize.SetTerm("Settings/LanguageChineseOff");
        var LanguageChineseOffLocalize = LanguageEnglish.GetComponent<Localize>();
        LanguageChineseOffLocalize.SetTerm("Settings/LanguageEnglish");
    }

    private void SetLanguage(string _Language)
    {
        if (LocalizationManager.HasLanguage(_Language))
        {
            LocalizationManager.CurrentLanguage = _Language;           
            PlayerPrefs.SetString("Language", _Language);            
        }
    }
}
