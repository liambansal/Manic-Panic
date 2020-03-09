using UnityEngine;

public class GameCamera : MonoBehaviour {
	[SerializeField]
	private string playerNumber = "";

	[SerializeField]
	private GameObject player = null;

	private int moveForce = 350;

	private Vector2 oldDirection = Vector2.zero; // The direction to the player before they moved.
	private Vector2 newDirection = Vector2.zero; // The direction to the player after they moved.

	private Rigidbody2D cameraRigidbody;

	/// <summary>
	/// Initializes the camera's variables.
	/// </summary>
	private void Start() {
		cameraRigidbody = GetComponent<Rigidbody2D>();
		oldDirection = newDirection = player.transform.position - transform.position;

	}

	/// <summary>
	/// Calls to move the camera towards the player if they're alive and a 
	/// great distance away, otherwise stops the camera from moving.
	/// </summary>
	private void FixedUpdate() {
		if (GameObject.Find("Player " + playerNumber) && (Vector2.Distance(transform.position, player.transform.position) > 1.5f)) {
			MoveCamera();
		} else {
			cameraRigidbody.velocity = Vector2.zero;
		}
	}

	/// <summary>
	/// Moves the camera towards the player's world position.
	/// </summary>
	private void MoveCamera() {
		newDirection = player.transform.position - transform.position;

		if (newDirection != oldDirection) {
			oldDirection = newDirection;
			cameraRigidbody.velocity = Vector2.zero;
		}

		cameraRigidbody.AddForce(newDirection * Time.deltaTime * (newDirection.magnitude * moveForce), ForceMode2D.Force);
	}
}