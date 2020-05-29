using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
	private void ReloadLevel() {
		// Delete all "PlayerPrefs" keys because we won't be using them past this point 
		// and don't want them to interfere with other scenes once we leave this one.
		PlayerPrefs.DeleteAll();
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
