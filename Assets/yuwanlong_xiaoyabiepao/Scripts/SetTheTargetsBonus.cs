using UnityEngine;
using System.Collections;
using ShootingGallery;

public class SetTheTargetsBonus : MonoBehaviour
{

    [SerializeField] private GameObject[] TargetsOfYellow;
    [SerializeField] private GameObject[] TargetsOfBrown;
    [SerializeField] private GameObject[] TargetsOfColored;
    [SerializeField] private int ScoreOfYellow;
    [SerializeField] private int ScoreOfBrown;
    [SerializeField] private int ScoreOfColored;
    [SerializeField] private AudioClip YellowHitClip;
    [SerializeField] private AudioClip BrownHitClip;
    [SerializeField] private AudioClip ColoredHitClip;
    [SerializeField] private GameObject YellowHitEffect;
    [SerializeField] private GameObject BrownHitEffect;
    [SerializeField] private GameObject ColoredHitEffect;


	// Use this for initialization
	void Start ()
    {
	    for (int i = 0; i < TargetsOfYellow.Length; i++)
	    {
	        TargetsOfYellow[i].transform.GetComponent<SGTTarget>().ScoreOfTargets = new Vector2(ScoreOfYellow,ScoreOfYellow);
	        if (YellowHitEffect)
	        {
                TargetsOfYellow[i].transform.GetComponent<SGTTarget>().specialEffect = YellowHitEffect;
            }
	        if (YellowHitClip)
	        {
	            TargetsOfYellow[i].transform.GetComponent<SGTTarget>().soundHit = YellowHitClip;
	        }
	    }
        for (int i = 0; i < TargetsOfBrown.Length; i++)
        {
            TargetsOfBrown[i].transform.GetComponent<SGTTarget>().ScoreOfTargets = new Vector2(ScoreOfBrown, ScoreOfBrown);
            if (BrownHitEffect)
            {
                TargetsOfBrown[i].transform.GetComponent<SGTTarget>().specialEffect = BrownHitEffect;
            }
            if (BrownHitClip)
            {
                TargetsOfBrown[i].transform.GetComponent<SGTTarget>().soundHit = BrownHitClip;
            }
        }
        for (int i = 0; i < TargetsOfColored.Length; i++)
        {
            TargetsOfColored[i].transform.GetComponent<SGTTarget>().ScoreOfTargets = new Vector2(ScoreOfColored, ScoreOfColored);
            if (ColoredHitEffect)
            {
                TargetsOfColored[i].transform.GetComponent<SGTTarget>().specialEffect = ColoredHitEffect;
            }
            if (ColoredHitClip)
            {
                TargetsOfColored[i].transform.GetComponent<SGTTarget>().soundHit = ColoredHitClip;
            }
        }
    }
}
