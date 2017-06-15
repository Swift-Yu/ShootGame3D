using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Showbaby.Bluetooth;
using Showbaby.Music;

public class MainUI : MonoBehaviour
{

    public string GameSceneName;
    public Text LoadingText;
    public Slider LoadingSlider;
    public GameObject LoadingWindow;
    public void GoGame()
    {
        MusciManager.Instance.PlayEffectMusic("CommonButton");
#if !UNITY_EDITOR
            var isconnected = BluetoothSDK.BluetoothSdk.IsBluetoothConnect();
            if (!isconnected)
            {
                return;
            }
#endif
        LoadingWindow.SetActive(true);
        Invoke("LoadingGame", 0.1f);      
    }

    void LoadingGame()
    {      
        StartCoroutine(StartLoading(GameSceneName));
    }

    private IEnumerator StartLoading(string sceneName)
    {
        int displayProgress = 0;
        int toProgress = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        while (op.progress < 0.9f)
        {
            toProgress = (int)op.progress * 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                SetLoadingPercentage(displayProgress);
                yield return new WaitForEndOfFrame();
            }
        }

        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            SetLoadingPercentage(displayProgress);
            yield return new WaitForEndOfFrame();
        }
        op.allowSceneActivation = true;
    }

    void SetLoadingPercentage(int displayProgress)
    {
        LoadingText.text = displayProgress + "%";
        LoadingSlider.value = displayProgress / 100.0f;
    }
}
