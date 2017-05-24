using UnityEngine;
using System.Collections;
using HedgehogTeam.CoreShooterKit;

public class Sight : MonoBehaviour {
    /*                 解决准心不准的问题                    */
    private Weapon weapon;
    public float maxDistance=50;
    public LayerMask mask;
    private RaycastHit hit;
    public Transform barrel;
	// Use this for initialization
	void Start ()
	{
	    weapon = GetComponent<Weapon>();
	    if (weapon)
	    {
	        weapon.onStartShooting.AddListener(
	            (s) =>
	            {
	                if (Physics.Raycast(
	                    Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen
	                                                                                   .height / 2, 0)), out hit,
	                    maxDistance,mask))
	                {
                        barrel.LookAt(hit.point);
	                }
	                else
	                {
                        //朝向最远处
	                    Vector3 target =Camera.main.transform.position+
                            Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2
                               , 0)).direction.normalized * maxDistance;
                        barrel.LookAt(target);
	                                         
	                }
	            });
	    }
	}
	
}
