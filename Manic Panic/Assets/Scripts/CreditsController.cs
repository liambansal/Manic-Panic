using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {
	private float timeInScene = 0.0f;
	private float sceneLength = 6.0f;

	private void Update() {
		timeInScene += Time.deltaTime;

		// Once we've been in the scene long enough...
		if (timeInScene >= sceneLength) {
			// ...load the main menu.
			SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
		}
	}
}