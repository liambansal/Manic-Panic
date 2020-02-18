using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	private string controllerPrefix = "";

	[SerializeField]
	private GameObject movePosition = null;

	[SerializeField]
	private Sprite uiBox = null;
	[SerializeField]
	private Sprite playerSprite = null;

	private enum MoveDirections { Up, Left, Down, Right, Jump };
	private enum PunchDirections { Up, Left, Down, Right };

	private const int moveDistance = 1;
	private const int jumpDistance = 2;
	private const int layerMask = 1 << 8;

	private const float playerRadius = 0.5f;
	private const float raycastDistance = 0.9f;
	private const float punchCooldownLength = 4.0f; // Cooldown length for punching.
	private const float stunLength = 2.0f;

	private float punchCooldown = 0.0f; // Time remaining before player can punch.
	private float stunTimer = 2.0f;

	private bool canMoveVertically = true;
	private bool canMoveHorizontally = true;
	private bool canJump = true;
	private bool canPunch = true;
	private bool stunned = false;

	private void Update() {
		if (!stunned) {
			if (Input.GetAxis(controllerPrefix + "Vertical") == 0.0f) {
				canMoveVertically = true;
			}

			if (Input.GetAxis(controllerPrefix + "Horizontal") == 0.0f) {
				canMoveHorizontally = true;
			}

			if (Input.GetAxis(controllerPrefix + "Jump") == 0.0f) {
				canJump = true;
			}

			if (!canPunch) {
				punchCooldown -= Time.deltaTime;
			}

			if (punchCooldown <= 0.0f) {
				canPunch = true;
				punchCooldown = punchCooldownLength;
			}
		} else if (stunned) {
			canMoveVertically = false;
			canMoveHorizontally = false;
			canJump = false;
			canPunch = false;
			stunTimer -= Time.deltaTime;
			// TODO: Play stun animation. + Enable movement once animation has finished.
		}

		if (stunTimer <= 0) {
			stunned = false;
			gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
			stunTimer = stunLength;
		}

		// Checks input for moving.
		if ((Input.GetAxis(controllerPrefix + "Vertical") > 0.0f) && (canMoveVertically)) {
			CheckMoveDirection(MoveDirections.Up);
			canMoveVertically = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Horizontal") < 0.0f) && (canMoveHorizontally)) {
			CheckMoveDirection(MoveDirections.Left);
			canMoveHorizontally = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Vertical") < 0.0f) && (canMoveVertically)) {
			CheckMoveDirection(MoveDirections.Down);
			canMoveVertically = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Horizontal") > 0.0f) && (canMoveHorizontally)) {
			CheckMoveDirection(MoveDirections.Right);
			canMoveHorizontally = false;
		}

		if ((Input.GetAxis(controllerPrefix + "Jump") > 0.0f) && (canJump)) {
			CheckMoveDirection(MoveDirections.Jump);
			canJump = false;
		}

		// Check input for punching.
		if ((Input.GetAxis(controllerPrefix + "FireVertical") < 0.0f) && canPunch) {
			Punch(PunchDirections.Up);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireHorizontal") < 0.0f) && canPunch) {
			Punch(PunchDirections.Left);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireVertical") > 0.0f) && canPunch) {
			Punch(PunchDirections.Down);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireHorizontal") > 0.0f) && canPunch) {
			Punch(PunchDirections.Right);
			canPunch = false;
		}
	}

	/// <summary>
	/// Checks if the character can move in the desired direction then calls a method to move.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void CheckMoveDirection(MoveDirections moveDirection) {
		// Used for checking objects around the player.
		RaycastHit2D ray;

		switch (moveDirection) {
			case MoveDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.up;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case MoveDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x - playerRadius, gameObject.transform.position.y), Vector2.left, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.left;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case MoveDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.down;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case MoveDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x + playerRadius, gameObject.transform.position.y), Vector2.right, raycastDistance, layerMask);

				if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position"))) {
					movePosition.transform.localPosition = Vector2.right;
					Move(moveDirection);
				} else if (ray.collider.CompareTag("Player")) {
					Push(moveDirection, ray);
				}

				break;
			}
			case MoveDirections.Jump: {
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
	private void Move(MoveDirections moveDirection) {
		switch (moveDirection) {
			case MoveDirections.Up: {
				gameObject.transform.Translate((Vector2.up * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case MoveDirections.Left: {
				gameObject.transform.Translate((Vector2.left * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case MoveDirections.Down: {
				gameObject.transform.Translate((Vector2.down * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case MoveDirections.Right: {
				gameObject.transform.Translate((Vector2.right * moveDistance), Space.World);
				movePosition.transform.localPosition = Vector2.zero;
				break;
			}
			case MoveDirections.Jump: {
				gameObject.transform.Translate((Vector2.up * jumpDistance), Space.World);
				break;
			}
		}
	}

	/// <summary>
	/// Pushes an object in the direction the player is trying to move, then moves the character into its place.
	/// </summary>
	/// <param name="pushDirection"> The direction to push. </param>
	private void Push(MoveDirections pushDirection, RaycastHit2D pushObject) {
		RaycastHit2D ray;

		switch (pushDirection) {
			case MoveDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.up * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
			case MoveDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x - playerRadius, pushObject.transform.position.y), Vector2.left, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.left * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
			case MoveDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.down * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
			case MoveDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x + playerRadius, pushObject.transform.position.y), Vector2.right, raycastDistance, layerMask);

				if ((!ray.collider) || (!ray.collider.CompareTag("Wall"))) {
					pushObject.transform.Translate((Vector2.right * moveDistance), Space.World);
					Move(pushDirection);
				}

				break;
			}
		}
	}

	private void Punch(PunchDirections punchDirection) {
		RaycastHit2D ray;

		switch (punchDirection) {
			case PunchDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
			case PunchDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left, raycastDistance, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
			case PunchDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
			case PunchDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right, raycastDistance, layerMask);

				if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
					// TODO: Play punch animation.
					ray.collider.gameObject.SendMessage("Stunned");
				}

				break;
			}
		}
	}

	private void Stunned() {
		stunned = true;
		gameObject.GetComponent<SpriteRenderer>().sprite = uiBox; // TODO: delete line once animation is in place
		gameObject.SendMessage("DropTreasure");
	}
}
