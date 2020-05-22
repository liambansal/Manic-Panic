using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour {
	// We need at least two players to play against each other.
	private const int minimumPlayers = 2;
	// We only support up to four players.
	private const int maximumPlayers = 4;
	// How many people are playing.
	private int playerCount = 0;

	// Which players are playing. Represents player 1 through 4.
	private bool[] players = new bool[4] { false, false, false, false };
	// Which players have selected a character.
	private bool[] characterSelected = new bool[4] { false, false, false, false };

	// Matching prefixes for project settings inputs.
	private string[] playerPrefixes = new string[4] { "P1", "P2", "P3", "P4"}; 

	/// <summary>
	/// Checks for player input to find out who wants to play and loads the game scene when everyone 
	/// has selected a character.
	/// </summary>
	private void Update() {
		HandleInput();
		ReadyPlayers();
	}

	private void HandleInput() {
		for (int i = 0; i < maximumPlayers; ++i) {
			// If the player hasn't already opted in to play...
			if (players[i] == false) {
				// ...check if they want to play.
				if (Input.GetAxis(playerPrefixes[i] + "Jump") > 0.0f) {
					players[i] = true;
					playerCount++;
				}
			} else { // They already opted in to play.
					 // Check for confirmation of their character.
				if (Input.GetAxis(playerPrefixes[i] + "Jump") > 0.0f) {
					characterSelected[i] = true; ;
				}
			}
		}
	}

	private void ReadyPlayers() {
		int playersReady = 0;

		// Checks if all the players have selected a character and confirms another player is ready.
		for (int i = 0; i < maximumPlayers; ++i) {
			if (characterSelected[i] == true) {
				++playersReady;
			}
		}

		// If we at least have the minimum number of players to play...
		if (playerCount >= minimumPlayers) {
			// ...check they're all ready.
			if (playersReady == playerCount) {
				SceneManager.LoadScene("Level One", LoadSceneMode.Single);
			}
		}
	}
}