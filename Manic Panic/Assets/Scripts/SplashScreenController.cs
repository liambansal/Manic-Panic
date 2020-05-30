using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour {
	float timeInScene = 0.0f;
	float sceneLength = 3.0f;

	void Start() {

	}

	void Update() {
		timeInScene += Time.deltaTime;

		if (timeInScene >= sceneLength) {
			SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
		}
	}
}