using UnityEngine;

public class Coin : MonoBehaviour {
	private int value = 10;

	private ScoreController scoreController = null;

	private void Start() {
		scoreController = GameObject.Find("Score Controller").GetComponent<ScoreController>();
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			scoreController.IncreaseScore(collider.gameObject.name, value);
			Destroy(gameObject);
		}
	}
}