using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[SerializeField]
	private GameObject player = null;

	private Rigidbody2D cameraRigidbody;

	private int moveForce = 1;

    void Start() {
		cameraRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update() {
		if (transform.position.y <= (player.transform.position.y - 2)) {
			cameraRigidbody.AddForce((Vector2.up * moveForce), ForceMode2D.Force);
		} else if (transform.position.y >= (player.transform.position.y + 2))
		{
			cameraRigidbody.AddForce((Vector2.down * moveForce), ForceMode2D.Force);
		}

		if ((transform.position.y >= (player.transform.position.y - 1)) && (transform.position.y <= (player.transform.position.y + 1)))
		{
			cameraRigidbody.velocity = Vector2.zero;
		}
	}
}