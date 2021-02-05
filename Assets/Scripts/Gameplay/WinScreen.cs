using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Has the functionality to load the main menu.
/// </summary>
public class WinScreen : MonoBehaviour {
	private void LoadMainMenu() {
		SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
	}
}