using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FinishLine : MonoBehaviour {
	/// <summary>
	/// Number of players that have finished the level.
	/// </summary>
	public int playersFinished { get; private set; } = 0;

	[SerializeField]
	private ScoreController scoreController = null;
	
	[SerializeField]
	private Text winTextbox = null;

	[SerializeField]
	private string[] playerGameObjectNames = new string[4];

	readonly private int maximumPlayers = 4;
	
	private Time animationTimeline;

	private void OnTriggerEnter2D(Collider2D other) {
		for (int i = 0; i < maximumPlayers; ++i) {
			if ((other.gameObject.name == playerGameObjectNames[i])) {
				Destroy(other.gameObject);
				++playersFinished;
			}
		}
	}

	public void DisplayWinner(LinkedList<string> playerList, int playersAlive) {
		if (playersFinished == playersAlive) {
			winTextbox.gameObject.SetActive(true);
			winTextbox.text = scoreController.CompareScore(playerList);
			winTextbox.GetComponent<Animator>().SetBool("playAnimation", true);
		}
	}
}