using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreController : MonoBehaviour {
	[SerializeField]
	private Text playerOneScoreText = null;
	[SerializeField]
	private Text playerTwoScoreText = null;
	[SerializeField]
	private Text playerThreeScoreText = null;
	[SerializeField]
	private Text playerFourScoreText = null;

	private int playerOneScore = 0;
	private int playerTwoScore = 0;
	private int playerThreeScore = 0;
	private int playerFourScore = 0;

	private void Start() {
		playerOneScore = GameObject.Find("Player One").GetComponent<TreasureBag>().CoinsCollected;
		playerTwoScore = GameObject.Find("Player Two").GetComponent<TreasureBag>().CoinsCollected;
		playerThreeScore = GameObject.Find("Player Three").GetComponent<TreasureBag>().CoinsCollected;
		playerFourScore = GameObject.Find("Player Four").GetComponent<TreasureBag>().CoinsCollected;
	}

	public void IncreaseScore(string playerName, int treasureValue) {
		if (playerName == "Player One") {
			playerOneScore += treasureValue;
			playerOneScoreText.text = ("Blue Score: " + (playerOneScore + playerThreeScore).ToString());
		}

		if (playerName == "Player Two") {
			playerTwoScore += treasureValue;
			playerTwoScoreText.text = ("Red Score: " + (playerTwoScore + playerFourScore).ToString());
		}

		if (playerName == "Player Three") {
			playerThreeScore += treasureValue;
			playerThreeScoreText.text = ("Blue Score: " + (playerOneScore + playerThreeScore).ToString());
		}

		if (playerName == "Player Four") {
			playerFourScore += treasureValue;
			playerFourScoreText.text = ("Red Score: " + (playerTwoScore + playerFourScore).ToString());
		}
	}

	public void DecreaseScore(string playerName, int scoreValue) {
		if (playerName == "Player One") {
			playerOneScore -= scoreValue;
			playerOneScoreText.text = ("Blue Score: " + (playerOneScore + playerThreeScore).ToString());
		}

		if (playerName == "Player Two") {
			playerTwoScore -= scoreValue;
			playerTwoScoreText.text = ("Red Score: " + (playerTwoScore + playerFourScore).ToString());
		}

		if (playerName == "Player Three") {
			playerThreeScore -= scoreValue;
			playerThreeScoreText.text = ("Blue Score: " + (playerOneScore + playerThreeScore).ToString());
		}

		if (playerName == "Player Four") {
			playerFourScore -= scoreValue;
			playerFourScoreText.text = ("Red Score: " + (playerTwoScore + playerFourScore).ToString());
		}
	}

	internal string CompareScore(LinkedList<string> players) {
		int topScore = 0;
		string winText = null;

		if (players.Count == 0) {
			winText = "It's A Draw!";
			return winText;
		} else if ((players.Contains("Player One") || players.Contains("Player Three")) && (players.Contains("Player Two") || players.Contains("Player Four"))) {
			if ((playerOneScore + playerThreeScore) == (playerTwoScore + playerFourScore)) {
				winText = "It's A Draw!";
				return winText;
			}
		} else if (players.Contains("Player One") || players.Contains("Player Three")) {
			if ((playerOneScore + playerThreeScore) > topScore) {
				winText = "Blue Team Wins!";
				topScore = (playerOneScore + playerThreeScore);
			}
		} else if (players.Contains("Player Two") || players.Contains("Player Four")) {
			if ((playerTwoScore + playerFourScore) > topScore) {
				winText = "Red Team Wins!";
				topScore = (playerTwoScore + playerFourScore);
			}
		}

		return winText;
	}
}