using UnityEngine;
using System.Collections;

public class RandomRotation : MonoBehaviour {

	private float force;

	void OnSpawn(){
		force= Random.Range(100,200);
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate( Vector3.right * force * Time.deltaTime,Space.Self);
	}
}
