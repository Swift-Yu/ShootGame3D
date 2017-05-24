using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.CoreShooterKit{

	[CustomEditor(typeof(LaserBeam)), CanEditMultipleObjects]
	public class LaserBeamInspector : Editor {

		string[] damageLabels;

		SerializedProperty layerMask;
		SerializedProperty damageType;
		SerializedProperty damageRadius;
		SerializedProperty damageNature;
		SerializedProperty damagePoint;
		SerializedProperty isPhysic;
		SerializedProperty force;
		SerializedProperty forceMode;
		SerializedProperty impactEffect;
		SerializedProperty rayAdvance;
		SerializedProperty rayRadius;

		SerializedProperty scale;
		SerializedProperty isUvAnimated;
		SerializedProperty animationSpeed;

		SerializedProperty isOscillation;
		SerializedProperty isDynamicOscillation;
		SerializedProperty subdivisionAmount;
		SerializedProperty frequency;
		SerializedProperty amplitude;

		void OnEnable(){

			layerMask = serializedObject.FindProperty("layerMask");
			damageType = serializedObject.FindProperty("damageType");
			damageRadius = serializedObject.FindProperty("damageRadius");
			damageNature = serializedObject.FindProperty("damageNature");
			damagePoint = serializedObject.FindProperty("damagePoint");

			isPhysic = serializedObject.FindProperty("isPhysic");
			force = serializedObject.FindProperty("force");
			forceMode = serializedObject.FindProperty("forceMode");
			impactEffect = serializedObject.FindProperty("impactEffect");

			rayAdvance = serializedObject.FindProperty("rayAdvance");
			rayRadius = serializedObject.FindProperty("rayRadius");

			scale = serializedObject.FindProperty("scale");
			isUvAnimated = serializedObject.FindProperty("isUvAnimated");
			animationSpeed = serializedObject.FindProperty("animationSpeed");

			isOscillation = serializedObject.FindProperty("isOscillation");
			isDynamicOscillation = serializedObject.FindProperty("isDynamicOscillation");
			subdivisionAmount = serializedObject.FindProperty("subdivisionAmount");
			frequency = serializedObject.FindProperty("frequency");
			amplitude = serializedObject.FindProperty("amplitude");

			damageLabels = CoreShooterKitEditor.GetDamageNature().damageNatures;
		}

		public override void OnInspectorGUI (){

			serializedObject.Update();
			EditorGUILayout.Space();

			#region Properties
			GUILayout.Toggle(true,"<b><size=11>" + "Properties" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{

				EditorGUILayout.PropertyField( rayAdvance, new GUIContent("laser length"));
				EditorGUILayout.PropertyField( rayRadius, new GUIContent("Radius"));

				// Layer mash
				EditorGUILayout.PropertyField( layerMask);

			}EditorTools.EndGroup();
			#endregion

			#region Damage
			GUILayout.Toggle(true,"<b><size=11>" + "Damage" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{

				// Type
				EditorGUILayout.PropertyField( damageType, new GUIContent("Type"));
				if (damageType.enumValueIndex == 1) {
					EditorGUILayout.PropertyField(damageRadius,new GUIContent("Explosion radius"));
				}

				// Nature
				if (!damageNature.hasMultipleDifferentValues){
					damageNature.intValue = EditorGUILayout.Popup("Nature",damageNature.intValue,damageLabels);
				}
				else{
					int tmpDamageNature = EditorGUILayout.Popup("Nature",-1,damageLabels);
					if (tmpDamageNature != damageNature.intValue && tmpDamageNature>-1){
						damageNature.intValue = tmpDamageNature;
					}
				}

				EditorGUILayout.PropertyField( damagePoint,new GUIContent("Point"));

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField( isPhysic, new GUIContent("Add force at impact"));
				if (isPhysic.boolValue || isPhysic.hasMultipleDifferentValues){
					EditorGUILayout.PropertyField( force);
					EditorGUILayout.PropertyField( forceMode);
				}
					
			}EditorTools.EndGroup();
			#endregion

			#region Effect
			GUILayout.Toggle(true,"<b><size=11>" + "Effect" + "</size></b>","dragtab");
			EditorTools.BeginGroup();{
				EditorGUILayout.PropertyField( scale, new GUIContent("UV scale"));

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField( isUvAnimated, new GUIContent("UV animation"));
				if (isUvAnimated.boolValue || isUvAnimated.hasMultipleDifferentValues){
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( animationSpeed);
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField( isOscillation, new GUIContent("Oscillation"));
				if (isOscillation.boolValue || isOscillation.hasMultipleDifferentValues){
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( isDynamicOscillation, new GUIContent("Dynamic"));
					EditorGUILayout.PropertyField( subdivisionAmount);
					EditorGUILayout.PropertyField( frequency);
					EditorGUILayout.PropertyField( amplitude);
					EditorGUI.indentLevel--;
				}

				EditorGUILayout.Space();

				EditorGUILayout.PropertyField( impactEffect);
			}EditorTools.EndGroup();
			#endregion

			serializedObject.ApplyModifiedProperties();
			if (GUI.changed && !EditorApplication.isPlaying){
				EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			}
		}
	}

}
