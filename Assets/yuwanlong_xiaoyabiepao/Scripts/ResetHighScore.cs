using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetHighScore : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ResetScore()
    {
        PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", 0);
    }
}
