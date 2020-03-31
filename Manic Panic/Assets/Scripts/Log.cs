using UnityEngine;
using UnityEngine.Tilemaps;

public class Log : MonoBehaviour {
	private const int speed = 8;

	private Vector2 moveDirection;
	private Vector2 spawnPosition;

	private Rigidbody2D rb = null;

	private TilemapCollider2D wallCollider = null;

	private void Start() {
		// Get the map's wall collider before ignoring collisions with it.
		wallCollider = GameObject.FindWithTag("Wall").GetComponent<TilemapCollider2D>();
		Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), wallCollider);
		rb = GetComponent<Rigidbody2D>();
		// Stores the position the object was spawned at.
		spawnPosition = transform.position;
		// Gets a Vector2 direction towards the center of the map.
		moveDirection = new Vector3(0.0f, transform.position.y, transform.position.z) - transform.position;
		// Adds a single force to move the object in a straight line.
		rb.AddForce(moveDirection * speed);
	}

	private void Update() {
		// Checks if the object is more than the maps width away from its spawn position.
		if (Vector2.Distance(transform.position, spawnPosition) >= 20.0f) {
			// Destroy the object because it's no longer on the map.
			Destroy(gameObject);
		}
	}
}
