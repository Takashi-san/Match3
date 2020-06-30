using UnityEngine;

public class ButtonSetRoundData : MonoBehaviour {
	[SerializeField] RoundData _roundData = null;
	[SerializeField] [Min(1)] int _round = 1;
	[SerializeField] [Min(0)] int _goalScore = 0;
	[SerializeField] [Min(1)] float _roundTime = 1;

	public void SetRoundData() {
		_roundData.round = _round;
		_roundData.goalScore = _goalScore;
		_roundData.roundTime = _roundTime;
	}
}
