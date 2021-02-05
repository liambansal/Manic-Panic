using UnityEngine;

/// <summary>
/// Controls the spawning of logs.
/// </summary>
public class LogManager : MonoBehaviour {
	[SerializeField]
	private GameObject[] spawnTiles = null;
	[SerializeField]
	private GameObject logPrefab = null;

	private const float minimumSpawnTime = 2.5f;
	private const float maximumSpawnTime = 6.0f;
	private float[] spawnTimers;

	/// <summary>
	/// Initializes the array of spawn timers.
	/// </summary>
	private void Start() {
		// Create a timer for each spawn point so they have individual timers 
		// for instantiating logs.
		spawnTimers = new float[spawnTiles.Length];

		// Set a random value for each spawn timer.
		for (int index = 0; index < spawnTimers.Length; ++index) {
			spawnTimers[index] = Random.Range(minimumSpawnTime, maximumSpawnTime);
		}
	}

	/// <summary>
	/// Updates each spawn timer for instantiating logs.
	/// </summary>
	private void Update() {
		// Loops through every spawn timer.
		for (int index = 0; index < spawnTimers.Length; ++index) {
			spawnTimers[index] -= Time.deltaTime;

			if (spawnTimers[index] <= 0.0f) {
				// The spawn tile is relative to the current timer.
				Instantiate(logPrefab, spawnTiles[index].transform.position, Quaternion.identity);
				// Reset the timer to a random value.
				spawnTimers[index] = Random.Range(minimumSpawnTime, maximumSpawnTime);
			}
		}
	}
}