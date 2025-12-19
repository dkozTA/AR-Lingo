using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Quiz Question Data Structure
/// </summary>
[System.Serializable]
public class QuizQuestion
{
    [Header("Visual")]
    public Sprite image;
    
    [Header("Question Text")]
    public string questionEn;
    public string questionVi;
    
    [Header("Answers")]
    public string[] answers = new string[4];
    public int correctIndex;
    
    [Header("Reference")]
    public string wordID;
    
    [Header("Hint Audio")]
    public AudioClip hintSound; // Animal sound for hint
}

/// <summary>
/// Quiz Manager - Handles quiz gameplay
/// </summary>
public class QuizManager : MonoBehaviour
{
    [Header("UI - Question Card")]
    public Image imageAnimal;
    public TextMeshProUGUI textProgress;
    public TextMeshProUGUI textQuestionEn;
    public TextMeshProUGUI textQuestionVi;
    public Button hintButton;

    [Header("UI - Answer Buttons")]
    public Button[] answerButtons;

    [Header("Quiz Data")]
    public QuizQuestion[] questions;
    public WordDatabase wordDatabase;

    [Header("Audio")]
    // REMOVED: No longer need local audioSource for hints
    // public AudioSource audioSource;
    public AudioClip soundCorrect;
    public AudioClip soundWrong;

    [Header("Settings")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;
    public Color defaultColor = Color.white;
    public float nextQuestionDelay = 2f;

    [Header("Auto-Generate Settings")]
    public bool autoGenerateOnStart = false;
    public int numberOfQuestions = 10;

    [Header("Hint Settings")]
    public int maxHintsPerQuestion = 3;
    public float hintCooldown = 2f;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    private int currentQuestionIndex = 0;
    private bool isAnswered = false;
    private int correctAnswersCount = 0;
    private Image[] buttonImages;
    private int hintsUsedThisQuestion = 0;
    private float lastHintTime = 0f;

    void Start()
    {
        // REMOVED: No longer create local audioSource
        // if (audioSource == null)
        // {
        //     audioSource = gameObject.AddComponent<AudioSource>();
        //     audioSource.playOnAwake = false;
        // }

        CacheButtonImages();

        if (autoGenerateOnStart && wordDatabase != null)
        {
            GenerateQuizFromDatabase(numberOfQuestions);
        }

        ResetQuiz();
    }

    void OnEnable()
    {
        if (autoGenerateOnStart && wordDatabase != null && (questions == null || questions.Length == 0))
        {
            GenerateQuizFromDatabase(numberOfQuestions);
        }

        ResetQuiz();
    }

    void CacheButtonImages()
    {
        buttonImages = new Image[answerButtons.Length];
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                buttonImages[i] = answerButtons[i].GetComponent<Image>();
            }
        }
    }

    public void ResetQuiz()
    {
        currentQuestionIndex = 0;
        correctAnswersCount = 0;
        isAnswered = false;

        if (questions != null && questions.Length > 0)
        {
            ShowQuestion();
        }
        else
        {
            Debug.LogWarning("[QuizManager] No questions available!");
        }
    }

    void ShowQuestion()
    {
        if (questions == null || questions.Length == 0)
        {
            Debug.LogError("[QuizManager] No questions to show!");
            return;
        }

        if (currentQuestionIndex >= questions.Length)
        {
            ShowQuizComplete();
            return;
        }

        QuizQuestion q = questions[currentQuestionIndex];
        isAnswered = false;
        hintsUsedThisQuestion = 0;

        if (imageAnimal != null && q.image != null)
        {
            imageAnimal.sprite = q.image;
            imageAnimal.gameObject.SetActive(true);
        }
        else if (imageAnimal != null)
        {
            imageAnimal.gameObject.SetActive(false);
        }

        if (textProgress != null)
        {
            textProgress.text = $"Question {currentQuestionIndex + 1} of {questions.Length}";
        }

        if (textQuestionEn != null)
        {
            textQuestionEn.text = q.questionEn;
        }

        if (textQuestionVi != null)
        {
            textQuestionVi.text = q.questionVi;
        }

        if (hintButton != null)
        {
            hintButton.interactable = true;
            hintButton.gameObject.SetActive(true);
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;

            answerButtons[i].interactable = true;
            
            if (buttonImages[i] != null)
            {
                buttonImages[i].color = defaultColor;
            }

            TextMeshProUGUI buttonText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null && i < q.answers.Length)
            {
                buttonText.text = q.answers[i];
            }

            answerButtons[i].gameObject.SetActive(true);
        }

        if (enableDebugLogs)
        {
            Debug.Log($"[QuizManager] Showing question {currentQuestionIndex + 1}: {q.questionEn}");
        }
    }

    // FIXED: Use AudioManager for hint sound
    public void OnHintButtonClicked()
    {
        if (isAnswered) return;
        if (questions == null || currentQuestionIndex >= questions.Length) return;

        if (Time.time - lastHintTime < hintCooldown)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[QuizManager] Hint on cooldown. Wait {hintCooldown - (Time.time - lastHintTime):F1}s");
            }
            return;
        }

        if (hintsUsedThisQuestion >= maxHintsPerQuestion)
        {
            if (enableDebugLogs)
            {
                Debug.Log($"[QuizManager] Maximum hints ({maxHintsPerQuestion}) used for this question");
            }
            
            if (hintButton != null)
            {
                hintButton.interactable = false;
            }
            return;
        }

        QuizQuestion q = questions[currentQuestionIndex];

        // FIXED: Use AudioManager's SFX channel instead of local audioSource
        if (q.hintSound != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(q.hintSound, false);
                hintsUsedThisQuestion++;
                lastHintTime = Time.time;

                if (enableDebugLogs)
                {
                    Debug.Log($"[QuizManager] Played hint sound ({hintsUsedThisQuestion}/{maxHintsPerQuestion})");
                }

                if (hintsUsedThisQuestion >= maxHintsPerQuestion && hintButton != null)
                {
                    hintButton.interactable = false;
                }
            }
            else
            {
                Debug.LogError("[QuizManager] AudioManager not found!");
            }
        }
        else
        {
            Debug.LogWarning("[QuizManager] No hint sound available for this question");
        }
    }

    public void OnAnswerClicked(int answerIndex)
    {
        if (isAnswered) return;
        if (questions == null || currentQuestionIndex >= questions.Length) return;
        if (answerIndex < 0 || answerIndex >= answerButtons.Length) return;

        QuizQuestion q = questions[currentQuestionIndex];
        bool isCorrect = (answerIndex == q.correctIndex);

        isAnswered = true;

        if (hintButton != null)
        {
            hintButton.interactable = false;
        }

        foreach (Button btn in answerButtons)
        {
            if (btn != null) btn.interactable = false;
        }

        if (isCorrect)
        {
            if (buttonImages[answerIndex] != null)
            {
                buttonImages[answerIndex].color = correctColor;
            }

            correctAnswersCount++;

            // FIXED: Use AudioManager for correct sound
            if (soundCorrect != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(soundCorrect, false);
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[QuizManager] ✓ Correct! Score: {correctAnswersCount}/{currentQuestionIndex + 1}");
            }

            Invoke(nameof(NextQuestion), nextQuestionDelay);
        }
        else
        {
            if (buttonImages[answerIndex] != null)
            {
                buttonImages[answerIndex].color = wrongColor;
            }

            if (q.correctIndex >= 0 && q.correctIndex < buttonImages.Length && buttonImages[q.correctIndex] != null)
            {
                buttonImages[q.correctIndex].color = correctColor;
            }

            // FIXED: Use AudioManager for wrong sound
            if (soundWrong != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(soundWrong, false);
            }

            if (enableDebugLogs)
            {
                Debug.Log($"[QuizManager] ✗ Wrong! Correct answer was: {q.answers[q.correctIndex]}");
            }

            Invoke(nameof(NextQuestion), nextQuestionDelay);
        }
    }

    void NextQuestion()
    {
        currentQuestionIndex++;
        ShowQuestion();
    }

    void ShowQuizComplete()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[QuizManager] Quiz Complete! Score: {correctAnswersCount}/{questions.Length}");
        }

        if (textQuestionEn != null)
        {
            textQuestionEn.text = "Quiz Complete!";
        }

        if (textQuestionVi != null)
        {
            textQuestionVi.text = $"Điểm: {correctAnswersCount}/{questions.Length}";
        }

        if (textProgress != null)
        {
            float percentage = (float)correctAnswersCount / questions.Length * 100f;
            textProgress.text = $"{percentage:F0}%";
        }

        if (imageAnimal != null)
        {
            imageAnimal.gameObject.SetActive(false);
        }

        if (hintButton != null)
        {
            hintButton.gameObject.SetActive(false);
        }

        foreach (Button btn in answerButtons)
        {
            if (btn != null) btn.gameObject.SetActive(false);
        }
    }

    public void GenerateQuizFromDatabase(int questionCount = 10)
    {
        if (wordDatabase == null)
        {
            Debug.LogError("[QuizManager] WordDatabase not assigned!");
            return;
        }

        List<WordData> allWords = wordDatabase.GetAllWords();
        if (allWords == null || allWords.Count < 4)
        {
            Debug.LogError("[QuizManager] Not enough words in database to generate quiz! Need at least 4 words.");
            return;
        }

        List<QuizQuestion> generatedQuestions = new List<QuizQuestion>();

        List<WordData> shuffled = allWords.OrderBy(x => Random.value).ToList();
        int count = Mathf.Min(questionCount, shuffled.Count);

        for (int i = 0; i < count; i++)
        {
            WordData word = shuffled[i];
            QuizQuestion q = new QuizQuestion();

            q.wordID = word.id;
            q.image = word.icon2D;
            q.questionEn = "What is this animal?";
            q.questionVi = "đây là con gì?";
            q.hintSound = word.audioSFX;

            List<string> answerOptions = new List<string>();
            answerOptions.Add(word.englishName);

            List<WordData> wrongAnswers = allWords
                .Where(w => w.id != word.id)
                .OrderBy(x => Random.value)
                .Take(3)
                .ToList();

            foreach (var wrongWord in wrongAnswers)
            {
                answerOptions.Add(wrongWord.englishName);
            }

            answerOptions = answerOptions.OrderBy(x => Random.value).ToList();

            q.answers = answerOptions.ToArray();
            q.correctIndex = answerOptions.IndexOf(word.englishName);

            generatedQuestions.Add(q);
        }

        questions = generatedQuestions.ToArray();

        if (enableDebugLogs)
        {
            Debug.Log($"[QuizManager] Generated {questions.Length} questions from database");
        }

        if (gameObject.activeInHierarchy)
        {
            ResetQuiz();
        }
    }
}