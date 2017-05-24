using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float speed = 6f;
	public LayerMask groundLayer;
	private Animation anim;

	private Vector3 direction;
	private CharacterController controller;

	void Awake (){
		anim = GetComponent <Animation>();
		controller = GetComponent <CharacterController> ();
	}

	void Update (){

		//Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked | CursorLockMode.Confined;

		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		// rotation
		RaycastHit hitInfo;
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		if(Physics.Raycast (camRay, out hitInfo,Mathf.Infinity,groundLayer)){
		
			Debug.DrawLine( transform.position, hitInfo.point-transform.position);
			Vector3 playerToMouse = hitInfo.point - transform.position;
			playerToMouse.y = 0f;
			Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);
			transform.rotation = newRotatation;
		}

		// movement
		direction.Set (h, 0, v);
		if (!controller.isGrounded){
			direction += new Vector3(0,-9.81f*Time.deltaTime,0);
		}
		direction = direction.normalized * speed * Time.deltaTime;
		controller.Move(direction);

		if (h!=0 || v!=0){
			anim.CrossFade("soldierWalk");
		}
		else{
			anim.CrossFade("soldierIdle");
		}
	}
}
