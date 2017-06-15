using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotateProgress : MonoBehaviour
{
    public Image Duck;
    public Vector3 rotateVec = new Vector3(0, 0, -5);
    // Use this for initialization
    void Start () {
        if (Duck)
        {
            Duck.transform.rotation = Quaternion.identity;
        }
	}
	
	// Update is called once per frame
	void Update () {
	    transform.Rotate(rotateVec);
	}

}
