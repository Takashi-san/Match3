using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupCamera : MonoBehaviour {
	[SerializeField] [Range(0.01f, 1)] float _heightLimit = 1;
	[SerializeField] [Range(0.01f, 1)] float _heightMargin = 1;
	Vector2Int _gridSize;

	void Awake() {
		_gridSize = FindObjectOfType<GridBoard>().GridSize;
		Setup();
	}

	void Setup() {
		// Get screen and camera information.
		float aspectRatio = (float)Screen.width / Screen.height;
		Vector2 camSize;
		camSize.y = Camera.main.orthographicSize * 2;
		camSize.x = camSize.y * aspectRatio;

		// Determines the correct camera size to fit the board.
		camSize.y = ((float)_gridSize.x + 1) / aspectRatio;
		Camera.main.orthographicSize = camSize.y / 2;
		if (camSize.y * _heightLimit < _gridSize.y) {
			Camera.main.orthographicSize = ((float)_gridSize.y / 2) / _heightLimit;
		}

		// Place grid at bottom of the camera view with a margin based on the height limit given.
		Camera.main.transform.position = new Vector3(((float)_gridSize.x - 1) / 2, Camera.main.orthographicSize - 0.5f - (1 - _heightLimit) * _heightMargin * Camera.main.orthographicSize * 2, -10);
	}
}
