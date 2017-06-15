#if UNITY_5_3 || UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ShootingGallery.Types;
using HedgehogTeam.CoreShooterKit;
using HedgehogTeam.EasyPoolManager;
using Showbaby.Bluetooth;
using UnityDebug.Log;
using UnityEngine.Events;
using UnityStandardAssets.Characters.FirstPerson;

namespace ShootingGallery
{
	/// <summary>
	/// This script controls the game, starting it, following game progress, and finishing it with game over.
	/// </summary>r
	public class SGTGameController:MonoBehaviour
	{
        [HideInInspector]
        private Weapon weapon;
	    public Weapon OneShootWeapon;
	    public Weapon MachineWeapon;
	    private float MachineGunTimeLimit = 0f;
	    public float MachineTime = 10f;
        [HideInInspector]
	    public bool isMachineTime = false;
        public StaticFirstPerson firstPerson;
	    public Camera MCamera;
	    private int shootCount = 0;
	    private int hitCount = 0;
	    //public GyroController gyro;
		// How long to wait before starting the game. Ready?GO! time
		public float startDelay = 1;

		// The effect displayed before starting the game
		public Transform readyGoEffect;

		// How many seconds are left before game over
		public float timeLeft = 30;

        //how many time used when you finish your shoot
        [HideInInspector]
	    public float shootTime;

        //level left time to bonus;
        [HideInInspector]
        public float leftTimeToBonus;

		// The text object that displays the time
		public Text timeText;

		// A list of moving targets ( The duck rows )
		public Transform[] movingTargets; 

		// The speed of the moving targets
		public float movingSpeed = 2;

		// How many targets to show at once
		public int maximumTargets = 5;

		// The horizontal warea within which targets are shown. Targets outside of this area will never be shown
		public float targetShowArea = 50;

		// How long to wait before showing the targets
		public float showDelay = 3;
		internal float showDelayCount = 0;
		
		// How long to wait before hiding the targets again
		public float hideDelay = 1;
		internal float hideDelayCount = 0;

		// The left and right edges of the game area. Targets bounce off these edges.
		public Transform leftEdge;
		public Transform rightEdge;

		// The shoot button, click it or tap it to shoot
		public string shootButton = "Fire1";

		// The keyboard/gamepad button for reloading
		public string reloadButton = "Jump";

		// The bullet/shot that appears when you shoot
		public GameObject shotObject;

	    //public Transform effectPoint;

		// The maximum number of bullets you can have
		public int ammo = 6;
		
		// The number of bullets left
		internal float ammoLeft;
		
		// The image showing how many bullets we have left
		public Image ammoBar;
		
		// The width of a single bullet in the ammo bar
		public float ammoBarWidth = 21;

		// The point at which you aim when shooting. Used for mobile and gamepad/keyboard controls
		public Transform crosshair;
		
		// How fast the crosshair moves
		public float crosshairSpeed = 15;

		// Are we using the mouse now?
		internal bool usingMouse = false;

		// The position we are aiming at now
		internal Vector3 aimPosition;

		// How many points we get when we hit a target. This bonus is multiplied by the number of targets on screen
		public int hitTargetBonus = 10;

	    public int BoomPenalty = 10;

        // The bonus multiplier that is affected by the type of target we hit
        public float bonusMultiplier = 1;

		// The bonus effect that shows how much bonus we got when we hit a target
		public Transform bonusEffect;

		// How many seconds we earn when we hit a target. This time bonus is multiplied by the number of targets on screen
		public int hitTargetTimeBonus = 0;

	    public GameObject levelUpEffect;
		
		// The time bonus multiplier that is affected by the type of target we hit
		public float timeBonusMultiplier = 0;
		
		// The effect that shows how much time bonus we got when we hit a target
		public Transform timeBonusEffect;

		internal Transform currentSpecialTarget;
		internal Transform currentReplacement;

		// Counts the current streak
		internal int streak = 1;

		// The score and score text of the player
		public int score = 0;
		public Transform scoreText;
		internal int highScore = 0;
		internal int scoreMultiplier = 1;

		// The overall game speed
		public float gameSpeed = 1;
		
		//How many points the player needs to collect before leveling up
		public Level[] levels;
		public int currentLevel = 0;
	    public int levelLimited = 18;

		// The game will continue forever after the last level
		public bool isEndless = false;

		// The chance for a special target to appear
		public float specialTargetChance = 0.01f;

		// The index number of the current special target
		internal int specialTargetIndex;

		// Various canvases for the UI
		public Transform gameCanvas;
		public Transform progressCanvas;
		public Transform pauseCanvas;
		public Transform gameOverCanvas;
		public Transform victoryCanvas;
        public Transform RestartProgress;
	    public TipUI TipUI;
        // Is the game over?
        internal bool  isGameOver = false;
		
		// The level of the main menu that can be loaded after the game ends
		public string mainMenuLevelName = "CS_StartMenu";
		
		// Various sounds and their source
		public AudioClip soundReload;
		public AudioClip soundLevelUp;
		public AudioClip soundGameOver;
		public AudioClip soundVictory;
        
        //背景音频
	    public AudioSource GameSource;
        //倒计时音频
	    public AudioSource timer;
        //倒计时最后一秒
	    public AudioSource timerlast;
        //readyGo音频
        public AudioSource ReadyGoSource;
        //游戏结束循环
	    public AudioSource GameoverLoop;

		public string soundSourceTag = "GameController";
		internal GameObject soundSource;
		
		// The button that will restart the game after game over
		public string confirmButton = "Submit";
		
		// The button that pauses the game. Clicking on the pause button in the UI also pauses the game
		public string pauseButton = "Cancel";
		internal bool  isPaused = false;

		// A general use index
		internal int index = 0;
	    public GameEntity player;
        //public Transform slowMotionEffect;
        [SerializeField]
        private UnityEvent onPerfectShoot;
        [SerializeField]
        private UnityEvent onCleanShoot;
	    [SerializeField]
        private UnityEvent onFastShoot;

        //游戏开始接口，提供事件接入
	    [SerializeField] private UnityEvent onStartGame;
        //射击炸弹接口，提供事件接入
	    [SerializeField] private UnityEvent onShootBoom;
        
	    [SerializeField]
        private SetEffect setEffect;
        [SerializeField]
        private GameObject AllHitEffect;
	    [SerializeField]
        private GameObject PerfectHitEffect;
	    [SerializeField]
        private GameObject FastHitEffect;

	    public bool isfound = false;
        //private bool isPerfect = false;
	    public int propCount = 0;    //道具计数，这里指的是每一波中道具的个数
        //private bool isFast = false;

        void Awake()
        {
            //OneShootWeapon.Controller = this.gameObject;
            //MachineWeapon.Controller = this.gameObject;
            weapon = OneShootWeapon;
		    weapon.Init(player.gameObject, player.faction);
            TipUI.ShowTipWindow("对准识别图，开始游戏", "Aim at the target to start the game");
		    // Activate the pause canvas early on, so it can detect info about sound volume state
		    //if (pauseCanvas) pauseCanvas.gameObject.SetActive(true);
		}

		/// <summary>
		/// Start is only called once in the lifetime of the behaviour.
		/// The difference between Awake and Start is that Start is only called if the script instance is enabled.
		/// This allows you to delay any initialization code, until it is really needed.
		/// Awake is always called before any Start functions.
		/// This allows you to order initialization of scripts
		/// </summary>
		void Start()
		{
            Debug.LogError("currentlevel is "+currentLevel);
            //gyro.EnableGyro();
            // Check if we are running on a mobile device. If so, remove the crosshair as we don't need it for taps
            if ( Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WP8Player )    
			{
				// If a crosshair is assigned, hide it
				//if ( crosshair )    crosshair.gameObject.SetActive(false);
				
				//crosshair = null;
			}

			//Update the score
			UpdateScore();

			// Set the ammo we have
			ammoLeft = ammo;

			// Update the ammo
			UpdateAmmo();

			//Hide the cavases
			if ( gameOverCanvas )    gameOverCanvas.gameObject.SetActive(false);
			if ( victoryCanvas )    victoryCanvas.gameObject.SetActive(false);
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);

			//Get the highscore for the player
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);
			#else
			highScore = PlayerPrefs.GetInt(Application.loadedLevelName + "HighScore", 0);
			#endif

			//Assign the sound source for easier access
			if ( GameObject.FindGameObjectWithTag(soundSourceTag) )    soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

			// Reset the spawn delay
			showDelayCount = 0;

            // Check what level we are on
            //UpdateLevel();
            //在开始的时候重置游戏，关卡数据重置到第一关
            // Update the timer
            UpdateTime();

            //正确避免关卡0的出现
            progressCanvas.Find("Text").GetComponent<Text>().text = (currentLevel + 1).ToString();

            // Set the maximum number of targets
            maximumTargets = levels[0].maximumTargets;

            // Update the game speed
            movingSpeed = levels[0].movingSpeed;

            GetGameProps(levels[0].GunRate,levels[0].BoomRate);
            // Move the targets from one side of the screen to the other, and then reset them
            foreach ( Transform movingTarget in movingTargets )
			{
			    if (movingTarget.gameObject.activeSelf)
			    {
                    movingTarget.SendMessage("HideTarget");
                }            
            }

			// Show the replacement target
			if ( currentReplacement )    currentReplacement.gameObject.SetActive(true);

			// ReadyGo放到识别图识别时
			//if ( readyGoEffect )    Instantiate( readyGoEffect );
		}

		/// <summary>
		/// Update is called every frame, if the MonoBehaviour is enabled.
		/// </summary>
		void  Update()
		{
            //若机枪模式则开启机枪道具计时
            if (isMachineTime && MachineGunTimeLimit > 0)
            {
                weapon = MachineWeapon;
                MachineGunTimeLimit -= Time.deltaTime;
            }
            else
            {
                //在从机枪切换回手枪模式时候需要正确的关闭机枪
                weapon.StopShoot();
                isMachineTime = false;
                weapon = OneShootWeapon;
            }
            // Delay the start of the game
            if ( startDelay > 0 )
			{
				startDelay -= Time.deltaTime;
			}
			else
			{
				// Move the targets from one side of the screen to the other, and then reset them
				foreach ( Transform movingTarget in movingTargets )
				{
					// Check the direction of movement
					if ( movingTarget.localScale.x > 0 )
					{
						// Move to the right
						movingTarget.position = new Vector3( movingTarget.position.x + movingSpeed * Time.deltaTime, movingTarget.position.y, movingTarget.position.z);

						// When the target reaches the right edge, reset it to the left edge
						if ( movingTarget.position.x > rightEdge.position.x )    movingTarget.position = new Vector3( leftEdge.position.x, movingTarget.position.y, movingTarget.position.z);
					}
					else
					{
						// Move to the left
						movingTarget.position = new Vector3( movingTarget.position.x - movingSpeed * Time.deltaTime, movingTarget.position.y, movingTarget.position.z);

						// When the target reaches the right edge, reset it to the left edge
						if ( movingTarget.position.x < leftEdge.position.x )    movingTarget.position = new Vector3( rightEdge.position.x, movingTarget.position.y, movingTarget.position.z);
					}
				}

				// Check the direction of movement
				//if ( currentSpecialTarget )
				//{

				//	if ( currentSpecialTarget.localScale.x > 0 )
				//	{
				//		// Move to the right
				//		currentSpecialTarget.position = new Vector3( currentSpecialTarget.position.x + movingSpeed * Time.deltaTime, currentSpecialTarget.position.y, currentSpecialTarget.position.z);
						
				//		// When the target reaches the right edge, reset it to the left edge
				//		if ( currentSpecialTarget.position.x > rightEdge.position.x )    currentSpecialTarget.position = new Vector3( leftEdge.position.x, currentSpecialTarget.position.y, currentSpecialTarget.position.z);
				//	}
				//	else
				//	{
				//		// Move to the left
				//		currentSpecialTarget.position = new Vector3( currentSpecialTarget.position.x - movingSpeed * Time.deltaTime, currentSpecialTarget.position.y, currentSpecialTarget.position.z);
						
				//		// When the target reaches the right edge, reset it to the left edge
				//		if ( currentSpecialTarget.position.x < leftEdge.position.x )    currentSpecialTarget.position = new Vector3( rightEdge.position.x, currentSpecialTarget.position.y, currentSpecialTarget.position.z);
				//	}
				//}

				//If the game is over, listen for the Restart and MainMenu buttons
				if ( isGameOver == true )
				{
                    weapon.gameObject.SetActive(false);
					//The jump button restarts the game
					if ( Input.GetButtonDown(confirmButton) )
					{
						Restart();
					}
					
					//The pause button goes to the main menu
					if ( Input.GetButtonDown(pauseButton) )
					{
						MainMenu();
					}
				}
				else
				{
					// If we press the reload button, reload!
					if ( Input.GetButtonDown(reloadButton) )
					{
						Reload();
					}

					// Count down the time until game over
					if ( timeLeft > 0 && isfound)
					{
						// Count down the time
						timeLeft -= Time.deltaTime;
                        if (timeLeft > 10)
                        {
                            timer.Stop();
                            timerlast.Stop();
                        }
                        else
                        {
                            if (timeLeft < 10 && timeLeft > 1 && !timer.isPlaying)
                            {
                                timer.Play();
                            }
                            if (timeLeft <= 1 && !timerlast.isPlaying)
                            {
                                timer.Stop();
                                timerlast.Play();
                            }
                        }
					    shootTime += Time.deltaTime;
						// Update the timer
						UpdateTime();
					}

					// Keyboard and Gamepad controls
					if ( crosshair )
					{
						// If we move the mouse in any direction, then mouse controls take effect
						if ( Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.GetMouseButtonDown(0) || Input.touchCount > 0 )    usingMouse = true;

						// We are using the mouse, hide the crosshair
						if ( usingMouse == true )
						{
							// Calculate the mouse/tap position
							//aimPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
						    aimPosition = MCamera.transform.position;
						    // Make sure it's 2D
						    //aimPosition.z = 0;
						}

                        // If we press gamepad or keyboard arrows, then mouse controls are turned off
                        if ( Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 )    
						{
							usingMouse = false;
						}

						// Move the crosshair based on gamepad/keyboard directions
						aimPosition += new Vector3( Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), aimPosition.z) * crosshairSpeed * Time.deltaTime;
					
						// Limit the position of the crosshair to the edges of the screen
						// Limit to the left screen edge
						if ( aimPosition.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x )    aimPosition = new Vector3( Camera.main.ScreenToWorldPoint(Vector3.zero).x, aimPosition.y, aimPosition.z);
						
						// Limit to the right screen edge
						if ( aimPosition.x > Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x )    aimPosition = new Vector3( Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x, aimPosition.y, aimPosition.z);
						
						// Limit to the bottom screen edge
						if ( aimPosition.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y )    aimPosition = new Vector3( aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y, aimPosition.z);
						
						// Limit to the top screen edge
						if ( aimPosition.y > Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y )    aimPosition = new Vector3( aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y, aimPosition.z);

                        // Place the crosshair at the position of the mouse/tap, with an added offset
                        //crosshair.position = new Vector3(aimPosition.x,aimPosition.y,0);
                        // If we press the shoot button, SHOOT!
                        //UnityLog.InfoBlue("usingmouse is "+usingMouse);
#if UNITY_EDITOR
                        if (usingMouse == true && Input.GetButtonDown(shootButton))
					    {
                            weapon.Shoot();                         
                        }
					    if (usingMouse == true && Input.GetButtonUp(shootButton))
					    {
					        weapon.StopShoot();
					    }
#elif UNITY_ANDROID || UNITY_IPHONE
                         if (usingMouse == true && Input.GetButtonDown(shootButton))
					    {
                            PauseGame();                         
                        }
#endif
                    }
                    // Count down to the next target spawn
                    if ( showDelayCount > 0 )    showDelayCount -= Time.deltaTime;
					else 
					{
						// Reset the spawn delay count
						showDelayCount = showDelay;

						ShowTarget(maximumTargets);
					}

					//Toggle pause/unpause in the game
					if ( Input.GetButtonDown(pauseButton) )
					{
						if ( isPaused == true )
                            Unpause();
						else
                            Pause();
					}
                }
			}
		}

		/// <summary>
		/// Updates the timer text, and checks if time is up
		/// </summary>
		void UpdateTime()
		{
			// Update the timer text
			if ( timeText )    
			{
				timeText.text = timeLeft.ToString("00");
			}

			// Time's up!
			if ( timeLeft <= 0 )    
			{
				StartCoroutine(GameOver(0));
			}
		}

        /// <summary>
        /// 重置鸭子状态
        /// </summary>
	    void ResetTargetsToDuck()
	    {
	        for (int i = 0; i < movingTargets.Length; i++)
	        {
	            movingTargets[i].GetComponent<SGTTarget>().ChangeSprite("Duck");
	        }
	    }

        /// <summary>
        /// 抽取鸭子来替换成道具
        /// </summary>
        /// <param name="GunRate"></param>
        /// <param name="BoomRate"></param>
	    void GetGameProps(int GunRate, int BoomRate)
	    {
	        int GunCount = (int)(movingTargets.Length*GunRate)/100;
            int BoomCount = (int)(movingTargets.Length * BoomRate) / 100;
            Debug.Log("GunCount is "+GunCount);
            Debug.Log("BoomCount is "+BoomCount);
	        while (GunCount > 0)
	        {
                int randomTarget1 = Mathf.FloorToInt(Random.Range(0, movingTargets.Length));
                SGTTarget currenTarget = movingTargets[randomTarget1].GetComponent<SGTTarget>();
	            if (!currenTarget.isGun && !currenTarget.isBoom)
	            {
	                currenTarget.ChangeSprite("Gun");
	                GunCount--;
	            }
	        }
	        while (BoomCount > 0)
	        {
                int randomTarget2 = Mathf.FloorToInt(Random.Range(0, movingTargets.Length));
                SGTTarget currenTarget = movingTargets[randomTarget2].GetComponent<SGTTarget>();
	            if (!currenTarget.isBoom && !currenTarget.isGun)
	            {
	                currenTarget.ChangeSprite("Boom");
	                BoomCount--;
	            }
	        }
	    }
        
		/// <summary>
		/// Shows a random batch of targets. Due to the random nature of selection, some targets may be selected more than once making the total number of targets to appear smaller.
		/// </summary>
		/// <param name="targetCount">The maximum number of target that will appear</param>
		void ShowTarget( int targetCount )
		{
            Debug.Log("targetCount = "+targetCount);
			// Limit the number of tries when showing targets, so we don't get stuck in an infinite loop
			//int maximumTries = targetCount * 10;
		    propCount = 0;
		    shootCount = 0;
		    hitCount = 0;
		    string showType = "AllDuck";
			// Show several targets that are within the game area
		    while (targetCount > 0)
		    {
                //maximumTries--;
		        // Choose a random target
		        int randomTarget = Mathf.FloorToInt(Random.Range(0, movingTargets.Length));
		        SGTTarget currenTarget = movingTargets[randomTarget].GetComponent<SGTTarget>();
		        // If the chosen target is hidden, and is within the game area, show it!
		        if (Mathf.Abs(movingTargets[randomTarget].position.x) < targetShowArea && movingTargets[randomTarget].GetComponent<SGTTarget>().isHidden)
		        {
		            // There is a chance to show a special target
		            //if (Random.value < specialTargetChance && levels[currentLevel].specialTarget)
		            //{
		            //    //Create a new special target
		            //    currentSpecialTarget = Instantiate(levels[currentLevel].specialTarget) as Transform;

		            //    // Place the new target inside the moving targets row
		            //    currentSpecialTarget.SetParent(movingTargets[randomTarget].parent);

		            //    // Set the position of the special target
		            //    currentSpecialTarget.position = movingTargets[randomTarget].position;

		            //    // Set the scale of the special target
		            //    currentSpecialTarget.localScale = movingTargets[randomTarget].localScale;

		            //    // Show a random targets from the list of moving targets
		            //    currentSpecialTarget.SendMessage("ShowTarget", hideDelay);

		            //    // Clear the special target as we don't need it anymore
		            //    levels[currentLevel].specialTarget = null;

		            //    // Set the replacement target
		            //    currentReplacement = movingTargets[randomTarget];

		            //    // Hide the replacement target
		            //    currentReplacement.gameObject.SetActive(false);
		            //}
		            //else
		            //{
		            // Show a random targets from the list of moving targets
                    //当抽中的是道具时候，先计数
		            if (currenTarget.isBoom || currenTarget.isGun)
		            {
		                propCount++;
                        //计数完毕后判断当前波的道具个数是否已达上线,若达到则取消此次抽取结果
                        if (propCount > levels[currentLevel].MaxPropCountInTargets)
                        {
                            Debug.Log("prop is max");
                            propCount--;
                        }
                        else
                        {
                            //如果出现了机枪道具则提醒射击机枪
                            if (currenTarget.isGun)
                            {
                                showType = "HasGun";
                            }
                            //道具未超过上限，显示
                            movingTargets[randomTarget].SendMessage("ShowTarget", hideDelay);
                            targetCount--;
                        }
                        Debug.Log("currentTarget is prop");
                    }
		            else
                    { 
                        //当前物体是鸭子，直接显示
		                movingTargets[randomTarget].SendMessage("ShowTarget", hideDelay);
		                targetCount--;                        
		            }
		        }
            }
		    if (!isMachineTime && isfound)
		    {
                switch (showType)
                {
                    case "HasGun":
                        TipUI.ShowTipWindow("击中白色圆形靶，获取连射效果", "Hit the round white Target to gain continuous shooting");
                        break;
                    case "AllDuck":
                        TipUI.ShowTipWindow("射击鸭子", "Shoot the duck ");
                        break;
                }
            }
		    // Reset the streak when showing a batch of new targets
			streak = 1;
		}

		/// <summary>
		/// Give a bonus when the target is hit. The bonus is multiplied by the number of targets on screen
		/// </summary>
		/// <param name="hitSource">The target that was hit</param>
		void HitBonus( Transform hitSource)
		{
		    SGTTarget currenTarget = hitSource.GetComponent<SGTTarget>();
			// If we have a bonus effect
			if ( bonusEffect && hitTargetBonus > 0 && bonusMultiplier > 0 )
			{
                //如果打中炸弹，则扣分，显示红字
			    if (currenTarget.isBoom)
			    {
                    onShootBoom.Invoke();
                    // Create a new bonus effect at the hitSource position
                    Transform newTimeEffect = Instantiate(bonusEffect, hitSource.position, Quaternion.identity) as Transform;
                    Text text = newTimeEffect.GetChild(0).GetComponent<Text>() as Text;
			        text.color = Color.red;
                    //显示时间减少特效
                    newTimeEffect.position = new Vector3(newTimeEffect.position.x, newTimeEffect.position.y, newTimeEffect.position.z - 8);               
                    newTimeEffect.Find("Text").GetComponent<Text>().text = "-" + (BoomPenalty).ToString();
                    newTimeEffect.eulerAngles = Vector3.forward * Random.Range(-10, 10);
			        timeLeft -= BoomPenalty;
                    UpdateTime();
			    }
			    else if (currenTarget.isGun)
			    {
                    ChangeMachineGun();
			        Debug.LogError("hereChangeTheGun");
                    Debug.LogError("currentweapon is"+weapon.gameObject.name);
			    }
			    else
			    {
                    // Create a new bonus effect at the hitSource position
                    Transform newBonusEffect = Instantiate(bonusEffect, hitSource.position, Quaternion.identity) as Transform;
                    //显示得分特效
                    newBonusEffect.position = new Vector3(newBonusEffect.position.x, newBonusEffect.position.y, newBonusEffect.position.z - 8);
                    //修改得分方式：
                    //newBonusEffect.Find("Text").GetComponent<Text>().text = "+" + (hitTargetBonus * streak * bonusMultiplier).ToString();
                    newBonusEffect.Find("Text").GetComponent<Text>().text = "+" + (hitTargetBonus).ToString();
                    // Rotate the bonus text slightly
                    newBonusEffect.eulerAngles = Vector3.forward * Random.Range(-10, 10);
                    streak++;
                    hitCount++;
                    //现在只有打中鸭子的时候才有加分
                    ChangeScore(hitTargetBonus);
                }
            }

			//// If we have a time bonus effect
			//if ( timeBonusEffect && hitTargetTimeBonus > 0 && timeBonusMultiplier > 0 )
			//{
			//	// Create a new bonus effect at the hitSource position
			//	Transform newTimeBonusEffect = Instantiate(timeBonusEffect, hitSource.position, Quaternion.identity) as Transform;
				
			//	// Display the bonus value multiplied by a streak
			//	newTimeBonusEffect.Find("Text").GetComponent<Text>().text = (hitTargetTimeBonus * streak * timeBonusMultiplier).ToString();
				
			//	// Rotate the bonus text slightly
			//	newTimeBonusEffect.eulerAngles = Vector3.forward * Random.Range(-10,10);
			//}

		    if (hitCount == levels[currentLevel].maximumTargets - propCount)
		    {
		        if (shootCount == hitCount)
		        {
                     ChangeScore(levels[currentLevel].PerfectBonus);
		            if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
		            {
                        StartCoroutine(setEffect.HitEffect(PerfectHitEffect, levels[currentLevel].PerfectBonus.ToString(), "百发百中"));
                    }
		            else
		            {
                        StartCoroutine(setEffect.HitEffect(PerfectHitEffect, levels[currentLevel].PerfectBonus.ToString(), "Perfect"));
                    }                  
                     onPerfectShoot.Invoke();
		        }
		        else
		        {
                    ChangeScore(levels[currentLevel].CleanBonus);
		            if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
		            {
                        StartCoroutine(setEffect.HitEffect(AllHitEffect, levels[currentLevel].CleanBonus.ToString(), "全部击中"));
                    }
		            else
		            {
                        StartCoroutine(setEffect.HitEffect(AllHitEffect, levels[currentLevel].CleanBonus.ToString(), "Good"));
                    }
                    onCleanShoot.Invoke();
                }
		    }
            //时间奖励，现在在击中单个目标不再有时间奖励，弃用
            //timeLeft += hitTargetTimeBonus * streak * timeBonusMultiplier;

            //Update the timer
            //UpdateTime();
		}

        /// <summary>
        /// 变换机枪模式
        /// </summary>
        public void ChangeMachineGun()
        {
            isMachineTime = true;
            MachineGunTimeLimit = MachineTime;
            TipUI.ShowTipWindow("长按射击按钮，可连射","Hold down the trigger for continuous shooting");
        }

        void SetHitTargetBonus(int targetBonus)
	    {
	        hitTargetBonus = targetBonus;
	    }

		void SetBonusMultiplier( float multiplierValue )
		{
			bonusMultiplier = multiplierValue;
		}

		void SetTimeBonusMultiplier( float multiplierValue )
		{
			timeBonusMultiplier = multiplierValue;
		}

		/// <summary>
		/// Change the score
		/// </summary>
		/// <param name="changeValue">Change value</param>
		public  void  ChangeScore( float changeValue )
		{
			score += (int)changeValue;

			//Update the score
			UpdateScore();
		}
		
		/// <summary>
		/// Updates the score value and checks if we got to the next level
		/// </summary>
		void  UpdateScore()
		{
			//Update the score text
			if ( scoreText )    scoreText.GetComponent<Text>().text = score.ToString();

			// If we reached the required number of points, level up!jiu 
			if ( score >= levels[currentLevel].scoreToNextLevel )
			{
				if ( currentLevel < levels.Length - 1 )    LevelUp();
				else    if ( isEndless == false )    StartCoroutine(Victory(0));
			}

			// Update the progress bar to show how far we are from the next level
			if ( progressCanvas )
			{
				if ( currentLevel == 0 )    progressCanvas.GetComponent<Image>().fillAmount = score * 1.0f/levels[currentLevel].scoreToNextLevel * 1.0f;
				else    progressCanvas.GetComponent<Image>().fillAmount = (score - levels[currentLevel - 1].scoreToNextLevel) * 1.0f/(levels[currentLevel].scoreToNextLevel - levels[currentLevel - 1].scoreToNextLevel) * 1.0f;
			}
		}

		/// <summary>
		/// Set the score multiplier ( Get double score for hitting and destroying targets )
		/// </summary>
		void SetScoreMultiplier( int setValue )
		{
			// Set the score multiplier
			scoreMultiplier = setValue;
		}
		
		/// <summary>
		/// Levels up, and increases the difficulty of the game
		/// </summary>
		void  LevelUp()
		{
            Debug.Log(shootTime);
            //触发时间奖励
            if (shootTime < levels[currentLevel].TimeToBonus)
            {
                score += (int)levels[currentLevel].extraBonusFromTime;
                onFastShoot.Invoke();
                if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
                {
                    StartCoroutine(setEffect.HitEffect(FastHitEffect, levels[currentLevel].extraBonusFromTime.ToString(),"眼疾手快"));
                }
                else
                {
                    StartCoroutine(setEffect.HitEffect(FastHitEffect, levels[currentLevel].extraBonusFromTime.ToString(), "Eagle Eye"));
                }
            }
            // Update the level attributes
            // 此处调用update只是为了更新一下时间
            UpdateLevel();

            LevelUpEffect();

            currentLevel++;

            //每次升级后重新刷新道具
            GetGameProps(levels[currentLevel].GunRate,levels[currentLevel].BoomRate);

		    if (currentLevel < levelLimited || !isEndless)
		    {
                //正确更新鸭子数量和速度
                // Set the maximum number of targets
                maximumTargets = levels[currentLevel].maximumTargets;

                // Update the game speed
                movingSpeed = levels[currentLevel].movingSpeed;
            }
            if (progressCanvas) progressCanvas.Find("Text").GetComponent<Text>().text = (currentLevel + 1).ToString();

            UpdateScore();
			//Run the level up effect, displaying a sound
		    shootTime = 0;
		}

		/// <summary>
		/// Updates the level and sets some values like maximum targets, throw angle, and level text
		/// </summary>
		void UpdateLevel()
		{
            ResetTargetsToDuck();
			// Display the current level we are on
			if ( progressCanvas )    progressCanvas.Find("Text").GetComponent<Text>().text = (currentLevel + 1).ToString();

		    if (currentLevel >= levelLimited && isEndless)
		    {
                timeLeft += currentLevel + 1;
                UpdateTime();
                levels[currentLevel].scoreToNextLevel = currentLevel * (currentLevel + 1) * 50;
		        levels[currentLevel].timeBonus = currentLevel + 1;
		    }
		    else
		    {
                // Give time bonus for winning the level
                timeLeft += levels[currentLevel].timeBonus;

                // Update the timer
                UpdateTime();

                // Set the maximum number of targets
                maximumTargets = levels[currentLevel].maximumTargets;

                // Update the game speed
                movingSpeed = levels[currentLevel].movingSpeed;

                // Set the number of bullets the player has this level
                ammo = levels[currentLevel].ammo;

                // Update the ammo
                UpdateAmmo();
            }
		}

		/// <summary>
		/// Shows the effect associated with leveling up ( a sound and text bubble )
		/// </summary>
		void  LevelUpEffect ()
		{
			// Show the time bonus effect when we level up
			if ( levelUpEffect )
			{
                // Transform newBonusEffect = Instantiate(bonusEffect);
                //newBonusEffect.position = readyGoEffect.position;
                //newBonusEffect.position = new Vector3( Camera.main.ScreenToWorldPoint(timeText.transform.position).x, Camera.main.ScreenToWorldPoint(timeText.transform.position).y - 2, 0);
			    while (levelUpEffect.activeSelf)
			    {
			        levelUpEffect.SetActive(false);
			    }
                // Display the bonus value multiplied by a streak
                levelUpEffect.transform.GetChild(0).GetComponent<Text>().text = "EXTRA TIME!\n+" + levels[currentLevel].timeBonus.ToString();
                levelUpEffect.SetActive(true);
			}

			//If there is a source and a sound, play it from the source
			if ( soundSource && soundLevelUp )    
			{
				soundSource.GetComponent<AudioSource>().pitch = 1;

				soundSource.GetComponent<AudioSource>().PlayOneShot(soundLevelUp);
			}
		}

	    public void StartGame()
	    {
            onStartGame.Invoke();
	        isfound = true;
	        if (readyGoEffect)
	        {
	            Instantiate(readyGoEffect);
                ReadyGoSource.Play();
	        }
            TipUI.ShowTipWindow("射击鸭子", "Shoot the duck ");
        }

        public void PauseGame()
        {
            Pause();
        }
        /// <summary>
        /// Pause the game
        /// </summary>
        void  Pause()
		{
			isPaused = true;
		    //gyro.DisableGyro();
		    firstPerson.mouseLook.lockCursor = false;
			//Set timescale to 0, preventing anything from moving
			Time.timeScale = 0;
			
			//Show the pause screen and hide the game screen
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(true);
			if ( gameCanvas )    gameCanvas.gameObject.SetActive(false);
		}

        public void UnpauseGame()
        {
            Unpause();
        }
        /// <summary>
        /// Resume the game
        /// </summary>
        void  Unpause()
		{
			isPaused = false;
		    firstPerson.enabled = true;
		    firstPerson.mouseLook.lockCursor = true;
			
			//Set timescale back to the current game speed
			Time.timeScale = gameSpeed;
			
			//Hide the pause screen and show the game screen
			if ( pauseCanvas )    pauseCanvas.gameObject.SetActive(false);
			if ( gameCanvas )    gameCanvas.gameObject.SetActive(true);
		}
		
		/// <summary>
		/// Runs the game over event and shows the game over screen
		/// </summary>
		IEnumerator GameOver(float delay)
		{
			isGameOver = true;
            GameSource.Stop();
            //gyro.DisableGyro();
            firstPerson.enabled = false;
		    firstPerson.mouseLook.lockCursor = false;
			yield return new WaitForSeconds(delay);
			
			//Remove the pause and game screens
			if ( pauseCanvas )    Destroy(pauseCanvas.gameObject);
			if ( gameCanvas )    Destroy(gameCanvas.gameObject);
			
			//Show the game over screen
			if ( gameOverCanvas )    
			{
				//Show the game over screen
				gameOverCanvas.gameObject.SetActive(true);
				
				//Write the score text
                //区分中英文
			    if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
			    {
                    gameOverCanvas.Find("TextScore").GetComponent<Text>().text = "得分 " + score.ToString();
                }
			    else
			    {
                    gameOverCanvas.Find("TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();
                }
				//Check if we got a high score
				if ( score > highScore )    
				{
					highScore = score;
					
					//Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
					PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
#else
					PlayerPrefs.SetInt(Application.loadedLevelName + "HighScore", score);
#endif
				}
				
				//Write the high sscore text
                //区分中英文
			    if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
			    {
                    gameOverCanvas.Find("TextHighScore").GetComponent<Text>().text = "最高分 " + highScore.ToString();
                }
			    else
			    {
                    gameOverCanvas.Find("TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();
                }

				//If there is a source and a sound, play it from the source
				if ( soundSource && soundGameOver )    
				{
					soundSource.GetComponent<AudioSource>().pitch = 1;
					
					soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);
				    while (soundSource.GetComponent<AudioSource>().isPlaying)
				    {
                       yield return null;
				    }
				    GameoverLoop.loop = true;
                    GameoverLoop.Play();
				}
			}
		}

		/// <summary>
		/// Runs the victory event and shows the victory screen
		/// </summary>
		IEnumerator Victory(float delay)
		{
			isGameOver = true;
		    //gyro.DisableGyro();
		    firstPerson.enabled = true;
		    firstPerson.mouseLook.lockCursor = false;
			yield return new WaitForSeconds(delay);
			
			//Remove the pause and game screens
			if ( pauseCanvas )    Destroy(pauseCanvas.gameObject);
			if ( gameCanvas )    Destroy(gameCanvas.gameObject);
			
			//Show the game over screen
			if ( victoryCanvas )    
			{
				//Show the game over screen
				victoryCanvas.gameObject.SetActive(true);
				
				//Write the score text
                //区分中英文
			    if(PlayerPrefs.GetString("Language", "").Equals("Chinese"))
			    {
			        victoryCanvas.Find("TextScore").GetComponent<Text>().text = "得分 " + score.ToString();
			    }
			    else
			    {
                    victoryCanvas.Find("TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();
                }
				//Check if we got a high score
				if ( score > highScore )    
				{
					highScore = score;

					//Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
					PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
#else
					PlayerPrefs.SetInt(Application.loadedLevelName + "HighScore", score);
#endif
				}
				
				//Write the high sscore text
                //区分中英文
			    if (PlayerPrefs.GetString("Language", "").Equals("Chinese"))
			    {
                    victoryCanvas.Find("TextHighScore").GetComponent<Text>().text = "最高分 " + highScore.ToString();
                }
			    else
			    {
                    victoryCanvas.Find("TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();
                }
				
				//If there is a source and a sound, play it from the source
				if ( soundSource && soundVictory )    
				{
					soundSource.GetComponent<AudioSource>().pitch = 1;
					
					soundSource.GetComponent<AudioSource>().PlayOneShot(soundVictory);
				}
			}
		}
		
		/// <summary>
		/// Restart the current level
		/// </summary>
	     public	void  Restart()
		{
            //RestartProgress.gameObject.SetActive(true);
//#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            StartCoroutine(RestartSceneAsyc(SceneManager.GetActiveScene().name));
//#else
//			Application.LoadLevel(Application.loadedLevelName);
//#endif
        }

	    IEnumerator RestartSceneAsyc(string sceneName)
	    {
            yield return new WaitForSeconds(0.5f);
            RestartProgress.gameObject.SetActive(true);
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
	        op.allowSceneActivation = true;
            UnityLog.InfoGreen("loadscene");
            yield break;
	    }

		/// <summary>
		/// Restart the current level
		/// </summary>
		void  MainMenu()
		{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			SceneManager.LoadScene(mainMenuLevelName);
#else
			Application.LoadLevel(mainMenuLevelName);
#endif
		}

		/// <summary>
		/// Updates the ammo we have
		/// </summary>
		public void UpdateAmmo()
		{
            //Debug.Log("controller7777777777");
            // Only update if the ammor bar has been assigned
            if ( ammoBar )    
			{
                //Debug.Log("controller8888888888");
                // Update the ammo left by scaling the width of the AmmoFull bar
                ammoBar.rectTransform.sizeDelta = new Vector2( ammoLeft * ammoBarWidth, ammoBar.rectTransform.sizeDelta.y);

				// Set the AmmoEmpty width based on the number of bullets we have multiplied by the width of a single bullet in the bar
				//ammoBar.transform.Find("Empty").GetComponent<Image>().rectTransform.sizeDelta = new Vector2( ammo * ammoBarWidth, ammoBar.rectTransform.sizeDelta.y);
                //Debug.Log("controller999999999999");

            }	
		}

		/// <summary>
		/// Shoots!
		/// </summary>
		public void Shoot()
		{
		    shootCount++;
            Debug.Log("controller is shooting>>>>>>111111");
            weapon.Shoot();
            //Debug.Log("controller00000000");
			if ( startDelay <= 0 && ammoLeft > 0 && shotObject && Time.deltaTime > 0 )
			{
                // Create a new shot at the position of the mouse/tap
                // GameObject newShot = GameObject.Instantiate(shotObject,effectPoint.position,effectPoint.rotation) as GameObject;               
                ////newShot.transform.SetParent(Camera.main.transform);
			    //StartCoroutine(ShowEffect());
                // 子弹减少，暂时弃用
                // ammoLeft--;
                //Debug.Log("controller555555555");
                // Update the ammo we have left
                UpdateAmmo();
                //Debug.Log("controller6666666666");
            }
		}

	    public void StopShoot()
	    {
	        weapon.StopShoot();
	    }

     //   #region 射击特效，供weapons调用
     //   public void ShowEffectRepeat()
	    //{
	    //    InvokeRepeating("ShowShootEffect",0f,0.1f);
	    //}

	    //public void ShowShootEffect()
	    //{
     //       StartCoroutine(ShowEffect());
	    //}

	    //public void StopShowEffectRepeat()
	    //{
	    //    CancelInvoke("ShowShootEffect");
	    //}

	    //private IEnumerator ShowEffect()
	    //{
     //       shotObject.gameObject.SetActive(true);
     //       yield return new  WaitForSeconds(0.1f);
     //       shotObject.gameObject.SetActive(false);
	    //}
     //   #endregion

        public void ReduceAmmo()
	    {
	        ammoLeft--;
            UpdateAmmo();
	    }

		/// <summary>
		/// Reloads the ammo
		/// </summary>
		public void Reload()
		{
		    //Debug.Log("reloading>>>>>>>>>>>>>>>>>1");
            weapon.ReloadWeapon();
            // Refill the ammo
            ammoLeft = ammo;

			// Update the ammo we have left
			UpdateAmmo();

			//If there is a source and a sound, play it from the source
			if ( soundSource && soundReload )    
			{
				soundSource.GetComponent<AudioSource>().pitch = 1;

				soundSource.GetComponent<AudioSource>().PlayOneShot(soundReload);
			}
		}
//FOR A FUTURE UPDATE
		/// <summary>
		/// Slows the game down to 0.5 speed for a few seconds
		/// </summary>
		/// <param name="duration">Duration of slowmotion effect</param>
//		IEnumerator SlowMotion(float duration)
//		{
//			Transform newEffect = null;
//
//			if ( slowMotionEffect )    
//			{
//				// Create a slow motion effect
//				newEffect = Instantiate( slowMotionEffect, Vector3.zero, Quaternion.identity) as Transform;
//
//				// Animate the effect
//				if ( newEffect.animation )
//				{
//					newEffect.animation[newEffect.animation.clip.name].speed = 1; 
//					newEffect.animation.Play(newEffect.animation.clip.name);
//				}
//			}
//
//			// Set the game speed to half
//			gameSpeed *= 0.5f;
//
//			// Set the timescale accordingly
//			Time.timeScale = gameSpeed;
//
//			// This makes sure the game runs smoothly even in slowmotion. Otherwise you will get clunky physics
//			Time.fixedDeltaTime = Time.timeScale * 0.02f;
//
//			// Wait for some time
//			yield return new WaitForSeconds(duration);
//
//			// Reverse the slowmotion animation
//			if ( slowMotionEffect && newEffect.animation )    
//			{
//				newEffect.animation[newEffect.animation.clip.name].speed = -1; 
//				newEffect.animation[newEffect.animation.clip.name].time = newEffect.animation[newEffect.animation.clip.name].length; 
//				newEffect.animation.Play(newEffect.animation.clip.name);
//			}
//
//			// Reset speed back to normal
//			gameSpeed *= 2;
//
//			// Set the timescale accordingly
//			Time.timeScale = gameSpeed;
//
//			Time.fixedDeltaTime = 0.02f;
//		}

		/// <summary>
		/// Raises the draw gizmos event.
		/// </summary>
		void OnDrawGizmos()
		{
			Gizmos.color = Color.red;

			// Draw the left limit of the area in which targets can be shown
			Gizmos.DrawLine( new Vector3( targetShowArea, -5, 0), new Vector3( targetShowArea, 5, 0) );

			// Draw the right limit of the area in which targets can be shown
			Gizmos.DrawLine( new Vector3( -targetShowArea, -5, 0), new Vector3( -targetShowArea, 5, 0) );
		}
	}
}