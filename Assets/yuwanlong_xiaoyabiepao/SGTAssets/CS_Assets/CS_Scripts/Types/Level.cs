﻿using UnityEngine;
using System;

namespace ShootingGallery.Types
{
	/// <summary>
	/// This script defines a level in the game. When the player reaches a certain score, the level is increased and the difficulty is changed accordingly
	/// This class is used in the Game Controller script
	/// </summary>
	[Serializable]
	public class Level
	{
        //   //levelLimited
        //百发百中
        public int PerfectBonus = 100;
        //全部击中
        public int CleanBonus = 100;

        // The score needed to win this level
        public int scoreToNextLevel = 200;

		// The time bonus for finishing this level
		public float timeBonus = 5;

		// The maximum number of targets allowed on screen at once
		public int maximumTargets = 2;

		// The number of bullets you have in this level
		public int ammo = 5;

		// The speed of moving targets
		public float movingSpeed = 1;

		// A special target that appears once per level
        [HideInInspector]
		public Transform specialTarget = null;

        //how many time we left will get the bonus when level up
        public float TimeToBonus;

        //Extra time will get bonus
	    public float extraBonusFromTime;
	}
}