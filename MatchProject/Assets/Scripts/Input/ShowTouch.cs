using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTouch : MonoBehaviour {
	public int[] id;

	void Update() {
		Vector3 center = new Vector3(transform.position.x, transform.position.y, 0);
		for (int i = 0; i < Input.touchCount; i++) {
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
			id[i] = Input.touches[i].fingerId;
			switch (i) {
				case 0:
					Debug.DrawLine(center, touchPosition, Color.red);
					break;
				case 1:
					Debug.DrawLine(center, touchPosition, Color.blue);
					break;
				case 2:
					Debug.DrawLine(center, touchPosition, Color.green);
					break;
				case 3:
					Debug.DrawLine(center, touchPosition, Color.magenta);
					break;
				case 4:
					Debug.DrawLine(center, touchPosition, Color.cyan);
					break;
				case 5:
					Debug.DrawLine(center, touchPosition, Color.yellow);
					break;
				default:
					Debug.DrawLine(center, touchPosition, Color.black);
					break;
			}
		}
	}
}
