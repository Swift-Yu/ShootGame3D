using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.EasyPoolManager{

	[CustomEditor(typeof(PoolManager))]
	public class PoolManagerInspector : Editor {

		public override void OnInspectorGUI (){

			PoolManager t = (PoolManager)target;

			EditorGUILayout.Space();

			t.dontDestroyOnLoad = EditorTools.Toggle("Don't destroy on load",t.dontDestroyOnLoad,true);
			EditorGUILayout.Space();

			t.isGarbageCollector = EditorTools.Toggle("Enable garbage collector",t.isGarbageCollector);
			if (t.isGarbageCollector){
				t.garbageCycletime = EditorGUILayout.FloatField("Garbage cycle time",t.garbageCycletime);
			}


			EditorGUILayout.Space();

			EditorGUILayout.BeginHorizontal();

			EditorTools.Title("Per-Object Pool options",false,Screen.width-55);
			if (EditorTools.ButtonTitle("+",false,22)){
				PoolProperty po = new PoolProperty();
				t.poolObjectsProperties.Add( po);
			}
			EditorGUILayout.EndHorizontal();


			if (t.poolObjectsProperties.Count>0){
				EditorTools.BeginGroup();{
					EditorGUILayout.Space();
					for (int i=0;i< t.poolObjectsProperties.Count;i++){

						EditorGUILayout.BeginHorizontal();
						if (EditorTools.Button("X",Color.red,19)){	
							PoolManager.DestroyPool(t.poolObjectsProperties[i].obj,true);
							t.poolObjectsProperties.RemoveAt(i);
							i--;
						}
						if (i>=0){
							string label = "Empty";
							if (t.poolObjectsProperties[i].obj){
								label = t.poolObjectsProperties[i].obj.name;
							}
							t.poolObjectsProperties[i].showInpsector = EditorTools.ChildFoldOut(t.poolObjectsProperties[i].showInpsector,label,Color.white,Screen.width-60);
						}
						EditorGUILayout.EndHorizontal();

						if (i>=0){	
							if (t.poolObjectsProperties[i].showInpsector){
								EditorTools.BeginGroup(25);{
									EditorGUI.indentLevel++;

									// Prefab
									GameObject tmpObj = (GameObject)EditorGUILayout.ObjectField("Prefab",t.poolObjectsProperties[i].obj,typeof(GameObject),true);
									if (tmpObj != t.poolObjectsProperties[i].obj){

										PoolManager.DestroyPool(t.poolObjectsProperties[i].obj,true);

										t.poolObjectsProperties[i].obj = tmpObj;

										//if (!t.poolObjectsProperties[i].preLoadAtStart){
										PoolManager.CreatePool( t.poolObjectsProperties[i].obj,t.poolObjectsProperties[i].poolAmount);
										//}
									}

									EditorGUILayout.Space();

									// Preload
									bool tmpBool = EditorTools.Toggle("Pre-Load at start",t.poolObjectsProperties[i].preLoadAtStart);
									if (tmpBool != t.poolObjectsProperties[i].preLoadAtStart){
										t.poolObjectsProperties[i].preLoadAtStart = tmpBool;
										if (t.poolObjectsProperties[i].preLoadAtStart){
											PoolManager.DestroyPool(t.poolObjectsProperties[i].obj,true);
										}
										else{
											PoolManager.DestroyPool(t.poolObjectsProperties[i].obj,true);
											PoolManager.CreatePool( t.poolObjectsProperties[i].obj,t.poolObjectsProperties[i].poolAmount);
										}
									}

									int amount = EditorGUILayout.IntField("Preload Amount",t.poolObjectsProperties[i].poolAmount);
									if (amount != t.poolObjectsProperties[i].poolAmount){
										t.poolObjectsProperties[i].poolAmount = amount;
										PoolManager.DestroyPool(t.poolObjectsProperties[i].obj,true);
										if (!t.poolObjectsProperties[i].preLoadAtStart){
											PoolManager.CreatePool( t.poolObjectsProperties[i].obj,t.poolObjectsProperties[i].poolAmount);
										}
									}

									EditorGUILayout.Space();
									// WillGrow
									t.poolObjectsProperties[i].allowGrowth = EditorTools.Toggle("Allow growth",t.poolObjectsProperties[i].allowGrowth);

									EditorGUILayout.BeginHorizontal();
									t.poolObjectsProperties[i].limitGrowth = EditorTools.Toggle("Limit growth",t.poolObjectsProperties[i].limitGrowth);
									if (t.poolObjectsProperties[i].limitGrowth){
										t.poolObjectsProperties[i].limit = EditorGUILayout.IntField("",t.poolObjectsProperties[i].limit, GUILayout.Width(50));
										if (t.poolObjectsProperties[i].limit <t.poolObjectsProperties[i].poolAmount){
											t.poolObjectsProperties[i].limit =t.poolObjectsProperties[i].poolAmount;
										}
									}
									EditorGUILayout.EndHorizontal();

									EditorGUI.indentLevel--;

								}EditorTools.EndGroup();
							}
						}
					}
				}EditorTools.EndGroup();
			}

			if (GUI.changed && !EditorApplication.isPlaying){
				EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene());
			}
		}
	}

}