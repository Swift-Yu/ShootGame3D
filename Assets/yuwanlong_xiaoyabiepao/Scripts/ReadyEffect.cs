using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ReadyEffect : MonoBehaviour {
    [SerializeField] private Text text1;

    [SerializeField] private Text text2;

	// Use this for initialization
	void Start () {
	    if (text1.gameObject.activeSelf||text2.gameObject.activeSelf)
	    {
	        text1.gameObject.SetActive(false);
            text2.gameObject.SetActive(false);
	    }
        StartCoroutine(changeText());
	}

    IEnumerator changeText()
    {
        text1.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(true);
    }
}
