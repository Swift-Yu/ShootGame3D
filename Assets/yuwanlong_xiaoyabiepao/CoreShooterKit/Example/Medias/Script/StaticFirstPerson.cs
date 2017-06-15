using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.FirstPerson{

	public class StaticFirstPerson : MonoBehaviour {

		public MouseLook mouseLook;

		private Camera cam;

		void Start(){
			cam = Camera.main;
			mouseLook.Init( transform, cam.transform);
		}

		void Update(){
			mouseLook.UpdateCursorLock();
			mouseLook.LookRotation (transform, cam.transform);
		}
	}

}