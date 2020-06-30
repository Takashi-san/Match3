using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuRoundPresentation : MonoBehaviour {
	[SerializeField] TextMeshProUGUI _round = null;
	[SerializeField] TextMeshProUGUI _goalScore = null;
	[SerializeField] TextMeshProUGUI _roundTime = null;
	RoundData _roundData;

	public void SetRoundData(RoundData roundData) {
		_roundData = roundData;
		SetTxtFields();
	}

	void SetTxtFields() {
		_round.text = _roundData.round.ToString();
		_goalScore.text = _roundData.goalScore.ToString();

		int minutes = Mathf.FloorToInt(_roundData.roundTime / 60);
		int seconds = Mathf.FloorToInt(_roundData.roundTime % 60);
		_roundTime.text = minutes.ToString("00") + ":" + seconds.ToString("00");
	}
}
