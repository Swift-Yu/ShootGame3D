/// <summary>
/// Laser beam.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using HedgehogTeam.EasyPoolManager;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Weapon System/LaserBeam")]
	[RequireComponent (typeof(LineRenderer))]
	public class LaserBeam : Ammunition {

		#region Member
		public float scale=1f;

		public bool isOneshot = false;

		public bool isUvAnimated = true;
		public float animationSpeed= 10;

		public bool isOscillation = false;
		public bool isDynamicOscillation = false;
		public  int subdivisionAmount = 5;
		public float frequency = 0.05f;
		public float amplitude = 0.5f;

		private GameObject cachedImpactEffect; 

		private LineRenderer cachedLineRender;
		private bool isImpactEffect = false;
		private float uvOffset=0;
		private float realDistance;
		#endregion

		#region Monobehaviour callback
		public override void Awake (){

			base.Awake ();
			cachedLineRender = GetComponent<LineRenderer>();
			cachedLineRender.useWorldSpace = false;
		}
			
		void Update(){

			if (!isOnDespawn){
				realDistance = rayAdvance;

				// Ray
				if (Physics.SphereCast( transform.position,rayRadius, transform.forward,out hitInfo, rayAdvance, layerMask,QueryTriggerInteraction.Ignore)){

					bool perSecond = isOneshot?false:true;

					DoDamage(hitInfo.collider.gameObject, hitInfo.point, hitInfo.normal,perSecond);

					realDistance = hitInfo.distance;


					if (!isImpactEffect && impactEffect){
						cachedImpactEffect = PoolManager.Spawn( impactEffect, hitInfo.point, Quaternion.identity);
						isImpactEffect = true;
					}
					else{
						if (cachedImpactEffect){
							cachedImpactEffect.transform.position = hitInfo.point;
						}
					}
				}
				else{
					
					if (isImpactEffect){
						PoolManager.Despawn( cachedImpactEffect);
						cachedImpactEffect = null;
						isImpactEffect = false;
					}
				}

				// Update line render length
				if (!isOscillation){
					cachedLineRender.SetPosition(1,new Vector3(0,0,realDistance));
				}

				// Line render scale
				float newScale = realDistance * (scale / 10f);
				cachedLineRender.material.SetTextureScale("_MainTex", new Vector2(newScale, 1f));

				// Uv animation
				if (isUvAnimated){
					cachedLineRender.material.SetTextureOffset("_MainTex", new Vector2(Time.time * animationSpeed *-1 + uvOffset, 0f));
				}
					
			}

		}
		#endregion

		#region Pool Manager
		public override void OnSpawn(){

			base.OnSpawn ();

			isImpactEffect = false;
			uvOffset = Random.Range(0f,10f);

			if (isOscillation && subdivisionAmount>0){
				oscillationStep = rayAdvance / subdivisionAmount;
				if (!isDynamicOscillation){
					cachedLineRender.SetVertexCount(subdivisionAmount+1);
				}
				InvokeRepeating("DoOscillation",0,frequency);
			}
				
		}

		public override void OnDespawn(){

			base.OnDespawn ();
			isOnDespawn = true;

			PoolManager.Despawn( cachedImpactEffect );
			cachedImpactEffect = null;

			CancelInvoke();
		}
		#endregion

		#region private Method
		private float oscillationStep;

		private void DoOscillation(){

			cachedLineRender.SetPosition(0, Vector3.zero);

			int ptCount = subdivisionAmount;
			if (isDynamicOscillation){
				ptCount = Mathf.CeilToInt(realDistance / oscillationStep);
				cachedLineRender.SetVertexCount(ptCount+1);
			}

			cachedLineRender.SetPosition(ptCount, new Vector3(0f, 0f, realDistance));

			for (int p=1;p<ptCount;p++){
				float distance =  (realDistance / (ptCount )) * p;
				if (isDynamicOscillation) distance = oscillationStep * p;
				cachedLineRender.SetPosition(p, new Vector3(Random.Range(-amplitude, amplitude), Random.Range(-amplitude, amplitude),distance));
			}
		}
		#endregion
	}

}
