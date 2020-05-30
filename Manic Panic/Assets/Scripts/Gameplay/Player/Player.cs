using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField]
	private string controllerPrefix = "";

	/// <summary>
	/// Used for casting rays one tile ahead of the player when jumping.
	/// </summary>
	[SerializeField]
	private GameObject movePosition = null;
	[SerializeField]
	private Sprite stunSprite = null;
	[SerializeField]
	private Sprite[] characterSprites = new Sprite[4];

	private const int moveDistance = 1;
	private const int jumpDistance = 2;
	// Bitshifted values are used for casting rays on specific unity layers.
	/// <summary>
	/// Level layer may have: Empty Tiles, Rocks, Players, Holes and Water.
	/// </summary>
	private const int levelLayerBitShifted = 1 << 8;
	/// <summary>
	/// The only objects that can be on the log layer are: Logs and Players.
	/// </summary>
	private const int logLayerBitShifted = 1 << 9;
	private int currentRaycastLayer = 0;

	private const float playerRadius = 0.5f;
	private const float raycastDistance = 0.9f;
	private const float moveCooldown = 0.3f;
	private const float jumpCooldown = 0.6f;
	private const float punchCooldown = 4.0f;
	private const float stunLength = 2.0f;
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
	/// <summary>
	/// Used for checking tiles around the player.
	/// </summary>
	private RaycastHit2D rayOne = new RaycastHit2D();
	/// <summary>
	/// Used for checking tiles around objects hit by ray one.
	/// </summary>
	private RaycastHit2D rayTwo = new RaycastHit2D();

	/// <summary>
	/// Sets the player's sprite and gets a reference to the pause menu.
	/// </summary>
	private void Start() {
		playerSprite = characterSprites[PlayerPrefs.GetInt(controllerPrefix)];
		gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
		pauseMenuController = GameObject.FindGameObjectWithTag("PauseMenuController").GetComponent<PauseMenuController>();
	}

	/// <summary>
	/// Updates various timers and handles input to control the player.
	/// </summary>
	private void Update() {
		if (!pauseMenuController.isPaused) {
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
				// Change the player's sprite back to normal.
				gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
				stunTimer = stunLength;
			}

			HandleInput();
		}
	}

	private void HandleInput() {
		// Check input for moving.
		if ((Input.GetAxis(controllerPrefix + "Vertical") > 0.0f) && canMove) {
			HandleWalking(Directions.Up, Vector2.up);
		} else if ((Input.GetAxis(controllerPrefix + "Vertical") < 0.0f) && canMove) {
			HandleWalking(Directions.Down, Vector2.down);
		}

		if ((Input.GetAxis(controllerPrefix + "Horizontal") < 0.0f) && canMove) {
			HandleWalking(Directions.Left, Vector2.left);
		} else if ((Input.GetAxis(controllerPrefix + "Horizontal") > 0.0f) && canMove) {
			HandleWalking(Directions.Right, Vector2.right);
		}

		// Check input for jumping.
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

	/// <summary>
	/// Checks if the player can walk in the desired direction.
	/// </summary>
	/// <param name="rayDirection"> Enum case for a direction. </param>
	/// <param name="moveDirection"> The direction the player wants to move. </param>
	private void HandleWalking(Directions rayDirection, Vector2 moveDirection) {
		// Loops through each layer that's part of the level.
		for (int i = 0; i < 2; ++i) {
			if (i == 0) {
				currentRaycastLayer = logLayerBitShifted;
			} else if (i == 1) {
				currentRaycastLayer = levelLayerBitShifted;
			}

			// Cast ray to check what's in the tile upward of the player.
			rayOne = Raycast(transform, rayDirection, raycastDistance, currentRaycastLayer);

			if (CheckMoveDirection(rayOne)) {
				// Tile is avaiable to move onto, just need to check its type.
				if (CheckTileForLog(rayOne)) {
					Move(moveDirection, moveDistance, rayOne); // Move player onto the log.
					return;
				} else if (!rayOne.collider ||
					(rayOne.collider &&
					(rayOne.collider.CompareTag("Empty Tile") ||
					rayOne.collider.CompareTag("Danger") ||
					rayOne.collider.CompareTag("Move Position")))) {
					Move(moveDirection, moveDistance, rayOne);
					return;
				}
			} else if (rayOne.collider && rayOne.collider.CompareTag("Player")) {
				// Tile has a player on there so we must push them.
				if (Push(rayDirection, moveDirection)) {
					for (int j = 0; j < 2; ++j) {
						if (j == 0) {
							currentRaycastLayer = logLayerBitShifted;
						} else if (j == 1) {
							currentRaycastLayer = levelLayerBitShifted;
						}

						// We just pushed the player. Cast a ray to find out what they were standing on.
						rayOne = Raycast(transform, rayDirection, raycastDistance, currentRaycastLayer);

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

	/// <summary>
	/// Checks if the player can jump forward by their jump distance.
	/// </summary>
	private void HandleJumping() {
		// Must check each level layer for obstructions.
		for (int i = 0; i < 2; ++i) {
			if (i == 0) {
				currentRaycastLayer = logLayerBitShifted; // Check log layer for collisions first.
			} else if (i == 1) {
				currentRaycastLayer = levelLayerBitShifted;
			}

			// Cast ray to check what's in the tile upward of the player.
			rayOne = Raycast(transform, Directions.Up, raycastDistance, currentRaycastLayer);

			if (CheckMoveDirection(rayOne)) {
				if (Vector2.Distance(gameObject.transform.position, movePosition.transform.position) < moveDistance) {
					// Move the move position onto the tile ahead.
					movePosition.transform.Translate((Vector2.up), Space.Self);
				}

				// Now check the tile upward of the move position.
				for (int j = 0; j < 2; ++j) {
					if (j == 0) {
						currentRaycastLayer = logLayerBitShifted; // Check log layer first after casting second ray.
					} else if (j == 1) {
						currentRaycastLayer = levelLayerBitShifted;
					}

					// Cast a ray to check what's in the tile upward of the player's move position.
					rayTwo = Raycast(movePosition.transform, Directions.Up, raycastDistance, currentRaycastLayer);

					if (CheckMoveDirection(rayTwo)) {
						if (CheckTileForLog(rayTwo)) {
							Move(Vector2.up, jumpDistance, rayTwo);
							return;
						} else if (!rayTwo.collider ||
							(rayTwo.collider &&
							(rayTwo.collider.CompareTag("Empty Tile") ||
							rayTwo.collider.CompareTag("Danger") ||
							rayTwo.collider.CompareTag("Move Position")))) {
							Move(Vector2.up, jumpDistance, rayTwo);
							return;
						}
					} else if (rayTwo.collider && rayTwo.collider.CompareTag("Player")) {
						// Return because we don't want to jump onto a tile underneath the player on a different layer.
						return;
					}
				}
			} else if (rayOne.collider && rayOne.collider.CompareTag("Player")) {
				// Return because we don't want to jump over the player.
				return;
			}
		}
	}

	/// <summary>
	/// Checks what type of object a raycast has collided with.
	/// </summary>
	/// <param name="rayHit"> Data about the object hit by a ray. </param>
	/// <returns> True if the collided object can be moved onto by the player. </returns>
	private bool CheckMoveDirection(RaycastHit2D rayHit) {
		if (rayHit.collider) {
			if ((rayHit.collider.CompareTag("Empty Tile") ||
				rayHit.collider.CompareTag("Danger") ||
				rayHit.collider.CompareTag("Log") ||
				rayHit.collider.CompareTag("Move Position"))) {
				return true;
			} else {
				// Hit object is something we want to collide with not move onto or though.
				return false;
			}
		} else {
			return false;
		}
	}

	/// <summary>
	/// Checks if the object hit by a ray is a log.
	/// </summary>
	/// <param name="rayHit"> Data about the object hit by a ray. </param>
	/// <returns> True if the ray hit an object tagged with "Log". </returns>
	private bool CheckTileForLog(RaycastHit2D rayHit) {
		if (rayHit.collider && rayHit.collider.CompareTag("Log")) {
			return true;
		} else {
			return false;
		}
	}

	/// <summary>
	/// Moves the player onto a tile in the argument direction and by the argument distance.
	/// </summary>
	/// <param name="moveDirection"> The direction to move. </param>
	/// <param name="distance"> The distance to move. </param>
	/// <param name="rayHit"> Data about the object hit by a ray. </param>
	private void Move(Vector2 moveDirection, float distance, RaycastHit2D rayHit) {
		if (CheckTileForLog(rayHit)) {
			// If the player isn't on the log layer move them onto it, so they've don't collide
			// with the river hazard.
			if (gameObject.layer != rayHit.collider.gameObject.layer) {
				gameObject.layer = rayHit.collider.gameObject.layer;
			}

			// Set the player's parent to log.
			transform.SetParent(rayHit.collider.gameObject.transform);
		} else if (rayHit.collider) {
			// If the player isn't on the level layer move them onto it.
			if (gameObject.layer != rayHit.collider.gameObject.layer) {
				gameObject.layer = rayHit.collider.gameObject.layer;
			}

			// The player won't have a parent whilst on this layer.
			transform.SetParent(null);
		}

		// Moves the player from their current position.
		gameObject.transform.Translate(moveDirection * distance, Space.World);
		// Clamp the player's position to a .5 decimal number.
		gameObject.transform.position = ClampPosition(gameObject.transform.position);
		canMove = false;
		canJump = false;
		// Resets the player's move position to their position.
		movePosition.transform.localPosition = Vector2.zero;
	}

	/// <summary>
	/// Pushes a player along a desired move direction if the tile behind them is free.
	/// </summary>
	/// <param name="rayDirection"> Enum case for a direction. </param>
	/// <param name="moveDirection"> The desired direction to move. </param>
	/// <returns> True if a player has been pushed. </returns>
	private bool Push(Directions rayDirection, Vector2 moveDirection) {
		// Loops though each layer and checks for collisions.
		for (int i = 0; i < 2; ++i) {
			if (i == 0) {
				currentRaycastLayer = logLayerBitShifted;
			} else if (i == 1) {
				currentRaycastLayer = levelLayerBitShifted;
			}

			// Cast a ray, along the move direction, to check what's in the adjacent tile to the 
			// player we're wanting to push.
			rayTwo = Raycast(rayOne.collider.transform, rayDirection, raycastDistance, currentRaycastLayer);

			if (CheckMoveDirection(rayTwo)) {
				if (CheckTileForLog(rayTwo)) {
					GameObject pushObjectsParent = null;

					// Check if player to push has a parent.
					if (rayOne.collider.gameObject.transform.parent) {
						pushObjectsParent = rayOne.collider.gameObject.transform.parent.gameObject;
					}

					// Check if the player to push is on the log layer.
					if (rayOne.collider.gameObject.layer != rayTwo.collider.gameObject.layer) {
						rayOne.collider.gameObject.layer = rayTwo.collider.gameObject.layer;
					}

					// Set the log as the player to push's parent.
					rayOne.collider.gameObject.transform.SetParent(rayTwo.collider.transform);
					// Push the player onto the log.
					rayOne.collider.gameObject.transform.position = rayTwo.collider.transform.position;
					// Return true after pushing the player.
					return true;
				} else { // Push them onto an free tile.
					// Check that the pushed player is on the level layer.
					if (rayOne.collider.gameObject.layer != rayTwo.collider.gameObject.layer) {
						rayOne.collider.gameObject.layer = rayTwo.collider.gameObject.layer;
					}

					rayOne.transform.SetParent(null);
					// Move pushed player onto the empty tile.
					rayOne.transform.Translate(moveDirection * moveDistance, Space.World);
					// Return true after pushing player.
					return true;
				}

			} else if (rayTwo.collider && rayTwo.collider.CompareTag("Player")) {
				// Can't push player into another player, return false.
				return false;
			}
		}

		// Can't push player, return false.
		return false;
	}

	/// <summary>
	/// Casts a ray in the desired direction, from the player, to stun another player.
	/// </summary>
	/// <param name="punchDirection"> Enum case for a direction. </param>
	private void Punch(Directions punchDirection) {
		switch (punchDirection) {
			case Directions.Up: {
				// Cast a ray upward from the player to check for collisions on the log layer.
				rayOne = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, raycastDistance, logLayerBitShifted);

				if (rayOne.collider) {
					// Break after hitting an object, even if it's not a player.
					break;
				} else {
					// Cast a ray to check for collisions on the level layer.
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
			// Stun any player hit by the ray.
			rayOne.collider.gameObject.GetComponent<Player>().Stunned();
		}
	}

	/// <summary>
	/// Stuns the player and drops a coin, decreasing their score.
	/// </summary>
	public void Stunned() {
		stunned = true;
		gameObject.GetComponent<SpriteRenderer>().sprite = stunSprite;
		// Casts a ray to check which direction to drop a coin in.
		gameObject.GetComponent<TreasureBag>().CastRay();
	}

	/// <summary>
	/// Method for casting rays in world space.
	/// </summary>
	/// <param name="objectTransform"> The origin of the ray in the form of a transform. </param>
	/// <param name="rayDirection"> Direction to cast the ray. </param>
	/// <param name="distance"> Length of the ray. 1.0f = 1 Unity unit. </param>
	/// <param name="layer"> The raycasts target layer. </param>
	/// <returns> Data about the object hit by the ray. </returns>
	private RaycastHit2D Raycast(Transform objectTransform, Directions rayDirection, float distance, int layer) {
		switch (rayDirection) {
			case Directions.Up: {
				// Casts a ray up from the transform.
				return Physics2D.Raycast(new Vector2(objectTransform.position.x, objectTransform.position.y + playerRadius), Vector2.up, distance, layer);
			}
			case Directions.Left: {
				return Physics2D.Raycast(new Vector2(objectTransform.position.x - playerRadius, objectTransform.position.y), Vector2.left, distance, layer);
			}
			case Directions.Down: {
				return Physics2D.Raycast(new Vector2(objectTransform.position.x, objectTransform.position.y - playerRadius), Vector2.down, distance, layer);
			}
			case Directions.Right: {
				return Physics2D.Raycast(new Vector2(objectTransform.position.x + playerRadius, objectTransform.position.y), Vector2.right, distance, layer);
			}
			default: {
				return Physics2D.Raycast(new Vector2(objectTransform.position.x, objectTransform.position.y), Vector2.up, 0.0f, layer);
			}
		}
	}

	/// <summary>
	/// Clamps a Vector2's x & y coordinates to values ending in .5.
	/// </summary>
	/// <param name="position"> The position to clamp. </param>
	/// <returns> The Vector2 clamped. </returns>
	private Vector2 ClampPosition(Vector2 position) {
		// Gets two scalar values for comparing whether or not we need to clamp the player's x/y 
		// position up or down to the nearest float ending in .5.
		Vector2 normalisedVector = position.normalized;
		const float positionOffset = 0.5f;

		// NormalisedVector.x/y > 0.0f would mean we need to clamp up.
		if (normalisedVector.x > 0.0f) {
			// Casting a float to an integer truncates the number so we get a whole number.
			// Simply add/subtract positionOffset to allign the scalar value to the level's
			// tilemap.
			position.x = (int)position.x + positionOffset;
		} else if (normalisedVector.x <= 0.0f) {
			position.x = (int)position.x - positionOffset;
		}

		if (normalisedVector.y > 0.0f) {
			position.y = (int)position.y + positionOffset;
		} else if (normalisedVector.y <= 0.0f) {
			position.y = (int)position.y - positionOffset;
		}

		return position;
	}
}