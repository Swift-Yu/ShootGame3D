using UnityEngine;
using System.Collections;

namespace HedgehogTeam{

	[AddComponentMenu("Hedgehog Team/Helper/Random Torque")]
	public class RandomizeTorque : MonoBehaviour {

		public float waitTime = 0;
		public Vector3 torque = Vector3.one;
		public ForceMode forceMode = ForceMode.Force;

		void OnSpawn(){
			Invoke( "ApllyTorque",waitTime);

		}

		private void ApllyTorque(){
			Rigidbody body = GetComponent<Rigidbody>();
			if (body){
				body.isKinematic = false;
				body.AddRelativeTorque( torque,forceMode);
			}
		}
	}

}