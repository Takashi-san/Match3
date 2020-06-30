using UnityEngine;

public class SetupBg : MonoBehaviour {
	void Start() {
		transform.localScale = Vector3.one * Camera.main.orthographicSize * 2 * 1.25f;
		transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
	}
}
