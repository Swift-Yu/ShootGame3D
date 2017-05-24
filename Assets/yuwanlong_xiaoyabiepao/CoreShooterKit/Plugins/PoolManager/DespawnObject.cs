using UnityEngine;
using System.Collections;

namespace HedgehogTeam.EasyPoolManager{
	[AddComponentMenu("Hedgehog Team/Easy Pool Manager/Despawn Object")]
	public class DespawnObject : MonoBehaviour {

		public float lifeTime = -1;
		public float despawnDelay =0;
		public bool forceDespawn = true;
		public bool isChild = false;
		[HideInInspector]
		public Transform dynamicParent;

		// Transform data
		private Transform parent;
		private Vector3 localPosition;

		// Rigidbody
		private Rigidbody rigidBody;

		// Particle System
		private ParticleSystem pSystem;
		private bool pPlayAwake = false;

		private TrailRenderer trail;
		private float trailTime;

		// Audio source
		private AudioSource audioSource;
		private bool audioPlayAwake;
		private bool audioLoop;

		private bool waitDespawn = false;
		private DespawnObject[] childsPooledObjects;

		private bool isAwake = false;
		private float despawnTime= 0;


		void Awake(){
			DoAwake();
		}
		
		void OnSpawn(){

			if (lifeTime>0 && !isChild){
				Invoke("Despawn",lifeTime);
			}

			// Physic
			if (rigidBody) rigidBody.isKinematic = false;

			// Particle
			if (pSystem && pPlayAwake) pSystem.Play();
			
			// Audio
			if (audioSource && audioPlayAwake){
				audioSource.Play();
			}

			if (!isChild){
				foreach (Transform child in transform) {
					child.gameObject.SetActive (true); 
				}
			}

			if (trail){
				StartCoroutine( ResetTrail());
			}
		}

		void OnDisable(){
			CancelInvoke();
		}

		private int frameCount =0;
		void Update(){

			if (waitDespawn){

				bool despawn = true;
				despawnTime += Time.deltaTime;

				if (!forceDespawn || isChild){
					// Particle system
					if (pSystem && pSystem.IsAlive()) despawn = false;
					// AudioSource
					if (audioSource && audioSource.isPlaying && !audioLoop) despawn = false;

					if (trail && despawnTime <= trailTime){
						despawn = false;
					}
						
					if (trail &&  despawnTime > trailTime && frameCount<25){
						trail.time = 0;
						despawn = false;
						frameCount++;
					}
				}

				// Real despawn
				if (despawn){
					waitDespawn = false;

					if (trail){trail.time = 0;}
											
					// Desactivated 
					if (rigidBody) rigidBody.isKinematic = true;

					gameObject.SetActive(false);

					if (isChild){
						transform.parent = parent;
						transform.localPosition = localPosition;
					}
				}

			}

			if (dynamicParent){
				transform.position = dynamicParent.position;
				transform.rotation = dynamicParent.rotation;
			}
		}

		void Despawn(){
			BroadcastMessage( "OnDespawn", SendMessageOptions.DontRequireReceiver);
		}

		void OnDespawn(){
			StartCoroutine( WaitDespawn());
		}

		IEnumerator WaitDespawn(){
			yield return new WaitForSeconds(despawnDelay);


			waitDespawn = true;
			
			// Stop physic
			if (rigidBody) rigidBody.isKinematic = true;
			// Stop particl
			if (pSystem) pSystem.Stop(true);


			// un parent
			if (isChild){
				transform.parent = null;
				frameCount =0;
			}

			despawnTime = 0;//Time.realtimeSinceStartup;
		}

		private void DoAwake(){
			if (!isAwake){
				parent = transform.parent;
				localPosition = transform.localPosition;
				
				rigidBody = GetComponent<Rigidbody>();
				
				pSystem = GetComponent<ParticleSystem>();
				if (pSystem){
					pPlayAwake = pSystem.playOnAwake;
				}

				trail = GetComponent<TrailRenderer>();
				if (trail){
					trailTime = trail.time;
				}

				audioSource = GetComponent<AudioSource>();
				if (audioSource){
					audioPlayAwake = audioSource.playOnAwake;
					audioLoop = audioSource.loop;
				}
				
				if (!isChild){
					childsPooledObjects = GetComponentsInChildren<DespawnObject>();
				}

				isAwake = true;
			}
		}

		public bool HaveActiveChild(){

			if (!isAwake) DoAwake();

			for (int i=0;i<childsPooledObjects.Length;i++){
				if (childsPooledObjects[i].gameObject.activeInHierarchy){
					return true;
				}
			}
			return false;

		}
	
		private IEnumerator ResetTrail(){
			trail.time =0;
			yield return 0;
			trail.time = trailTime;
		}
	}
}
