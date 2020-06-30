using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SfxPlayer : MonoBehaviour {
	[SerializeField] AudioMixerGroup _sfxMixerGroup = null;

	AudioSource _audioSource;
	SfxPlayer _instance;
	public SfxPlayer Instance => _instance;

	void Awake() {
		if (_instance == null) {
			_instance = this;
		}
		else {
			Debug.LogWarning("Duplicate of SfxPlayer in: " + gameObject.name);
			Destroy(this);
		}

		_audioSource = gameObject.AddComponent<AudioSource>();
		_audioSource.outputAudioMixerGroup = _sfxMixerGroup;
	}

	public void PlaySfx(AudioClip sfx) {
		_audioSource.PlayOneShot(sfx);
	}
}
