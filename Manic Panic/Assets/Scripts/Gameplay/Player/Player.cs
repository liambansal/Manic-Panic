using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	private string controllerPrefix = "";

	[SerializeField]
	private GameObject movePosition = null; // Used for casting rays one tile ahead of the player when jumping.

	[SerializeField]
	private Sprite stunSprite = null;
	[SerializeField]
	private Sprite[] characterSprites = new Sprite[4];

	private const int moveDistance = 1;
	private const int jumpDistance = 2;
	// Bitshifted values are used for casting rays on specific unity layers.
	// Level layer may have: Empty Tiles, Rocks, Players, Holes and Water.
	private const int levelLayerBitShifted = 1 << 8;
	// The only objects that can be on the log layer are: Logs and Players.
	private const int logLayerBitShifted = 1 << 9;

	private const float playerRadius = 0.5f;
	private const float raycastDistance = 0.9f;
	private const float moveCooldown = 0.3f;
	private const float jumpCooldown = 0.6f;
	private const float punchCooldown = 4.0f;
	private const float stunLength = 2.0f;

	// The layer we're currently casting rays onto.
	private int currentLayer = 0;

	private float moveTimer = 0.3f;
	private float jumpTimer = 0.6f;
	private float punchTimer = 0.0f;
	private float stunTimer = 2.0f;

	private bool canMove = true;
	private bool canJump = true;
	private bool canPunch = true;
	private bool stunned = false;

	private PauseMenuController pauseMenuController = null;

	private Sprite playerSprite = null;

	private enum Directions { Up, Left, Down, Right };

	private RaycastHit2D rayOne = new RaycastHit2D(); // Used for checking tiles around the player.
	private RaycastHit2D rayTwo = new RaycastHit2D(); // Used for checking tiles around objects hit by ray one.

	private void Start() {
		playerSprite = characterSprites[PlayerPrefs.GetInt(controllerPrefix)];
		gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
		pauseMenuController = GameObject.FindGameObjectWithTag("PauseMenuController").GetComponent<PauseMenuController>();
	}

	private void Update() {
		if (!pauseMenuController.isPaused) {
			// It's only possible for the player to have a log as a parent.
			if (transform.parent != null) {
				// Moves the player with their parent.
				transform.position = gameObject.transform.parent.position;
			}

			if (!stunned) {
				// Cooldown timer logic for moving.
				if (!canMove) {
					moveTimer -= Time.deltaTime;
				}

				if (moveTimer <= 0.0f) {
					canMove = true;
					moveTimer = moveCooldown;
				}

				// Cooldown timer logic for jumping.
				if (!canJump) {
					jumpTimer -= Time.deltaTime;
				}

				if (jumpTimer <= 0.0f) {
					canJump = true;
					jumpTimer = jumpCooldown;
				}

				// Cooldown timer logic for punching.
				if (!canPunch) {
					punchTimer -= Time.deltaTime;
				}

				if (punchTimer <= 0.0f) {
					canPunch = true;
					punchTimer = punchCooldown;
				}
			} else if (stunned) {
				canMove = false;
				canPunch = false;
				stunTimer -= Time.deltaTime;
			}

			if (stunTimer <= 0.0f) {
				stunned = false;
				gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
				stunTimer = stunLength;
			}

			HandleInput();
		}
	}

	void HandleInput() {
		// Checks input for moving.
		if (((Input.GetAxis(controllerPrefix + "Vertical2") < 0.0f) || (Input.GetAxis(controllerPrefix + "Vertical") > 0.0f)) && canMove) {
			HandleMovement(Directions.Up, Vector2.up);
		} else if (((Input.GetAxis(controllerPrefix + "Vertical2") > 0.0f) || (Input.GetAxis(controllerPrefix + "Vertical") < 0.0f)) && canMove) {
			HandleMovement(Directions.Down, Vector2.down);
		}

		if (((Input.GetAxis(controllerPrefix + "Horizontal2") < 0.0f) || (Input.GetAxis(controllerPrefix + "Horizontal")) < 0.0f) && canMove) {
			HandleMovement(Directions.Left, Vector2.left);
		} else if (((Input.GetAxis(controllerPrefix + "Horizontal2") > 0.0f) || (Input.GetAxis(controllerPrefix + "Horizontal") > 0.0f)) && canMove) {
			HandleMovement(Directions.Right, Vector2.right);
		}

		// Checks input for jumping.
		if ((Input.GetAxis(controllerPrefix + "Jump") > 0.0f) && canJump) {
			HandleJumping();
		}

		// Check input for punching.
		if ((Input.GetAxis(controllerPrefix + "FireVertical") < 0.0f) && canPunch) {
			Punch(Directions.Up);
			canPunch = false;
		} else if ((Input.GetAxis(controllerPrefix + "FireVertical") > 0.0f) && canPunch) {
			Punch(Directions.Down);
			canPunch = false;
		}

		if ((Input.GetAxis(controllerPrefix + "FireHorizontal") < 0.0f) && canPunch) {
			Punch(Directions.Left);
			canPunch = false;
		} else if ((Input.GetAxis(controllerPrefix + "FireHorizontal") > 0.0f) && canPunch) {
			Punch(Directions.Right);
			canPunch = false;
		}
	}

	void HandleMovement(Directions rayDirection, Vector2 moveDirection) {
		// Loops through each layer that's part of the level.
		for (int i = 0; i < 2; ++i) {
			if (i == 0) {
				currentLayer = logLayerBitShifted;
			} else if (i == 1) {
				currentLayer = levelLayerBitShifted;
			}

			// Cast ray to check what's in the tile upward of the player.
			rayOne = Raycast(transform, rayDirection, raycastDistance, currentLayer);

			if (CheckMoveDirection(rayOne)) {
				// Tile is avaiable to move onto, just need to check its type.
				if (CheckTileForLog(rayOne)) {
					Move(moveDirection, moveDistance, rayOne); // Move player onto the log.
					return;
				} else if (!rayOne.collider || (rayOne.collider && (rayOne.collider.CompareTag("Empty Tile") || rayOne.collider.CompareTag("Danger") || rayOne.collider.CompareTag("Move Position")))) {
					Move(moveDirection, moveDistance, rayOne);
					return;
				}
			} else if (rayOne.collider && rayOne.collider.CompareTag("Player")) {
				// Tile has a player on there so we must push them.
				if (Push(rayDirection, moveDirection)) {
					for (int y = 0; y < 2; ++y) {
						if (y == 0) {
							currentLayer = logLayerBitShifted;
						} else if (y == 1) {
							currentLayer = levelLayerBitShifted;
						}

						// We just pushed the player. Cast a ray to find out what they were standing on.
						rayOne = Raycast(transform, rayDirection, raycastDistance, currentLayer);

						if (CheckTileForLog(rayOne)) {
							Move(moveDirection, moveDistance, rayOne);
							return;
						} else if (!CheckTileForLog(rayOne)) {
							Move(moveDirection, moveDistance, rayOne);
							return;
						}
					}
				}
			}
		}
	}

	void HandleJumping() {
		for (int i = 0; i < 2; ++i) {
			if (i == 0) {
				currentLayer = logLayerBitShifted; // Check log layer for collisions first.
			} else if (i == 1) {
				currentLayer = levelLayerBitShifted;
			}

			// Cast ray to check what's in the tile upward of the player.
			rayOne = Raycast(transform, Directions.Up, raycastDistance, currentLayer);

			if (CheckMoveDirection(rayOne)) {
				if (Vector2.Distance(gameObject.transform.position, movePosition.transform.position) < (float)moveDistance) {
					// Move the move position if it hasn't been done already. A distance >= moveDistance means its already been done.
					movePosition.transform.Translate((Vector2.up), Space.Self);
				}

				for (int j = 0; j < 2; ++j) {
					if (j == 0) {
						currentLayer = logLayerBitShifted; // Check log layer first after casting second ray.
					} else if (j == 1) {
						currentLayer = levelLayerBitShifted;
					}

					// Cast ray to check what's in the second tile upward of the player.
					rayTwo = Raycast(movePosition.transform, Directions.Up, raycastDistance, currentLayer);

					if (CheckMoveDirection(rayTwo)) {
						if (CheckTileForLog(rayTwo)) {
							Move(Vector2.up, jumpDistance, rayTwo);
							return;
						} else if (!rayTwo.collider || (rayTwo.collider && (rayTwo.collider.CompareTag("Empty Tile") || rayTwo.collider.CompareTag("Danger") || rayTwo.collider.CompareTag("Move Position")))) {
							Move(Vector2.up, jumpDistance, rayTwo);
							return;
						}
					} else if (rayTwo.collider && rayTwo.collider.CompareTag("Player")) {
						return; // Return because we don't want to jump into a tile underneath the player on a different layer.
					}
				}
			} else if (rayOne.collider && rayOne.collider.CompareTag("Player")) {
				// Return because we don't want to jump over the player.
				return;
			}
		}
	}

	private bool CheckMoveDirection(RaycastHit2D ray) {
		if (ray.collider) {
			if ((ray.collider.CompareTag("Empty Tile") || ray.collider.CompareTag("Danger") || ray.collider.CompareTag("Log") || ray.collider.CompareTag("Move Position"))) {
				return true;
			} else { // Tag is something we want to collide with not move into.
				return false;
			}
		} else {
			return false;
		}
	}

	private bool CheckTileForLog(RaycastHit2D ray) {
		if (ray.collider && ray.collider.CompareTag("Log")) {
			return true;
		} else {
			return false;
		}
	}

	private void Move(Vector2 moveDirection, float distance, RaycastHit2D ray) {
		if (CheckTileForLog(ray)) {
			// If the player isn't on the log layer move them onto it, so they've don't collide with the river hazard.
			if (gameObject.layer != ray.collider.gameObject.layer) {
				gameObject.layer = ray.collider.gameObject.layer;
			}

			transform.SetParent(ray.collider.gameObject.transform); // Set the player's parent to log.
		} else if (ray.collider) {
			// If the player isn't on the level layer move them onto it.
			if (gameObject.layer != ray.collider.gameObject.layer) {
				gameObject.layer = ray.collider.gameObject.layer;
			}

			transform.SetParent(null);
		}

		// Moves the player from their current position.
		gameObject.transform.Translate(moveDirection * distance, Space.World);
		// Clamp the player's position to a .5 decimal number.
		gameObject.transform.position = ClampPosition(gameObject.transform.position);
		canMove = false;
		canJump = false;
		movePosition.transform.localPosition = Vector2.zero;
	}

	private bool Push(Directions rayDirection, Vector2 moveDirection) {
		// Loops though each layer and checks for collisions.
		for (int i = 0; i < 2; ++i) {
			if (i == 0) {
				currentLayer = logLayerBitShifted;
			} else if (i == 1) {
				currentLayer = levelLayerBitShifted;
			}

			// Cast ray to check what's in the tile upward of what we're pushing
			rayTwo = Raycast(rayOne.collider.transform, rayDirection, raycastDistance, currentLayer);

			if (CheckMoveDirection(rayTwo)) {
				// Can push player.
				if (CheckTileForLog(rayTwo)) {
					// Push them onto a log.
					GameObject pushObjectsParent = null;

					if (rayOne.collider.gameObject.transform.parent) { // Check if pushed player has a parent.
						pushObjectsParent = rayOne.collider.gameObject.transform.parent.gameObject;
					}

					if (rayOne.collider.gameObject.layer != rayTwo.collider.gameObject.layer) { // Check that the pushed player is on the log layer.
						rayOne.collider.gameObject.layer = rayTwo.collider.gameObject.layer; // Put pushed player on same layer as log. 
					}

					rayOne.collider.gameObject.transform.SetParent(rayTwo.collider.transform); // Set log as pushed player's parent.
					rayOne.collider.gameObject.transform.position = rayTwo.collider.transform.position; // Push player onto the log.
					return true; // Return true after pushing the player.
				} else { // Push them onto an empty tile.
					if (rayOne.collider.gameObject.layer != rayTwo.collider.gameObject.layer) { // Check that the pushed player is on the level layer.
						rayOne.collider.gameObject.layer = rayTwo.collider.gameObject.layer;
					}

					rayOne.transform.SetParent(null); // Delete parent.
					rayOne.transform.Translate(moveDirection * moveDistance, Space.World); // Move pushed player to the empty tile.
					return true; // Return true after pushing player.
				}

			} else if (rayTwo.collider && rayTwo.collider.CompareTag("Player")) {
				// Can't push player into another player, return false.
				return false;
			}
		}

		// Can't push player, return false.
		return false;
	}

	private void Punch(Directions punchDirection) {
		switch (punchDirection) {
			case Directions.Up: {
				rayOne = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, raycastDistance, logLayerBitShifted);

				if (rayOne.collider) {
					break;
				} else {
					rayOne = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, raycastDistance, levelLayerBitShifted);
				}

				break;
			}
			case Directions.Left: {
				rayOne = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left, raycastDistance, logLayerBitShifted);

				if (rayOne.collider) {
					break;
				} else {
					rayOne = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left, raycastDistance, levelLayerBitShifted);
				}
				break;
			}
			case Directions.Down: {
				rayOne = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down, raycastDistance, logLayerBitShifted);

				if (rayOne.collider) {
					break;
				} else {
					rayOne = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down, raycastDistance, levelLayerBitShifted);
				}
				break;
			}
			case Directions.Right: {
				rayOne = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right, raycastDistance, logLayerBitShifted);

				if (rayOne.collider) {
					break;
				} else {
					rayOne = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right, raycastDistance, levelLayerBitShifted);
				}
				break;
			}
			default: {
				return;
			}
		}

		if (rayOne.collider && rayOne.collider.gameObject.CompareTag("Player")) {
			rayOne.collider.gameObject.GetComponent<Player>().Stunned();
		}
	}

	public void Stunned() {
		stunned = true;
		gameObject.GetComponent<SpriteRenderer>().sprite = stunSprite;
		gameObject.GetComponent<TreasureBag>().DropTreasure();
	}

	private RaycastHit2D Raycast(Transform origin, Directions direction, float distance, int layer) {
		switch (direction) {
			case Directions.Up: {
				Debug.DrawRay(new Vector2(origin.position.x, origin.position.y + playerRadius), Vector2.up, Color.red, 10.0f);
				return Physics2D.Raycast(new Vector2(origin.position.x, origin.position.y + playerRadius), Vector2.up, distance, layer);
			}
			case Directions.Left: {
				Debug.DrawRay(new Vector2(origin.position.x - playerRadius, origin.position.y), Vector2.left, Color.red, 10.0f);
				return Physics2D.Raycast(new Vector2(origin.position.x - playerRadius, origin.position.y), Vector2.left, distance, layer);
			}
			case Directions.Down: {
				Debug.DrawRay(new Vector2(origin.position.x, origin.position.y - playerRadius), Vector2.down, Color.red, 10.0f);
				return Physics2D.Raycast(new Vector2(origin.position.x, origin.position.y - playerRadius), Vector2.down, distance, layer);
			}
			case Directions.Right: {
				Debug.DrawRay(new Vector2(origin.position.x + playerRadius, origin.position.y), Vector2.right, Color.red, 10.0f);
				return Physics2D.Raycast(new Vector2(origin.position.x + playerRadius, origin.position.y), Vector2.right, distance, layer);
			}
			default: {
				Debug.DrawRay(new Vector2(origin.position.x + playerRadius, origin.position.y), Vector2.up, Color.blue, 10.0f);
				return Physics2D.Raycast(new Vector2(origin.position.x, origin.position.y), Vector2.up, 0.0f, layer);
			}
		}
	}

	Vector2 ClampPosition(Vector2 position) {
		// Gets two scalar values for comparing whether or not we need to clamp the player's x/y 
		// position up or down to the nearest float ending in .5.
		// NormalisedVector.x/y > 0.0f would mean we need to clamp up.
		Vector2 normalisedVector = position.normalized;

		if (normalisedVector.x > 0.0f) {
			// Casting a float to an integer truncates the number so we get a whole number.
			// Simply add/subtract 0.5 to allign the scalar value to the level's tilemap.
			position.x = (int)position.x + 0.5f;
		} else if (normalisedVector.x <= 0.0f) {
			position.x = (int)position.x - 0.5f;
		}

		if (normalisedVector.y > 0.0f) {
			position.y = (int)position.y + 0.5f;
		} else if (normalisedVector.y <= 0.0f) {
			position.y = (int)position.y - 0.5f;
		}

		Vector2 clampedVector = position;
		return clampedVector;
	}
}