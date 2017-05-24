using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HedgehogTeam.EasyPoolManager;

namespace HedgehogTeam.CoreShooterKit{
	
	public static class CoreShooterKitEditor{

		#region Menu
		// Scriptable object
		[MenuItem("Assets/Create/Core Shooter Kit/Faction")]
		public static void CreateFaction() {
			EditorTools.CreateAsset<Faction>("new faction");
		}

		[MenuItem("Assets/Create/Core Shooter Kit/Damage Nature")]
		public static void CreateDamageNature() {

			DamageNature damage = GetDamageNature();
			if (GetDamageNature()==null){
				EditorTools.CreateAsset<DamageNature>("DamageNature");
			}
			else{
				Selection.activeObject = damage;
			}
		}
			
		// General
		[MenuItem ("GameObject/Core Shooter Kit/Game System/Game Entity",false,0)]
		public static void CreateDestructibleEntity(){
			Selection.activeObject = new GameObject("Game Entity",typeof(GameEntity));
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
		}

		[MenuItem ("GameObject/Core Shooter Kit/Game System/Game Manager",false,0)]
		public static void CreateGameManager(){
			Selection.activeObject = GameManager.Instance.gameObject;
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
		}

		[MenuItem ("GameObject/Core Shooter Kit/Game System/Spawner",false,0)]
		public static void CreateSpawner(){
			Selection.activeObject = new GameObject("Spawner",typeof(Spawner));
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
		}

		// Weapon
		[MenuItem ("GameObject/Core Shooter Kit/Weapon System/Turret",false,0)]
		public static void CreateTurret(){

			GameObject turret = new GameObject("Turret",typeof(Turret));
			GameObject hub = new GameObject("[Hub]");
			hub.transform.parent = turret.transform;
			GameObject axis = new GameObject("[Axis]");
			axis.transform.parent = hub.transform;

			Turret t = turret.GetComponent<Turret>();
			t.hub = hub.transform;
			t.axis = axis.transform;

			Selection.activeObject = turret;
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();

		}

		[MenuItem ("GameObject/Core Shooter Kit/Weapon System/Weapon",false,0)]
		public static void CreateLauncher(){
			GameObject launcher = new GameObject("Weapon",typeof(Weapon));
			launcher.GetComponent<AudioSource>().playOnAwake = false;
			Selection.activeObject = launcher;
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
		}


		[MenuItem ("GameObject/Core Shooter Kit/Weapon System/Bullet",false,0)]
		public static void CreateProjectile(){
			Selection.activeObject = new GameObject("Bullet",typeof(Bullet));
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
		}

		[MenuItem ("GameObject/Core Shooter Kit/Weapon System/LaserBeam",false,0)]
		public static void CreateLaserBeam(){
			Selection.activeObject = new GameObject("LaserBeam",typeof(LaserBeam));
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
		}

		[MenuItem ("GameObject/Core Shooter Kit/Weapon System/Missile",false,0)]
		public static void CreateMissile(){
			Selection.activeObject = new GameObject("Missile",typeof(Missile));
			UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
		}
			
		#endregion

		#region Scriptable object
		public static Faction[] GetAllFaction(){

			string[] guids = AssetDatabase.FindAssets( "t:Faction");
			List<Faction> factions = new List<Faction>();

			for (int i=0;i<guids.Length;i++){
				Faction faction = AssetDatabase.LoadAssetAtPath<Faction>(AssetDatabase.GUIDToAssetPath( guids[i]) );
				if (faction){
					factions.Add( faction);
				}
			}

			return factions.ToArray<Faction>();
		}

		public static DamageNature GetDamageNature(){

			string[] guids = AssetDatabase.FindAssets( "t:DamageNature");
			DamageNature damage = null;

			if (guids.Length>0){
				damage = AssetDatabase.LoadAssetAtPath<DamageNature>(AssetDatabase.GUIDToAssetPath( guids[0]) );
			}
	
			return damage;
		}

		#endregion
	}

}
