using UnityEngine;
using System.Collections;
using I2.Loc;

public class SetSystemLanguage : MonoBehaviour
{

    // Use this for initialization
    void Awake()
    {
        if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseSimplified || Application.systemLanguage == SystemLanguage.ChineseTraditional)
        {
            SetLanguage("Chinese");
        }
        else
        {
            SetLanguage("English");
        }
    }
    public void SetLanguage(string _Language)
    {
        if (LocalizationManager.HasLanguage(_Language))
        {
            LocalizationManager.CurrentLanguage = _Language;
        }
    }
}
