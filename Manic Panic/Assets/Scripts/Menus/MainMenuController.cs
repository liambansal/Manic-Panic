using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour {
	[SerializeField]
	private GameObject optionsScreen = null;
	[SerializeField]
	private GameObject controllerLayout = null;

	[SerializeField]
	private Button firstMenuButton = null;
	[SerializeField]
	private Button firstOptionsButton = null;
	[SerializeField]
	private Button firstControllerLayoutButton = null;

	// State of the controller layout overlay.
	private bool layoutActive = false;

	private EventSystem eventSystem = null;

	private void Start() {
		// Delete all "PlayerPrefs" keys and values whenever we visit the main menu so we refresh 
		// the stored player data.
		PlayerPrefs.DeleteAll();
		eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
	}

	/// <summary>
	/// Loads the next scene in the project's build index.
	/// </summary>
	private void PlayGame() {
		// Load the next scene in the build index.
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
	}

	/// <summary>
	/// Opens the options/settings screen and disables the menu buttons.
	/// </summary>
	private void OpenSettings() {
		// Activates the options/settings screen.
		optionsScreen.SetActive(true);
		// Select the options screen's first button.
		eventSystem.SetSelectedGameObject(firstOptionsButton.gameObject);
	}

	/// <summary>
	/// Loads the game's credits scene.
	/// </summary>
	private void LoadCredits() {
		SceneManager.LoadScene("Credits", LoadSceneMode.Single);
	}

	/// <summary>
	/// Toggles the controller layout overlay on or off depending on its current active state.
	/// </summary>
	private void ControllerLayout() {
		// Activate the controller layout overlay if it's inactive.
		if (!layoutActive) {
			controllerLayout.SetActive(true);
			layoutActive = true;
			// Select the controller layout overlay's first button.
			eventSystem.SetSelectedGameObject(firstControllerLayoutButton.gameObject);
		} else { // Disable it.
			controllerLayout.SetActive(false);
			layoutActive = false;
			// Select the options screen's first button.
			eventSystem.SetSelectedGameObject(firstOptionsButton.gameObject);
		}
	}

	/// <summary>
	/// Closes the options/settings screen and enables the menu buttons.
	/// </summary>
	private void Back() {
		// Hide the options/settings screen.
		optionsScreen.SetActive(false);
		// Select the first main menu button.
		eventSystem.SetSelectedGameObject(firstMenuButton.gameObject);
	}

	/// <summary>
	/// Closes the executeable application.
	/// </summary>
	private void QuitApplication() {
		Application.Quit();
	}
}