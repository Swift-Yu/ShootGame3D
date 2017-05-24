/// <summary>
/// Auto heal.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;

namespace HedgehogTeam.CoreShooterKit{

	[AddComponentMenu("Hedgehog Team/Core Shooter Kit/Game System/Auto Heal")]
	public class AutoHeal : MonoBehaviour {

		#region Members
		public float value = 10;
		public float frenquency =1;
		public float delay2Start = 0;


		private GameEntity entity;
		private bool inHeal = false;
		private float lastStart = 0;
		#endregion

		#region Monobehaviour Callback
		void Awake(){
			entity = GetComponentInChildren<GameEntity>();

		}

		void Update () {
			if (entity){
				if (entity.life< entity.maxLife && !inHeal && (Time.realtimeSinceStartup - lastStart)>= frenquency){
					inHeal = true;
					InvokeRepeating( "Heal",delay2Start,frenquency );
				}

				if (inHeal && entity.life == entity.maxLife ){
					CancelInvoke();
					inHeal = false;
					lastStart = Time.realtimeSinceStartup;
				}
					
			}
		}
		#endregion

		#region Private methods
		void Heal(){
			entity.LifeRecovery(value);
		}
		#endregion
	}
}
