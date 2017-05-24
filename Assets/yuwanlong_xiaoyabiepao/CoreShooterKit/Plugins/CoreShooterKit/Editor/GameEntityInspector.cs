using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.CoreShooterKit{


	[CustomEditor(typeof(GameEntity)),CanEditMultipleObjects]
	public class GameEntityInspector : Editor {

		#region Members
		string[] damageLabels;
		string[] factionLabels;
		Faction[] factions;

		SerializedProperty faction;
		SerializedProperty subEntity;
		SerializedProperty parentEntity;
		SerializedProperty entityType;
		SerializedProperty isInvulnerable;
		SerializedProperty invulnerableSpawn;
		SerializedProperty life;
		SerializedProperty scorePoint;
		SerializedProperty isPooled;
		SerializedProperty isExternalEndLife;
		SerializedProperty dontRegister;

		SerializedProperty isCollisionDamage;
		SerializedProperty damageNature;
		SerializedProperty damagePoint;
		SerializedProperty useTrigger;

		SerializedProperty addForceFromImpact;
		SerializedProperty damagesSensitivity;
		SerializedProperty damageThreshold;


		SerializedProperty spawnEffect;
		SerializedProperty healEffect;
		SerializedProperty hitEffect;
		SerializedProperty deathEffect;

		SerializedProperty spawnEffectOffset;
		SerializedProperty healEffectOffset;
		SerializedProperty hitEffectOffset;
		SerializedProperty deathEffectOffset;

		SerializedProperty onEntitySpawn;
		SerializedProperty onHit;
		SerializedProperty onHeal;
		SerializedProperty onEntityDestroy;

		SerializedProperty showPropertie;
		SerializedProperty showDamage;
		SerializedProperty showSensitivity;
		SerializedProperty showEffect;
		SerializedProperty showEvent;

		SerializedProperty randomLife;
		SerializedProperty rndMinLife;
		SerializedProperty rndMaxLife;
		int factionIndex =-1;
		#endregion

		void OnEnable(){
			faction = serializedObject.FindProperty("faction");
			entityType = serializedObject.FindProperty("entityType");

			subEntity = serializedObject.FindProperty("subEntity");
			parentEntity = serializedObject.FindProperty("parentEntity");
			isInvulnerable = serializedObject.FindProperty("isInvulnerable");
			invulnerableSpawn = serializedObject.FindProperty("invulnerableSpawn");
			randomLife = serializedObject.FindProperty("randomLife");
			rndMinLife = serializedObject.FindProperty("rndMinLife");
			rndMaxLife  = serializedObject.FindProperty("rndMaxLife");
			life = serializedObject.FindProperty("maxLife");
			scorePoint = serializedObject.FindProperty("scorePoint");
			isPooled = serializedObject.FindProperty("isPooled");
			isExternalEndLife = serializedObject.FindProperty("isExternalEndLife");

			isCollisionDamage = serializedObject.FindProperty("doDamageOnCollision");
			damageNature = serializedObject.FindProperty("damageNature");
			damagePoint = serializedObject.FindProperty("damagePoint");
			damageThreshold = serializedObject.FindProperty("damageThreshold");
			useTrigger = serializedObject.FindProperty("useTrigger");

			addForceFromImpact = serializedObject.FindProperty("addForceFromImpact");
			damagesSensitivity = serializedObject.FindProperty("damagesSensitivity");

			spawnEffect = serializedObject.FindProperty("spawnEffects");
			hitEffect = serializedObject.FindProperty("hitEffects");
			healEffect = serializedObject.FindProperty("healEffects");
			deathEffect = serializedObject.FindProperty("deathEffects");

			spawnEffectOffset = serializedObject.FindProperty("spawnEffectOffset");
			healEffectOffset = serializedObject.FindProperty("healEffectOffset");
			hitEffectOffset = serializedObject.FindProperty("hitEffectOffset");
			deathEffectOffset = serializedObject.FindProperty("deathEffectOffset");

			onHit = serializedObject.FindProperty("onHit");
			onEntitySpawn = serializedObject.FindProperty("onEntitySpawn");
			onHeal = serializedObject.FindProperty("onHeal");
			onEntityDestroy = serializedObject.FindProperty("onEntityDestroy");

			showPropertie = serializedObject.FindProperty("showPropertie");
			showDamage = serializedObject.FindProperty("showDamage");
			showSensitivity = serializedObject.FindProperty("showSensitivity");
			showEffect = serializedObject.FindProperty("showEffect");
			showEvent = serializedObject.FindProperty("showEvent");

			// Faction
			factions = CoreShooterKitEditor.GetAllFaction();
			Array.Resize<string>( ref factionLabels,factions.Length);
			for (int i=0;i<factions.Length;i++){
				factionLabels[i] = factions[i].name;
			}

			// Damage type
			GameEntity t = (GameEntity)target;
			DamageNature damagesNature =  CoreShooterKitEditor.GetDamageNature();
			if (damagesNature!=null){
				damageLabels =damagesNature.damageNatures;

				int size = damagesSensitivity.arraySize;
				Array.Resize<float>(ref t.damagesSensitivity,  damageLabels.Length);
				if (size==0){
					for(int i=0;i<damageLabels.Length;i++){
						t.damagesSensitivity[i]=1;
					}
				}
			}
			else{
				Debug.LogWarning("The nature of damage doesn't exit");
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

			#region Properties
			showPropertie.boolValue = EditorTools.BeginFoldOut("Entity properties",showPropertie.boolValue);
			if (showPropertie.boolValue || showPropertie.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{

					EditorGUILayout.PropertyField( subEntity);
					if (subEntity.boolValue || subEntity.hasMultipleDifferentValues){
						EditorGUILayout.PropertyField( parentEntity);
					}

					if (!subEntity.boolValue || subEntity.hasMultipleDifferentValues){
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

						// Type
						EditorGUILayout.PropertyField( entityType,new GUIContent("Type"));
					}

					EditorGUILayout.Space();

					// invulnerableSpawn at spawn
					EditorGUILayout.PropertyField( isInvulnerable,new GUIContent("Invulnerable"));

					EditorGUILayout.Space();

					// Life
					if (!isInvulnerable.boolValue || isInvulnerable.hasMultipleDifferentValues){

						EditorGUILayout.PropertyField( randomLife);

						if (!randomLife.boolValue || randomLife.hasMultipleDifferentValues){
							EditorGUILayout.PropertyField( life,new GUIContent("life"));
						}

						if (randomLife.boolValue || randomLife.hasMultipleDifferentValues){
							EditorGUILayout.PropertyField( rndMinLife,new GUIContent("Min life"));
							EditorGUILayout.PropertyField( rndMaxLife,new GUIContent("Max life"));
						}
						EditorGUILayout.PropertyField( invulnerableSpawn, new GUIContent("Invulnerable spawn time"));
						EditorGUILayout.PropertyField( scorePoint,new GUIContent("Score Point"));

						EditorGUILayout.Space();
					}
						
					// Advanced
					EditorGUILayout.PropertyField( isPooled,new GUIContent("Managed by a pool"));
					EditorGUILayout.PropertyField( isExternalEndLife,new GUIContent("External end of life"));

				}EditorTools.EndGroup();
			}
			#endregion

			#region Sensitivity to damage
			showSensitivity.boolValue = EditorTools.BeginFoldOut("Sensitivity to damage",showSensitivity.boolValue);
			if (showSensitivity.boolValue || showSensitivity.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					EditorGUILayout.PropertyField( addForceFromImpact,new GUIContent("Enable force from impact"));
					EditorGUILayout.PropertyField(damageThreshold, new GUIContent("DamageThreshold"));
				
					EditorGUILayout.Space();
					EditorGUILayout.LabelField("Natures",EditorStyles.boldLabel);
					for (int i=0;i<damagesSensitivity.arraySize;i++){
						EditorGUILayout.PropertyField( damagesSensitivity.GetArrayElementAtIndex(i),new GUIContent(damageLabels[i]));
					}
				}EditorTools.EndGroup();
			}
			#endregion

			#region Damage
			showDamage.boolValue = EditorTools.BeginFoldOut("Damage on collision",showDamage.boolValue);
			if (showDamage.boolValue || showDamage.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					EditorGUILayout.PropertyField( isCollisionDamage,new GUIContent("Enable"));
					EditorGUI.indentLevel++;
					if (isCollisionDamage.boolValue || isCollisionDamage.hasMultipleDifferentValues){
						EditorGUILayout.PropertyField(useTrigger);
						if (!damageNature.hasMultipleDifferentValues){
							damageNature.intValue = EditorGUILayout.Popup("Damage nature",damageNature.intValue,damageLabels);
						}
						else{
							int tmpDamageNature = EditorGUILayout.Popup("Damage nature",-1,damageLabels);
							if (tmpDamageNature != damageNature.intValue && tmpDamageNature>-1){
								damageNature.intValue = tmpDamageNature;
							}
						}

						EditorGUILayout.PropertyField( damagePoint,new GUIContent("Damage Point"));

					}
					EditorGUI.indentLevel--;
				}EditorTools.EndGroup();
			}
			#endregion

			#region Effect
			showEffect.boolValue = EditorTools.BeginFoldOut("Effects",showEffect.boolValue);
			if (showEffect.boolValue || showEffect.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{

					//spawnEffectOffset = serializedObject.FindProperty("spawnEffectOffset");
					//healEffectOffset = serializedObject.FindProperty("healEffectOffset");
					//hitEffectOffset = serializedObject.FindProperty("hitEffectOffset");
					//deathEffectOffset = serializedObject.FindProperty("deathEffectOffset");

					// Spawn
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( spawnEffect);
					if (spawnEffect.isExpanded){
						EditorGUILayout.PropertyField( spawnEffectOffset, new GUIContent("Offset"));
						SerializedProperty spawnE = serializedObject.FindProperty("spawnEffects");
						EditorGUILayout.Space();
						spawnE.Next(true);
						spawnE.Next(true);
						EditorGUILayout.PropertyField( spawnE);
						for (int i=0;i<spawnEffect.arraySize;i++){
							EditorGUILayout.PropertyField( spawnEffect.GetArrayElementAtIndex(i));
						}
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.Space();

					// hit
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( hitEffect);
					if (hitEffect.isExpanded){
						EditorGUILayout.PropertyField( hitEffectOffset, new GUIContent("Offset"));
						SerializedProperty hitE = serializedObject.FindProperty("hitEffects");
						EditorGUILayout.Space();
						hitE.Next(true);
						hitE.Next(true);
						EditorGUILayout.PropertyField( hitE);
						for (int i=0;i<hitEffect.arraySize;i++){
							EditorGUILayout.PropertyField( hitEffect.GetArrayElementAtIndex(i));
						}
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.Space();

					// Heal
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( healEffect);
					if (healEffect.isExpanded){
						EditorGUILayout.PropertyField( healEffectOffset, new GUIContent("Offset"));
						SerializedProperty healE = serializedObject.FindProperty("healEffects");
						EditorGUILayout.Space();
						healE.Next(true);
						healE.Next(true);
						EditorGUILayout.PropertyField( healE);
						for (int i=0;i<healEffect.arraySize;i++){
							EditorGUILayout.PropertyField( healEffect.GetArrayElementAtIndex(i));
						}
					}
					EditorGUI.indentLevel--;
					EditorGUILayout.Space();

					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField( deathEffect);
					if (deathEffect.isExpanded){
						EditorGUILayout.PropertyField( deathEffectOffset, new GUIContent("Offset"));
						SerializedProperty deathE = serializedObject.FindProperty("deathEffects");
						EditorGUILayout.Space();
						deathE.Next(true);
						deathE.Next(true);
						EditorGUILayout.PropertyField( deathE,true);
						for (int i=0;i<deathEffect.arraySize;i++){
							EditorGUILayout.PropertyField( deathEffect.GetArrayElementAtIndex(i));
						}
					}
					EditorGUI.indentLevel--;

				}EditorTools.EndGroup();
			}
			#endregion

			#region Events
			showEvent.boolValue = EditorTools.BeginFoldOut("Events",showEvent.boolValue);
			if (showEvent.boolValue || showEvent.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					EditorGUILayout.PropertyField( onEntitySpawn,true);
					EditorGUILayout.PropertyField( onHit,true);
					EditorGUILayout.PropertyField( onHeal,true);
					EditorGUILayout.PropertyField( onEntityDestroy,true);
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