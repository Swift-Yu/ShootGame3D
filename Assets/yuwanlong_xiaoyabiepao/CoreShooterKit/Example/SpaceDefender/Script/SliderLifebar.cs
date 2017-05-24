using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using HedgehogTeam.CoreShooterKit;

public class SliderLifebar : MonoBehaviour {

	public GameEntity entity;
	private Image sliderFill;
	private Slider slider;


	// Use this for initialization
	void Awake () {
		slider= GetComponentInChildren<Slider>();
		sliderFill = slider.GetComponentsInChildren<Image>().First(i=>i.name.Equals("Fill"));
	}
	
	// Update is called once per frame
	void Update () {
		if (entity.life>0.1f){
			sliderFill.enabled = true;
			float t = Mathf.InverseLerp(0, entity.maxLife, entity.life);
			slider.value = t;
		}
		else{
			sliderFill.enabled = false;
		}
	}
}
