using UnityEngine;
using System.Collections;

namespace HedgehogTeam{

	public static class BoundsHelper{

		public static Bounds GetGlobalBounds(GameObject gameobject,bool oriented=false ){

			Quaternion rotation = gameobject.transform.rotation;
			if (!oriented){
				gameobject.transform.rotation = Quaternion.identity;
			}

			MeshFilter[] meshes = gameobject.GetComponentsInChildren<MeshFilter>();

			Bounds bound = new Bounds();

			for(int i = 0; i < meshes.Length; i++) {
				Mesh ms = meshes[i].sharedMesh;
				Vector3 tr = meshes[i].gameObject.transform.position;
				Vector3 ls = meshes[i].gameObject.transform.lossyScale;
				Quaternion lr = meshes[i].gameObject.transform.rotation;
				int vc = ms.vertexCount;
				for(int j = 0; j < vc; j++) {
					if(i==0&&j==0){
						bound = new Bounds(tr + lr*Vector3.Scale(ls,ms.vertices[j]), Vector3.zero);
					}else{
						bound.Encapsulate(tr + lr*Vector3.Scale(ls,ms.vertices[j]));
					}
				}
			}

			if (!oriented){
				gameobject.transform.rotation = rotation;
			}

			return bound;
		}

		public static int GetCapsuleDirection( Bounds bound){

			int returnValue = 0;
			if (bound.extents.y> bound.extents.x){
				returnValue =1;
			}

			if (returnValue == 1 && bound.extents.z > bound.extents.y){
				returnValue = 2;
			}

			if (returnValue == 0 && bound.extents.z > bound.extents.x){
				returnValue = 2;
			}

			return returnValue;
		}

		public static int GetCapsuleDirection( Vector3 size){

			int returnValue = 0;

			if (size.y>size.x){
				returnValue =1;
			}

			if (returnValue == 1 && size.z > size.y){
				returnValue = 2;
			}

			if (returnValue == 0 && size.z >size.x){
				returnValue = 2;
			}

			return returnValue;

		}	
	
		public static float GetResizeCoefficient(Bounds bounds, float maxSize){

			Vector3 size = bounds.size;
			float max = Mathf.Max(Mathf.Max( size.x,size.y ),size.z);
			float sizeCoef = 1f-(1f/max);

			return sizeCoef / max * maxSize;
		}
	
	}

}
