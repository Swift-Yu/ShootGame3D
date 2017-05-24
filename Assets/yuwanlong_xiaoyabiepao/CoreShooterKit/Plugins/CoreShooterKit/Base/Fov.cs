using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HedgehogTeam.CoreShooterKit{

	[System.Serializable]
	public class Fov {

		public float minRange = 0;
		public float maxRange = 2000;
		public float horizontalLeft = 60;
		public float horizontalRight = 60;
		public float verticalUp = 60;
		public float verticalDown = 60;

		public bool debugFov = false;
		public Color fovColor = new Color(0,1,0,0.04f);

		public bool IsVisble(Transform from, GameEntity target, out float distance){

			RaycastHit hitInfo;
			if (Physics.Raycast( from.position, target.transform.position - from.position, out hitInfo, maxRange,~(1<<-1), QueryTriggerInteraction.Ignore)){
				
				Transform targetTransform = target.transform;
				if (target.subEntity) targetTransform = target.parentEntity.transform;

				if (!hitInfo.collider.transform.IsChildOf(targetTransform)){
					distance = hitInfo.distance;
					return false;
				}
			}

			return InFov( from,target.transform.position,out distance);
		}

		public bool IsVisble(Transform from, GameEntity target){
			float distance =0;
			return IsVisble( from,target, out distance);
		}

		public bool InFov(Transform from, Vector3 target, out float distance){

			if (InRange( from.position,target,out distance)){
				return InAngle( from,target);		
			}
			else{
				return false;
			}
		}

		public bool InFov(Transform from, Vector3 target){

			float distance =0;
			return InFov( from,target,out distance);
		}
			
		public bool InRange(Vector3 from, Vector3 target, out float distance){
			
			distance = (target - from).magnitude;
			if (distance >= minRange && distance <= maxRange){
				return true;
			}
			else{
				return false;
			}
		}
			
		public bool InAngle(Transform from, Vector3 target){

			Vector3 linear = from.InverseTransformDirection(target - from.position);
			if (linear == Vector3.zero){
				return false;
			}
			Quaternion lookAt = Quaternion.LookRotation( linear, from.up);

			float localVertical = lookAt.eulerAngles.x>180?-360+lookAt.eulerAngles.x:lookAt.eulerAngles.x ;
			float localHorizontal = lookAt.eulerAngles.y>180?-360+lookAt.eulerAngles.y:lookAt.eulerAngles.y;

			if (localVertical> -verticalUp && localVertical <verticalDown && localHorizontal < horizontalRight && localHorizontal > -horizontalLeft){
				return true;
			}
			else{
				return false;
			}			
		}

		public void DrawDebug( Transform tr){
			#if UNITY_EDITOR
			if (debugFov){

				Handles.color = fovColor;
				Handles.DrawSolidArc( tr.position,tr.up, tr.forward, -horizontalLeft,maxRange);
				Handles.DrawSolidArc( tr.position,tr.up, tr.forward, horizontalRight,maxRange);

				Handles.DrawSolidArc(tr.position,tr.right, tr.forward, -verticalUp,maxRange);
				Handles.DrawSolidArc( tr.position,tr.right, tr.forward, verticalDown,maxRange);

				Handles.color = new Color( 1,0,0,fovColor.a );
				Handles.DrawSolidArc( tr.position,tr.up, tr.forward, -horizontalLeft,minRange);
				Handles.DrawSolidArc( tr.position,tr.up, tr.forward, horizontalRight,minRange);
				Handles.DrawSolidArc(tr.position,tr.right, tr.forward, -verticalUp,minRange);
				Handles.DrawSolidArc( tr.position,tr.right, tr.forward, verticalDown,minRange);

				Handles.color = new Color( fovColor.r,fovColor.g,fovColor.b);		
				Handles.DrawWireArc(tr.position,tr.up, tr.forward, -horizontalLeft,maxRange);
				Handles.DrawWireArc(tr.position,tr.up, tr.forward, horizontalRight,maxRange);
				Handles.DrawWireArc(tr.position,tr.up, tr.forward, -horizontalLeft,minRange);
				Handles.DrawWireArc(tr.position,tr.up, tr.forward, horizontalRight,minRange);

				Handles.DrawWireArc(tr.position,tr.right, tr.forward, -verticalUp,minRange);
				Handles.DrawWireArc(tr.position,tr.right, tr.forward, verticalDown,minRange);
				Handles.DrawWireArc(tr.position,tr.right, tr.forward, -verticalUp,maxRange);
				Handles.DrawWireArc(tr.position,tr.right, tr.forward, verticalDown,maxRange);



			}
			#endif
		}
	}

}
