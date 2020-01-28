using UnityEngine;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour {
	private string playerOne = "Player One";
	private string playerTwo = "Player Two";

	[SerializeField]
	private Text winTextbox = null;

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.name == playerOne) {
			winTextbox.text = (playerOne + " Wins");
			Time.timeScale = 0;
		} else if (other.gameObject.name == playerTwo) {
			winTextbox.text = (playerTwo + " Wins");
			Time.timeScale = 0;
		}
	}
}