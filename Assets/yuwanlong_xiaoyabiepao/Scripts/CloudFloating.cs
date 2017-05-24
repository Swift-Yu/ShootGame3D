using UnityEngine;
using System.Collections;

public class CloudFloating : MonoBehaviour
{
    private Transform thisTransform;

    private Vector3 initialPosition;

    public GameObject rightEdge;

    public GameObject leftEdge;

    public float speed = 1f;
    // Use this for initialization
    void Start ()
    {
        speed = Random.Range(1f, 3f);
        thisTransform = transform;
        initialPosition = thisTransform.position;
    }
	
	// Update is called once per frame
	void Update ()
	{
        thisTransform.position = transform.position;
	    transform.position = new Vector3(thisTransform.position.x + speed * Time.deltaTime,thisTransform.position.y,thisTransform.position.z);
	    if (transform.position.x > rightEdge.transform.position.x)
	    {
           transform.position = new Vector3(leftEdge.transform.position.x,thisTransform.position.y,thisTransform.position.z);
	    }
	}
}
