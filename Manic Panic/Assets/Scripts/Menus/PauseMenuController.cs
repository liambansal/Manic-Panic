using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour {
	[HideInInspector]
	public bool isPaused { get; private set; } = false;

	[SerializeField]
	private GameObject pauseMenu = null;
	[SerializeField]
	private Button firstPauseMenuButton = null;

	private EventSystem eventSystem = null;

	/// <summary>
	/// Gets the scene's event system.
	/// </summary>
	private void Start() {
		eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
	}

	/// <summary>
	/// Checks if a player wants to pause/unpause the game.
	/// </summary>
	private void Update() {
		if (Input.GetButtonDown("Pause")) {
			Pause();
		}
	}

	/// <summary>
	/// Pauses or un-pauses the game based on its current pause state.
	/// </summary>
	private void Pause() {
		if (!isPaused) {
			pauseMenu.SetActive(true);
			Time.timeScale = 0.0f;
			isPaused = true;
			eventSystem.SetSelectedGameObject(firstPauseMenuButton.gameObject);
		} else {
			pauseMenu.SetActive(false);
			Time.timeScale = 1.0f;
			isPaused = false;
		}
	}

	private void LoadMainMenu() {
		// Set the default time scale in case we're paused.
		Time.timeScale = 1.0f;
		const string mainMenuName = "Main Menu";
		SceneManager.LoadScene(mainMenuName, LoadSceneMode.Single);
	}

	private void QuitGame() {
		Application.Quit();
	}
}