using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace HedgehogTeam{

	[AddComponentMenu("Hedgehog Team/Helper/Random force")]
	public class RandomizeForce : MonoBehaviour {

		public ForceMode forceMode;
		public float minForce;
		public float maxForce;

		void OnSpawn(){
			GetComponent<Rigidbody>().isKinematic = false;
			GetComponent<Rigidbody>().AddRelativeForce( Vector3.forward * Random.Range(minForce,maxForce),forceMode);
		}

	}
}
