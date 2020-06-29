using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour {
	[SerializeField] Canvas _menu = null;

	void Start() {
		if (!_menu) {
			Debug.LogWarning("No Menu specified to control");
		}
		else {
			_menu.enabled = false;
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (_menu.enabled) {
				Deactivate();
			}
			else {
				Activate();
			}
		}
	}

	void OnDestroy() {
		Time.timeScale = 1;
	}

	public void Activate() {
		_menu.enabled = true;
		Time.timeScale = 0;
	}

	public void Deactivate() {
		_menu.enabled = false;
		Time.timeScale = 1;
	}
}
