using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBoard : MonoBehaviour {
	[SerializeField] [Min(1)] int _sizeX = 1;
	[SerializeField] [Min(1)] int _sizeY = 1;
	[SerializeField] GameObject[] _gems = null;

	GameObject[,] _grid;

	void Awake() {
		SetupGrid();
		SetupCamera();
	}

	void SetupGrid() {
		_grid = new GameObject[_sizeX, _sizeY];
		for (int x = 0; x < _sizeX; x++) {
			for (int y = 0; y < _sizeY; y++) {
				_grid[x, y] = Instantiate(_gems[Random.Range(0, _gems.Length)], new Vector2(x, y), Quaternion.identity);
			}
		}
	}

	void SetupCamera() {
		float aspectRatio = (float)Screen.width / Screen.height;
		Vector2 camSize;
		camSize.y = Camera.main.orthographicSize * 2;
		camSize.x = camSize.y * aspectRatio;

		if (camSize.x < _sizeX) {
			camSize.y = (float)_sizeX / aspectRatio;
			Camera.main.orthographicSize = camSize.y / 2;
		}
		if (camSize.y < _sizeY) {
			Camera.main.orthographicSize = (float)_sizeY / 2;
		}

		Camera.main.transform.position = new Vector3(((float)_sizeX - 1) / 2, ((float)_sizeY - 1) / 2, -10);
	}
}
