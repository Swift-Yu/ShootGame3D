using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TipUI : MonoBehaviour {

    public void ShowTipWindow(string tiptext)
    {
        Text text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        if (!(text.text == tiptext))
        {
            Debug.Log("tiptext is "+tiptext);
            Debug.Log("text is "+text.text);
            gameObject.SetActive(false);
            text.text = tiptext;
            gameObject.SetActive(true);
        }
    }

    public void ShowTipWindow(string CnString, string EnString)
    {
        string tipText = "";
        Text text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
        {
            tipText = CnString;
        }
        else
        {
            tipText = EnString;
        }
        if (text.text != tipText)
        {
            Debug.Log("tiptext is " + tipText);
            Debug.Log("text is " + text.text);
            gameObject.SetActive(false);
            text.text = tipText;
            gameObject.SetActive(true);
        }
    }
}
