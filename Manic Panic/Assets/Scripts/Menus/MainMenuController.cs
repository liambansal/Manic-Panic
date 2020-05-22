using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
	private void PlayGame() {
		// Load the next scene in the build index.
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	private void OpenSettings() {

	}

	private void LoadCredits() {
		SceneManager.LoadScene("Credits", LoadSceneMode.Single);
	}

	private void QuitApplication() {
		// Close the application.
		Application.Quit();
	}
}