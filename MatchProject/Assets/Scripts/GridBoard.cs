using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour {
	[SerializeField] [Min(1)] Vector2Int _size = Vector2Int.one;
	[SerializeField] GameObject[] _gems = null;

	GameObject[,] _grid;

	void Awake() {
		SetupGrid();
		SetupCamera();
	}

	void Start() {
		PlayerInput.Instance.swipe += CheckMove;
	}

	void SetupGrid() {
		_grid = new GameObject[_size.x, _size.y];
		for (int x = 0; x < _size.x; x++) {
			for (int y = 0; y < _size.y; y++) {
				_grid[x, y] = Instantiate(_gems[Random.Range(0, _gems.Length)], new Vector2(x, y), Quaternion.identity);
			}
		}
	}

	void SetupCamera() {
		// Get screen and camera information.
		float aspectRatio = (float)Screen.width / Screen.height;
		Vector2 camSize;
		camSize.y = Camera.main.orthographicSize * 2;
		camSize.x = camSize.y * aspectRatio;

		// Determines the correct camera size to fit the board.
		//if (camSize.x < _size.x) {
		camSize.y = ((float)_size.x + 1) / aspectRatio;
		Camera.main.orthographicSize = camSize.y / 2;
		//}
		if (camSize.y < _size.y) {
			Camera.main.orthographicSize = (float)_size.y / 2;
		}

		// Put camera on board center.
		Camera.main.transform.position = new Vector3(((float)_size.x - 1) / 2, ((float)_size.y - 1) / 2, -10);
	}

	void CheckMove(Vector2 swipePosition, Enums.Direction direction) {
		//Debug.Log("" + swipePosition + " " + direction);

		// Check if input was on the board.
		swipePosition += new Vector2(0.5f, 0.5f);
		if ((swipePosition.x >= 0) && (swipePosition.y >= 0)) {
			if ((swipePosition.x < _size.x) && (swipePosition.y < _size.y)) {
				Vector2Int gemA = new Vector2Int(Mathf.FloorToInt(swipePosition.x), Mathf.FloorToInt(swipePosition.y));
				Vector2Int gemB = Vector2Int.zero;

				// Check if input is possible.
				switch (direction) {
					case Enums.Direction.Right:
						if (gemA.x + 1 == _size.x) {
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
						if (gemA.y + 1 == _size.y) {
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
			}
		}
	}

	void DoMove(Vector2Int gemA, Vector2Int gemB) {
		GameObject tmp = _grid[gemB.x, gemB.y];

		// Change position.
		_grid[gemA.x, gemA.y].transform.position = new Vector2(gemB.x, gemB.y);
		_grid[gemB.x, gemB.y].transform.position = new Vector2(gemA.x, gemA.y);

		// Change index.
		_grid[gemB.x, gemB.y] = _grid[gemA.x, gemA.y];
		_grid[gemA.x, gemA.y] = tmp;
	}
}
