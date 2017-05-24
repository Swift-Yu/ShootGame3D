using UnityEngine;

namespace HedgehogTeam{

	public static class MathH{

		public enum SpeedUnit {kmh, Mph, Kt, Mps, KTs};

		public static float kmh2kt = 0.53995680346039f;
		public static float kmh2Mph = 0.62137119223733f;
		public static float ms2Mps = 0.00062137119223733f;
		public static float ms2Kt = 1.9438444924574f;

		#region Angle
		public static float Yaw2Angle( Vector3 vector){

			Vector3 calcul = new Vector3(vector.x,0,vector.z);
			
			calcul.Normalize();
			
			float angle = Mathf.Acos( calcul.x) * Mathf.Rad2Deg;
			
			if (vector.z>=0){
				if (vector.x>=0){
					angle = (angle-90)*-1;
				}
				else{
					angle = 450-angle;
				}
			}
			else{
				if (vector.x>=0){
					angle = angle +90;
				}
				else{
					angle = 90 + angle;
				}
			}
			
			return angle;
		}

		public static float Yaw2Angle180( Vector3 vector){

			float angle = Yaw2Angle( vector);
			if (angle>180) angle = angle-360;

			return angle;
		}

		public static float Pitch2Angle( Vector3 vector){
			
			return -Mathf.Atan2(vector.y,Mathf.Sqrt(vector.x*vector.x + vector.z*vector.z)) * Mathf.Rad2Deg;
		}

		public static float MinYawAngle( Vector3 origine,Vector3 destination){
			
			float angleDestination = Yaw2Angle(destination);		
			return Mathf.DeltaAngle( origine.y, angleDestination);
		}

		public static float DeltaYawAngle180(Vector3 origine,Vector3 destination){
			float angleDestination = Yaw2Angle(destination);	
			angleDestination = origine.y - angleDestination;
			if (angleDestination>180) angleDestination = 360-angleDestination;

			return angleDestination;
		}

		public static float DeltaPitch(Vector3 origine,Vector3 destination){

			return Pitch2Angle( destination ) - Pitch2Angle( origine);

		}

		public static float DeltaPitch(float angle,Vector3 destination){
			if (angle>180) angle = angle-360;

			return (Pitch2Angle( destination)-angle);
		}

		public static Vector2 GetDeltaAngle(Transform from, Vector3 target, Vector3 offset){

			Vector3 linear = from.InverseTransformDirection(target - (from.position+offset));
			Quaternion lookAt =Quaternion.LookRotation( linear, Vector3.up);

			float localVertical = lookAt.eulerAngles.x>180?-360+lookAt.eulerAngles.x:lookAt.eulerAngles.x ;
			float localHorizontal = lookAt.eulerAngles.y>180?-360+lookAt.eulerAngles.y:lookAt.eulerAngles.y;


			return new Vector2( localVertical,localHorizontal);
		}
		#endregion

		#region other
		public static float Remap( float value, float low1, float high1,float low2, float high2){
			return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
		}
		#endregion

		#region Force & drag & speed
		public static float SpeedConverter(SpeedUnit speedUnit){

			switch (speedUnit){
			case SpeedUnit.kmh:
				return 1;

			case SpeedUnit.Kt:
				return kmh2kt;

			case SpeedUnit.Mph:
				return kmh2Mph;

			case SpeedUnit.Mps:
				return ms2Mps;

			case SpeedUnit.KTs:
				return ms2Kt;
			default:
				return 1;
			}
		}

		public static float GetFinalVelocity(float aVelocityChange, float aDrag){
			return aVelocityChange * (1 / Mathf.Clamp01(aDrag * Time.fixedDeltaTime) - 1);
		}

		public static float GetFinalVelocityFromAcceleration(float aAcceleration, float aDrag){
			return GetFinalVelocity(aAcceleration * Time.fixedDeltaTime, aDrag);
		}

		public static float GetDrag(float aVelocityChange, float aFinalVelocity){
			return aVelocityChange / ((aFinalVelocity + aVelocityChange) * Time.fixedDeltaTime);
		}

		public static float GetDragFromAcceleration(float aAcceleration, float aFinalVelocity){
			return GetDrag(aAcceleration * Time.fixedDeltaTime, aFinalVelocity);
		}
			
		public static float GetRequiredVelocityChange(float aFinalSpeed, float aDrag){
			float m = Mathf.Clamp01(aDrag * Time.fixedDeltaTime);
			return aFinalSpeed * m / (1 - m);
		}

		public static float GetRequiredAcceleraton(float aFinalSpeed, float aDrag){
			return GetRequiredVelocityChange(aFinalSpeed, aDrag) / Time.fixedDeltaTime;
		}
		#endregion

	}

}