using UnityEngine;
using System.Collections;

public class SpaceshipMove : MonoBehaviour {

	public float speed;
	
	// Update is called once per frame
	void Update () {
		transform.Translate( Vector3.forward * speed * Time.deltaTime);
	}
}
