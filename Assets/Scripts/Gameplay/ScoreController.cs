using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	[SerializeField]
	private Text[] playerScoreText = new Text[4];
	[SerializeField]
	private string[] playerGameObjectNames = new string[4];

	/// <summary>
	/// Scores for players one through four.
	/// Index 0 = player 1, index 1 = player 2 etc.
	/// </summary>
	private int[] playerScores = new int[4];
	private const int maximumPlayerCount = 4;

	/// <summary>
	/// Initializes the players' score values with the value of their collected coins.
	/// </summary>
	public void InitializeScores() {
		for (int i = 0; i < maximumPlayerCount; ++i) {
			const string letterPrefix = "P";

			if (PlayerPrefs.HasKey(letterPrefix + (i + 1).ToString())) {
				playerScores[i] = GameObject.Find(playerGameObjectNames[i]).GetComponent<TreasureBag>().coinsCollected;
			}
		}
	}

	/// <summary>
	/// Increases a player's score by a select amount.
	/// </summary>
	/// <param name="playerName"> The name of the player whose score will be increased. </param>
	/// <param name="scoreValue"> Amount to add onto the player's score. </param>
	public void IncreaseScore(string playerName, int scoreValue) {
		// Loops through each player to check whose score to increase.
		for (int i = 0; i < maximumPlayerCount; ++i) {
			if (playerName == playerGameObjectNames[i]) {
				playerScores[i] += scoreValue;
				const int modulusAmount = 2;

				// Player's 1 & 3 are on Blue team, 2 & 4 on Red.
				// Blue player indexes will evaluate even and red player indexes will evaluate odd.
				if (i % modulusAmount == 0) {
					playerScoreText[i].text = ("Blue Score: " + (playerScores[0] + playerScores[2]).ToString());
				} else {
					playerScoreText[i].text = ("Red Score: " + (playerScores[1] + playerScores[3]).ToString());
				}
			}
		}
	}

	/// <summary>
	/// Decreases a player's score by a select amount.
	/// </summary>
	/// <param name="playerName"> The name of the player whose score will be decreased. </param>
	/// <param name="scoreValue"> Amount to subtract from the player's score. </param>
	public void DecreaseScore(string playerName, int scoreValue) {
		// Loops through each player to check whose score to decrease.
		for (int i = 0; i < maximumPlayerCount; ++i) {
			if (playerName == playerGameObjectNames[i]) {
				playerScores[i] -= scoreValue;

				// Player's 1 & 3 are on Blue team, 2 & 4 on Red.
				// Blue player indexes will evaluate even and red player indexes will evaluate odd.
				if (i % 2 == 0) {
					playerScoreText[i].text = ("Blue Score: " + (playerScores[0] + playerScores[2]).ToString());
				} else {
					playerScoreText[i].text = ("Red Score: " + (playerScores[1] + playerScores[3]).ToString());
				}
			}
		}
	}

	/// <summary>
	/// Compares the score of each player to decide which team wins.
	/// </summary>
	/// <param name="players"> The list of players who are alive. </param>
	/// <returns> The win text sentence. </returns>
	internal string CompareScore(LinkedList<string> players) {
		int topScore = 0;
		string winText = null;

		if (players.Count == 0) {
			winText = "It's A Draw!";
			return winText;
		// At least one team member from a team is alive.
		} else if ((players.Contains("Player One(Clone)") || players.Contains("Player Three(Clone)")) && (players.Contains("Player Two(Clone)") || players.Contains("Player Four(Clone)"))) {
			// Comparing scores of players 1 & 3 with scores of players 2 & 4.
			if ((playerScores[0] + playerScores[2]) == (playerScores[1] + playerScores[3])) {
				winText = "It's A Draw!";
				return winText;
			}
			// Display blue team wins if they're alive.
		} else if (players.Contains("Player One(Clone)") || players.Contains("Player Three(Clone)")) {
			if ((playerScores[0] + playerScores[2]) > topScore) {
				winText = "Blue Team Wins!";
				topScore = (playerScores[0] + playerScores[2]);
			}
			// Display red team wins if they're alive.
			// If at least one blue team member is alive, red team only wins if they have the 
			// higher score.
		} else if (players.Contains("Player Two(Clone)") || players.Contains("Player Four(Clone)")) {
			if ((playerScores[1] + playerScores[3]) > topScore) {
				winText = "Red Team Wins!";
				topScore = (playerScores[1] + playerScores[3]);
			}
		}

		return winText;
	}
}