using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controls the process of selecting a character for each player. Saves necessary data about players/characters through "PlayerPrefs".
/// </summary>
public class CharacterSelect : MonoBehaviour {
	[SerializeField]
	private Sprite[] characterSprites = new Sprite[4];
	[SerializeField]
	private Image[] displayedImage = new Image[4];

	/// <summary>
	/// Records which character has been selected for each player.
	/// </summary>
	private readonly int[] characterPosition = new int[4];
	/// <summary>
	/// We need at least two players to play against each other.
	/// </summary>
	private const int minimumPlayers = 2;
	/// <summary>
	/// We only support up to four players.
	/// </summary>
	private const int maximumPlayers = 4;
	/// <summary>
	/// This should be one less than the maximum players.
	/// </summary>
	private const int maximumArrayIndex = 3;
	private int playerCount = 0;
	/// <summary>
	/// Decides whether or not a player can select a different character.
	/// </summary>
	private float[] selectTimer = new float[4];
	private const float selectCooldown = 0.25f;

	private readonly bool[] characterAvailable = new bool[4] { true, true, true, true };
	// The following arrays represent values for players one through four.
	/// <summary>
	/// Which players are playing.
	/// </summary>
	private readonly bool[] players = new bool[4] { false, false, false, false };
	private readonly bool[] playerReady = new bool[4] { false, false, false, false };
	private readonly bool[] canSelect = new bool[4] { false, false, false, false };
	/// <summary>
	/// Matching prefixes for project settings inputs. 
	/// </summary>
	private readonly string[] playerPrefixes = new string[4] { "P1", "P2", "P3", "P4" };
	private const string firstLevelName = "Level One";
	private const string playerCountKey = "Player Count";
	/// <summary>
	/// Name of axis that is responsible for confirming input.
	/// </summary>
	private const string confirmAxisName = "Jump";
	/// <summary>
	/// Name of axis that is responsible for selecting characters.
	/// </summary>
	private const string selectAxisName = "Vertical";

	/// <summary>
	/// Initializes the select timer for each player and their character position.
	/// </summary>
	private void Start() {
		for (int i = 0; i < maximumPlayers; ++i) {
			selectTimer[i] = selectCooldown;
			characterPosition[i] = 0;
		}
	}

	/// <summary>
	/// Handles all character select input (joining, selecting, confirmation).
	/// Updates each characters' timer for switching between characters.
	/// </summary>
	private void Update() {
		// Loops through each player.
		for (int i = 0; i < maximumPlayers; ++i) {
			// Checks if they're playing and un-ready.
			if (players[i] && !playerReady[i]) {
				selectTimer[i] -= Time.deltaTime;

				if (selectTimer[i] <= 0.0f) {
					canSelect[i] = true;
				}
			}
		}

		AddPlayer();
		SelectCharacter();
		ConfirmCharacter();
		LoadLevel();
	}

	/// <summary>
	/// Checks if a player wants to join the game.
	/// </summary>
	private void AddPlayer() {
		// Loops through each player.
		for (int i = 0; i < maximumPlayers; ++i) {
			// If the player hasn't already opted in to play.
			if (players[i] == false) {
				// Check for input to find out if the player wants to play.
				if (Input.GetAxis(playerPrefixes[i] + confirmAxisName) > 0.0f) {
					players[i] = true;
					playerCount++;
					// Display a character sprite for the player, replacing the prompt to join 
					// image.
					displayedImage[i].sprite = characterSprites[characterPosition[i]];
				}
			}
		}
	}

	/// <summary>
	/// Allows each player to browse through the different characters.
	/// </summary>
	private void SelectCharacter() {
		// Loop through each player.
		for (int i = 0; i < maximumPlayers; ++i) {
			// Check if the player is playing, they're not ready yet and their selection timer has 
			// cooled down.
			if (players[i] && !playerReady[i] && canSelect[i]) {
				// Check for input to select a different character.
				if (Input.GetAxis(playerPrefixes[i] + selectAxisName) > 0.0f) {
					// Start the cooldown time for this players character selection.
					// Stops the player from moving through the character selections at a rate too 
					// fast to control.
					canSelect[i] = false;
					selectTimer[i] = selectCooldown;
					// Selects the next playable character.
					++characterPosition[i];

					// Once the last playable character has been reached...
					if (characterPosition[i] > maximumArrayIndex) {
						// ...reset the character position to zero to loop back round to the first 
						// character.
						characterPosition[i] = 0;
					}

					// Display the newly selected character on screen.
					displayedImage[i].sprite = characterSprites[characterPosition[i]];

					// Checks if the selected character is available.
					// Sets the character's sprite colour based on its availability.
					if (characterAvailable[characterPosition[i]]) {
						displayedImage[i].color = Color.white;
					} else {
						displayedImage[i].color = Color.grey;
					}
				} else if (Input.GetAxis(playerPrefixes[i] + selectAxisName) < 0.0f) {
					canSelect[i] = false;
					selectTimer[i] = selectCooldown;
					--characterPosition[i];

					if (characterPosition[i] < 0) {
						characterPosition[i] = maximumArrayIndex;
					}

					displayedImage[i].sprite = characterSprites[characterPosition[i]];

					if (characterAvailable[characterPosition[i]]) {
						displayedImage[i].color = Color.white;
					} else {
						displayedImage[i].color = Color.grey;
					}
				}
			}
		}
	}

	/// <summary>
	/// Checks if the players want to confirm their character choice, marking them as ready.
	/// </summary>
	private void ConfirmCharacter() {
		for (int i = 0; i < maximumPlayers; ++i) {
			if (canSelect[i]) {
				if (players[i] && characterAvailable[characterPosition[i]]) {
					// Check for input to confirm their character choice.
					if (Input.GetAxis(playerPrefixes[i] + confirmAxisName) > 0.0f) {
						playerReady[i] = true;
						// Mark the selected character as unavailable so other players can't 
						// select them.
						characterAvailable[characterPosition[i]] = false;
						// Change the character image to yellow to show they're selected.
						displayedImage[i].color = Color.yellow;
						// Save the players selected character's position in an int format so we 
						// know which character they'll be playing as later on.
						PlayerPrefs.SetInt(playerPrefixes[i], characterPosition[i]);
					}
				}
			}
		}
	}

	/// <summary>
	/// Loads a level once all of the players are ready.
	/// </summary>
	private void LoadLevel() {
		int playersReady = 0;

		// Checks if any players have selected a character.
		for (int i = 0; i < maximumPlayers; ++i) {
			if (playerReady[i] == true) {
				++playersReady;
			}
		}

		// Check if we at least have the minimum number of players to play before loading a level.
		if (playerCount >= minimumPlayers) {
			if (playersReady == playerCount) {
				// Record how many players are playing so we spawn the appropriate amount of 
				// characters later on.
				PlayerPrefs.SetInt(playerCountKey, playersReady);
				SceneManager.LoadScene(firstLevelName, LoadSceneMode.Single);
			}
		}
	}
}