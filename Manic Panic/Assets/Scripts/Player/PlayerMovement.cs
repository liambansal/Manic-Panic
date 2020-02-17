using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	[SerializeField]
	private string controllerPrefix = "";

	[SerializeField]
	private GameObject movePosition = null;

	private enum Direction { Up, Left, Down, Right, Jump };

	// Tiles to move.
	private const int moveDistance = 1;
	private const int jumpDistance = 2;
	private const int layerMask = 1 << 8;

	private const float playerRadius = 0.5f;
	private const float raycastDistance = 0.9f;

	private bool canMoveVertically = true;
	private bool canMoveHorizontally = true;
	private bool canJump = true;

	private void FixedUpdate() {
		if (Input.GetAxis(controllerPrefix + "Vertical") == 0.0f) {
			canMoveVertically = true;
		}

		if (Input.GetAxis(controllerPrefix + "Horizontal") == 0.0f) {
			canMoveHorizontally = true;
		}

		if (Input.GetAxis(controllerPrefix + "Jump") == 0.0f) {
			canJump = true;
		}

		// Checks for player input.
		if ((Input.GetAxis(controllerPrefix + "Vertical") > 0.0f) && (canMoveVertically)) {
			CheckMoveDirection(Direction.Up);
			canMoveVertically = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Horizontal") < 0.0f) && (canMoveHorizontally)) {
			CheckMoveDirection(Direction.Left);
			canMoveHorizontally = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Vertical") < 0.0f) && (canMoveVertically)) {
			CheckMoveDirection(Direction.Down);
			canMoveVertically = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Horizontal") > 0.0f) && (canMoveHorizontally)) {
			CheckMoveDirection(Direction.Right);
			canMoveHorizontally = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Jump") > 0.0f) && (canJump))
		{
			CheckMoveDirection(Direction.Jump);
			canJump = false;
		}
	}

	/// <summary>
	/// Checks if the character can move in the desired direction then calls a method to move.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void CheckMoveDirection (Direction moveDirection) {
		// Used for checking objects around the player.
		RaycastHit2D ray;

		switch (moveDirection) {
			case Direction.Up: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.up;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case Direction.Left: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x - playerRadius, gameObject.transform.position.y), Vector2.left, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.left;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case Direction.Down: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.down;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case Direction.Right: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x + playerRadius, gameObject.transform.position.y), Vector2.right, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.right;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case Direction.Jump: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);

				if (!ray.collider || !ray.collider.CompareTag("Wall")) {
					Move(moveDirection);
				}

				break;
			}
		}
	}

	/// <summary>
	/// Moves the character and its move collider in the desired direction.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void Move(Direction moveDirection) {
		switch (moveDirection) {
			case Direction.Up: {
				gameObject.transform.Translate((Vector2.up * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case Direction.Left: {
				gameObject.transform.Translate((Vector2.left * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case Direction.Down: {
				gameObject.transform.Translate((Vector2.down * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case Direction.Right: {
				gameObject.transform.Translate((Vector2.right * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case Direction.Jump: {
				gameObject.transform.Translate((Vector2.up * jumpDistance), Space.World);
				break;
			}
		}
	}

	/// <summary>
	/// Pushes an object in the direction the player is trying to move, then moves the character into its place.
	/// </summary>
	/// <param name="pushDirection"> The direction to push. </param>
	private void Push(Direction pushDirection, RaycastHit2D pushObject) {
		RaycastHit2D ray;

		switch (pushDirection) {
			case Direction.Up: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.up * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
			case Direction.Left: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x - playerRadius, pushObject.transform.position.y), Vector2.left, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.left * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
			case Direction.Down: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.down * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
			case Direction.Right: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x + playerRadius, pushObject.transform.position.y), Vector2.right, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.right * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
		}
	}

	private void EnableMovement() {
		canMoveHorizontally = true;
		canMoveVertically = true;
	}

	private void DisableMovement() {
		canMoveHorizontally = false;
		canMoveVertically = false;
	}
}