using UnityEngine;
using System.Collections;
using ZXing;
using ZXing.QrCode;

public class QR
{
    /// <summary>  
    /// 解析二维码  
    /// </summary>  
    /// <param name="tex"></param>  
    /// <returns></returns>  
    public static string Decode(Texture2D tex)
    {
        return DecodeColData(tex.GetPixels32(), tex.width, tex.height); //通过reader解码    
    }
    public static string DecodeColData(Color32[] data, int w, int h)
    {
        BarcodeReader reader = new BarcodeReader();
        Result result = reader.Decode(data, w, h); //通过reader解码    
        //GC.Collect();  
        if (result == null)
            return "";
        else
        {
            return result.Text;
        }
    }
    /// <summary>  
    /// 生成二维码  
    /// </summary>  
    /// <param name="content"></param>  
    /// <param name="len"></param>  
    /// <returns></returns>  
    public static Texture2D GetQRTexture(string content, int len = 256)
    {
        var bw = new BarcodeWriter();
        bw.Format = BarcodeFormat.QR_CODE;
        bw.Options = new ZXing.Common.EncodingOptions()
        {
            Height = len,
            Width = len
        };
        var cols = bw.Write(content);
        Texture2D t = new Texture2D(len, len);
        t.SetPixels32(cols);
        t.Apply();
        return t;
    }
}