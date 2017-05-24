using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

namespace HedgehogTeam.CoreShooterKit{

	public class UIPlayerLifeBar : MonoBehaviour {

		public Gradient healthGradient;
		public Text lifeText;

		private GameEntity player;
		private Slider slider;
		private Image sliderFill;

		void Awake(){
			slider= GetComponentInChildren<Slider>();
			sliderFill = slider.GetComponentsInChildren<Image>().First(i=>i.name.Equals("Fill"));
		}

		void Update () {
			if (player){
				float t = Mathf.InverseLerp(0, player.maxLife,player.life);
				slider.value = t;
				sliderFill.color = healthGradient.Evaluate(t);

				if (lifeText){
					lifeText.text = player.life.ToString("f0");
				}
					
			}
			else{
				GetPlayer();

			}

			if (!player || !player.gameObject.activeInHierarchy){
				sliderFill.color = healthGradient.Evaluate(0);
				if (lifeText){
					lifeText.text ="0";
				}
			}
		}

		private void GetPlayer(){

			GameObject tmpObj = GameObject.FindGameObjectWithTag("Player");
			if (tmpObj){
				player = tmpObj.GetComponent<GameEntity>();
			}
		}
	}
}