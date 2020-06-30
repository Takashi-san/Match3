using UnityEngine;

public class ButtonSceneLoad : MonoBehaviour {
	public void LoadScene(string scene) {
		FindObjectOfType<SceneHandler>().LoadScene(scene);
	}
}
