using UnityEngine;

public class ButtonRoundStart : MonoBehaviour {
	public void StartRound() {
		FindObjectOfType<RoundManager>().RoundStart();
	}
}
