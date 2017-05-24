/// <summary>
/// Combat text.
/// 1.0.0
/// </summary>
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using HedgehogTeam.EasyPoolManager;
using UnityEngine.Networking;

namespace HedgehogTeam.CoreShooterKit{
	
	public class CombatText : MonoBehaviour,IHitEffect,IHealEffect {

		#region Members
		private static int order=0;

		[Header("Position")]
		public bool useObjectPivot = false;
		public bool localOffset = false;
		public Vector3 offset;

		[Header("Global properties")]
		public float maxDistanceFromCamera = 50;
		public float lifeTime = 1.5f;
		public float fadeTime = 0.8f;
		public string prefixLabel="";
		public string suffixLabel="";

		[Header("Value properties")]
		public float maxPointValue = 50;
		public Gradient gradient;
		public float minSize = 5f;
		public float maxSize = 8f;
		public bool scaleEffect = true;

		[Header("Fixe velocity")]
		public Vector3 velocity = Vector3.up;

		[Header("Random velocity")]
		public bool isRandomVelocity = false;
		public Vector3 minVelocity = Vector2.zero;
		public Vector3 maxVelocity = Vector3.zero;

		[Header("Gravity")]
		public Vector3 gravity = new Vector3(0,-9.81f,0);

		private Text textUI;
		private Transform cachedTransform;
		private Camera cachedCamera;
		private CanvasRenderer cachedCanvasRenderer;
		private Canvas cachedCanvas;
		private Rect visibleViewportRect = new Rect(0,0,1,1);
		private float life=0;
		private GameEntity entity;
		private Vector3 localVelocity;
		private float resizePoint;
		private float size;
		private Transform owner;
		#endregion

		#region Monobehaviour Callback
		void Awake(){
			textUI = GetComponentInChildren<Text>();
			cachedTransform = GetComponent<Transform>();
			cachedCamera = Camera.main;
			cachedCanvasRenderer = GetComponentInChildren<CanvasRenderer>();
			cachedCanvas = GetComponentInChildren<Canvas>();
			cachedCanvasRenderer.SetAlpha(0);
		}
			
		void Update () {

			life += Time.deltaTime;

			Vector3 entityViewportPoint = cachedCamera.WorldToViewportPoint(cachedTransform.position);
			bool visible = visibleViewportRect.Contains(entityViewportPoint) && entityViewportPoint.z <= maxDistanceFromCamera;


			if (visible && GameManager.instance.showCombatText){

				if (life>= fadeTime && visible){
					cachedCanvasRenderer.SetAlpha(1f - (life-fadeTime) * 1f/fadeTime  );
				}
				else{
					cachedCanvasRenderer.SetAlpha(1f);
				}

				// bilboard
				if (cachedCamera){
					cachedTransform.rotation = Quaternion.LookRotation(cachedCamera.transform.forward,  cachedCamera.transform.up);
					if (scaleEffect){
						currentSize -= (size/lifeTime) * Time.deltaTime;
						cachedTransform.localScale = new Vector3( currentSize,currentSize,currentSize);
					}
				}
			}
			else{
				cachedCanvasRenderer.SetAlpha(0);
			}

			// Velocity
			localVelocity += gravity * Time.deltaTime;
			cachedTransform.position += localVelocity*Time.deltaTime;

		}

		void OnSpawn(){

			if (!GameManager.instance.showCombatText){
				DestroyMe();
			}
				
			localVelocity = velocity;
			if (isRandomVelocity){
				localVelocity = new Vector3( Random.Range(minVelocity.x,maxVelocity.x),Random.Range(minVelocity.y,maxVelocity.y),Random.Range(minVelocity.z,maxVelocity.z));
			}
			life =0;
			Invoke( "DestroyMe",lifeTime);
			textUI.enabled = true;

			order++;
			cachedCanvas.sortingOrder = order;

		}

		void DestroyMe(){
			if (order == cachedCanvas.sortingOrder){
				order = 0;
			}
			cachedCanvasRenderer.SetAlpha(0);
			PoolManager.Despawn( gameObject);
		}
		#endregion

		#region Private methods
		private float currentSize;
		#endregion

		#region Public methods
		public void SetDamageValue(float point){

			if (Mathf.Floor(point)!=0){
				point = Mathf.Floor( point);
				resizePoint = point;
				textUI.text = prefixLabel +  point.ToString("f0") + suffixLabel;
				textUI.color = gradient.Evaluate( resizePoint * 1f/maxPointValue);

				size = Mathf.Clamp(resizePoint,10,maxPointValue)/maxPointValue;
				size = MathH.Remap( size,0,1,minSize,maxSize)/10;
				currentSize = size;
				transform.localScale = new Vector3( size,size,size);
			}
			else{
				textUI.text ="";
				CancelInvoke();
				DestroyMe();
			}
		}

		public void InitHitEffect(Impact impact){
			
			owner = impact.target.transform;

			if (useObjectPivot){
				if ( !localOffset){
					cachedTransform.position = owner.position + offset ;
				}
				else{
					cachedTransform.position = owner.TransformPoint( offset );
				}
			}

			SetDamageValue( impact.damagePoint );
		}

		public void InitHealEffect(float value){
			SetDamageValue( value );
		}
		#endregion
	}

}