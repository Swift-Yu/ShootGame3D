using UnityEngine;
using System.Collections;
using UnityEditor;

namespace HedgehogTeam.CoreShooterKit{
	
	[CustomEditor(typeof(Faction))]
	public class FactionInspector : Editor {

		public override void OnInspectorGUI(){	

			Faction t = (Faction)target;

			t.isNeutral = EditorGUILayout.ToggleLeft("Neutral faction",t.isNeutral);
			if (!t.isNeutral){

				EditorGUILayout.Space();

				t.unknow =(FactionAlignment) EditorGUILayout.EnumPopup("Unknow faction",t.unknow);

				EditorGUILayout.Space();

				serializedObject.Update();
				SerializedProperty allies = serializedObject.FindProperty("allies");
				EditorGUILayout.PropertyField(allies, true, null);
				serializedObject.ApplyModifiedProperties();

				EditorGUILayout.Space();

				serializedObject.Update();
				SerializedProperty enemies = serializedObject.FindProperty("enemies");
				EditorGUILayout.PropertyField(enemies, true, null);
				serializedObject.ApplyModifiedProperties();

			}

		}
	}

}