using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.CoreShooterKit{

	[CustomEditor(typeof(Spawner)), CanEditMultipleObjects]
	public class SpawnerInspector : Editor {

		SerializedProperty launchAtStart;
		SerializedProperty usePoolManager;
		SerializedProperty isChild;
		SerializedProperty spawnShape;
		SerializedProperty spawnRadius;
		SerializedProperty entities;

		SerializedProperty simultanemous;
		SerializedProperty frequency;
		SerializedProperty amount;

		SerializedProperty waveAmount;
		SerializedProperty reSpawnType;
		SerializedProperty waitingTime;

		SerializedProperty onSpawnerStart;
		SerializedProperty onSpawnerEnd;
		SerializedProperty onWaveStart;
		SerializedProperty onWaveKill;
		SerializedProperty onSpawnEntity;
		SerializedProperty onWaveEnd;
		SerializedProperty startTime;
		SerializedProperty managedByPool;

		void OnEnable(){

			isChild = serializedObject.FindProperty("isChild");
			launchAtStart = serializedObject.FindProperty("launchAtStart");
			startTime = serializedObject.FindProperty("startTime");
			usePoolManager = serializedObject.FindProperty("usePoolManager");
			spawnShape = serializedObject.FindProperty("spawnShape");
			spawnRadius = serializedObject.FindProperty("spawnRadius");
			entities = serializedObject.FindProperty("entities");
			managedByPool = serializedObject.FindProperty("managedByPool");
			frequency = serializedObject.FindProperty("frequency");
			amount = serializedObject.FindProperty("amount");
			waitingTime= serializedObject.FindProperty("waitingTime");
			reSpawnType = serializedObject.FindProperty("reSpawnType");
			waveAmount = serializedObject.FindProperty("waveAmount");
			simultanemous =  serializedObject.FindProperty("simultanemous");

			onSpawnerStart = serializedObject.FindProperty("onSpawnerStart");
			onSpawnerEnd = serializedObject.FindProperty("onSpawnerEnd");
			onWaveStart = serializedObject.FindProperty("onWaveStart");
			onWaveKill = serializedObject.FindProperty("onWaveKill");
			onSpawnEntity = serializedObject.FindProperty("onSpawnEntity");
			onWaveEnd = serializedObject.FindProperty("onWaveEnd");
		}
			
		public override void OnInspectorGUI (){

			serializedObject.Update();
			EditorGUILayout.Space();

			#region Properties
			GUILayout.Toggle(true,"<b><size=11>" + "Properties" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{
				EditorGUILayout.PropertyField(launchAtStart);
				EditorGUILayout.PropertyField(startTime);
				EditorGUILayout.PropertyField(spawnShape);
				EditorGUILayout.PropertyField(spawnRadius);
				EditorGUILayout.PropertyField(managedByPool);
			}EditorTools.EndGroup();
			#endregion

			#region Wave
			GUILayout.Toggle(true,"<b><size=11>" + "Waves" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{
				EditorGUILayout.PropertyField(usePoolManager);
				EditorGUILayout.PropertyField(isChild, new GUIContent("Child"));
				EditorGUILayout.Space();
				EditorGUI.indentLevel++;
				EditorGUILayout.PropertyField(entities,true);
				EditorGUI.indentLevel--;
				EditorGUILayout.Space();

				SpawnPropertiesInspector(amount);
				EditorGUILayout.Space();
				SpawnPropertiesInspector(simultanemous);
				EditorGUILayout.Space();
				SpawnPropertiesInspector(frequency);

				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(waveAmount);
				EditorGUILayout.PropertyField(reSpawnType);
				SpawnPropertiesInspector(waitingTime);
			
			}EditorTools.EndGroup();
			#endregion

			#region Events
			GUILayout.Toggle(true,"<b><size=11>" + "Events" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{
			EditorGUILayout.PropertyField(onSpawnerStart,true);
			EditorGUILayout.PropertyField(onSpawnerEnd,true);
			EditorGUILayout.PropertyField(onWaveStart,true);
			EditorGUILayout.PropertyField(onWaveEnd,true);
			EditorGUILayout.PropertyField(onWaveKill,true);
			EditorGUILayout.PropertyField(onSpawnEntity,true);
			}EditorTools.EndGroup();
			#endregion

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed && !EditorApplication.isPlaying){
				EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			}
		}
	
		private void SpawnPropertiesInspector( SerializedProperty obj){

			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(obj);
			if (obj.isExpanded){

				SerializedProperty spawnType = obj.FindPropertyRelative("spawnType" );
				SerializedProperty minValue = obj.FindPropertyRelative("minValue" );
				SerializedProperty maxValue = obj.FindPropertyRelative("maxValue" );
				SerializedProperty progressiveValue = obj.FindPropertyRelative("progressiveValue" );

				EditorGUILayout.PropertyField(spawnType);

				switch (spawnType.enumValueIndex){
				case 0:
					EditorGUILayout.PropertyField(minValue, new GUIContent("Value"));
					break;
				case 1:
					EditorGUILayout.PropertyField(minValue, new GUIContent("Min Value"));
					EditorGUILayout.PropertyField(maxValue, new GUIContent("Max Value"));
					break;
				case 2:
					EditorGUILayout.PropertyField(minValue, new GUIContent("Start Value"));
					EditorGUILayout.PropertyField(progressiveValue, new GUIContent("Add Value"));
					break;
				}
			}


			EditorGUI.indentLevel--;
		}
	}


}