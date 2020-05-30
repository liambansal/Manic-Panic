using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour {
	private const float sceneLength = 3.0f;
	private float timeInScene = 0.0f;

	/// <summary>
	/// Updates the time spent in this scene and loads the main menu after a set time.
	/// </summary>
	private void Update() {
		timeInScene += Time.deltaTime;

		if (timeInScene >= sceneLength) {
			const string mainMenuName = "Main Menu";
			SceneManager.LoadScene(mainMenuName, LoadSceneMode.Single);
		}
	}
}