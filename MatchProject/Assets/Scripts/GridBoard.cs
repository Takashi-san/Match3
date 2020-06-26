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

	// Important thing to take note is that the gem's index on the grid are the same as their position in world space.
	// This detail is important in multiple places in the code and how the scene is being setup in the game.
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

				// Check if one of the gems is null.
				if (_grid[gemA.x, gemA.y] == null) {
					return;
				}
				if (_grid[gemB.x, gemB.y] == null) {
					return;
				}

				Swap(gemA, gemB);

				// Check if valid movement.
				CheckMatchGem(gemA);
				CheckMatchGem(gemB);
				UpdateGrid();

				// Successfull movement.
				while (CheckMatchGrid()) {
					UpdateGrid();
				}
			}
		}
	}

	void Swap(Vector2Int gemA, Vector2Int gemB) {
		Gem tmp = _grid[gemB.x, gemB.y];

		// Change position.
		_grid[gemA.x, gemA.y].transform.position = new Vector2(gemB.x, gemB.y);
		_grid[gemB.x, gemB.y].transform.position = new Vector2(gemA.x, gemA.y);

		// Change index.
		_grid[gemB.x, gemB.y] = _grid[gemA.x, gemA.y];
		_grid[gemA.x, gemA.y] = tmp;
	}

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

		// Resolve matches.
		if (hasHoriMatch || hasVertMatch) {
			if (hasHoriMatch) {
				for (int i = gem.x + 1; i <= gem.x + matchHoriPos; i++) {
					// Clear gem.
					Destroy(_grid[i, gem.y].gameObject);
					_grid[i, gem.y] = null;
				}
				for (int i = gem.x - 1; i >= gem.x - matchHoriNeg; i--) {
					// Clear gem.
					Destroy(_grid[i, gem.y].gameObject);
					_grid[i, gem.y] = null;
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

			GiveMatchPoints(matchHoriNeg + matchHoriPos, matchVertNeg + matchVertPos);

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
						_grid[column, notNull].transform.position = new Vector2(column, isNull);
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
		for (; isNull < _gridSize.y; isNull++) {
			_grid[column, isNull] = Instantiate(_gems[Random.Range(0, _gems.Length)], new Vector2(column, isNull), Quaternion.identity).GetComponent<Gem>();
		}
	}

	void UpdateGrid() {
		for (int x = 0; x < _gridSize.x; x++) {
			UpdateColumn(x);
		}
	}

	void GiveMatchPoints(int horiMatch, int vertMatch) {
		if ((horiMatch > 1) && (vertMatch > 1)) {
			// crux.
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
}
