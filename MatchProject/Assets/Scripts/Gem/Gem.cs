using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, IPooledGameObject {
	[SerializeField] Enums.GemId _id = Enums.GemId.MILK;
	[SerializeField] [Min(0.001f)] float _moveDuration = 0.001f;
	public Enums.GemId Id => _id;
	float _timer = 0;
	bool _doMove = false;
	Vector2 _startPosition;
	Vector2 _endPosition;

	GameObjectPool _pool = null;
	public GameObjectPool Pool {
		get { return _pool; }
		set {
			if (_pool == null) {
				_pool = value;
			}
			else {
				Debug.LogWarning("Trying to change setted pool element pool.");
			}
		}
	}

	void Update() {
		if (_doMove) {
			_timer += Time.deltaTime;
			if (_timer > _moveDuration) {
				_timer = _moveDuration;
				_doMove = false;
				GemMoveManager.Instance.StoppedMoving(this);
			}

			float t = _timer / _moveDuration;
			t = 1 - (1 - t) * (1 - t) * (1 - t); // Smooth Stop 3.
			Vector3 position = new Vector3(Mathf.Lerp(_startPosition.x, _endPosition.x, t),
										   Mathf.Lerp(_startPosition.y, _endPosition.y, t),
										   transform.position.z);
			transform.position = position;
		}
	}

	public void Move(Vector2 position) {
		_startPosition = transform.position;
		_endPosition = position;
		_timer = 0;
		_doMove = true;
		GemMoveManager.Instance.StartedMoving(this);
	}

	void OnDisable() {
		_doMove = false;
		GemMoveManager.Instance.StoppedMoving(this);
	}
}
