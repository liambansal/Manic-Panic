﻿using UnityEngine;
using System.Collections.Generic;

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
	private Vector3[] spawnPositions = new Vector3[4];

	private int playersAlive = 0;
	private int maximumPlayerCount = 4;

	private string[] playerPrefixes = new string[4] { "P1", "P2", "P3", "P4"};
	private string[] playerGameObjectNames = new string[4] { "Player One(Clone)",
		"Player Two(Clone)",
		"Player Three(Clone)",
		"Player Four(Clone)" };
	private string playerTag = "Player";

	private GameObject scoreController = null;

	private LinkedList<string> playerList = new LinkedList<string>();

	void Start() {
		// Loop through all the player prefixes.
		for (int i = 0; i < maximumPlayerCount; ++i) {
			// Check if each player prefix has an assigned character.
			if (PlayerPrefs.HasKey(playerPrefixes[i])) {
				// Instantiate a player prefab for each player that chose a character in the 
				// selection screen.
				Instantiate(playerPrefabs[i], spawnPositions[i], Quaternion.identity);
				++playersAlive;
				playerCameras[i].GetComponent<GameCamera>().Initialize();
			}
		}

		foreach (GameObject player in GameObject.FindGameObjectsWithTag(playerTag)) {
			playerList.AddLast(player.name);
		}

		scoreController = GameObject.FindGameObjectWithTag("ScoreController");
		scoreController.GetComponent<ScoreController>().InitializeScores();
	}

	private void Update() {
		if (playersAlive == finishLine.playersFinished) {
			for (int i = 0; i < maximumPlayerCount; ++i) {
				deathScreens[i].SetActive(false);
			}

			finishLine.DisplayWinner(playerList, playersAlive);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="player"> The player that has died (their 'game object' name). </param>
	public void PlayerDied(string player) {
		if (playerList.Contains(player)) {
			playerList.Remove(player);
			--playersAlive;

			// Activates the death screen for the player that has died.
			for (int i = 0; i < maximumPlayerCount; ++i) {
				if (player == playerGameObjectNames[i]) {
					deathScreens[i].SetActive(true);
					deathScreens[i].GetComponent<Animator>().SetBool("fadeIn", true);
				}
			}
		}
	}
}