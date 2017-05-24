using UnityEngine;
using System.Collections;
using UnityEditor;

namespace HedgehogTeam.CoreShooterKit{

	[InitializeOnLoad]
	public class AutoSetUpPlayerPref{

		public static bool isDone = false;

		// Static constructor
		static AutoSetUpPlayerPref(){
			EditorApplication.projectWindowItemOnGUI += SetUpPref;
		}

		static void SetUpPref(string instanceID, Rect selectionRect){

			if (!isDone){

				if(!PlayerPrefs.HasKey("HedgehogTeam.CoreShooterKit.FriendlyFire")){
					PlayerPrefs.SetInt( "HedgehogTeam.CoreShooterKit.FriendlyFire",0);
				}

				if(!PlayerPrefs.HasKey("HedgehogTeam.CoreShooterKit.ShowLifeBar")){
					PlayerPrefs.SetInt("HedgehogTeam.CoreShooterKit.ShowLifeBar", 1);
				}

				if(!PlayerPrefs.HasKey("HedgehogTeam.CoreShooterKit.ShowCombatText")){
					PlayerPrefs.SetInt("HedgehogTeam.CoreShooterKit.ShowCombatText", 1);
				}

				if(!PlayerPrefs.HasKey("HedgehogTeam.CoreShooterKit.Score")){
					PlayerPrefs.SetInt("HedgehogTeam.CoreShooterKit.Score", 0);
				}

				if(!PlayerPrefs.HasKey("HedgehogTeam.CoreShooterKit.HighScore")){
					PlayerPrefs.SetInt("HedgehogTeam.CoreShooterKit.HighScore", 0);
				}

				PlayerPrefs.Save();

				isDone = true;

				EditorApplication.projectWindowItemOnGUI -= SetUpPref;
			}
		}
	}

}
