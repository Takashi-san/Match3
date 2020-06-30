using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoBehaviour {
	public Action<Vector2, Enums.Direction> swipe;

	[SerializeField] [Min(0)] float _swipeDeadzoneRadius = 0;

	Vector2 _swipePositionTouch;
	bool _swipeValidTouch = false;
	Vector2 _swipePositionMouse;
	bool _swipeValidMouse = false;


	static PlayerInput _instance;
	public static PlayerInput Instance => _instance;

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Debug.LogWarning("Duplicate of PlayerInput in: " + gameObject.name);
			Destroy(this);
		}
	}

	void Update() {
		CheckSwipeMouse();
		if (Input.touchCount > 0) {
			CheckSwipeTouch();
		}
	}

	void CheckSwipeTouch() {
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began) {
			_swipePositionTouch = Camera.main.ScreenToWorldPoint(touch.position);
			_swipeValidTouch = true;
		}
		else if (_swipeValidTouch && ((touch.phase == TouchPhase.Ended) || (touch.phase == TouchPhase.Canceled))) {
			Vector2 endPosition = Camera.main.ScreenToWorldPoint(touch.position);
			endPosition = endPosition - _swipePositionTouch;

			if (endPosition.magnitude >= _swipeDeadzoneRadius) {
				float angle = Mathf.Atan2(endPosition.y, endPosition.x);
				angle = Mathf.Rad2Deg * angle;

				if (angle <= 45 && angle > -45) {
					if (swipe != null) {
						swipe(_swipePositionTouch, Enums.Direction.Right);
					}
				}
				else if (angle <= 135 && angle > 45) {
					if (swipe != null) {
						swipe(_swipePositionTouch, Enums.Direction.Up);
					}
				}
				else if (angle <= -135 || angle > 135) {
					if (swipe != null) {
						swipe(_swipePositionTouch, Enums.Direction.Left);
					}
				}
				else if (angle <= -45 && angle > -135) {
					if (swipe != null) {
						swipe(_swipePositionTouch, Enums.Direction.Down);
					}
				}
			}

			_swipeValidTouch = false;
		}
	}

	void CheckSwipeMouse() {
		if (Input.GetMouseButtonDown(0)) {
			_swipePositionMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			_swipeValidMouse = true;
		}
		else if (_swipeValidMouse && Input.GetMouseButtonUp(0)) {
			Vector2 endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			endPosition = endPosition - _swipePositionMouse;

			if (endPosition.magnitude >= _swipeDeadzoneRadius) {
				float angle = Mathf.Atan2(endPosition.y, endPosition.x);
				angle = Mathf.Rad2Deg * angle;

				if (angle <= 45 && angle > -45) {
					if (swipe != null) {
						swipe(_swipePositionMouse, Enums.Direction.Right);
					}
				}
				else if (angle <= 135 && angle > 45) {
					if (swipe != null) {
						swipe(_swipePositionMouse, Enums.Direction.Up);
					}
				}
				else if (angle <= -135 || angle > 135) {
					if (swipe != null) {
						swipe(_swipePositionMouse, Enums.Direction.Left);
					}
				}
				else if (angle <= -45 && angle > -135) {
					if (swipe != null) {
						swipe(_swipePositionMouse, Enums.Direction.Down);
					}
				}
			}

			_swipeValidMouse = false;
		}
	}
}
