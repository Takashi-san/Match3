using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour {
	TextMeshProUGUI _text;

	void Awake() {
		_text = gameObject.GetComponent<TextMeshProUGUI>();
		FindObjectOfType<Timer>().timerUpdate += UpdateText;
	}

	void UpdateText(float timer) {
		int minutes = Mathf.FloorToInt(timer / 60);
		int seconds = Mathf.FloorToInt(timer % 60);
		_text.text = minutes.ToString("00") + ":" + seconds.ToString("00");
	}
}
