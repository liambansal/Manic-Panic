using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour {
	[SerializeField]
	private Sprite[] characterSprites = new Sprite[4];

	[SerializeField]
	private Image[] displayedImage = new Image[4];

	// We need at least two players to play against each other.
	private const int minimumPlayers = 1;
	// We only support up to four players.
	private const int maximumPlayers = 4;
	// This should be one less than the maximum players.
	private const int maximumArraySize = 3;

	// How many people are playing.
	private int[] characterPosition = new int[4];
	private int playerCount = 0;

	// Decides whether or not a player can select a different character.
	private float[] selectTimer = new float[4];
	private float selectCooldown = 0.25f;

	// Which players are playing. Represents player 1 through 4.
	private bool[] players = new bool[4] { false, false, false, false };
	// Which players have selected a character.
	private bool[] playerReady = new bool[4] { false, false, false, false };
	// Is the player allowed to select a different character?
	private bool[] canSelect = new bool[4] { false, false, false, false };
	// Is the selected character available.
	private bool[] characterAvailable = new bool[4] { true, true, true, true };

	// Matching prefixes for project settings inputs.
	private string[] playerPrefixes = new string[4] { "P1", "P2", "P3", "P4"};

	private void Start() {
		// Initialize arrays.
		for (int i = 0; i < maximumPlayers; ++i) {
			selectTimer[i] = selectCooldown;
			characterPosition[i] = 0;
		}
	}

	private void Update() {
		for (int i = 0; i < maximumPlayers; ++i) {
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
		for (int i = 0; i < maximumPlayers; ++i) {
			// If the player hasn't already opted in to play.
			if (players[i] == false) {
				// Check if the player wants to play.
				if (Input.GetAxis(playerPrefixes[i] + "Jump") > 0.0f) {
					// Note the player is playing.
					players[i] = true;
					// Increase the number of players who have confirmed to play.
					playerCount++;
					// Display a character sprite for the player, replacing the prompt to join 
					// image.
					displayedImage[i].sprite = characterSprites[characterPosition[i]];
				}
			}
		}
	}

	/// <summary>
	/// Allows each player to loop through the different characters.
	/// Automatically shows whether or not a character is available to play.
	/// </summary>
	private void SelectCharacter() {
		// Loop through each player.
		for (int i = 0; i < maximumPlayers; ++i) {
			// Check if the player is playing, they're not ready yet and their selection timer has 
			// cooled down.
			if (players[i] && !playerReady[i] && canSelect[i]) {
				// Check for input to select a different character.
				if (Input.GetAxis(playerPrefixes[i] + "Vertical") > 0.0f) {
					// Start the cooldown time for this players character selection.
					// Stops the player from moving through the character selections at a rate too 
					// fast to control.
					canSelect[i] = false;
					selectTimer[i] = selectCooldown;
					// Increase the character position to select the next playable character.
					++characterPosition[i];

					// Once the last playable character has been reached...
					if (characterPosition[i] > maximumArraySize) {
						// ...reset the character position to zero to loop back round to the first 
						// character.
						characterPosition[i] = 0;
					}

					// Display the newly selected character on screen.
					displayedImage[i].sprite = characterSprites[characterPosition[i]];

					// Checks if the selected character is available.
					if (characterAvailable[characterPosition[i]]) {
						// Set players character image to white to show that the selected 
						// character is available to play.
						displayedImage[i].color = Color.white;
					} else {
						// Set players character image to grey to show that the selected 
						// character is not available to play.
						displayedImage[i].color = Color.grey;
					}
				} else if (Input.GetAxis(playerPrefixes[i] + "Vertical") < 0.0f) {
					// Same code as previous if statement except we're selecting the previous 
					// character this time.
					canSelect[i] = false;
					selectTimer[i] = selectCooldown;
					--characterPosition[i];

					if (characterPosition[i] < 0) {
						characterPosition[i] = maximumArraySize;
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
	/// Checks if players want to confirm their character choice, marking them as ready.
	/// </summary>
	private void ConfirmCharacter() {
		for (int i = 0; i < maximumPlayers; ++i) {
			if (canSelect[i]) {
				// Check the player is playing and the character is available to play.
				if (players[i] && characterAvailable[characterPosition[i]]) {
					// Check for confirmation of their character.
					if (Input.GetAxis(playerPrefixes[i] + "Jump") > 0.0f) {
						// Mark the player as ready.
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

		// Checks if all the players have selected a character.
		for (int i = 0; i < maximumPlayers; ++i) {
			if (playerReady[i] == true) {
				// Confirm another player is ready.
				++playersReady;
			}
		}

		// If we at least have the minimum number of players to play...
		if (playerCount >= minimumPlayers) {
			// ...check they're all ready.
			if (playersReady == playerCount) {
				// Record how many players are playing so we spawn the appropriate amount of 
				// characters later on.
				PlayerPrefs.SetInt("Player Count", playersReady);
				// Load a gameplay scene here.
				SceneManager.LoadScene("Level One", LoadSceneMode.Single);
			}
		}
	}
}