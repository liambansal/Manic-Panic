using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {
	[SerializeField]
	private Text p1ScoreText = null;
	[SerializeField]
	private Text p2ScoreText = null;

	private int p1Score = 0;
	private int p2Score = 0;

	protected void IncreaseScore(string playerName, int scoreValue) {
		if (playerName == "Player One") {
			p1Score += scoreValue;
			p1ScoreText.text = ("Blue Score: " + p1Score.ToString());
		}

		if (playerName == "Player Two") {
			p2Score += scoreValue;
			p2ScoreText.text = ("Red Score: " + p2Score.ToString());
		}
	}

	protected string CompareScore() {
		if (p1Score > p2Score) {
			return "Player One Wins!";
		} else if (p2Score > p1Score) {
			return "Player Two Wins!";
		} else if (p1Score == p2Score) {
			return "It's A Tie!";
		}

		return null;
	}
}