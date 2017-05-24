using UnityEngine;
using System.Collections;
using HedgehogTeam.CoreShooterKit;

public class EnemyMovement : MonoBehaviour {

	private GameEntity player;              
	private NavMeshAgent nav;               
	private Animation anim; 

	void Awake (){
		nav = GetComponent <NavMeshAgent> ();
		anim = GetComponent<Animation>();
	}

	void OnSpawn(){
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<GameEntity>();
		anim.CrossFade("soldierWalk");
	}

	void Update (){

		if (player.life>0){
			nav.SetDestination (player.transform.position);

			// Look at player
			if (nav.remainingDistance<nav.stoppingDistance && nav.hasPath && nav.velocity.magnitude<0.1f){
				anim.CrossFade("soldierIdle");
				Quaternion lookAT = Quaternion.LookRotation(player.transform.position - transform.position);
				transform.rotation = Quaternion.Slerp( transform.rotation, Quaternion.Euler(0,lookAT.eulerAngles.y,0), nav.angularSpeed * Time.deltaTime);
			}
			else{
				anim.CrossFade("soldierWalk");
			}
	
		}
		else{
			nav.enabled = false;
		}


	}
}