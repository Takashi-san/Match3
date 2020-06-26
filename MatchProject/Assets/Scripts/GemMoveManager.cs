using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMoveManager : MonoBehaviour {
	List<Gem> _movingList = new List<Gem>();
	bool _hasMovement = false;
	public bool HasMovement => _hasMovement;

	static GemMoveManager _instance;
	public static GemMoveManager Instance => _instance;

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Debug.LogWarning("Duplicate of GemMoveManager in: " + gameObject.name);
			Destroy(this);
		}
	}

	public void StartedMoving(Gem gem) {
		if (!_movingList.Contains(gem)) {
			_movingList.Add(gem);
			_hasMovement = true;
		}
	}

	public void StoppedMoving(Gem gem) {
		_movingList.Remove(gem);
		if (_movingList.Count == 0) {
			_hasMovement = false;
		}
	}
}
