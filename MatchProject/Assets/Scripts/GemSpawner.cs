using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GemSpawner : MonoBehaviour {
	float _spawnHeight;

	void Start() {
		// Important to do this after the camera setup.
		// The game logic assumes that all gems have 1x1 unit dimension.
		_spawnHeight = Camera.main.orthographicSize + Camera.main.transform.position.y;
	}

	public Gem Spawn(GameObject gem, Vector2 position) {
		Gem spawned = Instantiate(gem, new Vector2(position.x, _spawnHeight), Quaternion.identity).GetComponent<Gem>();
		spawned.Move(position);
		return spawned;
	}
}
