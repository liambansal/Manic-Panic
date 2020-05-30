using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour {
	/// <summary>
	/// Loads the main menu scene.
	/// </summary>
	private void LoadMainMenu() {
		SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
	}
}