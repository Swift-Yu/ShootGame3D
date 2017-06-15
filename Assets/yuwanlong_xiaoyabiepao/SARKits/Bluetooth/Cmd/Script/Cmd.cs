using UnityEngine;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.IO;
using System;
using Showbaby.UI;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using System.Threading;
using System.Collections.Generic;

namespace Showbaby.Command
{
    /// <summary>
    /// 与服务器进行消息交互的类
    /// </summary>
    public class Cmd
    {
        #region 获取类的实例
        private static Cmd mInstance = null;
        /// <summary>
        /// 获取类的实例
        /// </summary>
        /// <returns></returns>
        public static Cmd Instance
        {
            get
            {
                if (null == mInstance)
                    mInstance = new Cmd();
                return mInstance;
            }
        }
        #endregion

        #region 私有变量
        private string appKey = "1806978425";//公钥
		private string appSecret = "f52894ac762f4832a31b42876afe1dc8";//私钥
        #endregion

        #region 消息接口
        public delegate void ReceiveMessageDelegate(string url, string json);
        public static event ReceiveMessageDelegate OnReceiveMessage;
        public delegate void NoneNetwork();
        public static event NoneNetwork OnNoneNetwork;
        public delegate void ErrorNetwork();
        public static event ErrorNetwork OnErrorNetwork;
        #endregion

        private int TimeoutTime = 10000;
        System.Threading.Timer timer = null;
        bool TimerInvaild = false;
        float createTimerTime;

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //如果没有错就表示验证成功              
            if (errors == SslPolicyErrors.None)
                return true;
            else
            {
                if ((SslPolicyErrors.RemoteCertificateNameMismatch & errors) == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    return false;
                   // Debug.Log("证书名称不匹配:"+ errors);
                }
                if ((SslPolicyErrors.RemoteCertificateChainErrors & errors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    //string msg = "";
                    //foreach (X509ChainStatus status in chain.ChainStatus)
                    //{
                    //    msg += "status code ={0} " + status.Status; msg += "Status info = " + status.StatusInformation + " ";
                    //}
                    //Debug.Log("证书链错误:"+msg);
                    return true;
                }            
             }
            return false;  
        }
        /// <summary>
        /// http post请求
        /// </summary>
        /// <param name="url"></param>
        public void HttpPost(string url)
        {
            Uri uri = new Uri(url);
            //Debug.Log("HttpPost Application.internetReachability " + Application.internetReachability);
            //判断当前是否有网
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //没网提示 提示面板显示网络异常
                if (null != OnNoneNetwork)
                {
                    OnNoneNetwork();
                }
                return;
            }

  //              Debug.Log("HttpPost url " + url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
                //HTTPSQ请求  
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
              
                request.ProtocolVersion = HttpVersion.Version10;            
                request.Method = "POST";
                request.Timeout = TimeoutTime;
                request.ReadWriteTimeout = TimeoutTime;
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = DefaultUserAgent;

                createTimerTime = Time.time;
                timer = new System.Threading.Timer(new TimerCallback(OnTimeOut), createTimerTime, TimeoutTime, Timeout.Infinite);
                TimerInvaild = false;
                request.BeginGetRequestStream(new AsyncCallback(RequestProceed), request);
        }
        private void OnTimeOut(object sender)
        {       
            float createTime = (float)sender;
            if(!TimerInvaild && createTimerTime == createTime)
            {
                if (null != OnErrorNetwork)
                {
 //                   Debug.LogError("OnErrorNetwork");
                    OnErrorNetwork();
                }
            }                            
        }
        /// <summary>
        /// 请求成功的回调
        /// </summary>
        /// <param name="asyncResult"></param>
        public void RequestProceed(IAsyncResult asyncResult)
        {
          //  Debug.LogError("RequestProceed");
            try
            {
                HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
                StreamWriter postDataWriter = new StreamWriter(request.EndGetRequestStream(asyncResult));
                postDataWriter.Write("ticker=NTES");
                postDataWriter.Write("&startdate=1-1-2009");
                postDataWriter.Write("&enddate=9-2-2010");
                postDataWriter.Close();
                request.BeginGetResponse(new AsyncCallback(ResponesProceed), request);
            }
            catch (WebException e)
            {
                TimerInvaild = true;
                if (null != OnErrorNetwork)
                {
   //                 Debug.LogError("OnErrorNetwork");
                    OnErrorNetwork();
                }
            }
}

        /// <summary>
        /// 收到服务器返回的结果
        /// </summary>
        /// <param name="asyncResult"></param>
        public void ResponesProceed(IAsyncResult asyncResult)
        {
            //Debug.LogError("ResponesProceed");
            WebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);
            StreamReader responseReader = new StreamReader(response.GetResponseStream());
            string data = responseReader.ReadToEnd();
            //返回结果处理
  //          Debug.Log("ResponesProceed data " + data);
            if (null != OnReceiveMessage)
            {               
                var url = getUrlFromUri(response.ResponseUri);
                //     Debug.Log("ResponesProceed url " + url + " data " + data);
                TimerInvaild = true;
                OnReceiveMessage(url, data);
            }
        }

        /// <summary>
        /// 获取消息对应的具体接口（以便区分是哪条消息）
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private string getUrlFromUri(Uri uri)
        {
            string result = "";
            string uriStr = uri.ToString().Split('?')[0];
            var startIndex = uriStr.LastIndexOf('/');
            var endIndex = uriStr.Length;
            result = uriStr.Substring(startIndex + 1, endIndex - startIndex - 1);
            return result;
        }

        /// <summary>
        /// 去掉非数字非字母非中文的字符
        /// </summary>
        /// <param name="biz"></param>
        /// <returns></returns>
        public string GetContent(string biz)
        {
            Regex reg = new Regex(@"[^0-9a-zA-Z\u4e00-\u9fa5]");
            string content = reg.Replace(biz, "");
            if (string.IsNullOrEmpty(content.Trim()))
            {
				content = mInstance.appKey;
            }
            return content;
        }

        /// <summary>
        /// 将字节流转换为十六进制
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string bytes2hex(byte[] bytes)
        {
            string HEX = "0123456789abcdef";
            StringBuilder sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                // 取出这个字节的高4位，然后与0x0f与运算，得到一个0-15之间的数据，通过HEX.charAt(0-15)即为16进制数
                int index1 = ((b >> 4) & 0x0f);
                sb.Append(HEX.Substring(index1, 1));
                // 取出这个字节的低位，与0x0f与运算，得到一个0-15之间的数据，通过HEX.charAt(0-15)即为16进制数
                int index2 = (b & 0x0f);
                sb.Append(HEX.Substring(index2, 1));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetSignString(string content)
        {
            try
            {
                System.Security.Cryptography.MD5 md = System.Security.Cryptography.MD5.Create();

                byte[] contentBytes = Encoding.UTF8.GetBytes(content.Replace(" ", ""));
                byte[] digestMD5 = md.ComputeHash(contentBytes);
				String signStr = mInstance.appSecret + bytes2hex(digestMD5) + mInstance.appSecret;
                byte[] signBytes = Encoding.UTF8.GetBytes(signStr);
                //SHA1编码
                System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create();
                byte[] digestSHA1 = sha1.ComputeHash(signBytes);
                String sign = bytes2hex(digestSHA1);
                return sign;
            }
            catch (Exception e)
            {
//                Debug.LogError("GetSignString Exception " + e.ToString());
            }
            return null;
        }

        /// <summary>
        /// 将给定时间转换为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ConvertDateTimeToInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (time.Ticks - startTime.Ticks) / 10000000;   //除10000000调整为10位
            string timer = t.ToString();
            return timer;
        }

        /// <summary>
        /// 根据参数biz得到pdata
        /// </summary>
        /// <param name="biz"></param>
        /// <returns></returns>
        public string GetPdataByBiz(string biz)
        {
            string content = GetContent(biz);
            string sign = GetSignString(content);
            string timer = ConvertDateTimeToInt(DateTime.Now);
            string pdata = string.Format("{0}\"appkey\":\"{1}\",\"signature\":\"{2}\",\"timestamp\":\"{3}\",\"sts\":\"\",\"rmk\":\"\",\"apkVersion\":\"1.0.0\",\"biz\":{4}{5}", "{", appKey, sign, timer.Replace("-", ""), biz, "}");
            return pdata;
        }  
    }
}

