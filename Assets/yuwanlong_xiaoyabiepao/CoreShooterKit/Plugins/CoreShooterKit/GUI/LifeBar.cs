/// <summary>
/// Life bar.
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

namespace HedgehogTeam.CoreShooterKit{
	
	public class LifeBar : MonoBehaviour {

		#region Members
		public Gradient healthGradient;
		public float maxDistanceFromCamera = 100;
		public bool keepRelativePosition = true;

		private Slider slider;
		private Rect visibleViewportRect = new Rect(0,0,1,1);

		private float alpha=0;
		private Camera cachedCamera;
		private CanvasRenderer[] cachedCanvasRenderers;
		private Image sliderFill;
		private GameEntity entity;

		private Vector3 offset;
		private Transform cachedTransform;
		#endregion

		#region Monobehaviour Callback
		void Awake () {
			cachedCamera = Camera.main;
			slider= GetComponentInChildren<Slider>();
			cachedCanvasRenderers = GetComponentsInChildren<CanvasRenderer>();
			entity = GetComponentInParent<GameEntity>();
			sliderFill = slider.GetComponentsInChildren<Image>().First(i=>i.name.Equals("Fill"));

			cachedTransform = transform;
			offset = cachedTransform.parent.transform.TransformVector( cachedTransform.localPosition);

		}
	
		void Update () {
		
			// Show distance & alpha
			Vector3 entityViewportPoint = cachedCamera.WorldToViewportPoint(transform.position);
			bool visible = visibleViewportRect.Contains(entityViewportPoint) && entityViewportPoint.z <= maxDistanceFromCamera && GameManager.instance.showLifeBar;

			alpha = Mathf.Clamp01( alpha + Time.deltaTime * (visible ? 1 : -1));	
			foreach(CanvasRenderer canvasRenderer in cachedCanvasRenderers){
				canvasRenderer.SetAlpha(alpha);
			}

			if (visible){
				// Update slider
				float t = Mathf.InverseLerp(0, entity.maxLife, entity.life);
				slider.value = t;
				sliderFill.color = healthGradient.Evaluate(t);
			}
		}

		void LateUpdate(){
		 
			if (keepRelativePosition){
				cachedTransform.position=  cachedTransform.parent.position + offset;
			}

			// bilboard
			if (cachedCamera){
				transform.rotation = Quaternion.LookRotation(cachedCamera.transform.forward,  cachedCamera.transform.up);
			}


		}
		#endregion

	}

}