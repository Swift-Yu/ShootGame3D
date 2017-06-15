using UnityEngine;
using System.Collections;
using com.showbaby.sar.uniwebview;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        WebView.Instance.OpenURL("https://www.baidu.com");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
