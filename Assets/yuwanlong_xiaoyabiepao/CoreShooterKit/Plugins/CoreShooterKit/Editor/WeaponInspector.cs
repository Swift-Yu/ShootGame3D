using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using UnityEditor.SceneManagement;
using System.ComponentModel;

namespace HedgehogTeam.CoreShooterKit{

	[CustomEditor(typeof(Weapon))]
	public class WeaponInspector : Editor {

		string[] factionLabels;
		Faction[] factions;

		SerializedProperty ammunition;
		SerializedProperty weaponType;
		SerializedProperty fireRate;
		SerializedProperty barrels; 
		SerializedProperty isSynchronizedBarrel;

		SerializedProperty isFullyAutmotic; 
		SerializedProperty isBurst; 
		SerializedProperty isOneShot; 
		SerializedProperty burstShotsAmount;

		SerializedProperty enableAmmunitionStock;
		SerializedProperty ammunitionMaxStock;
		SerializedProperty ammunitionStock;

		SerializedProperty enableMagazine;
		SerializedProperty magazineType;
		SerializedProperty magazineAutoReload;
		SerializedProperty magazineCapacity;
		SerializedProperty shotValue;
		SerializedProperty reloadValue;

		SerializedProperty muzzleEffect;
		SerializedProperty startShootingEffect;
		SerializedProperty StopShootingEffect;

		SerializedProperty isAutonomous;
		SerializedProperty faction;
		SerializedProperty fpsAlignment;
		SerializedProperty fixedAlignment;
		SerializedProperty alignmentPoint;

		SerializedProperty onStartShooting;
		SerializedProperty onShooting;
		SerializedProperty onStopShooting;
		SerializedProperty onReload;
		SerializedProperty onNeedReload;
		SerializedProperty OnAutoReload;
		SerializedProperty OnReloadEnd;


		SerializedProperty showProperties;
		SerializedProperty showMode;
		SerializedProperty showMagazine;
		SerializedProperty showEffect;
		SerializedProperty showEvents;
		SerializedProperty showStock;

		int factionIndex =-1;

		void OnEnable(){
			fpsAlignment = serializedObject.FindProperty("fpsAlignment");
			fixedAlignment = serializedObject.FindProperty("fixedAlignment");
			alignmentPoint = serializedObject.FindProperty("alignmentPoint");

			isAutonomous = serializedObject.FindProperty("isAutonomous");
			faction = serializedObject.FindProperty("faction");
			ammunition = serializedObject.FindProperty("ammunition");
			weaponType = serializedObject.FindProperty("weaponType");
			fireRate = serializedObject.FindProperty("fireRate");
			barrels = serializedObject.FindProperty("barrels");
			isSynchronizedBarrel = serializedObject.FindProperty("isSynchronizedBarrel");

			isFullyAutmotic = serializedObject.FindProperty("isAutmotic");
			isBurst = serializedObject.FindProperty("isBurst");
			isOneShot = serializedObject.FindProperty("isOneShot");
			burstShotsAmount = serializedObject.FindProperty("burstShotsAmount");

			enableMagazine = serializedObject.FindProperty("enableMagazine");
			magazineAutoReload = serializedObject.FindProperty("magazineAutoReload");
			magazineCapacity = serializedObject.FindProperty("magazineCapacity");
			shotValue = serializedObject.FindProperty("shotValue");
			reloadValue = serializedObject.FindProperty("reloadValue");
			magazineType = serializedObject.FindProperty("magazineType");

			enableAmmunitionStock = serializedObject.FindProperty("enableAmmunitionStock");
			ammunitionMaxStock = serializedObject.FindProperty("ammunitionMaxStock");
			ammunitionStock = serializedObject.FindProperty("ammunitionStock");

			muzzleEffect = serializedObject.FindProperty("muzzleEffect");
			startShootingEffect = serializedObject.FindProperty("startShootingEffect");
			StopShootingEffect = serializedObject.FindProperty("StopShootingEffect");

			onStartShooting = serializedObject.FindProperty("onStartShooting");
			onShooting = serializedObject.FindProperty("onShooting");
			onStopShooting = serializedObject.FindProperty("onStopShooting");
			onReload = serializedObject.FindProperty("onReload");
			onNeedReload = serializedObject.FindProperty("onNeedReload");
			OnReloadEnd = serializedObject.FindProperty("onReloadEnd");
			OnAutoReload = serializedObject.FindProperty("onAutoReload");

			showProperties = serializedObject.FindProperty("showProperties");
			showMode = serializedObject.FindProperty("showMode");
			showMagazine = serializedObject.FindProperty("showMagazine");
			showEffect= serializedObject.FindProperty("showEffect");
			showEvents = serializedObject.FindProperty("showEvents");
			showStock = serializedObject.FindProperty("showStock");

			// Faction
			factions = CoreShooterKitEditor.GetAllFaction();
			Array.Resize<string>( ref factionLabels,factions.Length);
			for (int i=0;i<factions.Length;i++){
				factionLabels[i] = factions[i].name;
			}

			for (int i=0;i<factions.Length;i++){
				if (faction.objectReferenceValue == factions[i]){
					factionIndex = i;
				}
			}
		}

		public override void OnInspectorGUI (){

			Weapon launcher = (Weapon)target;

			serializedObject.Update();
			EditorGUILayout.Space();

			if (EditorApplication.isPlaying){
				string label = "Shoot";
				if (launcher.isShooting){
					label="Stop shoot";
				}

				if ( GUILayout.Button(label)){
					if (launcher.isShooting){
						launcher.StopShoot();
					}
					else{
						launcher.Shoot();
					}
				}
			}

			#region Properties
			showProperties.boolValue = EditorTools.BeginFoldOut("Properties",showProperties.boolValue);
			if (showProperties.boolValue || showProperties.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					
					// Autonomous
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( isAutonomous, new GUIContent("Autonomous"));
					EditorGUI.indentLevel++;
					if (isAutonomous.boolValue){
						EditorGUILayout.HelpBox("Radar component required", MessageType.Info);
						factionIndex = EditorGUILayout.Popup("Faction",factionIndex,factionLabels);
						if (factionIndex>-1)
							faction.objectReferenceValue  = (UnityEngine.Object)factions[factionIndex] ;
					}
					EditorGUI.indentLevel--;

					EditorGUILayout.Space();

					// Weapon
					EditorGUI.BeginChangeCheck();
					EditorGUILayout.PropertyField( ammunition);
					if (EditorGUI.EndChangeCheck()){
						if (ammunition.objectReferenceValue != null){
							if (ammunition.objectReferenceValue.GetType() == typeof( LaserBeam)){
								weaponType.enumValueIndex = 1;
							}
							else{
								weaponType.enumValueIndex = 0;
							}
						}
					}
						

					// Fire Rate
					if (weaponType.enumValueIndex == 0){
						EditorGUILayout.PropertyField( fireRate, new GUIContent("Fire Rate /s"));
					}
					EditorGUILayout.Space();

					// Barrels
					EditorGUILayout.PropertyField( isSynchronizedBarrel, new GUIContent("Synchronized barrel"));
					EditorGUILayout.PropertyField( fpsAlignment);
					EditorGUILayout.PropertyField ( fixedAlignment );
					if (fixedAlignment.boolValue){
						EditorGUILayout.PropertyField( alignmentPoint);
					}
					EditorGUI.indentLevel++;
					EditorGUILayout.PropertyField(barrels,true);
					EditorGUI.indentLevel--;

				}EditorTools.EndGroup();
			}
			#endregion

			#region Weapon Mode
			if (weaponType.enumValueIndex == 0){
				showMode.boolValue = EditorTools.BeginFoldOut("Mode",showMode.boolValue);
				if (showMode.boolValue || showMode.hasMultipleDifferentValues){
					EditorTools.BeginGroup();{
						EditorGUILayout.PropertyField( isOneShot, new GUIContent("One shot"));
						EditorGUILayout.PropertyField( isBurst, new GUIContent("Burst"));
						if ( isBurst.boolValue){
							EditorGUI.indentLevel++;
							EditorGUILayout.PropertyField( burstShotsAmount, new GUIContent("Amount"));
							EditorGUI.indentLevel--;
						}
						EditorGUILayout.PropertyField( isFullyAutmotic, new GUIContent("Automatic"));
					}EditorTools.EndGroup();
				}
			}
			#endregion

			#region Magazine
			showMagazine.boolValue = EditorTools.BeginFoldOut("Magazine",showMagazine.boolValue);
			if (showMagazine.boolValue || showMagazine.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
				EditorGUILayout.PropertyField( enableMagazine, new GUIContent("Enable"));
				if (enableMagazine.boolValue){
					string shotValueLabel = "Shot value";
					string reloadValueLabel = "Reload Time";
					if (magazineType.enumValueIndex == 1){ 
						shotValueLabel = "Speed";
						reloadValueLabel= "Reload speed";
					} 

					EditorGUILayout.PropertyField( magazineType, new GUIContent("Type"));
					if (weaponType.enumValueIndex == 1){
						magazineType.enumValueIndex = 1;
					}
				
					EditorGUILayout.PropertyField( magazineCapacity, new GUIContent("Capacity " ));
					EditorGUILayout.PropertyField( shotValue, new GUIContent(shotValueLabel));

					EditorGUILayout.Space();

					EditorGUILayout.PropertyField( magazineAutoReload, new GUIContent("Auto reload"));
					EditorGUILayout.PropertyField( reloadValue, new GUIContent(reloadValueLabel ));
						
				}
				}EditorTools.EndGroup();
			}
			#endregion

			#region Ammunition stock
			if (enableMagazine.boolValue && magazineType.enumValueIndex == 0){
				showStock.boolValue = EditorTools.BeginFoldOut("Stock of ammuntion",showStock.boolValue);
				if (showStock.boolValue ){
					EditorTools.BeginGroup();{
						EditorGUILayout.PropertyField( enableAmmunitionStock, new GUIContent("Enable"));
						EditorGUILayout.PropertyField( ammunitionMaxStock, new GUIContent("Max Stock"));
						EditorGUILayout.PropertyField( ammunitionStock, new GUIContent("Initial stock"));
					}EditorTools.EndGroup();
				}
			}

			#endregion

			#region Effect
			showEffect.boolValue = EditorTools.BeginFoldOut("Effects",showEffect.boolValue);
			if (showEffect.boolValue || showEffect.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					EditorGUILayout.PropertyField( muzzleEffect);
					EditorGUILayout.PropertyField( startShootingEffect);
					EditorGUILayout.PropertyField( StopShootingEffect);
				}EditorTools.EndGroup();
			}
			#endregion

			#region event
			showEvents.boolValue = EditorTools.BeginFoldOut("Events",showEvents.boolValue);
			if (showEvents.boolValue || showEvents.hasMultipleDifferentValues){
				EditorTools.BeginGroup();{
					EditorGUILayout.PropertyField( onStartShooting,true);
					EditorGUILayout.PropertyField( onShooting,true);
					EditorGUILayout.PropertyField( onStopShooting,true);
					EditorGUILayout.PropertyField( onReload,true);
					EditorGUILayout.PropertyField( OnAutoReload,true);
					EditorGUILayout.PropertyField( onNeedReload,true);
					EditorGUILayout.PropertyField( OnReloadEnd,true);

				}EditorTools.EndGroup();
			}
			#endregion

			serializedObject.ApplyModifiedProperties();

			if (GUI.changed && !EditorApplication.isPlaying){
				EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			}
			GUI.enabled = true;
		}
	}
}
