using UnityEngine;

public class PlayerStunned : MonoBehaviour {
	private const float stunLength = 2.0f;
	private float stunTimer = 2.0f;

	private bool stunned = false;

	[SerializeField]
	private Sprite uiBox = null;
	[SerializeField]
	private Sprite playerSprite = null;

	private void Update() {
		if (stunned) {
			stunTimer -= Time.deltaTime;
			// TODO: Play stun animation. + Enable movement once animation has finished.
		}

		if (stunTimer <= 0) {
			stunned = false;
			gameObject.SendMessage("EnableMovement");
			gameObject.GetComponent<SpriteRenderer>().sprite = playerSprite;
			stunTimer = stunLength;
		}
	}

	private void Stunned() {
		stunned = true;
		gameObject.SendMessage("DisableMovement");
		gameObject.GetComponent<SpriteRenderer>().sprite = uiBox; // TODO: delete line once animation is in place
		gameObject.SendMessage("DropTreasure");
	}
}