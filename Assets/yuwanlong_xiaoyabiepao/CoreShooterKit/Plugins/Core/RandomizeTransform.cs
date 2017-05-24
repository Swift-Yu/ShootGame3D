using UnityEngine;
using System.Collections;

namespace HedgehogTeam{

	[AddComponentMenu("Hedgehog Team/Helper/Random Transform")]
	public class RandomizeTransform : MonoBehaviour {

		public bool isRandomScale = true;
		public bool isUniformScale = false;
		public Vector3 minScale = Vector3.one;
		public Vector3 maxScale = Vector3.one;


		public bool isRandomRotation = true;
		public Vector3 minRotation = Vector3.zero;
		public Vector3 maxRotation = Vector3.zero;

		private Vector3 defaultScale;
		private Vector3 defaultEulerRotation;

		void Awake(){
			defaultScale = transform.localScale;
			defaultEulerRotation = transform.rotation.eulerAngles;
		}

		void OnSpawn(){
			if (isRandomScale){
				if (!isUniformScale){
					transform.localScale = new Vector3( defaultScale.x * Random.Range(minScale.x, maxScale.x), defaultScale.y * Random.Range(minScale.y, maxScale.y),defaultScale.z* Random.Range(minScale.z, maxScale.z));
				}
				else{
					float scale = Random.Range(minScale.x, maxScale.x);
					transform.localScale = new Vector3( defaultScale.x * scale, defaultScale.y * scale,defaultScale.z* scale);
				}

			}
			
			if(isRandomRotation){
				transform.rotation *= Quaternion.Euler(new Vector3(defaultEulerRotation.x + Random.Range(minRotation.x, maxRotation.x), defaultEulerRotation.y+ Random.Range(minRotation.y, maxRotation.y),defaultEulerRotation.z + Random.Range(minRotation.z, maxRotation.z) ) );
			}
		}

	}

}
