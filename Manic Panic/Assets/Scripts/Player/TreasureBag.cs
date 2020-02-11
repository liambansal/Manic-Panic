using System.Collections.Generic;
using UnityEngine;

public class TreasureBag : MonoBehaviour {
	public int CoinsCollected { get { return coinsCollected; } private set { } }

	[SerializeField]
	private GameObject coin = null;
	[SerializeField]
	private GameObject scoreController = null;

	private int coinsCollected = 0;
	
	private const float playerRadius = 0.5f;

	private void Start() {
		scoreController = GameObject.Find("Score Controller");
	}

	private void PickupTreasure() {
		++coinsCollected;
		scoreController.GetComponent<ScoreController>().IncreaseScore(gameObject.name, 10);
	}

	// TODO: Finish implementing this method.
	private void DropTreasure() {
		if (coinsCollected > 0) {
			RaycastHit2D ray;
			int direction = 0;

			for (int i = 0; i < 1;) {
				switch (direction) {
					case 1: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + playerRadius), Vector2.up);

						if (!ray.collider) {
							Instantiate(coin, new Vector2(transform.position.x, transform.position.y + 1), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
							++i;
						}

						break;
					}

					case 2: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x - playerRadius, transform.position.y), Vector2.left);

						if (!ray.collider) {
							Instantiate(coin, new Vector2(transform.position.x - 1, transform.position.y), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
							++i;
						}

						break;
					}

					case 3: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - playerRadius), Vector2.down);

						if (!ray.collider) {
							Instantiate(coin, new Vector2(transform.position.x, transform.position.y - 1), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
							++i;
						}

						break;
					}

					case 4: {
						ray = Physics2D.Raycast(new Vector2(transform.position.x + playerRadius, transform.position.y), Vector2.right);

						if (!ray.collider) {
							Instantiate(coin, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity);
							--coinsCollected;
							scoreController.GetComponent<ScoreController>().DecreaseScore(gameObject.name, 10);
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