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
	// Bitshifted values are used for casting rays on specific unity layers.
	private const int levelLayerBitShifted = 1 << 8;
	private const int logLayerBitShifted = 1 << 9;
	// Integer values are used for setting the layer of gameObjects.
	private const int levelLayerInt = 8;
	private const int logLayerInt = 9;

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
		// If the player has a parent then it's a log.
		if (transform.parent != null) {
			// Move the player with their parent.
			transform.position = ray.collider.gameObject.transform.position;
		}

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
			// Casts a ray one tile in the respective case direction.
			case MoveDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, logLayerBitShifted);

				if (ray.collider && ray.collider.CompareTag("Log")) {
					direction = Vector2.up;
					break;
				} else {
					ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, levelLayerBitShifted);
				}

				direction = Vector2.up;
				break;
			}
			case MoveDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x - playerRadius, gameObject.transform.position.y), Vector2.left, raycastDistance, logLayerBitShifted);

				if (ray.collider && ray.collider.CompareTag("Log")) {
					direction = Vector2.left;
					break;
				} else {
					ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x - playerRadius, gameObject.transform.position.y), Vector2.left, raycastDistance, levelLayerBitShifted);
				}
				direction = Vector2.left;
				break;
			}
			case MoveDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, logLayerBitShifted);

				if (ray.collider && ray.collider.CompareTag("Log")) {
					direction = Vector2.down;
					break;
				} else {
					ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, levelLayerBitShifted);
				}
				direction = Vector2.down;
				break;
			}
			case MoveDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x + playerRadius, gameObject.transform.position.y), Vector2.right, raycastDistance, logLayerBitShifted);

				if (ray.collider && ray.collider.CompareTag("Log")) {
					direction = Vector2.right;
					break;
				} else {
					ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x + playerRadius, gameObject.transform.position.y), Vector2.right, raycastDistance, levelLayerBitShifted);
				}
				direction = Vector2.right;
				break;
			}
			case MoveDirections.Jump: {
				// Casts a ray two tiles ahead of the player's position.
				ray = Physics2D.Raycast(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + playerRadius), Vector2.up, raycastDistance * 2, levelLayerBitShifted);

				// If we cast a ray two tiles ahead of the player's position it will return the object first hit by the ray (Which is possibly only one tile ahead of our player's position). 
				// So, to find out if there's an object two tiles ahead we need to cast from one tile ahead of our player's current position.
				// Checks if the ray collided with something that isn't a wall/player/obstruction, meaning it clear to move across.
				if (ray.collider && (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
					// Casts a ray one tile ahead of our player's position.
					ray = Physics2D.Raycast(new Vector2(ray.collider.transform.position.x, ray.collider.transform.position.y + (playerRadius * 2)), Vector2.up, raycastDistance, levelLayerBitShifted);
					
					// Checks if the tile is clear of obstructions, except logs.
					if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
						
						// If the tile is a log then set it as the player's parent and move the player to its position.
						if (ray.collider && ray.collider.CompareTag("Log")) {
							if (gameObject.layer != logLayerInt) {
								gameObject.layer = 9; // Move the player onto the logs layer to avoid collisions with the water hazard.
							}
							transform.SetParent(ray.collider.gameObject.transform);
							transform.position = ray.collider.gameObject.transform.position;
						} else {
							if (gameObject.layer != levelLayerInt) {
								gameObject.layer = 8; // Move the player onto the level layer so they collide with any hazards.
							}
							transform.SetParent(null);
							// Moves the player two tiles ahead of their current position.
							Move(Vector2.up, jumpDistance);
						}
					}
				} else if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
					// If the tile is a log then set it as the player's parent and move the player to its position.
					if (ray.collider && ray.collider.CompareTag("Log")) {
						if (gameObject.layer != logLayerInt) {
							gameObject.layer = 9; // Move the player onto the logs layer to avoid collisions with the water hazard.
						}
						transform.SetParent(ray.collider.gameObject.transform);
						transform.position = ray.collider.gameObject.transform.position;
					} else {
						if (gameObject.layer != levelLayerInt) {
							gameObject.layer = 8; // Move the player onto the level layer so they collide with any hazards.
						}
						transform.SetParent(null);
						// Moves the player two tiles ahead of their current position.
						Move(Vector2.up, jumpDistance);
					}
				}

				return;
			}
		}

		// If the tile is a log then set it as the player's parent and move the player to its position.
		if (ray.collider && ray.collider.CompareTag("Log")) {
			if (gameObject.layer != logLayerInt) {
				gameObject.layer = 9; // Move the player onto the logs layer to avoid collisions with the water hazard.
			}
			transform.SetParent(ray.collider.gameObject.transform);
			transform.position = ray.collider.gameObject.transform.position;
			Debug.Log(gameObject.layer.ToString());
		} else if (!ray.collider || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Move Position") && !ray.collider.CompareTag("Obstruction"))) {
			if (gameObject.layer != levelLayerInt) {
				gameObject.layer = 8; // Move the player onto the level layer so they collide with any hazards.
			}
			transform.SetParent(null);
			movePosition.transform.localPosition = direction;
			Move(direction, moveDistance);
			Debug.Log(gameObject.layer.ToString());
		} else if (ray.collider && ray.collider.CompareTag("Player")) {
			Push(moveDirection, ray);
		}
	}

	/// <summary>
	/// Moves the character and its move collider in the desired direction.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	private void Move(Vector2 moveDirection, float distance) {
		// Moves the player up from their current position.
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
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y + playerRadius), Vector2.up, raycastDistance, levelLayerBitShifted);
				direction = Vector2.up;
				break;
			}
			case MoveDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x - playerRadius, pushObject.transform.position.y), Vector2.left, raycastDistance, levelLayerBitShifted);
				direction = Vector2.left;
				break;
			}
			case MoveDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x, pushObject.transform.position.y - playerRadius), Vector2.down, raycastDistance, levelLayerBitShifted);
				direction = Vector2.down;
				break;
			}
			case MoveDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(pushObject.transform.position.x + playerRadius, pushObject.transform.position.y), Vector2.right, raycastDistance, levelLayerBitShifted);
				direction = Vector2.right;
				break;
			}
		}

		// Handles pushing players if the pushObject was on a log.
		// Does the pushObject need to move onto a log?
		if (ray.collider && ray.collider.CompareTag("Log")) { // Pushed player will now move onto a log.
			// PushObject.
			pushObject.collider.gameObject.layer = logLayerInt; // Move the pushObject onto the logs layer so they avoid collisions with hazards.
			GameObject pushObjectsParent = pushObject.collider.gameObject.transform.parent.gameObject; // Stores the pushObject's parent (because it was on a log).
			pushObject.collider.gameObject.transform.SetParent(ray.collider.gameObject.transform);
			pushObject.collider.gameObject.transform.position = ray.collider.gameObject.transform.position;
			// Player.
			// PushObject must be on a log because they're on the log layer.
			if (pushObject.collider.gameObject.layer == logLayerInt) {
				gameObject.layer = logLayerInt; // Move the player onto the logs layer so they avoid collisions with hazards.
				transform.SetParent(pushObjectsParent.transform); // Set parent as the log which the pushed player was on.
				transform.position = pushObjectsParent.transform.position;
			} else { // PushObject was not on a log.
				gameObject.layer = 8; // Move the player onto the level layer so they collide with any hazards.
				transform.SetParent(null);
				Move(direction, moveDistance);
			}
		} else { // PushObject does not need to move onto a log
			if ((!ray.collider) || (!ray.collider.CompareTag("Wall") && !ray.collider.CompareTag("Player") && !ray.collider.CompareTag("Obstruction"))) {
				// Will store pushObject's parent, if it has one.
				GameObject pushObjectsParent;

				// Checks if the pushObject has a parent and stores it if returned as true.
				if (pushObject.collider.gameObject.transform.parent != null) {
					pushObjectsParent = pushObject.collider.gameObject.transform.parent.gameObject;
				} else { // If false then there's no parent so set it to null.
					pushObjectsParent = null;
				}

				pushObject.collider.gameObject.layer = 8; // Move the player onto the level layer so they collide with any hazards.
				pushObject.collider.gameObject.transform.SetParent(null); // Removes the pushObject's parent so they stop moving with it, if there is one.
				pushObject.transform.Translate((direction * moveDistance), Space.World);

				// Checks if the player needs to move onto a log. If true, pushObject's parent is a log.
				if (pushObject.collider.gameObject.transform.parent != null) { // Player will now need to move onto a log.
					gameObject.layer = logLayerInt; // Move the player onto the logs layer so they avoid collisions with hazards.
					gameObject.transform.parent = pushObjectsParent.transform; // Set player's parent so they move with the log.
					gameObject.transform.position = pushObjectsParent.transform.position; // Set player's position so they're on the log.
				} else { // Player does not need to move onto a log.
					gameObject.layer = 8; // Move the player onto the level layer so they collide with any hazards.
					transform.SetParent(null);
					Move(direction, moveDistance);
				}
			}
		}
	}

	private void Punch(PunchDirections punchDirection) {
		switch (punchDirection) {
			case PunchDirections.Up: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, raycastDistance, levelLayerBitShifted);
				break;
			}
			case PunchDirections.Left: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left, raycastDistance, levelLayerBitShifted);
				break;
			}
			case PunchDirections.Down: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down, raycastDistance, levelLayerBitShifted);
				break;
			}
			case PunchDirections.Right: {
				ray = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right, raycastDistance, levelLayerBitShifted);
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
