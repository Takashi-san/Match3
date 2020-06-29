using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : MonoBehaviour {
	[SerializeField] [Min(0)] float _goalScore = 0;
	[SerializeField] [Min(0)] float _roundTime = 0;

	static RoundManager _instance;
	public RoundManager Instance => _instance;
	public Action roundEnd;

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Debug.LogWarning("Duplicate of RoundManager in: " + gameObject.name);
			Destroy(this);
		}

		Timer timer = FindObjectOfType<Timer>();
		timer.timerEnd += RoundEnd;
		timer.SetTimer(_roundTime);
		timer.StartTimer();
	}

	void RoundEnd() {
		if (roundEnd != null)
			roundEnd();
		if (FindObjectOfType<ScoreManager>().Score >= _goalScore) {
			Debug.Log("Win!");
		}
		else {
			Debug.Log("Lose!");
		}
	}
}
