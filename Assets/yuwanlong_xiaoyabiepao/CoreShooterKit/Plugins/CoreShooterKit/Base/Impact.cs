using UnityEngine;
using System.Collections;

namespace HedgehogTeam.CoreShooterKit{

	public class Impact{

		public GameObject owner;
		public DamageType damageType;
		public GameObject target;

		public Faction faction;
		public int damageNature;
		public float damagePoint;

		public Vector3 hitPoint;
		public Vector3 hitNormal;

		public bool isPhysic = false;
		public float force;
		public ForceMode forceMode = ForceMode.Force;
		public float damageRadius;

		public static Impact GetImpact(LethalEntity entity, GameObject target,bool friendlyFire, Vector3 hitPoint, Vector3 hitNormal){

			Impact imp = new Impact();

			imp.owner = entity.owner;
			imp.target = target;
			imp.hitPoint = hitPoint;
			imp.hitNormal = hitNormal;

			imp.damageType = entity.damageType;
			imp.faction = entity.faction;
			imp.damageNature = entity.damageNature;

			if (entity.randomDamagePoint){
				imp.damagePoint = Random.Range( entity.damagePoint, entity.damagePointMax);
			}
			else{
				imp.damagePoint = entity.damagePoint;
			}

			imp.damageRadius = entity.damageRadius;

			imp.isPhysic = entity.isPhysic;
			imp.force = entity.force;
			imp.forceMode = entity.forceMode;

			return imp;
		}
	}

}
