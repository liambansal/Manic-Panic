using UnityEngine;

/// <summary>
/// Manages a player's collected coins.
/// </summary>
public class TreasureBag : MonoBehaviour {
	[HideInInspector]
	public int coinsCollected { get; private set; } = 0;

	[SerializeField]
	private GameObject coin = null;

	private const int coinValue = 10;
	private const int dropDirections = 4;
	private const float playerRadius = 0.5f;
	private const float raycastDistance = 0.9f;
	private bool coinDropped = false;

	private Vector2[] raycastOrigins = new Vector2[4] {
		Vector2.up,
		Vector2.left,
		Vector2.down,
		Vector2.right
	};
	private Vector2[] adjacentTileDirections = new Vector2[4] {
		Vector2.up,
		Vector2.left,
		Vector2.down,
		Vector2.right
	};

	public GameObject scoreController = null;

	/// <summary>
	/// Gets the scene's score controller gameObject.
	/// </summary>
	private void Start() {
		scoreController = GameObject.FindGameObjectWithTag("ScoreController");
	}

	/// <summary>
	/// Increments the player's coin count and increases their score by the coin's value.
	/// </summary>
	private void PickupTreasure() {
		++coinsCollected;
		scoreController.GetComponent<ScoreController>().IncreaseScore(gameObject.name, coinValue);
	}

	/// <summary>
	/// Casts a ray towards each tile adjacent to the player, testing if its clear to instantiate a coin.
	/// </summary>
	public void CastRay() {
		if (coinsCollected > 0) {
			RaycastHit2D ray;
			// Gets a ray origin for each adjacent tile.
			raycastOrigins[0] = new Vector2(transform.position.x, transform.position.y + playerRadius);
			raycastOrigins[1] = new Vector2(transform.position.x - playerRadius, transform.position.y);
			raycastOrigins[2] = new Vector2(transform.position.x, transform.position.y - playerRadius);
			raycastOrigins[3] = new Vector2(transform.position.x + playerRadius, transform.position.y);

			// Loops through each adjacent tile direction.
			for (int i = 0; i < dropDirections;) {
				if (!coinDropped) {
					// Casts a ray from the adjacent tile's ray origin along its direction from the player.
					ray = Physics2D.Raycast(raycastOrigins[i], adjacentTileDirections[i], raycastDistance);

					if (ray.collider && (ray.collider.CompareTag("Empty Tile") || ray.collider.CompareTag("Hazard"))) {
						DropCoin(adjacentTileDirections[i]);
					}
				} else {
					coinDropped = false;
					return;
				}
			}
		}
	}

	/// <summary>
	/// Decreases the player's score and instantiates a coin in an adjacent tile.
	/// </summary>
	private void DropCoin(Vector3 dropDirection) {
		Instantiate(coin, transform.position + dropDirection, Quaternion.identity);
		--coinsCollected;
		scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, coinValue);
		coinDropped = true;
	}
}