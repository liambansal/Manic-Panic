using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FinishLine : MonoBehaviour {
	[SerializeField]
	private ScoreController scoreController = null;

	[SerializeField]
	private Text winTextbox = null;

	private int playersFinished = 0;
	private int playersAlive = 2;

	private LinkedList<string> players = new LinkedList<string> { };

	private void Start() {
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			players.AddLast(player.name);
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if ((other.gameObject.name == "Player One") || (other.gameObject.name == "Player Two")) {
			Destroy(other.gameObject);
			++playersFinished;
			DisplayWinner();
		}
	}

	private void DisplayWinner() {
		if (playersFinished == playersAlive) {
			Time.timeScale = 0;
			winTextbox.text = scoreController.CompareScore(players);
		}
	}

	internal void PlayerDied(string player) {
		if (players.Contains(player)) {
			players.Remove(player);
			--playersAlive;
			DisplayWinner();
		}
	}
}