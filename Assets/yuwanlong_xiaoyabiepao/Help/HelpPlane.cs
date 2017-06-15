using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using Showbaby.UI;
using UnityEngine.UI;

public class HelpPlane : MonoBehaviour
{
    public static bool ImaSaveFlag;
    public Button help;
    public GameObject plane;
    public Button close;
    public Button save;
    public RawImage image;
    public string imageName = "SplitField.png";

    [DllImport("__Internal")]
    private static extern void initSaveImg(string OBJName,string methodName);
    [DllImport("__Internal")]
    private static extern void saveImg(string fileExitPath);
    // Use this for initialization
    void Awake()
    {
        string streamingAssetsPath = Application.streamingAssetsPath + "/" + imageName;
        if (!File.Exists(streamingAssetsPath))
        {
            Debug.LogError("streamingAssets文件夹下没有指定文件 ： " + imageName);
            return;
        }
        else
        {
            string dataPath = Application.persistentDataPath + "/" + imageName;
            if (!File.Exists(dataPath)) //如果没有指定文件就拷贝
            {
                File.Copy(streamingAssetsPath, dataPath);
            }
            Debug.Log(dataPath);
            //加载显示
            Texture2D texture = new Texture2D(1280, 1280);
            var buffer = File.ReadAllBytes(dataPath);
            texture.LoadImage(buffer);
            image.texture = texture;
        }
    }
    void Start()
    {
#if UNITY_IOS || UNITY_IPHONE
        initSaveImg(gameObject.name, "saveImageCallBack");
#endif
        //绑定按钮事件
        help.onClick.AddListener(() => plane.SetActive(true));
        close.onClick.AddListener(() => plane.SetActive(false));
        save.onClick.AddListener(ButtonClicked);
    }
    public void ButtonClicked ()
	{
        if (ImaSaveFlag)
        {
            TipWindow.Instance.ShowToastWindow("已经保存过了哦 ！", 1.5f);
        }
        else
        {
            var exitPath = Application.persistentDataPath + "/" + imageName;
            saveImg(exitPath);
        }
	}

    void saveImageCallBack(string message)
    {
        string code = GetCode(message);
        if (code == "0")
        {
            TipWindow.Instance.ShowToastWindow("已成功保存到相册 ！", 1.5f);
            ImaSaveFlag = true;
        }
        else if (code == "1")
        {
            TipWindow.Instance.ShowToastWindow("保存到相册失败 ！", 1.5f);
        }
    }

    string GetCode(string s)
    {
        return s.Split(',')[0].Split(':')[1].Trim('\'');
    }

}
