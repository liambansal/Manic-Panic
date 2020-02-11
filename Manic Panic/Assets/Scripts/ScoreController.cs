using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreController : MonoBehaviour {
	[SerializeField]
	private Text playerOneScoreText = null;
	[SerializeField]
	private Text playerTwoScoreText = null;

	private int playerOneScore = 0;
	private int playerTwoScore = 0;

	private void Start() {
		playerOneScore = GameObject.Find("Player One").GetComponent<TreasureBag>().CoinsCollected;
		playerTwoScore = GameObject.Find("Player Two").GetComponent<TreasureBag>().CoinsCollected;
	}

	public void IncreaseScore(string playerName, int treasureValue) {
		if (playerName == "Player One") {
			playerOneScore += treasureValue;
			playerOneScoreText.text = ("Blue Score: " + playerOneScore.ToString());
		}

		if (playerName == "Player Two") {
			playerTwoScore += treasureValue;
			playerTwoScoreText.text = ("Red Score: " + playerTwoScore.ToString());
		}
	}

	public void DecreaseScore(string playerName, int scoreValue) {
		if (playerName == "Player One") {
			playerOneScore -= scoreValue;
			playerOneScoreText.text = ("Blue Score: " + playerOneScore.ToString());
		}

		if (playerName == "Player Two") {
			playerTwoScore -= scoreValue;
			playerTwoScoreText.text = ("Red Score: " + playerTwoScore.ToString());
		}
	}

	internal string CompareScore(LinkedList<string> players) {
		int topScore = 0;
		string winText = null;

		if (players.Count == 0) {
			winText = "It's A Draw!";
			return winText;
		}

		if (players.Contains("Player Two") && players.Contains("Player One")) {
			if (playerOneScore == playerTwoScore) {
				winText = "It's A Draw!";
				return winText;
			}
		}

		if (players.Contains("Player One")) {
			if (playerOneScore > topScore) {
				winText = "Player One Wins!";
				topScore = playerOneScore;
			}
		}

		if (players.Contains("Player Two")) {
			if (playerTwoScore > topScore) {
				winText = "Player Two Wins!";
				topScore = playerTwoScore;
			}
		}

		return winText;
	}
}