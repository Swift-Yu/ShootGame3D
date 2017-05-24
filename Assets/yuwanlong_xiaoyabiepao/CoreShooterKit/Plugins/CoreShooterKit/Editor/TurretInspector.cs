using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.CoreShooterKit{

	[CustomEditor(typeof(Turret)),CanEditMultipleObjects]
	public class TurretInspector : Editor {

		#region Members
		string[] factionLabels;
		Faction[] factions;

		SerializedProperty isAutonomous;
		SerializedProperty faction;
		SerializedProperty enableAtStart;
		SerializedProperty speed;
		SerializedProperty hubRotation;
		SerializedProperty axisRotation;
		#endregion

		int factionIndex =-1;

		void OnEnable(){
			isAutonomous = serializedObject.FindProperty("isAutonomous");
			faction = serializedObject.FindProperty("faction");
			enableAtStart = serializedObject.FindProperty("enableAtStart");
			speed  = serializedObject.FindProperty("speed");

			hubRotation = serializedObject.FindProperty("hubRotation");
			axisRotation = serializedObject.FindProperty("axisRotation");

			// Faction
			factions = CoreShooterKitEditor.GetAllFaction();
			Array.Resize<string>( ref factionLabels,factions.Length);
			for (int i=0;i<factions.Length;i++){
				factionLabels[i] = factions[i].name;
			}

			// update faction 
			for (int i=0;i<factions.Length;i++){
				if (faction.objectReferenceValue == factions[i]){
					factionIndex = i;
				}
			}

		}

		public override void OnInspectorGUI(){	

			EditorGUILayout.Space();
			serializedObject.Update();

			EditorGUILayout.PropertyField( isAutonomous, new GUIContent("Autonomous"));

			if (isAutonomous.boolValue || isAutonomous.hasMultipleDifferentValues){
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField( enableAtStart );
					
				// Faction
				if (!faction.hasMultipleDifferentValues){
					
					factionIndex = EditorGUILayout.Popup("Faction",factionIndex,factionLabels);
					if (factionIndex>-1)
						faction.objectReferenceValue  = (UnityEngine.Object)factions[factionIndex] ;
				}
				else{
					int tmpfactionIndex = EditorGUILayout.Popup("Faction",-1,factionLabels);
					if (tmpfactionIndex != factionIndex && tmpfactionIndex>-1){
						factionIndex = tmpfactionIndex;
						faction.objectReferenceValue  = (UnityEngine.Object)factions[factionIndex];
					}
				}
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();
			EditorGUILayout.PropertyField( speed);
			EditorGUILayout.PropertyField( hubRotation);
			EditorGUILayout.PropertyField( axisRotation);

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed && !EditorApplication.isPlaying){
				EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			}
		}
	}

}
