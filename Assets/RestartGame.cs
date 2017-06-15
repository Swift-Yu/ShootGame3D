using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{

    public Transform ProGress;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void RestartScene()
    {
        StartCoroutine(Restart());
    }

    private IEnumerator Restart()
    {
        ProGress.gameObject.SetActive(true);
        SceneManager.LoadScene(1);
        yield break;
    }
}
