using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;
using System.ComponentModel;

namespace HedgehogTeam.CoreShooterKit{

	[CustomEditor(typeof(Missile)), CanEditMultipleObjects]
	public class MissileInspector : Editor {

		string[] damageLabels;

		SerializedProperty lifeTime;
		SerializedProperty layerMask;
		SerializedProperty autoTarget;
		SerializedProperty autoReTarget;
		SerializedProperty maxMissileOnTarget;
		SerializedProperty projectileDamage;
		SerializedProperty damageType;
		SerializedProperty damageRadius;
		SerializedProperty damageNature;
		SerializedProperty damagePoint;
		SerializedProperty randomDamagePoint;
		SerializedProperty damagePointMax;
		SerializedProperty subWeapon;

		SerializedProperty isPhysic;
		SerializedProperty force;
		SerializedProperty forceMode;

		SerializedProperty useAdvance;
		SerializedProperty velocity;
		SerializedProperty straightTime;
		SerializedProperty angularSpeed;
		SerializedProperty chaos;
		SerializedProperty chaosFrenquency;

		SerializedProperty rayAdvance;
		SerializedProperty rayRadius;

		SerializedProperty impactEffect;
		SerializedProperty deathEffect;
		SerializedProperty autonomous;

		SerializedProperty timedEffect;
		SerializedProperty time;

		SerializedProperty showProperties;
		SerializedProperty showDamage;
		SerializedProperty showPhysic;
		SerializedProperty showEffect;

		void OnEnable(){

			autonomous = serializedObject.FindProperty("autonomous");
			autoTarget = serializedObject.FindProperty("autoTarget");
			autoReTarget = serializedObject.FindProperty("autoReTarget");
			maxMissileOnTarget = serializedObject.FindProperty("maxMissileOnTarget");
			lifeTime = serializedObject.FindProperty("lifeTime");
			layerMask = serializedObject.FindProperty("layerMask");
			projectileDamage = serializedObject.FindProperty("projectileDamage");
			damageType = serializedObject.FindProperty("damageType");
			damageRadius = serializedObject.FindProperty("damageRadius");
			damageNature = serializedObject.FindProperty("damageNature");
			damagePoint = serializedObject.FindProperty("damagePoint");
			randomDamagePoint = serializedObject.FindProperty("randomDamagePoint");
			damagePointMax = serializedObject.FindProperty("damagePointMax");
			isPhysic = serializedObject.FindProperty("isPhysic");
			force = serializedObject.FindProperty("force");
			forceMode = serializedObject.FindProperty("forceMode");
			useAdvance = serializedObject.FindProperty("useAdvance");

			subWeapon = serializedObject.FindProperty("subWeapon");


			velocity = serializedObject.FindProperty("velocity");
			straightTime = serializedObject.FindProperty("straightTime");
			angularSpeed = serializedObject.FindProperty("angularSpeed");
			chaos = serializedObject.FindProperty("chaos");
			chaosFrenquency = serializedObject.FindProperty("chaosFrenquency");

			rayAdvance = serializedObject.FindProperty("rayAdvance");
			rayRadius = serializedObject.FindProperty("rayRadius");

			impactEffect = serializedObject.FindProperty("impactEffect");
			deathEffect = serializedObject.FindProperty("deathEffect");

			timedEffect = serializedObject.FindProperty("timedEffect");
			time = serializedObject.FindProperty("time");

			damageLabels = CoreShooterKitEditor.GetDamageNature().damageNatures;

			showProperties = serializedObject.FindProperty("showProperties");
			showDamage = serializedObject.FindProperty("showDamage");
			showPhysic = serializedObject.FindProperty("showPhysic");
			showEffect = serializedObject.FindProperty("showEffect");
		}

		public override void OnInspectorGUI (){

			serializedObject.Update();
			EditorGUILayout.Space();

			#region Properties
			showProperties.boolValue = EditorTools.BeginFoldOut("Properties",showProperties.boolValue);
			if (showProperties.boolValue || showProperties.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{

					EditorGUILayout.PropertyField(autonomous);
					EditorGUILayout.PropertyField(autoTarget);
					EditorGUILayout.PropertyField(autoReTarget,new GUIContent("Auto re-target"));
						if (autoTarget.boolValue || autoTarget.hasMultipleDifferentValues || autoReTarget.boolValue || autoReTarget.hasMultipleDifferentValues){
						EditorGUILayout.PropertyField(maxMissileOnTarget, new GUIContent("Max on target"));
					}
					if (autoTarget.boolValue){
						EditorGUILayout.HelpBox("Radar component required", MessageType.Info);
					}
					//Life time
					EditorGUILayout.PropertyField( lifeTime);

					// Layer mash
					EditorGUILayout.PropertyField( layerMask);

					//subweapon
					EditorGUILayout.PropertyField( subWeapon);
				}EditorTools.EndGroup();
			}
			#endregion

			#region Damage
			showDamage.boolValue = EditorTools.BeginFoldOut("Damage",showDamage.boolValue);
			if (showDamage.boolValue || showDamage.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					// Damge
					EditorGUILayout.PropertyField( projectileDamage, new GUIContent("At"));

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
					EditorGUILayout.Space();

					EditorGUILayout.PropertyField( randomDamagePoint ,new GUIContent("Random damage"));
					if (!randomDamagePoint.hasMultipleDifferentValues){
						if (!randomDamagePoint.boolValue){
							EditorGUILayout.PropertyField( damagePoint,new GUIContent("Point"));
						}
						else{
							EditorGUILayout.PropertyField( damagePoint,new GUIContent("Min Point"));
							EditorGUILayout.PropertyField( damagePointMax,new GUIContent("Max Point"));
						}
					}
					else{
						EditorGUILayout.PropertyField( damagePoint,new GUIContent("Point"));
						EditorGUILayout.PropertyField( damagePointMax,new GUIContent("Max Point"));
					}
					EditorGUILayout.Space();

					EditorGUILayout.PropertyField( isPhysic, new GUIContent("Add force at impact"));
					if (isPhysic.boolValue || isPhysic.hasMultipleDifferentValues){
						EditorGUILayout.PropertyField( force);
						EditorGUILayout.PropertyField( forceMode);
					}



				}EditorTools.EndGroup();
			}
			#endregion

			#region Projectile physic
			showPhysic.boolValue = EditorTools.BeginFoldOut("Physic",showPhysic.boolValue);
			if (showPhysic.boolValue || showPhysic.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					EditorGUILayout.PropertyField( useAdvance);
					EditorGUILayout.PropertyField( velocity, new GUIContent("Velocity"));
					EditorGUILayout.PropertyField( straightTime, new GUIContent("Straight time (s)"));
					EditorGUILayout.PropertyField( angularSpeed);
					EditorGUILayout.PropertyField( chaos);
					EditorGUILayout.PropertyField( chaosFrenquency);

					EditorGUILayout.Space();
					EditorGUILayout.PropertyField( rayAdvance, new GUIContent("Ray Advance"));
					EditorGUILayout.PropertyField( rayRadius, new GUIContent("Ray radius"));
				}EditorTools.EndGroup();
			}
			#endregion

			#region Effect
			showEffect.boolValue = EditorTools.BeginFoldOut("Effects",showEffect.boolValue);
			if (showEffect.boolValue || showEffect.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					EditorGUILayout.PropertyField( impactEffect);
					EditorGUILayout.PropertyField( deathEffect);
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField( timedEffect);
					EditorGUILayout.PropertyField( time);
				}EditorTools.EndGroup();
			}
			#endregion

			serializedObject.ApplyModifiedProperties();

			if (GUI.changed && !EditorApplication.isPlaying){
				EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			}
		}

	}
}
