using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour
{
    public GameObject title;
    private GameObject CNtitle;
    private GameObject ENtitle;
	// Use this for initialization
	void Start ()
	{
	    CNtitle = title.transform.GetChild(0).gameObject;
        ENtitle = title.transform.GetChild(1).gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	    if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
	    {
	        if (!CNtitle.activeSelf)
	        {
                ENtitle.SetActive(false);
	            CNtitle.SetActive(true);
	        }
	    }
	    else
	    {
	        if (!ENtitle.activeSelf)
	        {
	            CNtitle.SetActive(false);
                ENtitle.SetActive(true);
	        }
	    }
	
	}
}
