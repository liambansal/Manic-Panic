using UnityEngine;

public class CameraFollow : MonoBehaviour {
	[SerializeField]
	private int moveForce = 1;

	[SerializeField]
	private GameObject player = null;

	private Rigidbody2D cameraRigidbody;

    private void Start() {
		cameraRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
		if (GameObject.FindGameObjectWithTag("Player")) {
			if (transform.position.y <= (player.transform.position.y - 2)) {
				cameraRigidbody.AddForce((Vector2.up * Time.deltaTime * moveForce), ForceMode2D.Force);
			} else if (transform.position.y >= (player.transform.position.y + 2)) {
				cameraRigidbody.AddForce((Vector2.down * Time.deltaTime * moveForce), ForceMode2D.Force);
			}

			if ((transform.position.y >= (player.transform.position.y - 1)) && (transform.position.y <= (player.transform.position.y + 1))) {
				cameraRigidbody.velocity = Vector2.zero;
			}
		} else {
			return;
		}
	}
}