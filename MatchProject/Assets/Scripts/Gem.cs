using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {
	[SerializeField] Enums.GemId _id = Enums.GemId.MILK;
	public Enums.GemId Id => _id;
}
