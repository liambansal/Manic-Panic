using UnityEngine;

public class GameCamera : MonoBehaviour {
	[SerializeField]
	private string playerNumber = "";

	[SerializeField]
	private GameObject player = null;

	private readonly int moveForce = 100;
	// Number of tiles between the camera and player along the y axis (positive).
	private readonly int yMargin = 4; 

	// Number of tiles the player can mvoe before the camera follows them.
	private readonly float followBoundary = 1.5f; 

	private Vector2 oldDirection = Vector2.zero; // Direction to the player before they move.
	private Vector2 newDirection = Vector2.zero; // Direction to the player after they move.

	private Rigidbody2D cameraRigidbody;

	/// <summary>
	/// Initializes some camera variables.
	/// </summary>
	private void Start() {
		cameraRigidbody = GetComponent<Rigidbody2D>();
		oldDirection = newDirection = player.transform.position - transform.position;
	}

	/// <summary>
	/// Stops the camera from moving, then checks if the player is alive and moves towards them if 
	/// necessary.
	/// </summary>
	private void FixedUpdate() {
		cameraRigidbody.velocity = Vector2.zero;

		if (GameObject.Find("Player " + playerNumber)) {
			if (Vector2.Distance(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + yMargin)) > followBoundary)  {
				MoveCamera();
			}
		}
	}
	
	/// <summary>
	/// Calculates the direction towards a vector ahead of the player and moves towards it.
	/// </summary>
	private void MoveCamera() {
		// Gets the direction towards a vector yMargin tiles ahead of the player.
		newDirection = new Vector3(player.transform.position.x, player.transform.position.y + yMargin, player.transform.position.z) - transform.position;

		// If the direction has changed since last methdod call then stop moving.
		// This stops the camera from orbiting the vector when the direction changes.
		if (newDirection != oldDirection) {
			oldDirection = newDirection;
			cameraRigidbody.velocity = Vector2.zero;
		}

		// Moves the camera towards the vector.
		cameraRigidbody.AddForce(newDirection * Time.deltaTime * (newDirection.magnitude * moveForce), ForceMode2D.Force);
	}
}