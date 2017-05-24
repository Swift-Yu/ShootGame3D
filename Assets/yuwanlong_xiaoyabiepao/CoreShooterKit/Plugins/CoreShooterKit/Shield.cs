using UnityEngine;
using System.Collections;
using HedgehogTeam.CoreShooterKit;

namespace HedgehogTeam.CoreShooterKit{
	
	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Game System/Shield")]
	public class Shield : MonoBehaviour {

		[Range (0,1)]
		public float mitigation=0.5f;
		public GameEntity protectedEntity;
		private GameEntity entity;

		void Awake(){
			if (mitigation<=0) mitigation =1;
			entity = GetComponent<GameEntity>();
			entity.onHit.AddListener( DamageMitigation);
		}
			
		private void DamageMitigation(Impact impact) {

			if (mitigation>0){
				impact.damagePoint *= (1-mitigation);
				impact.target = protectedEntity.gameObject;
				protectedEntity.ReceiveDamage( impact);
			}
		}
	}
}
