
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Quiz Interface")]
    public GameObject quizPanel;
    public TMP_Text questionText;
    public TMP_Text feedbackText;

    public Button trueButton;
    public Button falseButton;

    [Header("Game References")]
    public LifeManager lifeManager;
    public PlayerMovement playerMovement;

    private bool correctAnswer;
    private GameObject currentObstacle;

    private bool quizActive;

    void Start()
    {
        quizPanel.SetActive(false);

        trueButton.onClick.AddListener(
            () => CheckAnswer(true)
        );

        falseButton.onClick.AddListener(
            () => CheckAnswer(false)
        );
    }

    public void StartQuiz(
        string question,
        bool answer,
        GameObject obstacle)
    {
        if (quizActive)
            return;

        quizActive = true;

        currentObstacle = obstacle;
        correctAnswer = answer;

        questionText.text = question;
        feedbackText.text = "";

        quizPanel.SetActive(true);

        playerMovement.SetMovementEnabled(false);

        Time.timeScale = 0f;
    }

    void CheckAnswer(bool selectedAnswer)
    {
        if (!quizActive)
            return;

        if (selectedAnswer == correctAnswer)
        {
            feedbackText.text = "¡Correcto! Obstáculo desbloqueado.";

            Destroy(currentObstacle);

            CloseQuiz();
        }
        else
        {
            lifeManager.LoseLife();

            // Check if the level is about to restart.
            if (lifeManager.CurrentLives <= 0)
                return;

            feedbackText.text =
                "¡Incorrecto! Has perdido una vida. Inténtalo nuevamente.";
        }
    }

    void CloseQuiz()
    {
        quizPanel.SetActive(false);

        quizActive = false;

        currentObstacle = null;

        Time.timeScale = 1f;

        playerMovement.SetMovementEnabled(true);
    }
}
