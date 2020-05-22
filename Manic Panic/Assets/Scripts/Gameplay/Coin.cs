using UnityEngine;

public class Coin : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			collider.gameObject.SendMessage("PickupTreasure");
			Destroy(gameObject);
		}
	}
}