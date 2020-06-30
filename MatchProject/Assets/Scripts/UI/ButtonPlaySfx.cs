using UnityEngine;

public class ButtonPlaySfx : MonoBehaviour {
	SfxPlayer _source;

	void Start() {
		_source = FindObjectOfType<SfxPlayer>();
	}

	public void PlaySfx(AudioClip sfx) {
		if (_source != null)
			_source.PlaySfx(sfx);
	}
}
