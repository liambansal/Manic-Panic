﻿using UnityEngine;

public class LavaCollisions : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			Destroy(collision.gameObject);
		}
	}
}