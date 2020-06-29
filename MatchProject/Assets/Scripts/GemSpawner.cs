using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GemSpawner : MonoBehaviour {
	[SerializeField] GemLineup _lineup = null;
	Dictionary<Enums.GemId, GameObject> _gems = new Dictionary<Enums.GemId, GameObject>();
	float _spawnHeight;

	public Enums.GemId[] Lineup => _gems.Keys.ToArray();

	void Awake() {
		Setup();
	}

	void Start() {
		// Important to do this after the camera setup.
		// The game logic assumes that all gems have 1x1 unit dimension.
		_spawnHeight = Camera.main.orthographicSize + Camera.main.transform.position.y + 1;
	}

	public Gem Spawn(Enums.GemId gem, Vector2 position) {
		Gem spawned = Instantiate(_gems[gem], new Vector2(position.x, _spawnHeight), Quaternion.identity).GetComponent<Gem>();
		spawned.Move(position);
		return spawned;
	}

	void Setup() {
		for (int i = 0; i < _lineup.gems.Length; i++) {
			_gems.Add(_lineup.gems[i].GetComponent<Gem>().Id, _lineup.gems[i]);
		}
	}
}
