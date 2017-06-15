using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Showbaby.Music;

namespace com.showbaby.sar.uniwebview
{


    public class WebView : MonoBehaviour
    {
        private static WebView mInstance = null;

        public static WebView Instance
        {
            get
            {
                return mInstance;
            }
        }

        public GameObject WebViewWindow;
        public GameObject ButtonsPannel;
        public GameObject Slider;

#if UNITY_ANDROID || UNITY_IOS
        private GameObject WebViewObject;
        private UniWebView uniWebView;        

        private string currentUrl;
        private bool isLoading = false;        

        // Use this for initialization
        void Awake()
        {
            mInstance = this;
        }
        void Update()
        {
            if(Slider.activeInHierarchy)
            {
                if (isLoading)
                {
                    if(Slider.GetComponent<Slider>().value < 0.5f)
                    {
                        Slider.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
                    }
                }
                else
                {
                    Slider.GetComponent<Slider>().value += Time.deltaTime * 0.5f;
                }
                if (Slider.GetComponent<Slider>().value == 1.0f)
                {
                    Slider.SetActive(false);
                }
            }          
        }
        public void OpenURL(string url)
        {
          
            currentUrl = url;
            WebViewWindow.SetActive(true);          
            WebViewObject = new GameObject();
            uniWebView = WebViewObject.AddComponent<UniWebView>();
            uniWebView.OnReceivedMessage += OnReceivedMessage;
            uniWebView.OnReceivedKeyCode += UniWebView_OnReceivedKeyCode;
            uniWebView.OnLoadBegin += OnLoadBegin;
            uniWebView.OnLoadComplete += OnLoadComplete;
            uniWebView.OnWebViewShouldClose += OnWebViewShouldClose;
            uniWebView.OnEvalJavaScriptFinished += OnEvalJavaScriptFinished;
            int BottomSet = (int)(UniWebViewHelper.screenHeight * 140.0f / 1080.0f);
            uniWebView.insets = new UniWebViewEdgeInsets(0, 0, BottomSet, 0);
            uniWebView.SetShowSpinnerWhenLoading(false);
            uniWebView.CleanCache();
            uniWebView.url = url;
            uniWebView.Load();
        }

        private void UniWebView_OnReceivedKeyCode(UniWebView webView, int keyCode)
        {
            //处理手机返回键
            if (keyCode == 4)
            {
                Close();
            }
        }

        //关闭按钮
        public void Close()
        {
            MusciManager.Instance.PlayEffectMusic("CommonButton");
            if (null != uniWebView)
            {
                uniWebView.OnReceivedMessage -= OnReceivedMessage;
                uniWebView.OnReceivedKeyCode -= UniWebView_OnReceivedKeyCode;
                uniWebView.OnLoadBegin -= OnLoadBegin;
                uniWebView.OnLoadComplete -= OnLoadComplete;
                uniWebView.OnWebViewShouldClose -= OnWebViewShouldClose;
                uniWebView.OnEvalJavaScriptFinished -= OnEvalJavaScriptFinished;
                uniWebView = null;                
            }
            if (null != WebViewObject)
            {
                Destroy(WebViewObject);
            }
            if (null != WebViewWindow)
            {
                WebViewWindow.SetActive(false);
            }            
        }
        //返回上一页
        public void GoBack()
        {
            MusciManager.Instance.PlayEffectMusic("CommonButton");
            if (uniWebView != null)
            {
                uniWebView.GoBack();
            }
        }
        //返回前一页
        public void GoForward()
        {
            MusciManager.Instance.PlayEffectMusic("CommonButton");
            if (uniWebView != null)
            {
                uniWebView.GoForward();
            }
        }
        //刷新
        public void Refresh()
        {
            MusciManager.Instance.PlayEffectMusic("CommonButton");
            if (uniWebView != null)
            {
                uniWebView.Reload();
            }
        }

        private void OnLoadBegin(UniWebView webView, string loadingUrl)
        {
          
            Slider.SetActive(true);
            Slider.GetComponent<Slider>().value = 0.0f;
            isLoading = true;
        }
        void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
        {
            isLoading = false;
            if (success)
            {
                webView.Show();
            }
            else
            {
                Debug.Log("Something wrong in webview loading: " + errorMessage);            
            }
        }
        void OnReceivedMessage(UniWebView webView, UniWebViewMessage message)
        {
           
        }

       
        void OnEvalJavaScriptFinished(UniWebView webView, string result)
        {        
        }
        bool OnWebViewShouldClose(UniWebView webView)
        {
            if (webView == uniWebView)
            {
                uniWebView = null;
                return true;
            }
            return false;
        }   
#endif
    }
}