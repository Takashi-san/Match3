using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {
	enum GemId {
		MILK, APPLE, LEMON, BREAD, BROCOLI, COCONUT, STAR
	}

	[SerializeField] GemId _id;

	int[] _gridPosition;
}
