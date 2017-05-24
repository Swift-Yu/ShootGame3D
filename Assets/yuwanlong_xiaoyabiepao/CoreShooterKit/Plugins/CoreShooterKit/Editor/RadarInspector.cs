using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditor.SceneManagement;

namespace HedgehogTeam.CoreShooterKit{

	[CustomEditor(typeof(Radar)),CanEditMultipleObjects]
	public class RadarInspector : Editor {

		void OnSceneGUI () {

			Radar t = (Radar)target;

			if (t.fov.debugFov){
				t.fov.maxRange = 	Handles.ScaleValueHandle(t.fov.maxRange,	t.transform.position + t.transform.forward*t.fov.maxRange,	t.transform.rotation,	HandleUtility.GetHandleSize(t.transform.position + t.transform.forward*t.fov.maxRange + Vector3.left ),	Handles.ConeCap,0);
				t.fov.minRange = 	Handles.ScaleValueHandle(t.fov.minRange,	t.transform.position + t.transform.forward*t.fov.minRange,	t.transform.rotation,	HandleUtility.GetHandleSize(t.transform.position + t.transform.forward*t.fov.minRange + Vector3.left ),	Handles.ConeCap,0);

				t.fov.horizontalLeft = Handles.ScaleValueHandle(t.fov.horizontalLeft,	t.transform.position + t.transform.forward*t.fov.maxRange +  -t.transform.right, 	t.transform.rotation * Quaternion.AngleAxis( -90,Vector3.up) ,	HandleUtility.GetHandleSize(t.transform.position + t.transform.forward*t.fov.maxRange + Vector3.left ),	Handles.ConeCap,0);
				t.fov.horizontalRight = Handles.ScaleValueHandle(t.fov.horizontalRight,	t.transform.position + t.transform.forward*t.fov.maxRange +  t.transform.right, 	t.transform.rotation * Quaternion.AngleAxis( 90,Vector3.up) ,	HandleUtility.GetHandleSize(t.transform.position + t.transform.forward*t.fov.maxRange + Vector3.right ),	Handles.ConeCap,0);
			
				t.fov.verticalDown = Handles.ScaleValueHandle(t.fov.verticalDown,	t.transform.position + t.transform.forward*t.fov.maxRange +  -t.transform.up, 	t.transform.rotation * Quaternion.AngleAxis( 90,Vector3.right) ,	HandleUtility.GetHandleSize(t.transform.position + t.transform.forward*t.fov.maxRange + Vector3.up ),	Handles.ConeCap,0);
				t.fov.verticalUp = Handles.ScaleValueHandle(t.fov.verticalUp,	t.transform.position + t.transform.forward*t.fov.maxRange +  t.transform.up, 	t.transform.rotation * Quaternion.AngleAxis( -90,Vector3.right) ,	HandleUtility.GetHandleSize(t.transform.position + t.transform.forward*t.fov.maxRange + Vector3.down ),	Handles.ConeCap,0);
			}
		
		}
	}

}