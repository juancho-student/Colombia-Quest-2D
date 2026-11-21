using UnityEngine;

// Manages the quiz interaction associated with a conqueror enemy.
public class ConquerorQuiz : MonoBehaviour
{
    // Defines a serializable data structure for storing individual quiz questions.
    [System.Serializable]
    public class Question
    {
        // Displays a multi-line text area in the Unity Inspector for entering the question.
        [TextArea(3, 5)]
        public string question;

        // Specifies the expected answer for the question.
        // True represents "Verdadero" and False represents "Falso".
        [Tooltip("True = Verdadero | False = Falso")]
        public bool correctAnswer;
    }

    // Defines the configuration parameters for the quiz system.
    [Header("Quiz Settings")]

    // Stores the collection of questions assigned to the conqueror.
    public Question[] questions;

    // Defines the external references required to manage the quiz interaction.
    [Header("References")]

    // Reference to the QuizManager responsible for displaying questions and processing answers.
    public QuizManager quizManager;

    // Reference to the conqueror's health component used to manage damage and defeat status.
    public ConquerorHealth conquerorHealth;

    // Tracks the index of the next question to be displayed.
    private int currentQuestionIndex = 0;

    // Indicates whether a quiz interaction is currently being processed.
    private bool quizInProgress;

    // Controls whether the quiz can be activated by an external interaction.
    private bool canActivate = true;

    // Validates the quiz conditions and initiates the next question.
    public void ActivateQuiz()
    {
        // Prevent quiz activation if the interaction is disabled, another quiz is active,
        // the conqueror reference is missing, the enemy is defeated,
        // or no questions have been configured.
        if (!canActivate ||
            quizInProgress ||
            conquerorHealth == null ||
            conquerorHealth.IsDefeated ||
            questions == null ||
            questions.Length == 0)
        {
            return;
        }

        // Validate the QuizManager reference before starting the quiz.
        if (quizManager == null)
        {
            // Display an error message if the QuizManager reference is missing.
            Debug.LogError("QuizManager reference is missing!");
            return;
        }

        // Disable further activation to prevent duplicate quiz interactions.
        canActivate = false;

        // Set the quiz state to indicate that the interaction has started.
        quizInProgress = true;

        // Retrieve the question corresponding to the current question index.
        Question selectedQuestion = questions[currentQuestionIndex];

        // Pass the selected question, its correct answer, and the conqueror health reference
        // to the QuizManager to initialize the quiz interface and interaction.
        quizManager.StartConquerorQuiz(
            selectedQuestion.question,
            selectedQuestion.correctAnswer,
            conquerorHealth
        );

        // Advance the question index to select the next question during the following interaction.
        // The modulo operator resets the index to zero when the end of the array is reached.
        currentQuestionIndex =
            (currentQuestionIndex + 1) % questions.Length;

        // Reset the quiz processing state after the quiz initialization call.
        quizInProgress = false;
    }

    // Re-enables quiz activation after the previous interaction has been completed.
    public void ResetActivation()
    {
        // Allow the quiz to be triggered again.
        canActivate = true;
    }
}
