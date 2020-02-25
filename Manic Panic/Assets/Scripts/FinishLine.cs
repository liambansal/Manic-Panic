using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FinishLine : MonoBehaviour {
	[SerializeField]
	private ScoreController scoreController = null;

	[SerializeField]
	private GameObject deathScreenOne = null;
	[SerializeField]
	private GameObject deathScreenTwo = null;

	[SerializeField]
	private Text winTextbox = null;

	private int playersFinished = 0;
	private int playersAlive = 2;

	private LinkedList<string> players = new LinkedList<string> { };

	private Time animationTimeline;

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
			deathScreenOne.SetActive(false);
			deathScreenTwo.SetActive(false);
			winTextbox.gameObject.SetActive(true);
			winTextbox.text = scoreController.CompareScore(players);
			winTextbox.GetComponent<Animator>().SetBool("playAnimation", true);
		}
	}

	internal void PlayerDied(string player) {
		if (players.Contains(player)) {
			players.Remove(player);
			--playersAlive;

			if (player == "Player One") {
				deathScreenOne.SetActive(true);
				deathScreenOne.GetComponent<Animator>().SetBool("fadeIn", true);
			} else if (player == "Player Two") {
				deathScreenTwo.SetActive(true);
				deathScreenTwo.GetComponent<Animator>().SetBool("fadeIn", true);
			}

			DisplayWinner();
		}
	}
}