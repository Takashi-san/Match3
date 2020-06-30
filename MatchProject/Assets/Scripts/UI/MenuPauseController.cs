using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuPauseController : MonoBehaviour {
	[SerializeField] GameObject _menu = null;

	void Start() {
		if (!_menu) {
			Debug.LogWarning("No Menu specified to control");
		}
		else {
			_menu.SetActive(false);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (_menu.activeInHierarchy) {
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
		_menu.SetActive(true);
		Time.timeScale = 0;
	}

	public void Deactivate() {
		_menu.SetActive(false);
		Time.timeScale = 1;
	}
}
