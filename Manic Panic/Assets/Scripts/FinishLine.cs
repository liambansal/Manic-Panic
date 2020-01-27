using UnityEngine;
using UnityEngine.UI;

public class FinishLine : MonoBehaviour {
	private string playerOne = "Player One";
	private string playerTwo = "Player Two";

	[SerializeField]
	private Text winTextbox = null;

	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Player")) {
			winTextbox.text = (playerOne + "Wins");
		} else if (other.gameObject.name == playerTwo) {
			winTextbox.text = (playerTwo + "Wins");
		}
	}
}