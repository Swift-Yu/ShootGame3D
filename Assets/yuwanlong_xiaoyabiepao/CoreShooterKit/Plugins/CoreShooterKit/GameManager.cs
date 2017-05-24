/// <summary>
/// Game manager.
/// 1.0.0
/// </summary>
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HedgehogTeam;
using HedgehogTeam.EasyPoolManager;

namespace HedgehogTeam.CoreShooterKit{
	
	public class GameManager : MonoBehaviour {

		#region Members
		// GUI
		[Header("GUI")]
		public string scoreFormat = "000000";
		public Text scoreText;

		// singleton
		public static GameManager instance = null;
		public static GameManager Instance{
			get{
				if( !instance ){

					instance = FindObjectOfType( typeof( GameManager ) ) as GameManager;

					// nope, create a new one
					if( !instance ){
						GameObject obj = new GameObject( "GameManager" );
						instance = obj.AddComponent<GameManager>();

						PoolManager.Instance.transform.parent = instance.transform;
					}
				}

				return instance;
			}
		}
	
		// Score
		public bool friendlyFire  {get;private set;}
		public bool showLifeBar  {get;private set;}
		public bool showCombatText {get;private set;}
		public int score {get;private set;}
		public int highScore {get;private set;}


		// Entities
		private List<GameEntity> entities = new List<GameEntity>();
		#endregion

		#region Monobehaviour Callback
		void Awake(){
			friendlyFire = PlayerPrefX.GetBool("HedgehogTeam.CoreShooterKit.FriendlyFire",false);
			showLifeBar =  PlayerPrefX.GetBool("HedgehogTeam.CoreShooterKit.ShowLifeBar",true);
			showCombatText =  PlayerPrefX.GetBool("HedgehogTeam.CoreShooterKit.ShowCombatText",true);
			score =  PlayerPrefs.GetInt("HedgehogTeam.CoreShooterKit.Score");
			highScore =  PlayerPrefs.GetInt("HedgehogTeam.CoreShooterKit.HighScore");
		}

		void Update(){
			if (scoreText){
				scoreText.text = score.ToString(scoreFormat);
			}

		}
		#endregion

		#region Public methods
		public void RegisterEntity(GameEntity entity){
			if (!entities.Contains(entity)){
				entities.Add( entity);
			}
		}

		public void UnregisterEntity(GameEntity entity){
			entities.Remove( entity);
		}		
			
		public GameEntity[] GetEnemiesByPriority(Faction faction, TargetPriority priority, bool extendScan){

			EntityType et = EntityType.Offensive;

			switch( priority){
			case TargetPriority.Defensive:
				et = EntityType.Defensive;
				break;
			case TargetPriority.Offensive:
				et = EntityType.Offensive;
				break;
			case TargetPriority.Structure:
				et = EntityType.Structure;
				break;
			}
				
			if (priority != TargetPriority.Closest){
				if (faction){
					GameEntity[] tmplst = entities.Where( x=>faction.GetAlignement(x.faction)==-1 && (x.entityType == et) && x.gameObject.activeInHierarchy).ToArray<GameEntity>();
					if (tmplst.Length ==0 && extendScan){
						tmplst = entities.Where( x=>faction.GetAlignement(x.faction)==-1 && x.entityType!= EntityType.Neutral && x.entityType!= EntityType.NotTraceable && x.gameObject.activeInHierarchy).ToArray<GameEntity>();
					}
					return tmplst.ToArray();

				}
				else{
					return entities.Where(x => (x.entityType == et || extendScan) && x.gameObject.activeInHierarchy).ToArray<GameEntity>();
				}
			}
			else{
				if (faction){
					return entities.Where( x=>faction.GetAlignement(x.faction)==-1 && x.entityType!= EntityType.Neutral && x.entityType!= EntityType.NotTraceable && x.gameObject.activeInHierarchy).ToArray<GameEntity>();
				}
				else{
					return entities.Where( x=> x.entityType!= EntityType.Neutral && x.entityType!= EntityType.NotTraceable && x.gameObject.activeInHierarchy).ToArray<GameEntity>();
				}
			}

		}
			
		public GameEntity[] GetEntityByFaction(Faction faction ){
			return entities.Where( x=>x.faction==faction && x.gameObject.activeInHierarchy).ToArray<GameEntity>();
		}

		public void Add2Score(int point){
			score += point;
		}

		public void ResetScore(){
			score = 0;
			PlayerPrefs.SetInt( "HedgehogTeam.CoreShooterKit.Score",score);
			PlayerPrefs.Save();
		}

		public void SetHighScore(){
			if (score > highScore){
				PlayerPrefs.SetInt( "HedgehogTeam.CoreShooterKit.HighScore",score);
				highScore = score;
				PlayerPrefs.Save();
			}
		}

		public void SavePref(){
			PlayerPrefs.Save();
		}
		#endregion
	}

}
