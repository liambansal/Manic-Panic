using UnityEngine;

public class Hazard : MonoBehaviour {
	[SerializeField]
	private FinishLine finishLine = null;

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Player")) {
			Destroy(other.gameObject);
			finishLine.PlayerDied(other.gameObject.name);
		}
	}
}