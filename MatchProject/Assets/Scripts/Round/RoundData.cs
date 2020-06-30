using UnityEngine;

[CreateAssetMenu(fileName = "RoundData", menuName = "ScriptableObjects/Round Data")]
public class RoundData : ScriptableObject {
	[Min(1)] public int round;
	[Min(0)] public int goalScore;
	[Min(1)] public float roundTime;
	[Min(0)] public int roundScaleAdd;
	[Min(1)] public float roundScaleMult;
}
