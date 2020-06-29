using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonTransition : MonoBehaviour {
	[SerializeField] GameObject _transition = null;
	SceneHandler _sceneHandler;

	void Awake() {
		_sceneHandler = FindObjectOfType<SceneHandler>() ? FindObjectOfType<SceneHandler>().GetComponent<SceneHandler>() : null;

		if (!_sceneHandler) {
			Debug.LogWarning("No SceneHandler in the scene!");
		}
	}

	public void LoadScene(string scene) {
		if (_transition) {
			_sceneHandler.LoadScene(scene, _transition);
		}
		else {
			_sceneHandler.LoadScene(scene);
		}
	}
}
