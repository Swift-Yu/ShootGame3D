using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Showbaby.Music;

public class GameUI : MonoBehaviour {

    public string MainSceneName;
   
    /// <summary>
    /// 回到Main场景
    /// </summary>
    public void GoMain()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
        SceneManager.LoadSceneAsync(MainSceneName);
    }
}
