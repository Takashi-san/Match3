using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour {
	public Action<Vector2, Enums.Direction> swipe;

	[SerializeField] [Min(0)] float _swipeDeadzoneRadius = 0;

	Vector2 _swipePosition;
	bool _swipeValid = false;


	static PlayerInput _instance;
	public static PlayerInput Instance {
		get {
			return _instance;
		}
	}

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Debug.LogWarning("Duplicate of PlayerInput, destroying: " + gameObject.name);
			Destroy(gameObject);
		}
	}

	void Update() {
		if (Input.touchCount > 0) {
			CheckSwipe();
		}
	}

	void CheckSwipe() {
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			_swipePosition = Camera.main.ScreenToWorldPoint(touch.position);
			_swipeValid = true;
		}
		else if (_swipeValid && ((touch.phase == TouchPhase.Ended) || (touch.phase == TouchPhase.Canceled))) {
			Vector2 endPosition = Camera.main.ScreenToWorldPoint(touch.position);
			endPosition = endPosition - _swipePosition;

			if (endPosition.magnitude >= _swipeDeadzoneRadius) {
				float angle = Mathf.Atan2(endPosition.y, endPosition.x);
				angle = Mathf.Rad2Deg * angle;

				if (angle <= 45 && angle > -45) {
					if (swipe != null) {
						swipe(_swipePosition, Enums.Direction.Right);
					}
				}
				else if (angle <= 135 && angle > 45) {
					if (swipe != null) {
						swipe(_swipePosition, Enums.Direction.Up);
					}
				}
				else if (angle <= -135 || angle > 135) {
					if (swipe != null) {
						swipe(_swipePosition, Enums.Direction.Left);
					}
				}
				else if (angle <= -45 && angle > -135) {
					if (swipe != null) {
						swipe(_swipePosition, Enums.Direction.Down);
					}
				}
			}

			_swipeValid = false;
		}
	}
}
