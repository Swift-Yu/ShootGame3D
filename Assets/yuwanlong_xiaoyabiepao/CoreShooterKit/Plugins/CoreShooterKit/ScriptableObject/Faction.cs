using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HedgehogTeam.CoreShooterKit{
	
	public class Faction : ScriptableObject {

		public bool isNeutral = false;
		public FactionAlignment unknow = FactionAlignment.Enemy;

		public Faction[] allies;
		public Faction[] enemies;

		public Dictionary<Faction,int> cache = new Dictionary<Faction, int>();

		public int GetAlignement( Faction faction){

			int result =(int)unknow;

			if (!faction) return result;

			// Same faction
			if (faction == this){ return 1;}

			// Faction is neutral
			if (isNeutral) return 0;

			if (cache.ContainsKey( faction)) return cache[faction];

			if (allies.Contains(faction)){result = 1;}

			if (enemies.Contains(faction)){result = -1;}

			if (faction.isNeutral){result =0;}

			cache.Add(faction,result);

			return result;
		}
			
	}
}
