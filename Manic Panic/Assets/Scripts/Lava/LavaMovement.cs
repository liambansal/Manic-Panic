﻿using UnityEngine;

public class LavaMovement : MonoBehaviour {
	private readonly float movementSpeed = 3.2f;

	private Rigidbody2D lavaRigidbody = null;

	private void Start() {
		lavaRigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate() {
		if (GameObject.FindGameObjectWithTag("Player")) {
			MoveUpwards();
		} else {
			lavaRigidbody.velocity = Vector2.zero;
		}
	}

	private void MoveUpwards() {
		lavaRigidbody.AddForce((Vector2.up * Time.deltaTime * movementSpeed), ForceMode2D.Force);
	}
}