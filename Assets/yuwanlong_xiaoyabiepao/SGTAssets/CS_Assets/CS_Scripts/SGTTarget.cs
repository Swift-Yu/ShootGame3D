using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ShootingGallery
{
	/// <summary>
	/// This script defines a target that can be shot and bounced and destroyed.
	/// </summary>
	public class SGTTarget:MonoBehaviour 
	{	
		internal GameObject GameController;

		// 得分累加器，暂时弃用
        [HideInInspector]
		public Vector2 bonusMultiplier = new Vector2(1,1);

		// 时间奖励累加器，暂时弃用
        [HideInInspector]
		public Vector2 timeBonusMultiplier = new Vector2(1,1);

        // 添加：鸭子皮肤自带分数，此参数由Setting进行分配设置
        [HideInInspector]
	    public Vector2 ScoreOfTargets;

		// How long to wait before showing the target
		internal float showTime = 0;

		// How long to wait before hiding the target, after it has been revealed
		internal float hideDelay = 0;

		// Various animations for showing/hiding, and hitting targets
		public string hitAnimation = "HitTarget";
		public string showAnimation = "ShowTarget";
		public string hideAnimation = "HideTarget";

		// Has the target been hit?
		public bool isHit = false;

		// Is the target hidden?
		public bool isHidden = true;

		// The sound that plays when this object is hit
		public AudioClip soundHit;

        //special effect
        [HideInInspector]
	    public GameObject specialEffect;

		// The source from which sound for this object play
		public string soundSourceTag = "GameController";

		// The audiosource from which sounds play
		internal GameObject soundSource;

		// A random range for the pitch of the audio source, to make the sound more varied
		public Vector2 pitchRange = new Vector2( 0.9f, 1.1f);

	    private BoxCollider box;
		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
            box = transform.GetComponent<BoxCollider>() as BoxCollider;
            // Hold the gamcontroller object in a variable for quicker access
            GameController = GameObject.FindGameObjectWithTag("GameController");

			//Assign the sound source for easier access
			if ( GameObject.FindGameObjectWithTag(soundSourceTag) )    soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);
		}

        
		/// <summary>
		/// Update this instance.
		/// </summary>
		void Update()
		{
           
            if ( isHit == false && isHidden == false && hideDelay > 0 )
			{
				hideDelay -= Time.deltaTime;

				if ( hideDelay <= 0 )    HideTarget();
			}
		    //if (isHidden)
		    //{
		    //    box.enabled = false;
		    //}
		    //else
		    //{
		    //    box.enabled = true;
		    //}
		}

	    public void HitTheTarget(Transform hitSource)
	    {
	        HitTarget(hitSource);
	    }
		/// <summary>
		/// Hits the target, giving hit bonus and playing a sound.
		/// </summary>
		/// <param name="hitSource">Hit source.</param>
		void HitTarget( Transform hitSource )
		{
			if ( isHit == false /*&& isHidden == false*/ )
			{
				// The target has been hit. It can't be hit again until it resets
				isHit = true;

				// Play the hit animation, which also hides the target
				GetComponent<Animation>().Play(hitAnimation);

				// Reset the target after waiting to the hit animation duration
				StartCoroutine(ResetTarget(GetComponent<Animation>().GetClip(hitAnimation).length));

				//修改得分累加器，弃用，分数固定
				//GameController.SendMessage("SetBonusMultiplier", Mathf.RoundToInt(Random.Range(bonusMultiplier.x, bonusMultiplier.y)));

				//时间累加器，弃用，值始终为0，单个击中目标时不再有时间奖励
				//GameController.SendMessage("SetTimeBonusMultiplier", Mathf.RoundToInt(Random.Range(timeBonusMultiplier.x, timeBonusMultiplier.y)));

                //获取当前目标的射击得分
                GameController.SendMessage("SetHitTargetBonus", Mathf.RoundToInt(Random.Range(ScoreOfTargets.x, ScoreOfTargets.y)));

				// Give hit bonus for this target
				GameController.SendMessage("HitBonus", hitSource);

                //If there is a special effect, show it when its lift time
                //if (specialEffect)
                //{
                //    while (specialEffect.gameObject.activeSelf)
                //    {
                //        specialEffect.gameObject.SetActive(false);
                //    }
                //    specialEffect.gameObject.SetActive(true);
                //}
			    if (specialEffect)
			    {
                    GameObject newSpecialEffect = Instantiate(specialEffect, hitSource.parent) as GameObject;
                    newSpecialEffect.GetComponent<RectTransform>().position = new Vector3(hitSource.position.x,hitSource.position.y+4,hitSource.position.z-4);;
			    }
                // If there is a sound source and a sound assigned, play it
                if ( soundSourceTag != "" && soundHit )    
				{
					//Reset the pitch back to normal
					GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().pitch = Random.Range( pitchRange.x, pitchRange.y);;
					
					//Play the sound
					GameObject.FindGameObjectWithTag(soundSourceTag).GetComponent<AudioSource>().PlayOneShot(soundHit);
				}

				// If there is a source and a sound, play it from the source
				if ( soundSource && soundHit )    soundSource.GetComponent<AudioSource>().PlayOneShot(soundHit);
			}
		}

		/// <summary>
		/// Hides the target, animating it and then sets it to hidden
		/// </summary>
		void HideTarget()
		{
		    if (box != null && box.enabled == true)
		    {
                box.enabled = false;
            }
			// Play the hiding animation
			GetComponent<Animation>().Play(hideAnimation);

			// Reset the target after waiting to the hiding animation duration
			StartCoroutine(ResetTarget(0));
		}

		/// <summary>
		/// Shows the target, animating it and then sets it to unhidden
		/// </summary>
		/// <returns>The target.</returns>
		IEnumerator ShowTarget( float showDuration )
		{
		    box.enabled = true;
			// Show the target only if it was hidden before
			if ( isHidden == true )
			{
                isHidden = false;
                // Play the show animation
                GetComponent<Animation>().Play(showAnimation);

				// Wait for the show animation duration
				yield return new WaitForSeconds(GetComponent<Animation>().GetClip(showAnimation).length);

				// The target is not hidden anymore
				//isHidden = false;

				// Set how long to wait before hiding the target again
				hideDelay = showDuration;
			}	
		}

		/// <summary>
		/// Resets the target to its hidden and unhit status
		/// </summary>
		/// <returns>The target.</returns>
		/// <param name="delay">How many seconds to wait before resetting the target</param>
		IEnumerator ResetTarget( float delay )
		{
			// Wait some time
			yield return new WaitForSeconds(delay);

			// The target is hidden
			isHidden = true;

			// The target is not hit
			isHit = false;
		}

	    public void onHit()
	    {
	        HitTarget(transform);
	    }
	}
}
