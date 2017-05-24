using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace HedgehogTeam.EasyPoolManager{

	[System.Serializable]
	public class PoolManager : MonoBehaviour{

		#region Member
		// Singleton
		static PoolManager instance = null;
		public static PoolManager Instance{
			get{
				if( !instance ){
					// check if an ObjectPoolManager is already available in the scene graph
					instance = FindObjectOfType( typeof( PoolManager ) ) as PoolManager;
					
					// nope, create a new one
					if( !instance ){
						GameObject obj = new GameObject( "PoolManager" );
						instance = obj.AddComponent<PoolManager>();
					}
				}
				
				return instance;
			}
		}

		public bool dontDestroyOnLoad = false;
		public bool isGarbageCollector = false;
		public float garbageCycletime = 5;

		// static pool object
		[SerializeField]
		public List<PoolProperty> poolObjectsProperties = new List<PoolProperty>();

		// Pool
		[SerializeField]
		private List<GameObject> _keys = new List<GameObject>();
		[SerializeField]
		private List<Pool> _values = new List<Pool>();

		// internal use
		private Pool tmpPool;
		
		#endregion

		#region MonoBehaviour Callback
		void Awake(){
			if (dontDestroyOnLoad){
				DontDestroyOnLoad(transform.gameObject);
			}

		}
			
		void Start(){

			if (isGarbageCollector){
				InvokeRepeating( "_GarbageCollector",garbageCycletime,garbageCycletime);
			}

			for (int i=0;i<poolObjectsProperties.Count;i++){
				if (poolObjectsProperties[i].preLoadAtStart){
					_CreatePool( poolObjectsProperties[i].obj, poolObjectsProperties[i].poolAmount );
				}
			}
		}
		#endregion
		
		#region Private methods
		private bool _CreatePool(GameObject obj,int amount){

			if (!_keys.Contains( obj) && obj){
				Pool tmpPooled = new Pool();
				for(int i=0;i<amount;i++){
					GameObject newObj = Instantiate( obj.gameObject);
					newObj.transform.parent = transform;
					newObj.SetActive( false);
					tmpPooled.pooledObjects.Add( newObj);
					tmpPooled.haveDespawn.Add(obj.GetComponent<DespawnObject>());
					tmpPooled.objectsLastUse.Add( Time.realtimeSinceStartup);
		
				}
				_keys.Add(obj );
				_values.Add(tmpPooled );

				return true;
			}
			else{
				return false;
			}

		}

		private List<GameObject> _GetPool( GameObject obj){
			int index = FindPoolObjectIndex( obj);
			if ( index>-1){
				return _values[index].pooledObjects;
			}
			else{
				return null;
			}
		}

		private bool _DestroyPool(GameObject obj, bool editor=false){
			if (_keys.Contains( obj)){
				int index = FindPoolObjectIndex( obj);
				tmpPool = _values[index];
				for (int i=0;i<tmpPool.pooledObjects.Count;i++){
					if (editor){
						DestroyImmediate( tmpPool.pooledObjects[i]);
					}
					else{
						Destroy( tmpPool.pooledObjects[i]);
					}
				}
				tmpPool = null;
				_values[index]=null;
				_values.RemoveAt(index);
				_keys.RemoveAt(index);
				return true;
			}
			else{
				return false;
			}
		}

		private GameObject _Spawn(GameObject obj, int amount=1, bool allowGrowth=true, int limit=-1){

			// Automatic pool creation at spawn
			if (!_keys.Contains( obj) ){
				int indexPooled = FindPoolObjectIndex( obj);
				if ( indexPooled==-1){
					PoolProperty pooledObj = new PoolProperty();
					pooledObj.allowGrowth = allowGrowth;
					pooledObj.obj = obj;
					if (limit>-1){
						pooledObj.limitGrowth = true;
						pooledObj.limit = limit;
					}
					else{
						pooledObj.limitGrowth = false;
					}
					poolObjectsProperties.Add( pooledObj);
				}
				_CreatePool(obj, amount);
			}

		
			// Get Pool
			int index = FindPoolObjectIndex( obj);
			if ( index>-1){
				tmpPool = _values[index];
			
				// Get first available obj
				for (int i=0;i<tmpPool.pooledObjects.Count;i++){
					if ( !tmpPool.pooledObjects[i].activeInHierarchy && (( !tmpPool.haveDespawn[i] ) || (tmpPool.haveDespawn[i] && !tmpPool.pooledObjects[i].GetComponent<DespawnObject>().HaveActiveChild() )) ){
						tmpPool.objectsLastUse[i] = Time.realtimeSinceStartup;
						return tmpPool.pooledObjects[i];
					}
				}

			

				// No object available, but allows growth
				int propertyIndex = FindPooledObjectOptionIndex( obj);
				if (propertyIndex>-1){
					
					if ((poolObjectsProperties[propertyIndex].allowGrowth && !poolObjectsProperties[propertyIndex].limitGrowth) ||
					    (poolObjectsProperties[propertyIndex].allowGrowth && poolObjectsProperties[propertyIndex].limitGrowth && _values[index].pooledObjects.Count<=poolObjectsProperties[propertyIndex].limit)){
						
						GameObject newObj = Instantiate( obj.gameObject);
						newObj.transform.parent = transform;
						newObj.SetActive( false);
						
						_values[index].pooledObjects.Add( newObj);
						_values[index].haveDespawn.Add(newObj.GetComponent<DespawnObject>());
						_values[index].objectsLastUse.Add(Time.realtimeSinceStartup);
						return newObj; 
					}
				}
			}

			return null;
		}
		
		private GameObject _Instantiate(GameObject obj,Vector3 position,Quaternion rotation,int amount = 2, bool allowGrowth=true, int limit=-1 ){
			
			GameObject newObj = _Spawn( obj,amount,allowGrowth,limit);
			if (newObj){
				newObj.transform.position = position;
				newObj.transform.rotation = rotation;
				//newObj.BroadcastMessage("OnSpawnStart", SendMessageOptions.DontRequireReceiver);
				newObj.SetActive( true);
				newObj.BroadcastMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
			}
			
			return newObj;
		}

		private GameObject _StartSpawn(GameObject obj,Vector3 position,Quaternion rotation,int amount = 2, bool allowGrowth=true, int limit=-1 ){

			GameObject newObj = _Spawn( obj,amount,allowGrowth,limit);
			if (newObj){
				newObj.transform.position = position;
				newObj.transform.rotation = rotation;
				//newObj.BroadcastMessage("OnSpawnStart", SendMessageOptions.DontRequireReceiver);
			}
			
			return newObj;
		}

		private int FindPoolObjectIndex( GameObject obj){

			int result = _keys.FindIndex(
				delegate(GameObject o) {
				return o == obj;
			}
			);

			return result;
		}

		private int FindPooledObjectOptionIndex(GameObject obj){


			int result = poolObjectsProperties.FindIndex(
				delegate(PoolProperty o) {
				return o.obj == obj;
			}
			);

			return result;
		}	


		private void _GarbageCollector(){
			float inactivity=-1;
			float localCycle = inactivity<0?garbageCycletime : inactivity;

			for (int o=0;o<_keys.Count;o++){
				tmpPool = _values[o];
				for (int p=0;p<tmpPool.pooledObjects.Count;p++){
					
					if (Time.realtimeSinceStartup - tmpPool.objectsLastUse[p] > localCycle &&  !tmpPool.pooledObjects[p].activeSelf){
						Destroy( tmpPool.pooledObjects[p]);
						tmpPool.pooledObjects[p] = null;
						tmpPool.pooledObjects.RemoveAt( p);
						tmpPool.haveDespawn.RemoveAt( p);
						tmpPool.objectsLastUse.RemoveAt( p);
						p--;
					}
				}
			}
			
		}

		private void _DespawnObject( GameObject obj){
			obj.BroadcastMessage( "OnDespawn", SendMessageOptions.DontRequireReceiver);
			if (!obj.GetComponent<DespawnObject>()){
				obj.SetActive(false);
			}
			//obj.SetActive(false);
			//StartCoroutine(_internalDespawnObject(obj));
		}
		
		IEnumerator _internalDespawnObject( GameObject obj){
			yield return new WaitForEndOfFrame();
			//obj.BroadcastMessage( "OnDespawn", SendMessageOptions.DontRequireReceiver);
			//obj.SetActive(false);
		}
		#endregion

		public static PoolManager Create(){
			return Instance;
		}

		public static void CreatePool(GameObject obj, int amount){
			Instance._CreatePool(obj, amount );
		}

		public static GameObject[] GetPool(GameObject obj){
			List<GameObject> objs = Instance._GetPool( obj);
			if (objs!=null){
				return objs.ToArray();
			}
			else{
				GameObject[] empty = new GameObject[0];
				return empty;
			}
		}

		public static int GetObjectCountInPool(GameObject obj){
			List<GameObject> objs = Instance._GetPool( obj);
			if (objs != null){
				return objs.Count;
			}
			else{
				return -1;
			}
		}

		public static int GetActiveObjectCountInPool(GameObject obj){
			List<GameObject> objs = Instance._GetPool( obj);
			if (objs!=null){
				int countActive=0;
				foreach( GameObject objAc in objs){
					if (objAc.activeInHierarchy){
						countActive++;
					}
				}
				return countActive;
			}
			else{
				return -1;
			}
		}

		public static int GetPoolCount(){
			return Instance._keys.Count;
		}

		public static void DespawnPool( GameObject obj){
			List<GameObject> objs = Instance._GetPool( obj);
			if (objs != null){
				for(int i=0;i<objs.Count;i++){
					Despawn( objs[i].gameObject);
				}
			}
		}

		public static GameObject[] GetPooledObjects(){
			return Instance._keys.ToArray();
		}

		public static bool DestroyPool(GameObject obj, bool editor=false ){
			if (obj){
				return Instance._DestroyPool( obj, editor);
			}
			else{
				return false;
			}
		}

		public static void GarbageCollector(float inactivity){
			//Instance._GarbageCollector( inactivity);

		}

		public static GameObject Spawn(GameObject obj,Vector3 position,Quaternion rotation,int amount = 1, bool allowGrowth=true, int limit=-1){

			return Instance._Instantiate( obj,position,rotation,amount,allowGrowth,limit);
			
		}
		
		public static GameObject SpawnStart(GameObject obj,Vector3 position,Quaternion rotation,int amount = 2, bool allowGrowth=true, int limit=-1){
			return Instance._StartSpawn( obj,position,rotation,amount,allowGrowth,limit);
		}
		
		public static void SpawnEnd(GameObject obj){
			
			obj.SetActive( true);
			obj.BroadcastMessage("OnSpawn", SendMessageOptions.DontRequireReceiver);
			
		}
		
		public static void Despawn( GameObject obj){
			if (obj){
				Instance._DespawnObject(obj);
			}
		}
	}
}
 