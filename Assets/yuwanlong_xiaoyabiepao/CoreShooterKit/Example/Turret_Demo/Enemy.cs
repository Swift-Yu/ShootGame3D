using UnityEngine;
using System.Collections;
using System.IO;

public class Enemy : MonoBehaviour {

	public Vector3 destination;
	private  NavMeshAgent agent;

	void OnSpawn () {
		agent = GetComponent<NavMeshAgent>();
		NavMeshPath path = new NavMeshPath();
		agent.CalculatePath( destination, path);
		agent.SetPath( path );
	}
	
}
