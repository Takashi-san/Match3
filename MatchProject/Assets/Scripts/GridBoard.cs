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
		PlayerInput.Instance.swipe += checkMove;
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
		float aspectRatio = (float)Screen.width / Screen.height;
		Vector2 camSize;
		camSize.y = Camera.main.orthographicSize * 2;
		camSize.x = camSize.y * aspectRatio;

		if (camSize.x < _size.x) {
			camSize.y = (float)_size.x / aspectRatio;
			Camera.main.orthographicSize = camSize.y / 2;
		}
		if (camSize.y < _size.y) {
			Camera.main.orthographicSize = (float)_size.y / 2;
		}

		Camera.main.transform.position = new Vector3(((float)_size.x - 1) / 2, ((float)_size.y - 1) / 2, -10);
	}

	void checkMove(Vector2 swipePosition, Enums.Direction direction) {
		//Debug.Log("" + swipePosition + " " + direction);
	}
}
