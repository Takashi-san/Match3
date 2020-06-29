using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : MonoBehaviour {
	[SerializeField] RoundData _roundData = null;

	static RoundManager _instance;
	public RoundManager Instance => _instance;
	public Action roundStart;
	public Action roundEnd;

	SceneHandler _sceneHandler;

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
		timer.SetTimer(_roundData.roundTime);
		timer.StartTimer();

		_sceneHandler = FindObjectOfType<SceneHandler>();
		_sceneHandler.LoadSceneAdditive("RoundUI");
	}

	void RoundEnd() {
		if (roundEnd != null)
			roundEnd();
		if (FindObjectOfType<ScoreManager>().Score >= _roundData.goalScore) {
			_roundData.round++;
			_roundData.goalScore += 500;
			_sceneHandler.LoadScene("Grid");
		}
		else {
			_sceneHandler.LoadScene("Grid");
		}
	}
}
