using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ReLoadScene : MonoBehaviour {

	public void ReStart(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name );
	}
}
