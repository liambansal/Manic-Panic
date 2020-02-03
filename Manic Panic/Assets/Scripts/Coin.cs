using UnityEngine;

public class Coin : ScoreController {
	private int value = 10;

	private void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			IncreaseScore(collider.gameObject.name, value);
			Destroy(gameObject);
		}
	}
}