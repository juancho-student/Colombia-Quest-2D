
using UnityEngine;

public class ConquerorQuiz : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        [TextArea(3, 5)]
        public string question;

        [Tooltip("True = Verdadero | False = Falso")]
        public bool correctAnswer;
    }

    [Header("Quiz Settings")]
    public Question[] questions;

    [Header("References")]
    public QuizManager quizManager;
    public ConquerorHealth conquerorHealth;

    private int currentQuestionIndex = 0;
    private bool quizInProgress;

    private bool canActivate = true;


    public void ActivateQuiz()
    {
        if (!canActivate ||
            quizInProgress ||
            conquerorHealth == null ||
            conquerorHealth.IsDefeated ||
            questions == null ||
            questions.Length == 0)
        {
            return;
        }

        if (quizManager == null)
        {
            Debug.LogError("QuizManager reference is missing!");
            return;
        }

        canActivate = false;
        quizInProgress = true;

        Question selectedQuestion = questions[currentQuestionIndex];

        quizManager.StartConquerorQuiz(
            selectedQuestion.question,
            selectedQuestion.correctAnswer,
            conquerorHealth
        );

        currentQuestionIndex =
            (currentQuestionIndex + 1) % questions.Length;

        quizInProgress = false;
    }

    public void ResetActivation()
    {
        canActivate = true;
    }

}
