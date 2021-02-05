using UnityEngine;

/// <summary>
/// Controls the movement of the lava gameObject.
/// </summary>
public class LavaMovement : MonoBehaviour {
	private const int moveForce = 37;
	private bool isMoving = false;
	private Rigidbody2D lavaRigidbody = null;

	/// <summary>
	/// Stores a reference to this gameObject's rigidbody.
	/// </summary>
	private void Start() {
		lavaRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	/// <summary>
	/// Moves the lava up the screen if at least one player is alive.
	/// </summary>
	private void FixedUpdate() {
		if (GameObject.FindGameObjectWithTag("Player")) {
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