using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the spawning of players, their deaths and the game's end state.
/// </summary>
public class PlayerManager : MonoBehaviour {
	[SerializeField]
	private FinishLine finishLine = null;

	[SerializeField]
	private GameObject[] playerPrefabs = new GameObject[4];
	[SerializeField]
	private GameObject[] playerCameras = new GameObject[4];
	[SerializeField]
	private GameObject[] deathScreens = new GameObject[4];
	[SerializeField]
	private GameObject deathAnimation = null;

	[SerializeField]
	private Vector3[] spawnPositions = new Vector3[4];

	private int playersAlive = 0;
	private const int maximumPlayerCount = 4;

	private readonly string[] playerPrefixes = new string[4] { "P1", "P2", "P3", "P4" };
	private readonly string[] playerGameObjectNames = new string[4] { "Player One(Clone)",
		"Player Two(Clone)",
		"Player Three(Clone)",
		"Player Four(Clone)" };
	private readonly string playerTag = "Player";

	private GameObject scoreController = null;
	// A name list of the scene's players.
	private LinkedList<string> playerList = new LinkedList<string>();

	/// <summary>
	/// Spawns the appropriate amount of players with their correct characters.
	/// Initializes the player cameras and score controller after this.
	/// </summary>
	void Start() {
		// Loop through all the player prefixes checking which ones have an assigned character.
		for (int i = 0; i < maximumPlayerCount; ++i) {
			if (PlayerPrefs.HasKey(playerPrefixes[i])) {
				// Sets up a character prefab.
				Instantiate(playerPrefabs[i], spawnPositions[i], Quaternion.identity);
				++playersAlive;
				playerCameras[i].GetComponent<GameCamera>().Initialize();
			}
		}

		// Stores a list of active characters once they've all been created.
		foreach (GameObject player in GameObject.FindGameObjectsWithTag(playerTag)) {
			playerList.AddLast(player.name);
		}

		scoreController = GameObject.FindGameObjectWithTag("ScoreController");
		scoreController.GetComponent<ScoreController>().InitializeScores();
	}

	/// <summary>
	/// Checks if the win screen should be activated in place of the death screens.
	/// </summary>
	private void Update() {
		if (playersAlive == finishLine.playersFinished) {
			for (int i = 0; i < maximumPlayerCount; ++i) {
				deathScreens[i].SetActive(false);
			}

			finishLine.DisplayWinner(playerList, playersAlive);
		}
	}

	/// <summary>
	/// Removes a player from the scene and activates their death screen.
	/// </summary>
	/// <param name="player"> The player that has died. </param>
	public void PlayerDied(GameObject player) {
		if (playerList.Contains(player.name)) {
			// Spawn a death animation prefabs to show the player has died.
			Instantiate(deathAnimation, player.transform.position, Quaternion.identity);
			playerList.Remove(player.name);
			Destroy(player);
			--playersAlive;

			// Activates the death screen for the player that has died.
			for (int i = 0; i < maximumPlayerCount; ++i) {
				if (player.name == playerGameObjectNames[i]) {
					deathScreens[i].SetActive(true);
					deathScreens[i].GetComponent<Animator>().SetBool("fadeIn", true);
				}
			}
		}
	}
}