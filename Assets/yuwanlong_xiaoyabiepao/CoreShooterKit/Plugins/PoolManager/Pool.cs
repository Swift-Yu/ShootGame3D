using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HedgehogTeam.EasyPoolManager{
	[System.Serializable]
	public class Pool{

		[SerializeField]
		public List<GameObject> pooledObjects = new List<GameObject>();
		public List<bool> haveDespawn = new List<bool>();
		public List<float> objectsLastUse = new List<float>();
	}
}