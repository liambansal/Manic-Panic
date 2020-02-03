using UnityEngine;
using UnityEngine.UI;

public class FinishLine : ScoreController {
	[SerializeField]
	private Text winTextbox = null;

	private int playersFinished = 0;

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.name == "Player One") {
			Destroy(other.gameObject);
			++playersFinished;
			DisplayWinner();
		} else if (other.gameObject.name == "Player Two") {
			Destroy(other.gameObject);
			++playersFinished;
			DisplayWinner();
		}
	}

	private void DisplayWinner() {
		if (playersFinished == 2) {
			Time.timeScale = 0;
			winTextbox.text = CompareScore();
		}
	}
}