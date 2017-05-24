/// <summary>
/// Lethal entity.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;

namespace HedgehogTeam.CoreShooterKit{

	public abstract class LethalEntity : MonoBehaviour {

		#region Members
		public GameObject owner;
		public Faction faction;
		public LayerMask layerMask;

		public DamageType damageType = DamageType.Simple;
		public int damageNature = 0;
		public bool randomDamagePoint =false;
		public float damagePoint = 1;
		public float damagePointMax = 1;
		public float damageRadius = 1;

		public bool isPhysic = false;
		public float force = 100;
		public ForceMode forceMode = ForceMode.Force;

		public bool doDamageOnCollision = false;

		protected Transform cachedTransform;
		#endregion

		#region Monobehaviour Callback
		public virtual void Awake(){
			cachedTransform = transform;
		}
		#endregion

		#region Protected methods
		protected void DoDamage(GameObject target, Vector3 hitPosition, Vector3 hitNormal, bool perSecond = false){

			if (target){
				
				bool isFriendlyFire = GameManager.Instance.friendlyFire;

				switch (damageType){
				case DamageType.Simple:
					GameEntity entity = target.GetComponentInParent<GameEntity>();

					if (entity &&  entity.gameObject.activeInHierarchy &&(isFriendlyFire || ((!isFriendlyFire && faction &&  faction.GetAlignement(entity.faction)<1) || (!isFriendlyFire && faction == null ) )     ) ){
						Impact imp = Impact.GetImpact( this,target,isFriendlyFire, hitPosition,hitNormal);
						if (perSecond) imp.damagePoint *= Time.deltaTime;
						entity.ReceiveDamage( imp);
					}
					break;
				case DamageType.Area:
					
					DoAreaDamage( hitPosition,perSecond);
					break;
				}
			}
		}

		protected void DoAreaDamage(Vector3 position,bool perSecond = false){

			Collider[] hitColliders = Physics.OverlapSphere(position, damageRadius,layerMask, QueryTriggerInteraction.Ignore);

			bool isFriendlyFire = GameManager.instance.friendlyFire;

			for (int i=0;i<hitColliders.Length;i++){
				GameEntity entity = hitColliders[i].GetComponentInParent<GameEntity>();

				if (entity && (isFriendlyFire || !isFriendlyFire && faction &&  faction.GetAlignement(entity.faction)<1 && entity.gameObject.activeInHierarchy)){
					Vector3 closestPoint = hitColliders[i].ClosestPointOnBounds(position);
					float distance = Vector3.Distance( closestPoint, position );
					float damage = 1f - Mathf.Clamp01(distance/damageRadius);

					Impact imp = Impact.GetImpact( this,hitColliders[i].gameObject,isFriendlyFire, closestPoint,Vector3.zero);
					imp.damagePoint *= damage;
					if (perSecond) imp.damagePoint *= Time.deltaTime;

					entity.ReceiveDamage( imp);
				}
			}

		}
		#endregion

	}
}
