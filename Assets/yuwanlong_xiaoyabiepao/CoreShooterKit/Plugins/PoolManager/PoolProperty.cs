using UnityEngine;
using System.Collections;

namespace HedgehogTeam.EasyPoolManager{
	[System.Serializable]
	public class PoolProperty{

		public GameObject obj;
		public int poolAmount;
		public bool preLoadAtStart = false;
		public bool allowGrowth = true;
		public bool limitGrowth = false;
		public int  limit;

		public bool enableGarbage = false;
		public float idleTime = 15;

		public bool showInpsector = true;
	}
}
