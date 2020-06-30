﻿using System.Collections;
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
	int _score;
	bool _boardResolving = false;

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

		FindObjectOfType<GridBoard>().startMove += BoardResolving;
		FindObjectOfType<GridBoard>().endMove += BoardStill;

		FindObjectOfType<ScoreManager>().scoreUpdate += ScoreUpdate;
	}

	void RoundEnd() {
		if (roundEnd != null)
			roundEnd();
		StartCoroutine(AvaliateRound());
	}

	IEnumerator AvaliateRound() {
		while (_boardResolving) {
			yield return null;
		}

		if (_score >= _roundData.goalScore) {
			_roundData.round++;
			_roundData.goalScore += 500;
			_sceneHandler.LoadScene("Grid");
		}
		else {
			_sceneHandler.LoadScene("Grid");
		}
	}

	void BoardResolving() {
		_boardResolving = true;
	}

	void BoardStill() {
		_boardResolving = false;
	}

	void ScoreUpdate(int score) {
		_score = score;
	}
}
