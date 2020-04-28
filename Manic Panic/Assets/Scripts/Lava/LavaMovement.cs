using UnityEngine;

public class LavaMovement : MonoBehaviour {
	private readonly int moveForce = 45;

	private bool isMoving = false;

	private Rigidbody2D lavaRigidbody = null;

	private void Start() {
		lavaRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		// Only moves if at least one player is alive.
		if (GameObject.FindGameObjectWithTag("Player")) {
			// Checks if we have already started to move.
			if (!isMoving) {
				MoveUpwards();
			}
		} else {
			// Stops moving when no one is alive.
			lavaRigidbody.velocity = Vector2.zero;
		}
	}

	private void MoveUpwards() {
		lavaRigidbody.AddForce((Vector2.up * moveForce), ForceMode2D.Force);
		isMoving = true;
	}
}