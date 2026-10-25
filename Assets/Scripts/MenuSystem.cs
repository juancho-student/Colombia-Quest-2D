using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manages the main menu functionality.
public class MenuSystem : MonoBehaviour
{
    // Starts the game by loading the next scene in the Build Settings.
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Exits the application.
    public void QuitGame()
    {
        // Displays a message in the Unity Console.
        Debug.Log("Exiting Game");

        // Closes the application.
        Application.Quit();
    }
}
