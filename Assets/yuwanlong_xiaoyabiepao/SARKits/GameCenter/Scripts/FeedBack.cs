using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Showbaby.UI;
using Showbaby.Music;

public class FeedBack : MonoBehaviour
{
    public GameObject FeedBackWindow = null;//反馈框

    private static FeedBack mInstance = null;
    public static FeedBack Instance
    {
        get
        {
            return mInstance;
        }
    }
    void Awake()
    {
        mInstance = this;
    }

    public void ShowFeedBackWindow(Action<string,string> callback)
    {
        FeedBackWindow.SetActive(true);
        FeedBackWindow.transform.Find("Background/CancelButton").GetComponent<Button>().onClick.AddListener(() => { MusciManager.Instance.PlayEffectMusic("CommonButton"); hideFeedBackWindow(); });
        FeedBackWindow.transform.Find("Background/ConfirmButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            MusciManager.Instance.PlayEffectMusic("CommonButton");
            if (FeedBackWindow.transform.Find("Background/Content").GetComponent<InputField>().text.Trim().Equals(""))
            {
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowToastWindow(I2.Loc.ScriptLocalization.FeedBack.FeedBackEmpty,2.0f);
                }
            }
            else
            { 
                callback(FeedBackWindow.transform.Find("Background/Content").GetComponent<InputField>().text, FeedBackWindow.transform.Find("Background/Contact").GetComponent<InputField>().text);
                if (null != TipWindow.Instance)
                {
                    TipWindow.Instance.ShowWaitingWindow(I2.Loc.ScriptLocalization.TipWindow.ProcessText, false);
                }
            }
          
        });
    }

    public void CloseFeedBackWindow()
    {
        if(TipWindow.Instance!=null)
        {
            TipWindow.Instance.CloseWaitingWindow();
        }
        hideFeedBackWindow();
    }

    //关闭窗口
    public void hideFeedBackWindow()
    {
        FeedBackWindow.transform.Find("Background/CancelButton").GetComponent<Button>().onClick.RemoveAllListeners();
        FeedBackWindow.transform.Find("Background/ConfirmButton").GetComponent<Button>().onClick.RemoveAllListeners();
        FeedBackWindow.transform.Find("Background/Content").GetComponent<InputField>().text = "";
        FeedBackWindow.transform.Find("Background/Contact").GetComponent<InputField>().text = "";
        FeedBackWindow.SetActive(false);
    }
}
