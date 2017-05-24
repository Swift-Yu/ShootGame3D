using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.CoreShooterKit{

	[CustomEditor(typeof(Bullet)), CanEditMultipleObjects]
	public class BulletInspector : Editor {

		string[] damageLabels;


		SerializedProperty projectileType;
		SerializedProperty lifeTime;
		SerializedProperty layerMask;
		SerializedProperty projectileDamage;
		SerializedProperty subWeapon;
		SerializedProperty damageType;
		SerializedProperty damageRadius;
		SerializedProperty damageNature;
		SerializedProperty damagePoint;
		SerializedProperty randomDamagePoint;
		SerializedProperty damagePointMax;
		SerializedProperty isPhysic;
		SerializedProperty force;
		SerializedProperty forceMode;

		SerializedProperty velocity;
		SerializedProperty rayAdvance;
		SerializedProperty rayRadius;
		SerializedProperty projectileForceMode;

		SerializedProperty impactEffect;
		SerializedProperty deathEffect;

		SerializedProperty timedEffect;
		SerializedProperty time;

		SerializedProperty showProperties;
		SerializedProperty showDamage;
		SerializedProperty showPhysic;
		SerializedProperty showEffect;


		void OnEnable(){
			projectileType = serializedObject.FindProperty("projectileType");
			lifeTime = serializedObject.FindProperty("lifeTime");
			layerMask = serializedObject.FindProperty("layerMask");
			projectileDamage = serializedObject.FindProperty("projectileDamage");
			damageType = serializedObject.FindProperty("damageType");
			damageRadius = serializedObject.FindProperty("damageRadius");
			damageNature = serializedObject.FindProperty("damageNature");
			damagePoint = serializedObject.FindProperty("damagePoint");
			randomDamagePoint = serializedObject.FindProperty("randomDamagePoint");
			damagePointMax = serializedObject.FindProperty("damagePointMax");

			subWeapon = serializedObject.FindProperty("subWeapon");

			isPhysic = serializedObject.FindProperty("isPhysic");
			force = serializedObject.FindProperty("force");
			forceMode = serializedObject.FindProperty("forceMode");

			velocity = serializedObject.FindProperty("velocity");
			rayAdvance = serializedObject.FindProperty("rayAdvance");
			rayRadius = serializedObject.FindProperty("rayRadius");
			projectileForceMode = serializedObject.FindProperty("projectileForceMode");
			 
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
			//GUILayout.Toggle(true,"<b><size=11>" + "Properties" + "</size></b>","dragtab");
			showProperties.boolValue = EditorTools.BeginFoldOut("Properties",showProperties.boolValue);
			if (showProperties.boolValue || showProperties.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					// Type
					EditorGUILayout.PropertyField( projectileType,new GUIContent("Type"));

					// Life
					if (projectileType.enumValueIndex != 1){
						EditorGUILayout.PropertyField( lifeTime);
					}

					// Layer mash
					EditorGUILayout.PropertyField( layerMask);

					//subweapon
					if (projectileType.enumValueIndex != 1){
						EditorGUILayout.PropertyField( subWeapon);
					}
				}EditorTools.EndGroup();
			}
			#endregion

			#region Damage
			//GUILayout.Toggle(true,"<b><size=11>" + "Damage" + "</size></b>","dragtab");
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
			//GUILayout.Toggle(true,"<b><size=11>" + "Projectile physic" + "</size></b>","dragtab");
			showPhysic.boolValue = EditorTools.BeginFoldOut("Physic",showPhysic.boolValue);
			if (showPhysic.boolValue || showPhysic.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{

					//Velocity, Instant, Physic, Guided
					switch( projectileType.enumValueIndex){
					// Velocity
					case 0:
						EditorGUILayout.PropertyField( velocity, new GUIContent("Velocity (km/h)"));
						EditorGUILayout.PropertyField( rayAdvance, new GUIContent("Ray Advance"));
						EditorGUILayout.PropertyField( rayRadius, new GUIContent("Ray radius"));
						break;
					// Instant
					case 1:
						EditorGUILayout.PropertyField( rayAdvance, new GUIContent("Distance"));
						EditorGUILayout.PropertyField( rayRadius, new GUIContent("Ray radius"));
						break;
					// Physic
					case 2:
						EditorGUILayout.PropertyField( velocity, new GUIContent("shooting Force"));
						EditorGUILayout.PropertyField( projectileForceMode, new GUIContent("Force mode"));
		
						break;
					}
				}EditorTools.EndGroup();
			}
			#endregion

			#region Effect
			//GUILayout.Toggle(true,"<b><size=11>" + "Effect" + "</size></b>","dragtab");
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
