using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Settings UI - Manages app settings (Audio, Quiz, etc.)
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("Audio Settings UI")]
    public Slider sliderSFX;
    public Slider sliderVoice;
    public TextMeshProUGUI textSFXValue;
    public TextMeshProUGUI textVoiceValue;

    [Header("Quiz Settings UI")]
    public GameObject quizSettingsPanel;
    public GameObject quizSettingsOverlay;
    public Slider sliderQuestionCount;
    public TextMeshProUGUI textQuestionCount;
    public Toggle toggleAutoGenerate;
    public Button buttonSaveQuizSettings;

    [Header("Navigation")]
    public Button buttonBack;
    public Button buttonOpenQuizSettings;
    public SimpleMenuController menuController;

    [Header("References")]
    public QuizManager quizManager;

    private void Start()
    {
        // Setup audio sliders
        if (sliderSFX != null)
        {
            sliderSFX.onValueChanged.AddListener(OnSFXVolumeChanged);
            sliderSFX.value = PlayerPrefs.GetFloat("Settings_SFX", 1f);
        }

        if (sliderVoice != null)
        {
            sliderVoice.onValueChanged.AddListener(OnVoiceVolumeChanged);
            sliderVoice.value = PlayerPrefs.GetFloat("Settings_Voice", 1f);
        }

        // Setup quiz settings
        if (sliderQuestionCount != null)
        {
            sliderQuestionCount.onValueChanged.AddListener(OnQuestionCountChanged);
            sliderQuestionCount.value = PlayerPrefs.GetInt("Quiz_QuestionCount", 10);
        }

        if (toggleAutoGenerate != null)
        {
            toggleAutoGenerate.isOn = PlayerPrefs.GetInt("Quiz_AutoGenerate", 1) == 1;
        }

        // Setup buttons
        if (buttonBack != null)
        {
            buttonBack.onClick.AddListener(OnBackClicked);
        }

        if (buttonOpenQuizSettings != null)
        {
            buttonOpenQuizSettings.onClick.AddListener(OnOpenQuizSettingsClicked);
        }

        if (buttonSaveQuizSettings != null)
        {
            buttonSaveQuizSettings.onClick.AddListener(OnSaveQuizSettingsClicked);
        }

        // Initially hide quiz settings panel and overlay
        if (quizSettingsPanel != null)
        {
            quizSettingsPanel.SetActive(false);
        }

        // Initially hide overlay
        if (quizSettingsOverlay != null)
        {
            quizSettingsOverlay.SetActive(false);
        }

        // Update UI text
        UpdateAudioUI();
        UpdateQuizUI();
    }

    void OnDestroy()
    {
        if (sliderSFX != null)
            sliderSFX.onValueChanged.RemoveListener(OnSFXVolumeChanged);

        if (sliderVoice != null)
            sliderVoice.onValueChanged.RemoveListener(OnVoiceVolumeChanged);

        if (sliderQuestionCount != null)
            sliderQuestionCount.onValueChanged.RemoveListener(OnQuestionCountChanged);

        if (buttonBack != null)
            buttonBack.onClick.RemoveListener(OnBackClicked);

        if (buttonOpenQuizSettings != null)
            buttonOpenQuizSettings.onClick.RemoveListener(OnOpenQuizSettingsClicked);

        if (buttonSaveQuizSettings != null)
            buttonSaveQuizSettings.onClick.RemoveListener(OnSaveQuizSettingsClicked);
    }

    // === AUDIO SETTINGS ===

    void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXLevel(value);
        }

        PlayerPrefs.SetFloat("Settings_SFX", value);
        PlayerPrefs.Save();

        UpdateAudioUI();
    }

    void OnVoiceVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetVoiceLevel(value);
        }

        PlayerPrefs.SetFloat("Settings_Voice", value);
        PlayerPrefs.Save();

        UpdateAudioUI();
    }

    void UpdateAudioUI()
    {
        if (textSFXValue != null && sliderSFX != null)
        {
            textSFXValue.text = $"{(sliderSFX.value * 100):F0}%";
        }

        if (textVoiceValue != null && sliderVoice != null)
        {
            textVoiceValue.text = $"{(sliderVoice.value * 100):F0}%";
        }
    }

    // === QUIZ SETTINGS ===

    void OnQuestionCountChanged(float value)
    {
        UpdateQuizUI();
    }

    void UpdateQuizUI()
    {
        if (textQuestionCount != null && sliderQuestionCount != null)
        {
            textQuestionCount.text = $"{sliderQuestionCount.value:F0} Questions";
        }
    }

    public void OnOpenQuizSettingsClicked()
    {
        // Show overlay first (to darken background)
        if (quizSettingsOverlay != null)
        {
            quizSettingsOverlay.SetActive(true);
        }

        // Then show quiz settings panel on top
        if (quizSettingsPanel != null)
        {
            quizSettingsPanel.SetActive(true);
        }
    }

    public void OnSaveQuizSettingsClicked()
    {
        if (sliderQuestionCount != null)
        {
            int questionCount = (int)sliderQuestionCount.value;
            PlayerPrefs.SetInt("Quiz_QuestionCount", questionCount);

            // Update QuizManager if assigned
            if (quizManager != null)
            {
                quizManager.numberOfQuestions = questionCount;
                
                // Regenerate quiz with new settings immediately
                if (quizManager.autoGenerateOnStart)
                {
                    quizManager.GenerateQuizFromDatabase(questionCount);
                    Debug.Log($"[SettingsUI] Quiz regenerated with {questionCount} questions");
                }
            }
        }

        if (toggleAutoGenerate != null)
        {
            int autoGen = toggleAutoGenerate.isOn ? 1 : 0;
            PlayerPrefs.SetInt("Quiz_AutoGenerate", autoGen);

            // Update QuizManager if assigned
            if (quizManager != null)
            {
                quizManager.autoGenerateOnStart = toggleAutoGenerate.isOn;
            }
        }

        PlayerPrefs.Save();

        Debug.Log($"[SettingsUI] Quiz settings saved: {sliderQuestionCount.value} questions, Auto-gen: {toggleAutoGenerate.isOn}");

        // Close quiz settings panel
        CloseQuizSettings();
    }

    // Helper method to close quiz settings (used by Save and Cancel)
    private void CloseQuizSettings()
    {
        if (quizSettingsPanel != null)
        {
            quizSettingsPanel.SetActive(false);
        }

        // Hide overlay when closing
        if (quizSettingsOverlay != null)
        {
            quizSettingsOverlay.SetActive(false);
        }
    }

    public void OnBackClicked()
    {
        // Close quiz settings if open
        if (quizSettingsPanel != null && quizSettingsPanel.activeSelf)
        {
            CloseQuizSettings(); // Use helper method
            return;
        }

        // Go back to home
        if (menuController != null)
        {
            menuController.BackToHome();
        }
    }
}