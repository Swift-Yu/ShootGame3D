using UnityEngine;
using System.Collections;

namespace HedgehogTeam{

	[AddComponentMenu("Hedgehog Team/Helper/Random Sound")]
	[RequireComponent (typeof(AudioSource))]
	public class RandomizeSound : MonoBehaviour {

		public AudioClip[] sounds;
		private AudioSource audioSource;

		void Awake(){
			audioSource = GetComponent<AudioSource>();
		}

		void OnSpawn(){

			int rndIndex= Random.Range(0,sounds.Length);
			audioSource.clip = sounds[rndIndex ];
			audioSource.Play();
		}
	}

}
