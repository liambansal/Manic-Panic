using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FinishLine : MonoBehaviour {
	[SerializeField]
	private ScoreController scoreController = null;

	[SerializeField]
	private Text winTextbox = null;

	private int playersFinished = 0;
	private int playersDead = 0;

	private LinkedList<string> players = new LinkedList<string> { };

	private void Start() {
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			players.AddLast(player.name);
		}

		scoreController.CompareScore(players); // temp code
	}

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
		if (playersDead > 0) {
			Time.timeScale = 0;
			winTextbox.text = scoreController.CompareScore(players);
		} else if (playersFinished == 2) {
			Time.timeScale = 0;
			winTextbox.text = scoreController.CompareScore(players);
		}
	}

	internal void PlayerDied() {
		++playersDead;
	}
}