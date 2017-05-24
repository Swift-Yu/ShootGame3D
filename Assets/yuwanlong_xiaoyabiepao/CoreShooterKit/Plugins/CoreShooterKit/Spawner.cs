/// <summary>
/// Spawner.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using HedgehogTeam.EasyPoolManager;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HedgehogTeam.CoreShooterKit{
	
	public class Spawner : MonoBehaviour {

		#region internal classes
		[System.Serializable]
		public class SpawnProperties{
			public SpawnType spawnType;
			public float minValue = 1;
			public float maxValue = 1;
			public float progressiveValue = 1;

			private float oldValue;

			public float Init(bool first=false){

				float value =0;

				switch (spawnType){
				case SpawnType.Fixe:
					value= minValue;
					break;
				case SpawnType.Progressive:
					if (first){
						value = minValue;
					}
					else{
						value = oldValue + progressiveValue;
					}
					break;
				case SpawnType.Random:
					value = Random.Range( minValue, maxValue);
					break;
				}

				oldValue = value;
				return value;
			}
		}

		private class Wave{
			public int spawnCount;
			public float waitTime;
			public int count;
			public bool endWave=false;
			public List<GameObject> entities = new List<GameObject>();
		}
		#endregion

		#region Unity events
		[System.Serializable] public class OnSpawnerStart : UnityEvent{};
		[System.Serializable] public class OnSpawnerEnd : UnityEvent{};

		[System.Serializable] public class OnWaveStart : UnityEvent{};
		[System.Serializable] public class OnWaveKill : UnityEvent{};
		[System.Serializable] public class OnWaveEnd : UnityEvent{};

		[System.Serializable] public class OnSpawnEntity : UnityEvent<GameObject>{};

		[SerializeField] public OnSpawnerStart onSpawnerStart;
		[SerializeField] public OnSpawnerEnd onSpawnerEnd;

		[SerializeField] public OnWaveStart onWaveStart;
		[SerializeField] public OnWaveKill onWaveKill;
		[SerializeField] public OnWaveEnd onWaveEnd;

		[SerializeField] public OnSpawnEntity onSpawnEntity;
	
		#endregion

		#region Members
		public bool launchAtStart = true;
		public float startTime = 0;
		public bool managedByPool = false;
		public SpawnShape spawnShape;
		public float spawnRadius = 0;
		public List<GameObject> entities = new List<GameObject>();

		public bool usePoolManager;
		public bool isChild = false;
		public SpawnProperties frequency = new SpawnProperties();
		public SpawnProperties simultanemous = new SpawnProperties(); 
		public SpawnProperties amount = new SpawnProperties();

		public int waveAmount = 1;
		public ReSpawnType reSpawnType;
		public SpawnProperties waitingTime = new SpawnProperties();


		private float desiredSpawnCount=0;
		private int waveCount =0;

		private List<Wave> waves = new List<Wave>();
		private bool isStarting = false;
		private bool firstWave = true;
		#endregion

		#region Monobehaviour Callback
		void Start(){

			if (!managedByPool){
				if (launchAtStart){
					StartSpawner();
				}
			}
		}

		void OnEnable(){
			if (managedByPool){
				if (launchAtStart){
					StartSpawner();
				}
			}
		}

		void Update(){

			if (isStarting){
				for (int i=0;i<waves.Count;i++){
					if (waves[i].endWave){
						// Check entities are living
						for( int j=0;j<waves[i].entities.Count;j++){
							if (waves[i].entities[j] == null || (usePoolManager && !waves[i].entities[j].activeInHierarchy )){
								waves[i].entities.RemoveAt(j);
							}
						}

						if (waves[i].entities.Count == 0){
							waves[i].endWave = false;
							if (waveCount< waveAmount){
								if (reSpawnType == ReSpawnType.AllKill){
									Invoke("LaunchWave",waves[waveCount-1].waitTime);
								}
							}

							onWaveKill.Invoke();

							if (waveCount == waveAmount){
								onSpawnerEnd.Invoke();
								waves.Clear();
								isStarting = false;
							}
						}
					}
				}
			}
		}

		void OnDrawGizmos(){

			#if UNITY_EDITOR
			switch( spawnShape){
			case SpawnShape.OnSphere:
			case SpawnShape.InSphere:
				Handles.DrawWireArc( transform.position, Vector3.up,transform.forward, 360, spawnRadius);
				Handles.DrawWireArc( transform.position, Vector3.left,transform.forward, 360, spawnRadius);
				break;
			case SpawnShape.InCircle:
			case SpawnShape.OnCircle:
				Handles.DrawWireArc( transform.position, Vector3.up,transform.forward, 360, spawnRadius);
				break;
			}
			#endif
		}
		#endregion

		#region Pool Manager
		void OnDespawn(){
			CancelInvoke();
		}
		#endregion

		#region Private methods
		private void SpawnEntity(){

			float spawnCount = simultanemous.Init(firstWave);

			if ( waves[waveCount-1].spawnCount + spawnCount > waves[waveCount-1].count){
				spawnCount =  waves[waveCount-1].count - waves[waveCount-1].spawnCount;
			}


			for (int i=0 ;i<spawnCount ; i++){

				Vector3 position = transform.position;
				// Spawn shape
				switch (spawnShape){
				case SpawnShape.OnSphere:
					position += Random.onUnitSphere * spawnRadius;
					break;
				case SpawnShape.InSphere:
					position += Random.insideUnitSphere * spawnRadius;
					break;
				case SpawnShape.InCircle:
					position += (Vector3)Random.insideUnitCircle * spawnRadius;
					break;
				case SpawnShape.OnCircle:
					float angle = Random.Range(0,360) * Mathf.Deg2Rad;
					position.x += Mathf.Cos(angle) * spawnRadius;
					position.z += Mathf.Sin(angle) *spawnRadius;
					break;
				}

				// spawn entitie
				GameObject objInstance = null;
				GameObject tmp = entities[Random.Range(0,entities.Count)];
				if (usePoolManager){
					
					objInstance = PoolManager.Spawn( tmp, position,transform.rotation, (int)desiredSpawnCount);
				}
				else{
					objInstance = (GameObject)Instantiate( tmp,position,transform.rotation);
				}
					
				if (isChild){
					objInstance.transform.parent = transform;
				}
				onSpawnEntity.Invoke( objInstance);
				waves[waveCount-1].entities.Add( objInstance);
				waves[waveCount-1].spawnCount++;
			}

			// check end wave
			if( waves[waveCount-1].spawnCount == waves[waveCount-1].count){
				waves[waveCount-1].endWave = true;
				onWaveEnd.Invoke();
				CancelInvoke();

				if (waveCount< waveAmount){
					if (reSpawnType == ReSpawnType.EndWave){
						Invoke("LaunchWave",waves[waveCount-1].waitTime);
					}
				}
			}
				
			// Update frenquecy for random
			if (frequency.spawnType == SpawnType.Random){
				CancelInvoke();
				InvokeRepeating( "SpawnEntity",frequency.Init(),0);
			}
				
		}
			
		private void LaunchWave(){
			
			Wave wave = new Wave();
			wave.endWave = false;
			wave.spawnCount = 0;
			wave.count = (int)amount.Init( firstWave);
			wave.waitTime = waitingTime.Init(firstWave);

			waves.Add(wave );

			waveCount++;
			onWaveStart.Invoke();

			float repeatingTime = frequency.Init( firstWave);
			if (repeatingTime > 0){
				InvokeRepeating( "SpawnEntity",0,repeatingTime);
			}
			else{
				onSpawnerEnd.Invoke();
				waves.Clear();
				isStarting = false;
			}

			firstWave = false;
		}
			
		public void StartSpawner(){
			if (!isStarting && waveAmount>0){
				firstWave = true;
				isStarting = true;
				waveCount = 0;
				Invoke( "LaunchWave", startTime);
				onSpawnerStart.Invoke();
			}
		}
		#endregion
	}


}

