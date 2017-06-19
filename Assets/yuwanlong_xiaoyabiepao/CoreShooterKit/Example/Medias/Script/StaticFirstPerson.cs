using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Characters.FirstPerson{

	public class StaticFirstPerson : MonoBehaviour {

		public MouseLook mouseLook;
	    //[SerializeField] private GyroController gyo;
        [SerializeField]
		private Camera cam;

		void Start(){
			mouseLook.Init( transform, cam.transform);
            //gyo.EnableGyro();
		}

		void Update(){
			mouseLook.UpdateCursorLock();
			mouseLook.LookRotation (transform, cam.transform);
		}
	}

}