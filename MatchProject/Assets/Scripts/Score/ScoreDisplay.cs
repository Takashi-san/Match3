using UnityEngine;
using TMPro;

public class ScoreDisplay : MonoBehaviour {
	TextMeshProUGUI _text;

	void Awake() {
		_text = gameObject.GetComponent<TextMeshProUGUI>();
		FindObjectOfType<ScoreManager>().scoreUpdate += UpdateText;
	}

	void UpdateText(int score) {
		_text.text = "" + score;
	}
}
