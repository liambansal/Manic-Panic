using UnityEngine;

public class Hazard : MonoBehaviour {
	private string playerTag = "Player";
	private string playerManagerTag = "PlayerManager";

	private PlayerManager playerManager = null;

	private void Start() {
		playerManager = GameObject.FindGameObjectWithTag(playerManagerTag).GetComponent<PlayerManager>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag(playerTag)) {
			Destroy(other.gameObject);
			playerManager.PlayerDied(other.gameObject.name);
		}
	}
}