using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupCamera : MonoBehaviour {
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
		//if (camSize.x < _gridSize.x) {
		camSize.y = ((float)_gridSize.x + 1) / aspectRatio;
		Camera.main.orthographicSize = camSize.y / 2;
		//}
		if (camSize.y < _gridSize.y) {
			Camera.main.orthographicSize = (float)_gridSize.y / 2;
		}

		// Put camera on board center.
		Camera.main.transform.position = new Vector3(((float)_gridSize.x - 1) / 2, ((float)_gridSize.y - 1) / 2, -10);
	}
}
