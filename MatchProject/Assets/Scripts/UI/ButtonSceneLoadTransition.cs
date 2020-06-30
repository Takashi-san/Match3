using UnityEngine;

public class ButtonSceneLoadTransition : MonoBehaviour {
	[SerializeField] GameObject _transition = null;

	public void LoadScene(string scene) {
		if (_transition != null) {
			FindObjectOfType<SceneHandler>().LoadScene(scene, _transition);
		}
		else {
			FindObjectOfType<SceneHandler>().LoadScene(scene);
		}
	}
}
