using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour {
	[SerializeField] TransitionData _transitionData = null;
	[SerializeField] GameObject _defaultTransition = null;
	Animator _animator = null;

	void Awake() {
		if (_transitionData) {
			if (_transitionData.transition) {
				_animator = Instantiate(_transitionData.transition).GetComponent<Animator>();
			}
			else if (_defaultTransition) {
				_animator = Instantiate(_defaultTransition).GetComponent<Animator>();
			}
		}
		else if (_defaultTransition) {
			_animator = Instantiate(_defaultTransition).GetComponent<Animator>();
		}
	}

	public void LoadScene(string scene) {
		StartCoroutine(DoSceneLoad(scene));
	}

	public void LoadScene(string scene, GameObject transition) {
		StartCoroutine(DoSceneLoad(scene, transition));
	}

	IEnumerator DoSceneLoad(string scene) {
		if (_defaultTransition) {
			if (_transitionData) {
				_transitionData.transition = _defaultTransition;
			}
			_animator = Instantiate(_defaultTransition).GetComponent<Animator>();
		}
		else {
			if (_transitionData) {
				_transitionData.transition = null;
			}
			_animator = null;
		}

		if (_animator) {
			_animator.SetBool("in", true);
			yield return new WaitForEndOfFrame();
			yield return new WaitForSecondsRealtime(_animator.GetCurrentAnimatorStateInfo(0).length);
		}
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		yield break;
	}

	IEnumerator DoSceneLoad(string scene, GameObject transition) {
		if (transition) {
			if (_transitionData) {
				_transitionData.transition = transition;
			}
			_animator = Instantiate(transition).GetComponent<Animator>();
		}
		else if (_defaultTransition) {
			if (_transitionData) {
				_transitionData.transition = _defaultTransition;
			}
			_animator = Instantiate(_defaultTransition).GetComponent<Animator>();
		}
		else {
			if (_transitionData) {
				_transitionData.transition = null;
			}
			_animator = null;
		}

		if (_animator) {
			_animator.SetBool("in", true);
			yield return new WaitForEndOfFrame();
			yield return new WaitForSecondsRealtime(_animator.GetCurrentAnimatorStateInfo(0).length);
		}
		AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
		yield break;
	}

	public void QuitGame() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
         //Application.OpenURL("google.com");
#else
         Application.Quit();
#endif
	}

	void OnApplicationQuit() {
		if (_transitionData) {
			_transitionData.transition = null;
		}
	}
}
