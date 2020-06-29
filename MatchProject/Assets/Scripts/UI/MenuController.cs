using UnityEngine;
using UnityEngine.InputSystem;

public class MenuController : MonoBehaviour {
	[SerializeField] GameObject _menu = null;

	void Start() {
		if (!_menu) {
			Debug.LogWarning("No Menu specified to control");
		}
		else {
			_menu.SetActive(false);
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
