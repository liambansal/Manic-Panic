using System.Collections.Generic;
using UnityEngine;

public class TreasureBag : MonoBehaviour {
	public int CoinsCollected { get { return coinsCollected; } private set { } }

	[SerializeField]
	private GameObject coin = null;

	private int coinsCollected = 0;
	
	private const float playerRadius = 0.5f;
	private const float raycastDistance = 0.9f;
	private const float dropDistance = 1.0f;

	public GameObject scoreController = null;

	private void Start() {
		scoreController = GameObject.FindGameObjectWithTag("ScoreController");
	}

	private void PickupTreasure() {
		++coinsCollected;
		scoreController.GetComponent<ScoreController>().IncreaseScore(gameObject.name, 10);
	}
	
	public void DropTreasure() {
		if (coinsCollected > 0) {
			RaycastHit2D ray;

			for (int i = 1; i <= 4;) {
				switch (i) {
					case 1: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up, raycastDistance);

						if (!ray.collider || (ray.collider && (ray.collider.CompareTag("Empty Tile") || ray.collider.CompareTag("Hazard")))) {
							Instantiate(coin, new Vector2(transform.position.x, transform.position.y + dropDistance), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
							i += 4;
						} else {
							++i;
						}

						break;
					}

					case 2: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left, raycastDistance);

						if (!ray.collider || (ray.collider && (ray.collider.CompareTag("Empty Tile") || ray.collider.CompareTag("Hazard")))) {
							Instantiate(coin, new Vector2(transform.position.x - dropDistance, transform.position.y), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
							i += 4;
						} else {
							++i;
						}

						break;
					}

					case 3: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down, raycastDistance);

						if (!ray.collider || (ray.collider && (ray.collider.CompareTag("Empty Tile") || ray.collider.CompareTag("Hazard")))) {
							Instantiate(coin, new Vector2(transform.position.x, transform.position.y - dropDistance), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
							i += 4;
						} else {
							++i;
						}

						break;
					}

					case 4: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right, raycastDistance);

						if (!ray.collider || (ray.collider && (ray.collider.CompareTag("Empty Tile") || ray.collider.CompareTag("Hazard")))) {
							Instantiate(coin, new Vector2(transform.position.x + dropDistance, transform.position.y), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
							i += 4;
						} else {
							++i;
						}

						break;
					}

					default: {
						break;
					}
				}
			}
		}
	}
}