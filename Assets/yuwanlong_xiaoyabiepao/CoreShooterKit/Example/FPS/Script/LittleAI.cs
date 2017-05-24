using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;

public class LittleAI : MonoBehaviour {

	public Transform[] fleePosition;

	private GameObject player;
	private NavMeshAgent agent;
	private int fleeIndex = 0;
	private bool update2Player = false;
	private bool inFlee = false;
	private bool wait2Flee = false;
	private Animation anim;

	void Awake(){
		player = GameObject.FindWithTag("Player");
		agent = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animation>();
		Random.seed = 153;
	}

	// Update is called once per frame
	void Update () {

		if (update2Player && !inFlee){
			
			Quaternion lookat = Quaternion.LookRotation( player.transform.position-transform.position);
			lookat = Quaternion.Euler(0, lookat.eulerAngles.y,0);
			transform.rotation = lookat;
		}
	
		if (inFlee){
			agent.SetDestination( fleePosition[fleeIndex].position);
			agent.transform.position = transform.position;

			if (Vector3.Distance( fleePosition[fleeIndex].position, transform.position)<=1f){
				inFlee = false;
				anim.CrossFade("soldierIdle");
				agent.Stop();
			}
		}
	}

	public void  OnKill(){
		StopAllCoroutines();
	}
		
	public void OnStartShoot(){
		update2Player = true;
	}

	public void OnHit(){

		if (fleePosition.Length>0 && !inFlee && !wait2Flee){
			int tmpIndex = fleeIndex;
			while (tmpIndex == fleeIndex){
				tmpIndex = Random.Range(0,fleePosition.Length);
			}
			wait2Flee = true;
			fleeIndex = tmpIndex;
			StartCoroutine( LaunchFlee());
		}
	}

	IEnumerator LaunchFlee(){
		yield return new WaitForSeconds(0.5f);
		if (agent){
			agent.Resume();
			inFlee = true;
			wait2Flee = false;
			anim.CrossFade("soldierRun");
		}

	}

}



/*
using UnityEngine;

namespace UnitySampleAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof (NavMeshAgent))]
	[RequireComponent(typeof (ThirdPersonCharacter))]
	public class AICharacterControl : MonoBehaviour
	{

		public NavMeshAgent agent { get; private set; } // the navmesh agent required for the path finding
		public ThirdPersonCharacter character { get; private set; } // the character we are controlling
		public Transform target; // target to aim for
		public float targetChangeTolerance = 1; // distance to target before target can be changed

		private Vector3 targetPos;

		// Use this for initialization
		private void Start()
		{
			// get the components on the object we need ( should not be null due to require component so no need to check )
			agent = GetComponentInChildren<NavMeshAgent>();
			character = GetComponent<ThirdPersonCharacter>();
		}


		// Update is called once per frame
		private void Update()
		{
			if (target != null)
			{
				// update the progress if the character has made it to the previous target
				if ((target.position - targetPos).magnitude > targetChangeTolerance)
				{
					targetPos = target.position;
					agent.SetDestination(targetPos);
				}

				// update the agents posiiton 
				agent.transform.position = transform.position;

				// use the values to move the character
				character.Move(agent.desiredVelocity, false, false, targetPos);
			}
			else
			{
				// We still need to call the character's move function, but we send zeroed input as the move param.
				character.Move(Vector3.zero, false, false, transform.position + transform.forward*100);

			}
		}

		public void SetTarget(Transform target)
		{
			this.target = target;
		}
	}

}
*/


