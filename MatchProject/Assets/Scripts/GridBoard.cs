using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour {
	[SerializeField] [Min(1)] Vector2Int _gridSize = Vector2Int.one;
	[SerializeField] GameObject[] _gems = null;

	public Vector2Int GridSize => _gridSize;

	Gem[,] _grid;

	void Awake() {
		SetupGrid();
	}

	void Start() {
		PlayerInput.Instance.swipe += CheckMove;
	}

	void SetupGrid() {
		_grid = new Gem[_gridSize.x, _gridSize.y];
		for (int x = 0; x < _gridSize.x; x++) {
			for (int y = 0; y < _gridSize.y; y++) {
				_grid[x, y] = Instantiate(_gems[Random.Range(0, _gems.Length)], new Vector2(x, y), Quaternion.identity).GetComponent<Gem>();
			}
		}
	}

	void CheckMove(Vector2 swipePosition, Enums.Direction direction) {
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

				DoMove(gemA, gemB);

				CheckMatch(gemA);
				CheckMatch(gemB);
			}
		}
	}

	void DoMove(Vector2Int gemA, Vector2Int gemB) {
		Gem tmp = _grid[gemB.x, gemB.y];

		// Change position.
		_grid[gemA.x, gemA.y].transform.position = new Vector2(gemB.x, gemB.y);
		_grid[gemB.x, gemB.y].transform.position = new Vector2(gemA.x, gemA.y);

		// Change index.
		_grid[gemB.x, gemB.y] = _grid[gemA.x, gemA.y];
		_grid[gemA.x, gemA.y] = tmp;
	}

	bool CheckMatch(Vector2Int gem) {
		Enums.GemId gemId = _grid[gem.x, gem.y].Id;
		bool hasHoriMatch = false;
		int matchHoriPos = 0;
		int matchHoriNeg = 0;
		bool hasVertMatch = false;
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

		// Resolve matches.
		if (hasHoriMatch || hasVertMatch) {
			if (hasHoriMatch) {
				for (int i = gem.x + 1; i <= gem.x + matchHoriPos; i++) {
					// Clear gem.
					Destroy(_grid[i, gem.y].gameObject);
					_grid[i, gem.y] = null;
					MoveColumn(i);
				}
				for (int i = gem.x - 1; i >= gem.x - matchHoriNeg; i--) {
					// Clear gem.
					Destroy(_grid[i, gem.y].gameObject);
					_grid[i, gem.y] = null;
					MoveColumn(i);
				}
			}

			if (hasVertMatch) {
				for (int i = gem.y + 1; i <= gem.y + matchVertPos; i++) {
					// Clear gem.
					Destroy(_grid[gem.x, i].gameObject);
					_grid[gem.x, i] = null;
				}
				for (int i = gem.y - 1; i >= gem.y - matchVertNeg; i--) {
					// Clear gem.
					Destroy(_grid[gem.x, i].gameObject);
					_grid[gem.x, i] = null;
				}
			}

			// Clear target gem.
			Destroy(_grid[gem.x, gem.y].gameObject);
			_grid[gem.x, gem.y] = null;
			MoveColumn(gem.x);

			return true;
		}
		else {
			return false;
		}
	}

	void MoveColumn(int column) {
		int noNull = 0;

		// Search for null.
		for (int i = 0; i < _gridSize.y; i++) {
			if (_grid[column, i] == null) {
				// Search for not null to replace.
				if (noNull < i) {
					noNull = i;
				}
				for (int j = noNull + 1; j < _gridSize.y; j++) {
					noNull = j;
					if (_grid[column, j] != null) {
						// Change position.
						_grid[column, j].transform.position = new Vector2(column, i);
						// Change index.
						_grid[column, i] = _grid[column, j];
						_grid[column, j] = null;
						break;
					}
				}

				// No more elements on the column to move.
				if (noNull + 1 == _gridSize.y) {
					break;
				}
			}
		}
	}
}
