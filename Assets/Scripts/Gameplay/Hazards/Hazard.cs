using UnityEngine;

/// <summary>
/// The default behaviour of hazardous gameObjects.
/// Will destroy any player that collides with this gameObject.
/// </summary>
public class Hazard : MonoBehaviour {
	private const string playerTag = "Player";
	private const string playerManagerTag = "PlayerManager";
	private PlayerManager playerManager = null;

	/// <summary>
	/// Stores a reference to the scene's player manager.
	/// </summary>
	private void Start() {
		playerManager = GameObject.FindGameObjectWithTag(playerManagerTag).GetComponent<PlayerManager>();
	}

	/// <summary>
	/// Destroys the player that triggers a collision with this gameObject.
	/// </summary>
	/// <param name="other"> The collider that triggered a collision with this gameObject. </param>
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(playerTag)) {
			playerManager.PlayerDied(other.gameObject);
		}
	}
}