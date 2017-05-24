using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.CoreShooterKit{

	//[CustomEditor(typeof(Armory))]
	public class ArmoryInspector : Editor {

		public string[] unityAxes;
		SerializedProperty slots;
		SerializedProperty primaryFire;
		SerializedProperty reload;
		SerializedProperty next;
		SerializedProperty previous;

		void OnEnable(){

			primaryFire = serializedObject.FindProperty("primaryFire");
			reload = serializedObject.FindProperty("reload");
			next = serializedObject.FindProperty("next");
			previous = serializedObject.FindProperty("previous");
					
			var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
			SerializedObject obj = new SerializedObject(inputManager);
			SerializedProperty axisArray = obj.FindProperty("m_Axes");
			if (axisArray.arraySize > 0){
				unityAxes = new string[axisArray.arraySize];
				for( int i = 0; i < axisArray.arraySize; ++i ){
					var axis = axisArray.GetArrayElementAtIndex(i);
					unityAxes[i] = axis.FindPropertyRelative("m_Name").stringValue;
				}
			}

		}

		public override void OnInspectorGUI(){	

			EditorGUILayout.Space();
			serializedObject.Update();

			GUILayout.Toggle(true,"<b><size=11>" + "Launcher slots" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{
				
				slots = serializedObject.FindProperty("slots");
				slots.Next(true);
				slots.Next(true);
				//EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( slots,new GUIContent("Slot count"));
				slots =serializedObject.FindProperty("slots");
				int size = slots.arraySize;
				for (int i=0;i<size;i++){
					EditorGUI.indentLevel++;
					SerializedProperty launcher = slots.GetArrayElementAtIndex(i);
					EditorGUILayout.PropertyField(launcher,true );
					EditorGUI.indentLevel--;
				}


				/*
				slots.Next(true);
				EditorGUI.indentLevel++;
				for (int i=0;i<size;i++){
					EditorGUILayout.PropertyField(slots, new GUIContent("Slot " + i.ToString("f0")),false);

					slots.Next(true);
					slots.Next(true);
					slots.Next(true);
					slots.Next(true);
					slots.Next(true);
					slots.Next(true);
					slots.Next(true);
					slots.Next(true);
					slots.Next(true);
				}
				EditorGUI.indentLevel--;*/

			}EditorTools.EndGroup();

			GUILayout.Toggle(true,"<b><size=11>" + "Button trigger" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{

				// Fire
				int index = System.Array.IndexOf(unityAxes,primaryFire.stringValue );
				int tmpIndex = EditorGUILayout.Popup("Fire",index,unityAxes);
				if (tmpIndex != index){
					primaryFire.stringValue = unityAxes[tmpIndex];
				}

				// Reload
				index = System.Array.IndexOf(unityAxes,reload.stringValue );
				tmpIndex = EditorGUILayout.Popup("reload",index,unityAxes);
				if (tmpIndex != index){
					reload.stringValue = unityAxes[tmpIndex];
				}

				// Next
				index = System.Array.IndexOf(unityAxes,next.stringValue );
				tmpIndex = EditorGUILayout.Popup("Next",index,unityAxes);
				if (tmpIndex != index){
					next.stringValue = unityAxes[tmpIndex];
				}

				// Previous
				index = System.Array.IndexOf(unityAxes,previous.stringValue );
				tmpIndex = EditorGUILayout.Popup("Previous",index,unityAxes);
				if (tmpIndex != index){
					previous.stringValue = unityAxes[tmpIndex];
				}

			}EditorTools.EndGroup();


			serializedObject.ApplyModifiedProperties();
			if (GUI.changed && !EditorApplication.isPlaying){
				EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			}
		}
	}

}