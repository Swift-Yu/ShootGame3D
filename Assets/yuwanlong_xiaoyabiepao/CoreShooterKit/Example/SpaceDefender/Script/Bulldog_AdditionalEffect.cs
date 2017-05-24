using UnityEngine;
using System.Collections;

public class Bulldog_AdditionalEffect : MonoBehaviour {

	public AudioClip need2Reload;
	public AudioClip reload;
	public AudioSource audioSource;

	public void NeedToReload(){
		audioSource.PlayOneShot( need2Reload,1);
	}

	public void Reload(){
		audioSource.PlayOneShot( reload,1);
	}
}
