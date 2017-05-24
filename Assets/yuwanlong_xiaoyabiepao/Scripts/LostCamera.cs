using UnityEngine;
using System.Collections;

public class LostCamera : MonoBehaviour
{

    public Transform cameratTransform;
    //private Transform foundPosition;
	// Use this for initialization
	void Start ()
	{
	    //cameratTransform = Camera.main.transform.parent;
	    //foundPosition = cameratTransform.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //public void SetFoundCamera()
    //{
    //    camera.transform.position = foundPosition.position;
    //    camera.transform.rotation = foundPosition.rotation;
    //}

    public void SetLostCamera()
    {
        cameratTransform.position = transform.position;
        cameratTransform.localRotation = transform.rotation;
    }

    //IEnumerator SetPosition()
    //{
    //    yield return new  WaitForSeconds(0.1f);
    //    cameratTransform.position = transform.position;
    //    cameratTransform.rotation = transform.rotation;
    //}
}
