using UnityEngine;
using System.Collections;

public class QRCreater : MonoBehaviour
{
    Texture2D texture;
	// Use this for initialization
	void Start ()
    {
        texture =  QR.GetQRTexture("baidu.com",256);
    }
    void OnGUI()
    {
        if(texture)
        {
            GUI.DrawTexture(new Rect(200, 200, 256, 256), texture);
        }     
    }
}
