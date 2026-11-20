
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
    [Header("Sound Effects")]
    public AudioClip correctSound;

    private bool correctAnswer;
    private GameObject currentObstacle;
    private ConquerorHealth currentConqueror;

    private ConquerorPatrol currentConquerorPatrol;

    private bool quizActive;
    private bool isConquerorQuiz;

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

    // HISTORICAL OBSTACLE QUIZ
    public void StartQuiz(
        string question,
        bool answer,
        GameObject obstacle)
    {
        if (quizActive)
            return;

        quizActive = true;
        isConquerorQuiz = false;

        currentObstacle = obstacle;
        currentConqueror = null;
        correctAnswer = answer;

        ShowQuestion(question);
    }

    // CONQUEROR QUIZ
    public void StartConquerorQuiz(
        string question,
        bool answer,
        ConquerorHealth conqueror)
    {
        if (quizActive || conqueror == null || conqueror.IsDefeated)
            return;

        quizActive = true;
        isConquerorQuiz = true;

        currentConqueror = conqueror;
        currentObstacle = null;
        correctAnswer = answer;
        currentConquerorPatrol =
    conqueror.GetComponent<ConquerorPatrol>();

        if (currentConquerorPatrol != null)
        {
            currentConquerorPatrol.SetPatrolEnabled(false);
        }

        ShowQuestion(question);
    }

    // SHOW QUIZ PANEL
    void ShowQuestion(string question)
    {
        questionText.text = question;
        feedbackText.text = "";

        quizPanel.SetActive(true);

        playerMovement.SetMovementEnabled(false);

        Time.timeScale = 0f;
    }

    // CHECK ANSWER
    void CheckAnswer(bool selectedAnswer)
    {
        if (!quizActive)
            return;

        if (selectedAnswer == correctAnswer)
        {
            if (lifeManager.audioSource != null &&
            correctSound != null)
            {
                lifeManager.audioSource.PlayOneShot(correctSound);
            }
            if (isConquerorQuiz)
            {
                feedbackText.text = "¡Correcto! El Conqueror ha perdido un punto de vida.";

                currentConqueror.TakeDamage();
            }
            else
            {
                feedbackText.text = "¡Correcto! Obstáculo desbloqueado.";

                Destroy(currentObstacle);
            }

            CloseQuiz();
        }
        else
        {
            lifeManager.LoseLife(lifeManager.errorSound);

            if (lifeManager.CurrentLives <= 0)
                return;

            feedbackText.text =
                "¡Incorrecto! Has perdido una vida. Inténtalo nuevamente.";

            if (isConquerorQuiz)
            {
                CloseQuiz();
            }
        }
    }

    // CLOSE QUIZ

    void CloseQuiz()
    {
        // Remember which type of quiz is closing.
        bool wasConquerorQuiz = isConquerorQuiz;

        quizPanel.SetActive(false);
        quizActive = false;

        // Resume Conqueror patrol if it is still alive.
        if (currentConquerorPatrol != null &&
            currentConqueror != null &&
            !currentConqueror.IsDefeated)
        {
            currentConquerorPatrol.SetPatrolEnabled(true);
        }

        currentConquerorPatrol = null;
        currentObstacle = null;
        currentConqueror = null;

        Time.timeScale = 1f;

        playerMovement.SetMovementEnabled(true);

        // Separate the player after a Conqueror quiz.
        if (wasConquerorQuiz)
        {
            float pushDirection =
                playerMovement.transform.localScale.x >= 0 ? 1f : -1f;

            playerMovement.ApplyConquerorKnockback(
                new Vector2(pushDirection * 7f, 3f)
            );
        }

        isConquerorQuiz = false;
    }
}



