using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour {
	static ScoreManager _instance;
	int _score = 0;

	public Action<int> scoreUpdate;
	public int Score => _score;
	public static ScoreManager Instance => _instance;

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Debug.LogWarning("Duplicate of ScoreManager, destroying: " + gameObject.name);
			Destroy(gameObject);
		}
	}

	void Start() {
		if (scoreUpdate != null) {
			scoreUpdate(_score);
		}
	}

	public void AddScore(int add) {
		if (add > 0) {
			_score += add;

			if (scoreUpdate != null) {
				scoreUpdate(_score);
			}
		}
	}

	public void SubScore(int sub) {
		if (sub > 0) {
			_score -= sub;

			if (scoreUpdate != null) {
				scoreUpdate(_score);
			}
		}
	}
}
