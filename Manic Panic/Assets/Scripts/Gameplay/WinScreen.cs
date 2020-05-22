using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
	private void ReloadLevel() {
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
