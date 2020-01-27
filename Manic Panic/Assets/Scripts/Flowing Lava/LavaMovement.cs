using UnityEngine;

public class LavaMovement : MonoBehaviour {
	private Rigidbody2D lavaRigidbody = null;

	[SerializeField]
	private float movementSpeed = 1;

	[SerializeField]
	private float s;

	private void Start() {
		lavaRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		s = lavaRigidbody.velocity.y;
		lavaRigidbody.AddForce((Vector2.up * Time.deltaTime * movementSpeed), ForceMode2D.Force);
    }
}