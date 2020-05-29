using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreController : MonoBehaviour {
	[SerializeField]
	private Text[] playerScoreText = new Text[4];

	[SerializeField]
	private string[] playerGameObjectNames = new string[4];

	private int maximumPlayerCount = 4;
	private int[] playerScores = new int[4];

	/// <summary>
	/// Initializes the players' score values with the value of their collected coins.
	/// Public so it can be called by the player manager after its spawned the players.
	/// </summary>
	public void InitializeScores() {
		for (int i = 0; i < maximumPlayerCount; ++i) {
			if (PlayerPrefs.HasKey("P" + (i + 1).ToString())) {
				// Initialize each player's score.
				playerScores[i] = GameObject.Find(playerGameObjectNames[i]).GetComponent<TreasureBag>().CoinsCollected;
			}
		}
	}

	public void IncreaseScore(string playerName, int treasureValue) {
		for (int i = 0; i < maximumPlayerCount; ++i) {
			if (playerName == playerGameObjectNames[i]) {
				playerScores[i] += treasureValue;

				// If I modulus 2 is event then we're examining data for player one or three (they're on the 
				// same team).
				if (i % 2 == 0) {
					playerScoreText[i].text = ("Blue Score: " + (playerScores[0] + playerScores[2]).ToString());
				} else {
					playerScoreText[i].text = ("Red Score: " + (playerScores[1] + playerScores[3]).ToString());
				}
			}
		}
	}

	public void DecreaseScore(string playerName, int scoreValue) {
		for (int i = 0; i < maximumPlayerCount; ++i) {
			if (playerName == playerGameObjectNames[i]) {
				playerScores[i] -= scoreValue;

				// If I modulus 2 is event then we're examining data for player one or three (they're on the 
				// same team).
				if (i % 2 == 0) {
					playerScoreText[i].text = ("Blue Score: " + (playerScores[0] + playerScores[2]).ToString());
				} else {
					playerScoreText[i].text = ("Red Score: " + (playerScores[1] + playerScores[3]).ToString());
				}
			}
		}
	}

	internal string CompareScore(LinkedList<string> players) {
		int topScore = 0;
		string winText = null;

		if (players.Count == 0) {
			winText = "It's A Draw!";
			return winText;
		} else if ((players.Contains("Player One(Clone)") || players.Contains("Player Three(Clone)")) && (players.Contains("Player Two(Clone)") || players.Contains("Player Four(Clone)"))) {
			if ((playerScores[0] + playerScores[2]) == (playerScores[1] + playerScores[3])) {
				winText = "It's A Draw!";
				return winText;
			}
		} else if (players.Contains("Player One(Clone)") || players.Contains("Player Three(Clone)")) {
			if ((playerScores[0] + playerScores[2]) > topScore) {
				winText = "Blue Team Wins!";
				topScore = (playerScores[0] + playerScores[2]);
			}
		} else if (players.Contains("Player Two(Clone)") || players.Contains("Player Four(Clone)")) {
			if ((playerScores[1] + playerScores[3]) > topScore) {
				winText = "Red Team Wins!";
				topScore = (playerScores[1] + playerScores[3]);
			}
		}

		return winText;
	}
}