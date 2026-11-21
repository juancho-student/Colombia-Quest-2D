using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Manages the quiz system, including question display, answer validation,
// player feedback, health management, and interactions with conqueror enemies.
public class QuizManager : MonoBehaviour
{
    // QUIZ INTERFACE

    // Defines the UI elements used to display and manage the quiz interface.
    [Header("Quiz Interface")]

    // Reference to the main UI panel containing the quiz elements.
    public GameObject quizPanel;

    // TextMeshPro component used to display the current quiz question.
    public TMP_Text questionText;

    // TextMeshPro component used to display feedback after answer submission.
    public TMP_Text feedbackText;

    // Button used by the player to submit a True answer.
    public Button trueButton;

    // Button used by the player to submit a False answer.
    public Button falseButton;


    // GAME REFERENCES

    // Defines the external references required for quiz and gameplay management.
    [Header("Game References")]

    // Reference to the LifeManager responsible for managing the player's health.
    public LifeManager lifeManager;

    // Reference to the PlayerMovement component used to control player movement.
    public PlayerMovement playerMovement;


    // SOUND EFFECTS

    // Defines the audio resources associated with quiz interactions.
    [Header("Sound Effects")]

    // Audio clip played when the player submits a correct answer.
    public AudioClip correctSound;


    // QUIZ STATE VARIABLES

    // Stores the expected correct answer for the currently active question.
    private bool correctAnswer;

    // Stores the reference to the obstacle associated with the current quiz.
    private GameObject currentObstacle;

    // Stores the reference to the conqueror whose health is affected by the quiz result.
    private ConquerorHealth currentConqueror;

    // Stores the patrol component associated with the current conqueror.
    private ConquerorPatrol currentConquerorPatrol;

    // Indicates whether a quiz interaction is currently active.
    private bool quizActive;

    // Identifies whether the current quiz belongs to a conqueror or a historical obstacle.
    private bool isConquerorQuiz;


    // INITIALIZATION

    // Initializes the quiz interface and registers the answer button event listeners.
    void Start()
    {
        // Hide the quiz panel when the scene starts.
        quizPanel.SetActive(false);

        // Register a callback that submits a True answer when the corresponding button is clicked.
        trueButton.onClick.AddListener(
            () => CheckAnswer(true)
        );

        // Register a callback that submits a False answer when the corresponding button is clicked.
        falseButton.onClick.AddListener(
            () => CheckAnswer(false)
        );
    }


    // HISTORICAL OBSTACLE QUIZ

    // Initializes a quiz associated with a historical obstacle.
    // Receives the question text, expected answer, and obstacle reference.
    public void StartQuiz(
        string question,
        bool answer,
        GameObject obstacle)
    {
        // Prevent the initialization of a new quiz while another quiz is active.
        if (quizActive)
            return;

        // Set the quiz state to active.
        quizActive = true;

        // Identify the current interaction as a historical obstacle quiz.
        isConquerorQuiz = false;

        // Store the reference to the obstacle associated with the question.
        currentObstacle = obstacle;

        // Clear any previously stored conqueror reference.
        currentConqueror = null;

        // Store the expected correct answer for validation.
        correctAnswer = answer;

        // Display the selected question and activate the quiz interface.
        ShowQuestion(question);
    }


    // CONQUEROR QUIZ

    // Initializes a quiz associated with a conqueror enemy.
    // Receives the question text, expected answer, and conqueror health reference.
    public void StartConquerorQuiz(
        string question,
        bool answer,
        ConquerorHealth conqueror)
    {
        // Prevent quiz initialization if another quiz is active,
        // the conqueror reference is missing, or the enemy has already been defeated.
        if (quizActive || conqueror == null || conqueror.IsDefeated)
            return;

        // Set the quiz state to active.
        quizActive = true;

        // Identify the current interaction as a conqueror quiz.
        isConquerorQuiz = true;

        // Store the reference to the conqueror associated with the question.
        currentConqueror = conqueror;

        // Clear any previously stored obstacle reference.
        currentObstacle = null;

        // Store the expected correct answer for validation.
        correctAnswer = answer;

        // Retrieve the patrol component attached to the conqueror GameObject.
        currentConquerorPatrol =
            conqueror.GetComponent<ConquerorPatrol>();

        // Check whether the patrol component is available.
        if (currentConquerorPatrol != null)
        {
            // Temporarily disable the conqueror's patrol movement during the quiz.
            currentConquerorPatrol.SetPatrolEnabled(false);
        }

        // Display the selected question and activate the quiz interface.
        ShowQuestion(question);
    }


    // SHOW QUIZ PANEL

    // Displays the question and prepares the game state for player interaction.
    void ShowQuestion(string question)
    {
        // Assign the selected question to the question text UI element.
        questionText.text = question;

        // Clear any feedback message from the previous quiz interaction.
        feedbackText.text = "";

        // Activate the quiz panel to make the interface visible.
        quizPanel.SetActive(true);

        // Disable player movement while the quiz interface is active.
        playerMovement.SetMovementEnabled(false);

        // Pause the game by setting the global time scale to zero.
        Time.timeScale = 0f;
    }


    // CHECK ANSWER

    // Validates the player's selected answer and executes the corresponding gameplay response.
    void CheckAnswer(bool selectedAnswer)
    {
        // Ignore answer submissions if there is no active quiz.
        if (!quizActive)
            return;

        // Compare the player's selected answer with the expected correct answer.
        if (selectedAnswer == correctAnswer)
        {
            // Play the correct answer sound if the required audio references are available.
            if (lifeManager.audioSource != null &&
                correctSound != null)
            {
                lifeManager.audioSource.PlayOneShot(correctSound);
            }

            // Process the correct answer according to the quiz type.
            if (isConquerorQuiz)
            {
                // Display feedback indicating that the conqueror has lost one health point.
                feedbackText.text = "¡Correcto! El Conqueror ha perdido un punto de vida.";

                // Apply damage to the current conqueror.
                currentConqueror.TakeDamage();
            }
            else
            {
                // Display feedback indicating that the historical obstacle has been unlocked.
                feedbackText.text = "¡Correcto! Obstáculo desbloqueado.";

                // Remove the correctly answered obstacle from the active scene.
                Destroy(currentObstacle);
            }

            // Close the quiz and restore normal gameplay.
            CloseQuiz();
        }
        else
        {
            // Remove one player life and play the configured incorrect answer sound.
            lifeManager.LoseLife(lifeManager.errorSound);

            // Stop further processing if the player has lost all available lives.
            if (lifeManager.CurrentLives <= 0)
                return;

            // Display feedback indicating that the selected answer was incorrect.
            feedbackText.text =
                "¡Incorrecto! Has perdido una vida. Inténtalo nuevamente.";

            // Close the quiz after an incorrect answer when interacting with a conqueror.
            if (isConquerorQuiz)
            {
                CloseQuiz();
            }
        }
    }


    // CLOSE QUIZ

    // Closes the quiz interface, restores gameplay, and handles post-quiz interactions.
    void CloseQuiz()
    {
        // Store the current quiz type before resetting the quiz state.
        // This value determines whether knockback must be applied to the player.
        bool wasConquerorQuiz = isConquerorQuiz;

        // Hide the quiz interface.
        quizPanel.SetActive(false);

        // Update the quiz state to indicate that no quiz is currently active.
        quizActive = false;

        // Check whether the current conqueror and its patrol component are still valid.
        // Resume patrol movement only if the enemy has not been defeated.
        if (currentConquerorPatrol != null &&
            currentConqueror != null &&
            !currentConqueror.IsDefeated)
        {
            // Re-enable the conqueror's patrol behavior after the quiz interaction.
            currentConquerorPatrol.SetPatrolEnabled(true);
        }

        // Clear the stored patrol component reference.
        currentConquerorPatrol = null;

        // Clear the stored obstacle reference.
        currentObstacle = null;

        // Clear the stored conqueror reference.
        currentConqueror = null;

        // Restore the normal game time scale and resume gameplay.
        Time.timeScale = 1f;

        // Re-enable player movement and input processing.
        playerMovement.SetMovementEnabled(true);

        // Apply knockback only if the completed interaction involved a conqueror.
        if (wasConquerorQuiz)
        {
            // Determine the horizontal push direction based on the player's current orientation.
            // A positive value pushes the player to the right,
            // while a negative value pushes the player to the left.
            float pushDirection =
                playerMovement.transform.localScale.x >= 0 ? 1f : -1f;

            // Apply a horizontal and vertical impulse to separate the player from the conqueror.
            playerMovement.ApplyConquerorKnockback(
                new Vector2(pushDirection * 7f, 3f)
            );
        }

        // Reset the quiz type state after completing the closing procedure.
        isConquerorQuiz = false;
    }
}


