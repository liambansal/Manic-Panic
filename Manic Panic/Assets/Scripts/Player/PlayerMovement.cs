using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[SerializeField]
	private string controllerPrefix = "";

	private enum Movement { Up, Left, Down, Right, Jump };

	// Tiles to move.
	const int moveDistance = 1;
	const int jumpDistance = 2;

	private bool canMoveVertically = true;
	private bool canMoveHorizontally = true;
	private bool canJump = true;

	void Update() {
		if (Input.GetAxis(controllerPrefix + "Vertical") == 0) {
			canMoveVertically = true;
		}

		if (Input.GetAxis(controllerPrefix + "Horizontal") == 0) {
			canMoveHorizontally = true;
		}

		if (Input.GetAxis(controllerPrefix + "Jump") == 0) {
			canJump = true;
		}

		// Checks for player input.
		if ((Input.GetAxis(controllerPrefix + "Vertical") > 0) && (canMoveVertically)) {
			CheckMoveDirection(Movement.Up);
			canMoveVertically = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Horizontal") < 0) && (canMoveHorizontally)) {
			CheckMoveDirection(Movement.Left);
			canMoveHorizontally = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Vertical") < 0) && (canMoveVertically)) {
			CheckMoveDirection(Movement.Down);
			canMoveVertically = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Horizontal") > 0) && (canMoveHorizontally)) {
			CheckMoveDirection(Movement.Right);
			canMoveHorizontally = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Jump") > 0) && (canJump))
		{
			CheckMoveDirection(Movement.Jump);
			canJump = false;
		}
	}

	/// <summary>
	/// Checks if the player can move in the desired direction then calls a method to move.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void CheckMoveDirection (Movement moveDirection) {
		// Used for checking objects around the player.
		RaycastHit2D ray;

		int layerMask = 1 << 8;

		switch (moveDirection) {
			case Movement.Up: {
				ray = Physics2D.Raycast(gameObject.transform.position, Vector2.up, moveDistance, layerMask);

				if (!ray.collider) {
					Move(moveDirection);
				}

				break;
			}
			case Movement.Left: {
				ray = Physics2D.Raycast(gameObject.transform.position, Vector2.left, moveDistance, layerMask);

				if (!ray.collider) {
					Move(moveDirection);
				}

				break;
			}
			case Movement.Down: {
				ray = Physics2D.Raycast(gameObject.transform.position, Vector2.down, moveDistance, layerMask);

				if (!ray.collider) {
					Move(moveDirection);
				}

				break;
			}
			case Movement.Right: {
				ray = Physics2D.Raycast(gameObject.transform.position, Vector2.right, moveDistance, layerMask);

				if (!ray.collider) {
					Move(moveDirection);
				}

				break;
			}
			case Movement.Jump: {
				ray = Physics2D.Raycast(gameObject.transform.position, Vector2.up, jumpDistance, layerMask);

				if (!ray.collider) {
					Move(moveDirection);
				}

				break;
			}
		}
	}

	/// <summary>
	/// Moves the player in the desired direction.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void Move(Movement moveDirection) {
		switch (moveDirection) {
			case Movement.Up: {
				gameObject.transform.Translate((Vector2.up * moveDistance), Space.World);
				break;
			}
			case Movement.Left: {
				gameObject.transform.Translate((Vector2.left * moveDistance), Space.World);
				break;
			}
			case Movement.Down: {
				gameObject.transform.Translate((Vector2.down * moveDistance), Space.World);
				break;
			}
			case Movement.Right: {
				gameObject.transform.Translate((Vector2.right * moveDistance), Space.World);
				break;
			}
			case Movement.Jump: {
				gameObject.transform.Translate((Vector2.up * jumpDistance), Space.World);
				break;
			}
		}
	}
}