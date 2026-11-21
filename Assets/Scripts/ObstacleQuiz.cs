using UnityEngine;

// Manages historical quiz interactions triggered when the player approaches an obstacle.
public class ObstacleQuiz : MonoBehaviour
{
    // Defines the historical question configuration displayed to the player.
    [Header("Historical Question")]

    // Stores the question text displayed in the quiz interface.
    // The TextArea attribute allows multi-line text editing in the Unity Inspector.
    [TextArea(3, 5)]
    public string question;

    // Defines the expected answer for the historical question.
    // True represents "Verdadero" and False represents "Falso".
    [Tooltip("True = Verdadero | False = Falso")]
    public bool correctAnswer;

    // Defines the external references required to manage the quiz interaction.
    [Header("References")]

    // Reference to the QuizManager responsible for displaying questions and processing answers.
    public QuizManager quizManager;

    // Indicates whether the player is currently within the obstacle's trigger area.
    private bool playerNearby;

    // Prevents the same obstacle quiz from being activated multiple times.
    private bool quizStarted;

    // Stores a reference to the PlayerMovement component of the player currently detected.
    private PlayerMovement currentPlayer;

    // Detects when another Collider2D enters the obstacle's trigger area.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions from objects that are not tagged as Player.
        if (!other.CompareTag("Player"))
            return;

        // Update the proximity state when the player enters the trigger area.
        playerNearby = true;

        // Retrieve the PlayerMovement component from the detected player GameObject.
        currentPlayer = other.GetComponent<PlayerMovement>();

        // Verify that the PlayerMovement component was successfully retrieved.
        if (currentPlayer != null)
        {
            // Register this obstacle as the player's current nearby obstacle.
            // This allows the player controller to activate the corresponding quiz.
            currentPlayer.SetNearbyObstacle(this);
        }
    }

    // Detects when another Collider2D exits the obstacle's trigger area.
    void OnTriggerExit2D(Collider2D other)
    {
        // Ignore collisions from objects that are not tagged as Player.
        if (!other.CompareTag("Player"))
            return;

        // Update the proximity state when the player leaves the trigger area.
        playerNearby = false;

        // Check whether a valid player reference is currently stored.
        if (currentPlayer != null)
        {
            // Remove this obstacle from the player's nearby obstacle reference.
            currentPlayer.ClearNearbyObstacle(this);

            // Clear the stored player reference after leaving the trigger area.
            currentPlayer = null;
        }
    }

    // Validates the interaction conditions and initializes the obstacle quiz.
    public void ActivateQuiz()
    {
        // Prevent quiz activation if the player is not nearby or the quiz has already started.
        if (!playerNearby || quizStarted)
            return;

        // Update the activation state to prevent duplicate quiz initialization.
        quizStarted = true;

        // Pass the question, expected answer, and obstacle GameObject to the QuizManager.
        // The QuizManager handles the quiz interface and the answer evaluation process.
        quizManager.StartQuiz(
            question,
            correctAnswer,
            gameObject
        );
    }
}
