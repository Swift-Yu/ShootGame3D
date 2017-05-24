/// <summary>
/// Radar.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Game System/Radar")]
	public class Radar : MonoBehaviour {

		#region Members
		public TargetPriority targetPriority;
		public bool isExtendScan = false;
		public float refreshRate = 0;
		public bool visible = false;
		public Fov fov = new Fov();

		private GameEntity[] radarResult;
		private float lastRefresh;
		private GameEntity oldTarget;
		#endregion

		#region Moonobehaviour Callback
		void Start(){
			lastRefresh = 0;
		}

		void OnDrawGizmos(){
			fov.DrawDebug(transform);
		}
		#endregion

		#region Public methods
		public GameEntity GetTarget(Transform tr,Faction faction, bool checkLOS = false){

			if (Time.realtimeSinceStartup - lastRefresh >= refreshRate){
				lastRefresh = Time.realtimeSinceStartup;
				radarResult = GameManager.Instance.GetEnemiesByPriority( faction, targetPriority, isExtendScan);

				float distance = Mathf.Infinity;
				int indexResult = -1;
				for (int i=0;i<radarResult.Length;i++){
					
					float localDistance = 0;

					bool inFov = false;
					if (!checkLOS){
						inFov = fov.InFov(tr, radarResult[i].transform.position, out localDistance );
					}
					else{
						inFov = fov.IsVisble( tr, radarResult[i], out localDistance);
					}
						
					if (inFov && localDistance<distance){
						
						indexResult = i;
						distance = localDistance;
					}

				}

				if (indexResult>-1){
					oldTarget = radarResult[indexResult];
					return radarResult[indexResult];
				}
				else{
					oldTarget = null;
				}
			}

			if (oldTarget == null || !oldTarget.gameObject.activeInHierarchy){
				oldTarget = null;
			}
			return oldTarget;

		}

		public GameEntity GetTargetRestricted(Transform tr,Faction faction, float maxOnTarget){

			if (Time.realtimeSinceStartup - lastRefresh >= refreshRate){
				lastRefresh = Time.realtimeSinceStartup;
				radarResult = GameManager.Instance.GetEnemiesByPriority( faction, targetPriority, isExtendScan);

				Missile[] missiles = FindObjectsOfType<Missile>();

				float distance = Mathf.Infinity;
				int indexResult = -1;
				for (int i=0;i<radarResult.Length;i++){

					float localDistance =0;
					if ( fov.InFov(tr, radarResult[i].transform.position, out localDistance  ) ){
						if (localDistance< distance){

							if (maxOnTarget>0){
								List<Missile> result = missiles.Where( x => x.target ==radarResult[i] && x.gameObject != gameObject).ToList<Missile>();
								if ( result.Count < maxOnTarget){
									indexResult = i;
									distance = localDistance;
								}
							}
							else{
								indexResult = i;
								distance = localDistance;
							}
						}
					}

				}

				if (indexResult>-1){
					return radarResult[indexResult];
				}
			}

			return null;

		}

		/*
		public GameEntity[] GetAllTarget(Transform tr,Faction faction){

			if (Time.realtimeSinceStartup - lastRefresh >= refreshRate){
				lastRefresh = Time.realtimeSinceStartup;
				radarResult = GameManager.Instance.GetEnemies( faction, targetPriority, isExtendScan);
			}

			List<GameEntity> entitiesResult = new List<GameEntity>();
			if (radarResult.Length>0){
				for (int i=0;i<radarResult.Length;i++){
					if ( fov.InFov(tr, radarResult[i].transform.position)){
						entitiesResult.Add( radarResult[i]);
					}
				}
			}

			return entitiesResult.ToArray<GameEntity>();
		}*/
		#endregion

	}

}
