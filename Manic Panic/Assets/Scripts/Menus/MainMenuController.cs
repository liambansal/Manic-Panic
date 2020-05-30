using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

	/// <summary>
	/// Increase the scene build index by this amount.
	/// </summary>
	private const int indexAddition = 1;
	/// <summary>
	/// State of the controller layout overlay.
	/// </summary>
	private bool layoutActive = false;
	private EventSystem eventSystem = null;

	/// <summary>
	/// Deletes all "PlayerPrefs" keys and values to 'refresh' the stored player data and finds
	/// necessary unassigned variable objects.
	/// </summary>
	private void Start() {
		PlayerPrefs.DeleteAll();
		eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
	}

	/// <summary>
	/// Loads the next scene in the project's build index.
	/// </summary>
	private void PlayGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + indexAddition, LoadSceneMode.Single);
	}

	private void OpenSettings() {
		optionsScreen.SetActive(true);
		// Select the options screen's first button.
		eventSystem.SetSelectedGameObject(firstOptionsButton.gameObject);
	}

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
		} else {
			controllerLayout.SetActive(false);
			layoutActive = false;
			eventSystem.SetSelectedGameObject(firstOptionsButton.gameObject);
		}
	}

	/// <summary>
	/// Closes the options/settings screen.
	/// </summary>
	private void Back() {
		optionsScreen.SetActive(false);
		// Select the main menu's first button.
		eventSystem.SetSelectedGameObject(firstMenuButton.gameObject);
	}

	private void QuitApplication() {
		Application.Quit();
	}
}