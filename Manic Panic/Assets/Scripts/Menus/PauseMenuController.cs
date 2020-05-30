using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuController : MonoBehaviour {
	/// <summary>
	/// Should the game be unpaused?
	/// </summary>
	public bool isPaused { get; private set; } = false;

	[SerializeField]
	private GameObject pauseMenu = null;

	[SerializeField]
	private Button firstPauseMenuButton = null;

	private EventSystem eventSystem = null;

	private void Start() {
		eventSystem = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
	}

	private void Update() {
		if (Input.GetButtonDown("Pause")) {
			Pause();
		}
	}

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

	private void ResumeGame() {
		Pause();
	}

	/// <summary>
	/// Loads the main menu scene.
	/// </summary>
	private void LoadMainMenu() {
		SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
	}

	/// <summary>
	/// Closes the executeable application.
	/// </summary>
	private void QuitGame() {
		Application.Quit();
	}
}