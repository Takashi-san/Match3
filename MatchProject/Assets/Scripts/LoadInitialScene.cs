using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadInitialScene : MonoBehaviour {
	[SerializeField] string _scene = "";

	void Start() {
		SceneManager.LoadScene(_scene);
	}
}
