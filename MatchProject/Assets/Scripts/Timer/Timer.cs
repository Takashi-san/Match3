using UnityEngine;
using System;

public class Timer : MonoBehaviour {
	[SerializeField] [Min(0)] float _StartTime = 0;

	public Action<float> timerUpdate;
	public Action timerEnd;

	float _timer = 0;
	bool _active = false;

	void Awake() {
		_timer = _StartTime;
	}

	void Start() {
		if (timerUpdate != null) {
			timerUpdate(_timer);
		}
		StartTimer();
	}

	void Update() {
		if (_active) {
			_timer -= Time.deltaTime;
			if (_timer <= 0) {
				_timer = 0;
				_active = false;
				if (timerEnd != null)
					timerEnd();
			}
			if (timerUpdate != null)
				timerUpdate(_timer);
		}
	}

	public void StartTimer() {
		_active = true;
	}

	public void StopTimer() {
		_active = false;
	}
}
