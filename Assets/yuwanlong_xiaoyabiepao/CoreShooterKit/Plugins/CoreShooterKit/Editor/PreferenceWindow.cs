using UnityEngine;
using UnityEditor;
using System.Collections;

namespace HedgehogTeam.CoreShooterKit{
	
	public class PreferenceWindow : EditorWindow {

		bool friendlyFire;
		bool showLifeBar;
		bool combatText;

		#region Member
		private static GUIStyle titleStyle;
		#endregion

		[MenuItem ("Window/Core shooter Kit/Preference",false,0)]
		static void  SpaceshipWizard(){
			EditorWindow.GetWindow<PreferenceWindow>(true,"Core Shooter Kit");
		}

		void OnEnable(){
			this.minSize = new Vector2(400,150);
			this.maxSize = new Vector2(400,150);
		}

		void OnGUI(){
			InitStyle();
			GUI.Label( new Rect(5,10,200,50),"Preferences", titleStyle);
		
			GUILayout.Space(40);
			EditorGUILayout.LabelField("Game Play", EditorStyles.boldLabel);
			friendlyFire = EditorGUILayout.Toggle("Friendly Fire", PlayerPrefX.GetBool("HedgehogTeam.CoreShooterKit.FriendlyFire",false));

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("GUI", EditorStyles.boldLabel);
			showLifeBar = EditorGUILayout.Toggle("Show life bar", PlayerPrefX.GetBool("HedgehogTeam.CoreShooterKit.ShowLifeBar",false));
			combatText = EditorGUILayout.Toggle("Show combat text", PlayerPrefX.GetBool("HedgehogTeam.CoreShooterKit.ShowCombatText",false));

			if (GUI.changed){

				PlayerPrefs.SetInt( "HedgehogTeam.CoreShooterKit.FriendlyFire",friendlyFire?1:0);
				PlayerPrefs.SetInt("HedgehogTeam.CoreShooterKit.ShowLifeBar", showLifeBar?1:0);
				PlayerPrefs.SetInt("HedgehogTeam.CoreShooterKit.ShowCombatText",combatText?1:0);

				PlayerPrefs.Save();
			}
		}

		void InitStyle(){
			if (titleStyle == null){
				titleStyle = new GUIStyle("Label");
				titleStyle.fontSize = 20;
				titleStyle.fontStyle = FontStyle.Bold;
				titleStyle.normal.textColor = new Color(0.4f,0.4f,0.4f);
			}
		}
	}

}
