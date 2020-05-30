using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls displaying the game's win screen.
/// </summary>
public class FinishLine : MonoBehaviour {
	[HideInInspector]
	public int playersFinished { get; private set; } = 0;

	[SerializeField]
	private string[] playerGameObjectNames = new string[4];

	[SerializeField]
	private ScoreController scoreController = null;
	[SerializeField]
	private Text winTextbox = null;

	private const int maximumPlayers = 4;

	/// <summary>
	/// Destroys the player that triggers a collision with this gameObject.
	/// </summary>
	/// <param name="other"> The collider that triggered a collision with this gameObject. </param>
	private void OnTriggerEnter2D(Collider2D other) {
		for (int i = 0; i < maximumPlayers; ++i) {
			if ((other.gameObject.name == playerGameObjectNames[i])) {
				// Don't remove the player from the player manager because they haven't died.
				Destroy(other.gameObject);
				++playersFinished;
			}
		}
	}

	/// <summary>
	/// Displays the game's winner once enough players have finished the level or died.
	/// </summary>
	/// <param name="playerList"> A list of the scene's remaining players. </param>
	/// <param name="playersAlive"> Number of players who are still alive. </param>
	public void DisplayWinner(LinkedList<string> playerList, int playersAlive) {
		if (playersFinished == playersAlive) {
			winTextbox.gameObject.SetActive(true);
			// Gets the game's winning team, if there is one, in a string format.
			winTextbox.text = scoreController.CompareScore(playerList);
			// Plays an animation to load the main menu after a set time.
			winTextbox.GetComponent<Animator>().SetBool("playAnimation", true);
		}
	}
}