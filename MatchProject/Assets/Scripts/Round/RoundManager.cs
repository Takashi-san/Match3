using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoundManager : MonoBehaviour {
	[SerializeField] RoundData _roundData = null;
	[SerializeField] GameObject _roundPresentation = null;
	[SerializeField] GameObject _roundWin = null;
	[SerializeField] GameObject _roundLose = null;

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

		_sceneHandler = FindObjectOfType<SceneHandler>();
		_sceneHandler.LoadSceneAdditive("RoundUI");

		FindObjectOfType<GridBoard>().startMove += BoardResolving;
		FindObjectOfType<GridBoard>().endMove += BoardStill;

		FindObjectOfType<ScoreManager>().scoreUpdate += ScoreUpdate;

		_roundPresentation.GetComponent<MenuRoundPresentation>().SetRoundData(_roundData);
		_roundPresentation.SetActive(true);
	}

	public void RoundStart() {
		if (roundStart != null)
			roundStart();

		Timer timer = FindObjectOfType<Timer>();
		timer.timerEnd += RoundEnd;
		timer.SetTimer(_roundData.roundTime);
		timer.StartTimer();

		_roundPresentation.SetActive(false);
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
			_roundData.goalScore += _roundData.roundScaleAdd;
			_roundData.goalScore = Mathf.FloorToInt(_roundData.goalScore * _roundData.roundScaleMult);
			_roundWin.SetActive(true);
		}
		else {
			_roundLose.SetActive(true);
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
