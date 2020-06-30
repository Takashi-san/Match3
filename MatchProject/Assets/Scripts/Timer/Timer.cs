using UnityEngine;
using System;

public class Timer : MonoBehaviour {
	public Action<float> timerUpdate;
	public Action timerEnd;

	float _timer = 0;
	bool _active = false;

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

	public void SetTimer(float time) {
		if (time >= 0) {
			_timer = time;
			if (timerUpdate != null)
				timerUpdate(_timer);
		}
	}
}
