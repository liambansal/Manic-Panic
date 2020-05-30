using UnityEngine;

/// <summary>
/// Listens for collisions with players to increase their score and delete this gameObject.
/// </summary>
public class Coin : MonoBehaviour {
	/// <summary>
	/// Increases the colliding player's score and destroys this gameGbject.
	/// </summary>
	/// <param name="collider"></param>
	private void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			collider.gameObject.SendMessage("PickupTreasure");
			Destroy(gameObject);
		}
	}
}