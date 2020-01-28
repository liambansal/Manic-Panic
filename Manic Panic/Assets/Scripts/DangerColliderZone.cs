using UnityEngine;

public class DangerColliderZone : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			Destroy(other.gameObject);
		}
	}
}