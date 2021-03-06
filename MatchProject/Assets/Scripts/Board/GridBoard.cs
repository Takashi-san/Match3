﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Linq;

public class GridBoard : MonoBehaviour {
	[SerializeField] [Min(3)] Vector2Int _gridSize = Vector2Int.one;
	[SerializeField] AudioClip _swapSfx = null;
	[SerializeField] AudioClip _matchSfx = null;
	[SerializeField] [Min(0)] float _matchPitchInc = 0;
	[SerializeField] [Min(0)] float _matchPitchMax = 1;
	[SerializeField] AudioMixer _mixer = null;
	[SerializeField] AudioSource _matchSource = null;
	[SerializeField] AudioSource _sfxSource = null;

	public Vector2Int GridSize => _gridSize;
	public Action startMove;
	public Action endMove;

	Gem[,] _grid;
	GemSpawner _spawner;
	bool _inMove = false;
	bool _canMove = false;
	float _pitch = 1;
	Dictionary<Vector2Int, int[]> _matches = new Dictionary<Vector2Int, int[]>();

	void Start() {
		_spawner = FindObjectOfType<GemSpawner>();
		SetupGrid();
		while (CheckDeadlockGrid()) {
			ShuffleGrid();
			while (CheckMatchGrid()) {
				UpdateGrid();
			}
		}
		PlayerInput.Instance.swipe += CheckMove;
		FindObjectOfType<RoundManager>().roundStart += RoundStart;
	}

	void RoundStart() {
		_canMove = true;
	}

	// Important thing to take note is that the gem's index on the grid are the same as their position in world space.
	// This detail is important in multiple places in the code and how the scene is being setup in the game.
	void SetupGrid() {
		Enums.GemId gem, doubleX, doubleY;
		Enums.GemId[] lineup = _spawner.Lineup;
		_grid = new Gem[_gridSize.x, _gridSize.y];

		for (int x = 0; x < _gridSize.x; x++) {
			for (int y = 0; y < _gridSize.y; y++) {
				if (x - 2 >= 0) {
					if (_grid[x - 1, y].Id == _grid[x - 2, y].Id) {
						doubleX = _grid[x - 1, y].Id;
					}
					else {
						doubleX = Enums.GemId.NULL;
					}
				}
				else {
					doubleX = Enums.GemId.NULL;
				}
				if (y - 2 >= 0) {
					if (_grid[x, y - 1].Id == _grid[x, y - 2].Id) {
						doubleY = _grid[x, y - 1].Id;
					}
					else {
						doubleY = Enums.GemId.NULL;
					}
				}
				else {
					doubleY = Enums.GemId.NULL;
				}

				gem = lineup[UnityEngine.Random.Range(0, lineup.Length)];
				if (gem == doubleX || gem == doubleY) {
					Enums.GemId[] tmp = Array.FindAll(lineup, v => (v != doubleX) && (v != doubleY));
					if (tmp.Length != 0) {
						gem = tmp[UnityEngine.Random.Range(0, tmp.Length)];
					}
				}

				_grid[x, y] = _spawner.Spawn(gem, new Vector2(x, y));
			}
		}
	}

	void ShuffleGrid() {
		List<Gem> tmp = new List<Gem>();
		for (int x = 0; x < _gridSize.x; x++) {
			for (int y = 0; y < _gridSize.y; y++) {
				tmp.Add(_grid[x, y]);
			}
		}

		for (int x = 0; x < _gridSize.x; x++) {
			for (int y = 0; y < _gridSize.y; y++) {
				int rand = UnityEngine.Random.Range(0, tmp.Count);
				_grid[x, y] = tmp[rand];
				tmp[rand].Move(new Vector2(x, y));
				tmp.RemoveAt(rand);
			}
		}
	}

	#region Player Move
	void CheckMove(Vector2 swipePosition, Enums.Direction direction) {
		if (!_canMove)
			return;

		// Check if it is already in the middle of another player move.
		if (_inMove) {
			return;
		}
		_inMove = true;
		if (startMove != null) {
			startMove();
		}
		_pitch = 1 - _matchPitchInc;

		// Check if input was on the board.
		swipePosition += new Vector2(0.5f, 0.5f);
		if ((swipePosition.x >= 0) && (swipePosition.y >= 0)) {
			if ((swipePosition.x < _gridSize.x) && (swipePosition.y < _gridSize.y)) {
				Vector2Int gemA = new Vector2Int(Mathf.FloorToInt(swipePosition.x), Mathf.FloorToInt(swipePosition.y));
				Vector2Int gemB = Vector2Int.zero;

				// Check if input is possible.
				switch (direction) {
					case Enums.Direction.Right:
						if (gemA.x + 1 == _gridSize.x) {
							return;
						}
						gemB = new Vector2Int(gemA.x + 1, gemA.y);
						break;

					case Enums.Direction.Left:
						if (gemA.x - 1 < 0) {
							return;
						}
						gemB = new Vector2Int(gemA.x - 1, gemA.y);
						break;

					case Enums.Direction.Up:
						if (gemA.y + 1 == _gridSize.y) {
							return;
						}
						gemB = new Vector2Int(gemA.x, gemA.y + 1);
						break;

					case Enums.Direction.Down:
						if (gemA.y - 1 < 0) {
							return;
						}
						gemB = new Vector2Int(gemA.x, gemA.y - 1);
						break;
				}

				// Check if one of the gems is null.
				if (_grid[gemA.x, gemA.y] == null) {
					return;
				}
				if (_grid[gemB.x, gemB.y] == null) {
					return;
				}

				StartCoroutine(CheckMoveValidity(gemA, gemB));
			}
		}
	}

	IEnumerator CheckMoveValidity(Vector2Int gemA, Vector2Int gemB) {
		Swap(gemA, gemB);
		while (GemMoveManager.Instance.HasMovement) {
			yield return null;
		}

		// Check if valid movement.
		bool tmp1, tmp2;
		tmp1 = CheckMatchGem(gemA);
		tmp2 = CheckMatchGem(gemB);
		if (tmp1 || tmp2) {
			PlayMatchSfx();
			UpdateGrid();
			while (GemMoveManager.Instance.HasMovement) {
				yield return null;
			}
		}
		else {
			Swap(gemA, gemB);
			while (GemMoveManager.Instance.HasMovement) {
				yield return null;
			}
			_inMove = false;
			if (endMove != null) {
				endMove();
			}
			yield break;
		}

		// Successfull movement.
		while (CheckMatchGrid()) {
			PlayMatchSfx();
			UpdateGrid();
			while (GemMoveManager.Instance.HasMovement) {
				yield return null;
			}
		}

		while (CheckDeadlockGrid()) {
			ShuffleGrid();
			while (GemMoveManager.Instance.HasMovement) {
				yield return null;
			}
			while (CheckMatchGrid()) {
				PlayMatchSfx();
				UpdateGrid();
				while (GemMoveManager.Instance.HasMovement) {
					yield return null;
				}
			}
		}

		// Finish player move.
		_inMove = false;
		if (endMove != null) {
			endMove();
		}
	}

	void Swap(Vector2Int gemA, Vector2Int gemB) {
		Gem tmp = _grid[gemB.x, gemB.y];

		// Change position.
		_grid[gemA.x, gemA.y].Move(new Vector2(gemB.x, gemB.y));
		_grid[gemB.x, gemB.y].Move(new Vector2(gemA.x, gemA.y));

		// Change index.
		_grid[gemB.x, gemB.y] = _grid[gemA.x, gemA.y];
		_grid[gemA.x, gemA.y] = tmp;

		_sfxSource.PlayOneShot(_swapSfx);
	}
	#endregion

	#region Match Check
	bool CheckMatchGem(Vector2Int gem) {
		if (_grid[gem.x, gem.y] == null) {
			return false;
		}
		Enums.GemId gemId = _grid[gem.x, gem.y].Id;
		bool hasHoriMatch = false;
		bool hasVertMatch = false;
		// number of matching gems in positive/negative direction.
		int matchHoriPos = 0;
		int matchHoriNeg = 0;
		int matchVertPos = 0;
		int matchVertNeg = 0;

		// Check horizontal match.
		for (int i = gem.x + 1; i < _gridSize.x; i++) {
			if (_grid[i, gem.y] == null) {
				break;
			}
			else if (_grid[i, gem.y].Id != gemId) {
				break;
			}
			else {
				matchHoriPos++;
			}
		}
		for (int i = gem.x - 1; i >= 0; i--) {
			if (_grid[i, gem.y] == null) {
				break;
			}
			else if (_grid[i, gem.y].Id != gemId) {
				break;
			}
			else {
				matchHoriNeg++;
			}
		}
		if (matchHoriPos + matchHoriNeg >= 2) {
			hasHoriMatch = true;
		}

		// Check vertical match.
		for (int i = gem.y + 1; i < _gridSize.y; i++) {
			if (_grid[gem.x, i] == null) {
				break;
			}
			else if (_grid[gem.x, i].Id != gemId) {
				break;
			}
			else {
				matchVertPos++;
			}
		}
		for (int i = gem.y - 1; i >= 0; i--) {
			if (_grid[gem.x, i] == null) {
				break;
			}
			else if (_grid[gem.x, i].Id != gemId) {
				break;
			}
			else {
				matchVertNeg++;
			}
		}
		if (matchVertPos + matchVertNeg >= 2) {
			hasVertMatch = true;
		}

		// Verify check results.
		if (hasHoriMatch || hasVertMatch) {
			// Store the match data.
			if (hasHoriMatch && hasVertMatch) {
				int[] dir = { matchHoriPos, matchHoriNeg, matchVertPos, matchVertNeg };
				_matches.Add(gem, dir);
			}
			else if (hasHoriMatch) {
				int[] dir = { matchHoriPos, matchHoriNeg, 0, 0 };
				_matches.Add(gem, dir);
			}
			else if (hasVertMatch) {
				int[] dir = { 0, 0, matchVertPos, matchVertNeg };
				_matches.Add(gem, dir);
			}

			return true;
		}
		else {
			return false;
		}
	}

	bool CheckMatchGrid() {
		bool hadMatches = false;

		for (int x = 0; x < _gridSize.x; x++) {
			for (int y = 0; y < _gridSize.y; y++) {
				if (CheckMatchGem(new Vector2Int(x, y))) {
					hadMatches = true;
				}
			}
		}

		return hadMatches;
	}

	void GiveMatchPoints(int horiMatch, int vertMatch) {
		if ((horiMatch > 1) && (vertMatch > 1)) {
			// cross.
			ScoreManager.Instance.AddScore((horiMatch + vertMatch - 3) * 225);
		}
		else if (horiMatch > 1) {
			// only horizontal match.
			ScoreManager.Instance.AddScore((horiMatch - 1) * 75);
		}
		else if (vertMatch > 1) {
			// only vertical match.
			ScoreManager.Instance.AddScore((vertMatch - 1) * 75);
		}
	}

	void PlayMatchSfx() {
		_matchSource.PlayOneShot(_matchSfx);
		_pitch += _matchPitchInc;
		if (_pitch > _matchPitchMax) {
			_pitch = _matchPitchMax;
		}
		_mixer.SetFloat("MatchPitch", _pitch);
	}
	#endregion

	#region Grid Update
	void UpdateColumn(int column) {
		int isNull = 0;
		int notNull = 0;

		// Search for null.
		for (isNull = 0; isNull < _gridSize.y; isNull++) {
			if (_grid[column, isNull] == null) {

				// Search for not null to replace.
				if (notNull <= isNull) {
					notNull = isNull + 1;
				}
				for (; notNull < _gridSize.y; notNull++) {
					if (_grid[column, notNull] != null) {
						// Change position.
						_grid[column, notNull].Move(new Vector2(column, isNull));
						// Change index.
						_grid[column, isNull] = _grid[column, notNull];
						_grid[column, notNull] = null;

						notNull++;
						if (notNull >= _gridSize.y) {
							isNull++;
						}
						break;
					}
				}

				// No more elements on the column that can be moved.
				if (notNull >= _gridSize.y) {
					break;
				}
			}
		}

		// Replenish the column.
		Enums.GemId[] lineup = _spawner.Lineup;
		for (; isNull < _gridSize.y; isNull++) {
			_grid[column, isNull] = _spawner.Spawn(lineup[UnityEngine.Random.Range(0, lineup.Length)], new Vector2(column, isNull));
		}
	}

	void UpdateGrid() {
		// Clear matches.
		foreach (KeyValuePair<Vector2Int, int[]> kvp in _matches.OrderByDescending(i => i.Value.Sum())) {
			if (_grid[kvp.Key.x, kvp.Key.y] != null) {
				// Verify if all gems in the match exist.
				bool isNull = false;
				if (kvp.Value[0] + kvp.Value[1] > 0) {
					for (int i = kvp.Key.x + 1; i <= kvp.Key.x + kvp.Value[0]; i++) {
						if (_grid[i, kvp.Key.y] == null)
							isNull = true;
					}
					if (isNull)
						continue;
					for (int i = kvp.Key.x - 1; i >= kvp.Key.x - kvp.Value[1]; i--) {
						if (_grid[i, kvp.Key.y] == null)
							isNull = true;
					}
					if (isNull)
						continue;
				}
				if (kvp.Value[2] + kvp.Value[3] > 0) {
					for (int i = kvp.Key.y + 1; i <= kvp.Key.y + kvp.Value[2]; i++) {
						if (_grid[kvp.Key.x, i] == null)
							isNull = true;
					}
					if (isNull)
						continue;
					for (int i = kvp.Key.y - 1; i >= kvp.Key.y - kvp.Value[3]; i--) {
						if (_grid[kvp.Key.x, i] == null)
							isNull = true;
					}
					if (isNull)
						continue;
				}

				// Remove gems from board.
				if (kvp.Value[0] + kvp.Value[1] > 0) {
					for (int i = kvp.Key.x + 1; i <= kvp.Key.x + kvp.Value[0]; i++) {
						_grid[i, kvp.Key.y].Pool.ReturnToPool(_grid[i, kvp.Key.y].gameObject);
						_grid[i, kvp.Key.y] = null;
					}
					for (int i = kvp.Key.x - 1; i >= kvp.Key.x - kvp.Value[1]; i--) {
						_grid[i, kvp.Key.y].Pool.ReturnToPool(_grid[i, kvp.Key.y].gameObject);
						_grid[i, kvp.Key.y] = null;
					}
				}
				if (kvp.Value[2] + kvp.Value[3] > 0) {
					for (int i = kvp.Key.y + 1; i <= kvp.Key.y + kvp.Value[2]; i++) {
						_grid[kvp.Key.x, i].Pool.ReturnToPool(_grid[kvp.Key.x, i].gameObject);
						_grid[kvp.Key.x, i] = null;
					}
					for (int i = kvp.Key.y - 1; i >= kvp.Key.y - kvp.Value[3]; i--) {
						_grid[kvp.Key.x, i].Pool.ReturnToPool(_grid[kvp.Key.x, i].gameObject);
						_grid[kvp.Key.x, i] = null;
					}
				}
				_grid[kvp.Key.x, kvp.Key.y].Pool.ReturnToPool(_grid[kvp.Key.x, kvp.Key.y].gameObject);
				_grid[kvp.Key.x, kvp.Key.y] = null;

				GiveMatchPoints(kvp.Value[0] + kvp.Value[1], kvp.Value[2] + kvp.Value[3]);
			}
		}
		_matches.Clear();

		// Move down the gems and replenish the board
		for (int x = 0; x < _gridSize.x; x++) {
			UpdateColumn(x);
		}
	}
	#endregion

	#region Deadlock Check
	bool CheckDeadlockGrid() {
		for (int x = 0; x < _gridSize.x; x++) {
			for (int y = 0; y < _gridSize.y; y++) {
				if (!CheckDeadlockGem(new Vector2Int(x, y))) {
					return false;
				}
			}
		}

		return true;
	}

	// Obs: this method on his own do not indicate if the argument gem is a deadlock gem.
	bool CheckDeadlockGem(Vector2Int gem) {
		// Check double on top.
		if (gem.y + 1 < _gridSize.y) {
			if (_grid[gem.x, gem.y].Id == _grid[gem.x, gem.y + 1].Id) {
				// Verify double's extremities.
				if (gem.y - 1 >= 0) {
					if (gem.y - 2 >= 0)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x, gem.y - 2].Id)
							return false;
					if (gem.x + 1 < _gridSize.x)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x + 1, gem.y - 1].Id)
							return false;
					if (gem.x - 1 >= 0)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x - 1, gem.y - 1].Id)
							return false;
				}
				if (gem.y + 2 < _gridSize.y) {
					if (gem.y + 3 < _gridSize.y)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x, gem.y + 3].Id)
							return false;
					if (gem.x + 1 < _gridSize.x)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x + 1, gem.y + 2].Id)
							return false;
					if (gem.x - 1 >= 0)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x - 1, gem.y + 2].Id)
							return false;
				}
			}
		}
		// Check double to the right.
		if (gem.x + 1 < _gridSize.x) {
			if (_grid[gem.x, gem.y].Id == _grid[gem.x + 1, gem.y].Id) {
				// Verify double's extremities.
				if (gem.x - 1 >= 0) {
					if (gem.x - 2 >= 0)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x - 2, gem.y].Id)
							return false;
					if (gem.y + 1 < _gridSize.y)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x - 1, gem.y + 1].Id)
							return false;
					if (gem.y - 1 >= 0)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x - 1, gem.y - 1].Id)
							return false;
				}
				if (gem.x + 2 < _gridSize.x) {
					if (gem.x + 3 < _gridSize.x)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x + 3, gem.y].Id)
							return false;
					if (gem.y + 1 < _gridSize.y)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x + 2, gem.y + 1].Id)
							return false;
					if (gem.y - 1 >= 0)
						if (_grid[gem.x, gem.y].Id == _grid[gem.x + 2, gem.y - 1].Id)
							return false;
				}
			}
		}

		// Check open middle vertical.
		if ((gem.y + 1 < _gridSize.y) && (gem.y - 1 >= 0))
			if (_grid[gem.x, gem.y + 1].Id == _grid[gem.x, gem.y - 1].Id) {
				// Verify opening's neighbors.
				if (gem.x + 1 < _gridSize.x)
					if (_grid[gem.x, gem.y + 1].Id == _grid[gem.x + 1, gem.y].Id)
						return false;
				if (gem.x - 1 >= 0)
					if (_grid[gem.x, gem.y + 1].Id == _grid[gem.x - 1, gem.y].Id)
						return false;
			}
		// Check open middle horizontal.
		if ((gem.x + 1 < _gridSize.x) && (gem.x - 1 >= 0))
			if (_grid[gem.x + 1, gem.y].Id == _grid[gem.x - 1, gem.y].Id) {
				// Verify opening's neightbors.
				if (gem.y + 1 < _gridSize.y)
					if (_grid[gem.x + 1, gem.y].Id == _grid[gem.x, gem.y + 1].Id)
						return false;
				if (gem.y - 1 >= 0)
					if (_grid[gem.x + 1, gem.y].Id == _grid[gem.x, gem.y - 1].Id)
						return false;
			}

		return true;
	}
	#endregion
}
