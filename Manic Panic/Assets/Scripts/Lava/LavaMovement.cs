using UnityEngine;

public class LavaMovement : MonoBehaviour {
	[SerializeField]
	private float movementSpeed = 1;

	private Rigidbody2D lavaRigidbody = null;

	private float deltaTime;

	private void Start() {
		lavaRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		lavaRigidbody.AddForce((Vector2.up * Time.deltaTime * movementSpeed), ForceMode2D.Force);
    }
}