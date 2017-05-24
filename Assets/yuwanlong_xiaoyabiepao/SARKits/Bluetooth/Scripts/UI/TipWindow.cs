using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

namespace Showbaby.UI
{
    /// <summary>
    /// 提示界面
    /// </summary>
    public class TipWindow : MonoBehaviour
    {
        #region 面板设置      
        public GameObject ToastWindow = null; //提示框，一段时间自动关闭
        public GameObject AlertWindow = null; //警告框，需要手动关闭
        public GameObject ConfirmWindow = null;//确认框，选择确定/取消
        public GameObject WaitingWindow = null;//等待框，如网络请求时
        #endregion        
       
        private Coroutine waiCoroutine = null;
        private GameObject curScrollBarWindow = null;

        private static TipWindow mInstance = null;

        public static TipWindow Instance
        {
            get
            {
                return mInstance;
            }
        }      

        private bool waitingWindowIsActive = false;

        void Awake()
        {
//            DontDestroyOnLoad(transform.gameObject);
            mInstance = this;
        }
     
        void Update()
        {
            if(waitingWindowIsActive)
            {
                if(Input.anyKeyDown)
                {
                    CloseWaitingWindow();
                }
            }
        }
        //-----------------------操作windows---------------------------------------------------------
        public void ShowToastWindow(string content, float time)
        {
            if (ToastWindow.activeInHierarchy)
            {
                CancelInvoke("HideToastWindow");
            }
            ToastWindow.SetActive(true);
            ToastWindow.transform.Find("Content").GetComponent<Text>().text = content;           
            if(time>0)
            {
                Invoke("HideToastWindow", time);
            }            
        }

        public void ShowAlertWindow(string content, string buttonText, Action callback)
        {
            AlertWindow.SetActive(true);
            AlertWindow.transform.Find("Background/Content").GetComponent<Text>().text = content;
            AlertWindow.transform.Find("Background/ConfirmButton").GetComponent<Button>().onClick.AddListener(() => { callback(); AlertWindow.SetActive(false); });
            AlertWindow.transform.Find("Background/ConfirmButton").GetComponentInChildren<Text>().text = buttonText;
        }

        public void ShowConfirmWindow(string content, string confirmText, string cancelText, Action callback)
        {
            ConfirmWindow.SetActive(true);
            ConfirmWindow.transform.Find("Background/Content").GetComponent<Text>().text = content;
            ConfirmWindow.transform.Find("Background/CancelButton").GetComponent<Button>().onClick.AddListener(() => {ConfirmWindow.SetActive(false); });
            ConfirmWindow.transform.Find("Background/CancelButton").GetComponentInChildren<Text>().text = cancelText;
            ConfirmWindow.transform.Find("Background/ConfirmButton").GetComponent<Button>().onClick.AddListener(() => { callback(); ConfirmWindow.SetActive(false); });
            ConfirmWindow.transform.Find("Background/ConfirmButton").GetComponentInChildren<Text>().text = confirmText;
        }
        public void ShowConfirmWindowInput(string content, string confirmText, string cancelText, Action<string> callback)
        {
            ConfirmWindow.SetActive(true);
            ConfirmWindow.transform.Find("Background/Content").GetComponent<Text>().text = "";
            ConfirmWindow.transform.Find("Background/InputField").gameObject.SetActive(true);
            ConfirmWindow.transform.Find("Background/InputField").GetComponent<InputField>().text = content;
            ConfirmWindow.transform.Find("Background/CancelButton").GetComponent<Button>().onClick.AddListener(() => 
            { ConfirmWindow.transform.Find("Background/InputField").gameObject.SetActive(false); ConfirmWindow.SetActive(false); });
            ConfirmWindow.transform.Find("Background/CancelButton").GetComponentInChildren<Text>().text = cancelText;
            ConfirmWindow.transform.Find("Background/ConfirmButton").GetComponent<Button>().onClick.AddListener(() => 
            {   callback(ConfirmWindow.transform.Find("Background/InputField").GetComponent<InputField>().text);
                ConfirmWindow.transform.Find("Background/InputField").gameObject.SetActive(false);
                ConfirmWindow.SetActive(false); });
            ConfirmWindow.transform.Find("Background/ConfirmButton").GetComponentInChildren<Text>().text = confirmText;
        }
        public void ShowWaitingWindow(string content, bool canCancel)
        {
            WaitingWindow.SetActive(true);
            WaitingWindow.transform.Find("Background/Content").GetComponent<Text>().text = content;
            if (canCancel)
            {
                waitingWindowIsActive = true;
            }            
        }
        public void CloseWaitingWindow()
        {
            WaitingWindow.SetActive(false);
            waitingWindowIsActive = false;
        }
       
        //-----------------------操作windows---------------------------------------------------------

        private void HideToastWindow()
        {
            ToastWindow.SetActive(false);
        }         
    }
}