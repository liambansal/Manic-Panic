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

	private const int moveDistance = 1;
	private const int jumpDistance = 2;
	private const int layerMask = 1 << 8;

	private const float playerRadius = 0.5f;
	private const float raycastDistance = 0.9f;
	private const float punchCooldownLength = 4.0f; // Cooldown length for punching.
	private const float stunLength = 2.0f;
	private const float moveCooldown = 0.25f;

	private float punchCooldown = 0.0f; // Time remaining before player can punch.
	private float stunTimer = 2.0f;
	private float moveTimer = 0.25f;

	private bool canMoveVertically = true;
	private bool canMoveHorizontally = true;
	private bool moved = false; // Tells us whether or not the player has moved.
	private bool canJump = true;
	private bool canPunch = true;
	private bool stunned = false;

	private enum MoveDirections { Up, Left, Down, Right, Jump };
	private enum PunchDirections { Up, Left, Down, Right };

	private RaycastHit2D ray = new RaycastHit2D(); // Used for checking objects around the player.

	private Vector2 direction = new Vector2(); // Stores a shorthand Vector2 direction.

	private void Update() {
		if (!stunned) {
			if (moved) {
				moveTimer -= Time.deltaTime;
			}

			if (/*Input.GetAxis(controllerPrefix + "Vertical") == 0.0f*/moveTimer <= 0.0f) {
				moved = false;
				canMoveVertically = true;
				canMoveHorizontally = true;
				canJump = true;
				moveTimer = moveCooldown;
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

		if (stunTimer <= 0.0f) {
			stunned = false;
			gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
			stunTimer = stunLength;
		}

		// Checks input for moving.
		if (((Input.GetAxis(controllerPrefix + "Vertical2") < 0.0f) || (Input.GetAxis(controllerPrefix + "Vertical") > 0.0f)) && canMoveVertically && !moved) {
			CheckMoveDirection(MoveDirections.Up);
			canMoveVertically = false;
			moved = true;
		} else if (((Input.GetAxis(controllerPrefix + "Vertical2") > 0.0f) || (Input.GetAxis(controllerPrefix + "Vertical") < 0.0f)) && canMoveVertically && !moved) {
			CheckMoveDirection(MoveDirections.Down);
			canMoveVertically = false;
			moved = true;
		}

		if (((Input.GetAxis(controllerPrefix + "Horizontal2") < 0.0f) || (Input.GetAxis(controllerPrefix + "Horizontal")) < 0.0f) && canMoveHorizontally && !moved) {
			CheckMoveDirection(MoveDirections.Left);
			canMoveHorizontally = false;
			moved = true;
		} else if (((Input.GetAxis(controllerPrefix + "Horizontal2") > 0.0f) || (Input.GetAxis(controllerPrefix + "Horizontal") > 0.0f)) && canMoveHorizontally && !moved) {
			CheckMoveDirection(MoveDirections.Right);
			canMoveHorizontally = false;
			moved = true;
		}

		if ((Input.GetAxis(controllerPrefix + "Jump") > 0.0f) && canJump && !moved) {
			CheckMoveDirection(MoveDirections.Jump);
			canJump = false;
			moved = true;
		}

		// Check input for punching.
		if ((Input.GetAxis(controllerPrefix + "FireVertical") < 0.0f) && canPunch) {
			Punch(PunchDirections.Up);
			canPunch = false;
		} else if ((Input.GetAxis(controllerPrefix + "FireVertical") > 0.0f) && canPunch) {
			Punch(PunchDirections.Down);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireHorizontal") < 0.0f) && canPunch) {
			Punch(PunchDirections.Left);
			canPunch = false;
		} else if ((Input.GetAxis(controllerPrefix + "FireHorizontal") > 0.0f) && canPunch) {
			Punch(PunchDirections.Right);
			canPunch = false;
		}
	}

	/// <summary>
	/// Checks if the character can move in the desired direction then calls a method to move.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void CheckMoveDirection(MoveDirections moveDirection) {
		switch (moveDirection) {
			case MoveDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);
				direction = Vector2.up;
				break;
			}
			case MoveDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x - playerRadius, gameObject.transform.position.y), Vector2.left, raycastDistance, layerMask);
				direction = Vector2.left;
				break;
			}
			case MoveDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);
				direction = Vector2.down;
				break;
			}
			case MoveDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x + playerRadius, gameObject.transform.position.y), Vector2.right, raycastDistance, layerMask);
				direction = Vector2.right;
				break;
			}
			case MoveDirections.Jump: {
				// Casts a ray two tiles ahead of the player's position.
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance * 2, layerMask);

				// If we cast a ray two tiles ahead of the player's position it will return the object first hit by the ray (Which is possibly only one tile ahead of our player's position). 
				// So, to find out if there's an object two tiles ahead we need to cast from one tile ahead of our player's current position.
				// Checks if the ray collided with something that isn't a wall/player/obstruction, meaning it clear to move across.
				if (ray.collider && (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
					// Casts a ray one tile ahead of our player's position.
					ray = Physics2D.Raycast(new Vector2(ray.collider.transform.position.x, ray.collider.transform.position.y + (playerRadius * 2)), Vector2.up, raycastDistance, layerMask);
					
					// Checks if the tile is clear of obstructions.
					if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
						// Moves the player two tiles ahead of their current position.
						Move(Vector2.up, jumpDistance);
					}
				} else if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
					// Moves the player two tiles ahead of their current position.
					Move(Vector2.up, jumpDistance);
				}

				return;
			}
		}

		if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position") && !ray.collider.CompareTag("Obstruction"))) {
			movePosition.transform.localPosition = direction;
			Move(direction, moveDistance);
		} else if (ray.collider.CompareTag("Player")) {
			Push(moveDirection, ray);
		}
	}

	/// <summary>
	/// Moves the character and its move collider in the desired direction.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void Move(Vector2 moveDirection, float distance) {
		gameObject.transform.Translate((moveDirection * distance), Space.World);

		if (distance < 1.0f) {
			movePosition.transform.localPosition = Vector2.zero;
		}
	}

	/// <summary>
	/// Pushes an object in the direction the player is trying to move, then moves the character into its place.
	/// </summary>
	/// <param name="pushDirection"> The direction to push. </param>
	private void Push(MoveDirections pushDirection, RaycastHit2D pushObject) {
		switch (pushDirection) {
			case MoveDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);
				direction = Vector2.up;
				break;
			}
			case MoveDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x - playerRadius, pushObject.transform.position.y), Vector2.left, raycastDistance, layerMask);
				direction = Vector2.left;
				break;
			}
			case MoveDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);
				direction = Vector2.down;
				break;
			}
			case MoveDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x + playerRadius, pushObject.transform.position.y), Vector2.right, raycastDistance, layerMask);
				direction = Vector2.right;
				break;
			}
		}

		if ((!ray.collider) || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
			pushObject.transform.Translate((direction * moveDistance), Space.World);
			Move(direction, moveDistance);
		}
	}

	private void Punch(PunchDirections punchDirection) {
		switch (punchDirection) {
			case PunchDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, raycastDistance, layerMask);
				break;
			}
			case PunchDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left, raycastDistance, layerMask);
				break;
			}
			case PunchDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down, raycastDistance, layerMask);
				break;
			}
			case PunchDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right, raycastDistance, layerMask);
				break;
			}
		}

		if (ray.collider && ray.collider.gameObject.CompareTag("Player")) {
			ray.collider.gameObject.SendMessage("Stunned");
		}
	}

	private void Stunned() {
		stunned = true;
		gameObject.GetComponent<SpriteRenderer>().sprite = uiBox; // TODO: delete line once animation is in place
		gameObject.SendMessage("DropTreasure");
	}
}
