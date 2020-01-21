using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaMovement : MonoBehaviour {
	private Rigidbody2D lavaRigidbody = null;

	private int movementSpeed = 2;

	private void Start() {
		lavaRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	private void Update() {
		lavaRigidbody.AddForce(Vector2.up * Time.deltaTime * movementSpeed);
    }
}