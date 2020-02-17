﻿using UnityEngine;

public class PlayerPunching : MonoBehaviour {
	[SerializeField]
	private string controllerPrefix = ""; // Used to identify the player.

	private const float playerRadius = 0.5f;
	private const float cooldownLength = 4.0f; // Cooldown length for punching.
	private float punchCooldown = 0.0f; // Time remaining before player can punch.

	private bool canPunch = true;

	private enum Directions { Up, Left, Down, Right };
	private Directions punchDirection;

	private void Update() {
		if (!canPunch) {
			punchCooldown -= Time.deltaTime;
		}

		if (punchCooldown <= 0.0f) {
			canPunch = true;
			punchCooldown = cooldownLength;
		}

		if ((Input.GetAxis(controllerPrefix + "FireVertical") < 0.0f) && canPunch) {
			punchDirection = Directions.Up;
			Punch(punchDirection);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireHorizontal") < 0.0f) && canPunch) {
			punchDirection = Directions.Left;
			Punch(punchDirection);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireVertical") > 0.0f) && canPunch) {
			punchDirection = Directions.Down;
			Punch(punchDirection);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireHorizontal") > 0.0f) && canPunch) {
			punchDirection = Directions.Right;
			Punch(punchDirection);
			canPunch = false;
		}
	}

	private void Punch(Directions punchDirection) {
		RaycastHit2D ray;
		const int layerMask = 1 << 8;
		const float punchReach = 0.9f;

		switch (punchDirection) {
			case Directions.Up: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, punchReach, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
			case Directions.Left: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left, punchReach, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
			case Directions.Down: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down, punchReach, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
			case Directions.Right: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right, punchReach, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
		}
	}
}