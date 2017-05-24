using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	private float force = 50;
	private Rigidbody body;

	void Awake(){
		body = GetComponent<Rigidbody>();
	}

	void OnSpawn(){
		//force = Random.Range(0.2f,1f);
		force = Random.Range(4f,10f);
		body.transform.LookAt( Vector3.zero);

	}
	// Update is called once per frame
	void FixedUpdate () {
		body.AddRelativeForce( Vector3.forward * force,ForceMode.Acceleration);
	}
}
