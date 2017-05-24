using UnityEngine;
using System.Collections;

public class CrosshairLaser : MonoBehaviour {

	public float distance = 10;
	private LineRenderer cachedLineRender;
	private Transform cachedtransform;
	private RaycastHit hitInfo;

	void Awake(){
		cachedLineRender = GetComponent<LineRenderer>();
		cachedtransform = transform;
	}

	void Update(){

		float localDistance = distance;
		if (Physics.Raycast (cachedtransform.position, cachedtransform.forward, out hitInfo)){
			localDistance = (hitInfo.point- cachedtransform.position).magnitude;
		}

		cachedLineRender.SetPosition(0, cachedtransform.position);
		cachedLineRender.SetPosition(1, cachedtransform.position+ cachedtransform.forward* localDistance);
	}

}
