using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Default behaviour for the log gameObject.
/// </summary>
public class Log : MonoBehaviour {
	private const int speed = 8;
	private const int mapWidth = 20;
	private const string playerManagerTag = "PlayerManager";
	private const string wallTag = "Wall";
	private Vector2 moveDirection;
	private Vector2 spawnPosition;
	private PlayerManager playerManager = null;
	private Rigidbody2D logRigidbody = null;
	private TilemapCollider2D wallCollider = null;

	/// <summary>
	/// Gets references to necessary unnasigned class variable objects and projects the log along 
	/// a course.
	/// </summary>
	private void Start() {
		logRigidbody = GetComponent<Rigidbody2D>();
		playerManager = GameObject.FindWithTag(playerManagerTag).GetComponent<PlayerManager>();
		wallCollider = GameObject.FindWithTag(wallTag).GetComponent<TilemapCollider2D>();
		Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), wallCollider);
		// Stores the logs spawn position.
		spawnPosition = transform.position;
		// Gets a direction towards the map's vertical center.
		moveDirection = new Vector3(0.0f, transform.position.y, transform.position.z) - transform.position;
		logRigidbody.AddForce(moveDirection * speed);
	}

	/// <summary>
	/// Destroys the player standing on this log if it crosses its spawn position's opposing map wall.
	/// </summary>
	private void Update() {
		// Checks if the object is more than the maps width away from its spawn position.
		if (Vector2.Distance(transform.position, spawnPosition) >= mapWidth) {
			// Check if there's a player standing on the log.
			if (gameObject.GetComponentInChildren<Player>()) {
				playerManager.PlayerDied(gameObject.GetComponentInChildren<Player>().gameObject);
			}
		}
	}
}