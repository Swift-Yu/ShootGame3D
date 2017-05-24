using UnityEngine;
using System.Collections;

public class PlayerBoat : MonoBehaviour {

	float speed = 10;

	void Update(){

		if (Input.GetKey( KeyCode.LeftArrow)){
			transform.Translate(new Vector3(-speed*Time.deltaTime,0,0));
		}

		if (Input.GetKey( KeyCode.RightArrow)){
			transform.Translate(new Vector3(speed*Time.deltaTime,0,0));
		}
	}
}
