using UnityEngine;

/// <summary>
/// The default behaviour for player cameras. Causes them to follow a target player across the level.
/// </summary>
public class GameCamera : MonoBehaviour {
	[SerializeField]
	private string playerNumber = "";

	private const int moveForce = 100;
	/// <summary>
	/// Number of tiles between the camera and player along the y axis (positive).
	/// </summary>
	private const int yMargin = 4;
	/// <summary>
	/// Number of tiles the player can move before the camera follows them.
	/// </summary>
	private const float followBoundary = 1.5f;

	private bool isInitialized = false;
	private string playerName = null;

	/// <summary>
	/// Direction to the player before they move.
	/// </summary>
	private Vector2 oldDirection = Vector2.zero;
	/// <summary>
	/// Direction to the player after they move.
	/// </summary>
	private Vector2 newDirection = Vector2.zero;

	private Rigidbody2D cameraRigidbody;
	private GameObject player = null;

	/// <summary>
	/// Assigns some of the camera's variables.
	/// </summary>
	public void Initialize() {
		cameraRigidbody = GetComponent<Rigidbody2D>();
		playerName = "Player " + playerNumber + "(Clone)";
		player = GameObject.Find(playerName);
		newDirection = player.transform.position - transform.position;
		oldDirection = newDirection;
		isInitialized = true;
	}

	/// <summary>
	/// Checks if the camera needs to move towards its target player.
	/// </summary>
	private void FixedUpdate() {
		if (isInitialized) {
			cameraRigidbody.velocity = Vector2.zero;

			if (player != null) {
				// Checks if the distance from the camera to its target player exceeds the follow boundary.
				if (Vector2.Distance(transform.position, new Vector2(player.transform.position.x, player.transform.position.y + yMargin)) > followBoundary) {
					MoveCamera();
				}
			}
		}
	}
	
	/// <summary>
	/// Calculates the direction towards the target player and moves along it.
	/// </summary>
	private void MoveCamera() {
		if (isInitialized) {
			// Gets the direction towards a vector "yMargin" tiles ahead of the player.
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
}