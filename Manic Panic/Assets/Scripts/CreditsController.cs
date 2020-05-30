using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {
	private float timeInScene = 0.0f;
	private const float sceneLength = 5.0f;

	/// <summary>
	/// Updates the time in this scene and loads the main menu after a set time.
	/// </summary>
	private void Update() {
		timeInScene += Time.deltaTime;

		if (timeInScene >= sceneLength) {
			const string mainMenuName = "Main Menu";
			SceneManager.LoadScene(mainMenuName, LoadSceneMode.Single);
		}
	}
}