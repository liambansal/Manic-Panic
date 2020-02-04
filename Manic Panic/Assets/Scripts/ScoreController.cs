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

	internal void IncreaseScore(string playerName, int scoreValue) {
		if (playerName == "Player One") {
			playerOneScore += scoreValue;
			playerOneScoreText.text = ("Blue Score: " + playerOneScore.ToString());
		}

		if (playerName == "Player Two") {
			playerTwoScore += scoreValue;
			playerTwoScoreText.text = ("Red Score: " + playerTwoScore.ToString());
		}
	}

	internal string CompareScore(LinkedList<string> players) {
		//int score = 0;
		//string topScorer = null;

		foreach (string player in players) {
			string playerString = player.Substring(0, 6).ToLower();
			string playerIentifier = player.Substring(7);
			string convertedString = playerString + playerIentifier;

			//if ( > score) {
			//	score = ;
			//	topScorer = player;
			//}
		}

		//if (p1Score > p2Score) {
		//	return "Player One Wins!";
		//} else if (p2Score > p1Score) {
		//	return "Player Two Wins!";
		//} else if (p1Score == p2Score) {
		//	return "It's A Tie!";
		//}

		return null;
	}
}