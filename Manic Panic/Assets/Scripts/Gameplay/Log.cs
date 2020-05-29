using UnityEngine;
using UnityEngine.Tilemaps;

public class Log : MonoBehaviour {
	private const int speed = 8;

	private Vector2 moveDirection;
	private Vector2 spawnPosition;

	private PlayerManager playerManager = null;

	private Rigidbody2D logRigidbody = null;

	private TilemapCollider2D wallCollider = null;

	private void Start() {
		// Get the map's wall collider before ignoring collisions with it.
		wallCollider = GameObject.FindWithTag("Wall").GetComponent<TilemapCollider2D>();
		Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), wallCollider);
		playerManager = GameObject.FindWithTag("PlayerManager").GetComponent<PlayerManager>();
		logRigidbody = GetComponent<Rigidbody2D>();
		// Stores the position the object was spawned at.
		spawnPosition = transform.position;
		// Gets a Vector2 direction towards the map's vertical center.
		moveDirection = new Vector3(0.0f, transform.position.y, transform.position.z) - transform.position;
		// Adds a single force to move the object in a straight line.
		logRigidbody.AddForce(moveDirection * speed);
	}

	private void Update() {
		// Checks if the object is more than the maps width away from its spawn position.
		if (Vector2.Distance(transform.position, spawnPosition) >= 20.0f) {
			// Checks if the log has a player as a child.
			if (gameObject.GetComponentInChildren<Player>()) {
				playerManager.PlayerDied(gameObject.GetComponentInChildren<Player>().gameObject.name);
			}

			// Destroy the object because it's no longer on the map.
			Destroy(gameObject);
		}
	}
}
