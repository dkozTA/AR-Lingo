using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class QuizQuestion
{
    public Sprite image;
    public string questionEn;
    public string questionVi;
    public string[] answers = new string[4];
    public int correctIndex;
}

public class QuizManager : MonoBehaviour
{
    [Header("UI")]
    public Image imageAnimal;
    public TextMeshProUGUI textProgress;
    public TextMeshProUGUI textQuestionEn;
    public TextMeshProUGUI textQuestionVi;
    public Button[] answerButtons;

    [Header("Data")]
    public QuizQuestion[] questions;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip soundCorrect;
    public AudioClip soundWrong;

    private int currentIndex = 0;
    private Color defaultButtonColor;
    private bool isAnswered = false;

    void Start()
    {
        if (answerButtons != null && answerButtons.Length > 0)
        {
            defaultButtonColor = answerButtons[0].colors.normalColor;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ShowQuestion();
    }

    void ShowQuestion()
    {
        if (questions == null || questions.Length == 0) return;
        if (currentIndex < 0 || currentIndex >= questions.Length) return;

        QuizQuestion q = questions[currentIndex];
        isAnswered = false;

        if (imageAnimal != null) imageAnimal.sprite = q.image;
        if (textProgress != null) textProgress.text = "Question " + (currentIndex + 1) + " of " + questions.Length;
        if (textQuestionEn != null) textQuestionEn.text = q.questionEn;
        if (textQuestionVi != null) textQuestionVi.text = q.questionVi;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;

            answerButtons[i].interactable = true;

            TextMeshProUGUI txt = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null && i < q.answers.Length)
            {
                txt.text = q.answers[i];
            }

            ColorBlock colors = answerButtons[i].colors;
            colors.normalColor = defaultButtonColor;
            answerButtons[i].colors = colors;
        }
    }

    public void OnClickAnswer(int index)
    {
        if (questions == null || questions.Length == 0) return;
        if (isAnswered) return;
        if (currentIndex < 0 || currentIndex >= questions.Length) return;
        if (index < 0 || index >= answerButtons.Length) return;

        QuizQuestion q = questions[currentIndex];
        bool isCorrect = (index == q.correctIndex);

        isAnswered = true;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
            {
                answerButtons[i].interactable = false;
            }
        }

        if (isCorrect)
        {
            if (answerButtons[index] != null)
            {
                ColorBlock colors = answerButtons[index].colors;
                colors.normalColor = Color.green;
                answerButtons[index].colors = colors;
            }

            if (audioSource != null && soundCorrect != null)
            {
                audioSource.PlayOneShot(soundCorrect);
            }

            Invoke("NextQuestion", 3f);
        }
        else
        {
            if (audioSource != null && soundWrong != null)
            {
                audioSource.PlayOneShot(soundWrong);
            }

            Debug.Log("Sai rồi, thử lại!");
            isAnswered = false;
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i] != null)
                {
                    answerButtons[i].interactable = true;
                }
            }
        }
    }

    void NextQuestion()
    {
        currentIndex++;
        if (currentIndex >= questions.Length)
        {
            Debug.Log("Hoàn thành quiz!");
            currentIndex = questions.Length - 1;
        }
        else
        {
            ShowQuestion();
        }
    }
}