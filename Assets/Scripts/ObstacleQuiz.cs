
using UnityEngine;

public class ObstacleQuiz : MonoBehaviour
{
    [Header("Historical Question")]
    [TextArea(3, 5)]
    public string question;

    [Tooltip("True = Verdadero | False = Falso")]
    public bool correctAnswer;

    [Header("References")]
    public QuizManager quizManager;

    private bool playerNearby;
    private bool quizStarted;

    private PlayerMovement currentPlayer;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = true;

        currentPlayer = other.GetComponent<PlayerMovement>();

        if (currentPlayer != null)
        {
            currentPlayer.SetNearbyObstacle(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;

        if (currentPlayer != null)
        {
            currentPlayer.ClearNearbyObstacle(this);
            currentPlayer = null;
        }
    }

    public void ActivateQuiz()
    {
        if (!playerNearby || quizStarted)
            return;

        quizStarted = true;

        quizManager.StartQuiz(
            question,
            correctAnswer,
            gameObject
        );
    }
}
