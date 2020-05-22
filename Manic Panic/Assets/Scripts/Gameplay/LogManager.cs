using UnityEngine;

public class LogManager : MonoBehaviour {
	[SerializeField]
	private GameObject[] spawnTiles = null;

	[SerializeField]
	private GameObject log = null;

	private float[] spawnTimers;

	private const float minSpawnTime = 2.5f;
	private const float maxSpawnTime = 6.0f; // If a log spawns with a timer 
	// value of 3.0f and this is set to 14.0f then at least one log is 
	// guaranteed to spawn in the row above/below before the log reaches the 
	// edge of the map.

	private void Start() {
		// Create a timer for each spawn point so they have individual timers 
		// for instantiating logs.
		spawnTimers = new float[spawnTiles.Length];

		// Randomly set the value for each spawn timer.
		for (int index = 0; index < spawnTimers.Length; ++index) {
			spawnTimers[index] = Random.Range(minSpawnTime, maxSpawnTime);
		}
	}

	private void Update() {
		// Loops through every spawn timer.
		for (int index = 0; index < spawnTimers.Length; ++index) {
			// Decrease the timer by time.
			spawnTimers[index] -= Time.deltaTime;

			// When it reaches zero, or is less than...
			if (spawnTimers[index] <= 0.0f) {
				// ...spawn a log at the approprtiate spawn point. The 
				// spawn point is determined by the timers' index.
				Instantiate(log, spawnTiles[index].transform.position, Quaternion.identity);
				// Reset the timer to a random value.
				spawnTimers[index] = Random.Range(minSpawnTime, maxSpawnTime);
			}
		}
	}
}
