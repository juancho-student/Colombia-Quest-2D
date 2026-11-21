using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class VictoryManager : MonoBehaviour
{
    private const string LastLevelKey = "LastLevelBuildIndex";

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void RestartLevel()
    {
        int lastLevel = PlayerPrefs.GetInt(LastLevelKey, 1);

        SceneManager.LoadScene(lastLevel);
    }

    public void ContinueLevel()
    {
        int lastLevel = PlayerPrefs.GetInt(LastLevelKey, 1);
        int nextLevel = lastLevel + 1;

        if (nextLevel < SceneManager.sceneCountInBuildSettings)
        {
            string nextScenePath =
                SceneUtility.GetScenePathByBuildIndex(nextLevel);

            string nextSceneName =
                Path.GetFileNameWithoutExtension(nextScenePath);

            if (nextSceneName != "Victory" && nextSceneName != "Menu")
            {
                SceneManager.LoadScene(nextLevel);
                return;
            }
        }

        // Si no hay otro nivel jugable, regresar al menú.
        GoToMenu();
    }
}
