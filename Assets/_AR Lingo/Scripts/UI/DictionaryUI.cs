using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dictionary UI - Displays word information in detail view
/// Navigation is handled by DictionaryNavigationHandler
/// </summary>
public class DictionaryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI englishNameText;
    [SerializeField] private TextMeshProUGUI vietnameseNameText;
    [SerializeField] private TextMeshProUGUI pronunciationText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI vietnameseDescriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button playPronunciationButton;

    private WordData currentWordData;

    void OnEnable()
    {
        if (playPronunciationButton != null)
            playPronunciationButton.onClick.AddListener(PlayPronunciation);
    }

    void OnDisable()
    {
        if (playPronunciationButton != null)
            playPronunciationButton.onClick.RemoveListener(PlayPronunciation);
    }

    public void DisplayWord(WordData wordData, bool fromARView = false)
    {
        if (wordData == null)
        {
            Debug.LogWarning("DictionaryUI: WordData is null!");
            return;
        }

        currentWordData = wordData;

        if (englishNameText != null)
            englishNameText.text = wordData.englishName;

        if (vietnameseNameText != null)
            vietnameseNameText.text = wordData.vietnameseName;

        if (pronunciationText != null)
        {
            if (!string.IsNullOrEmpty(wordData.pronunciation))
            {
                pronunciationText.text = wordData.pronunciation;
                pronunciationText.gameObject.SetActive(true);
            }
            else
            {
                pronunciationText.gameObject.SetActive(false);
            }
        }

        if (descriptionText != null)
            descriptionText.text = wordData.description;

        if (vietnameseDescriptionText != null)
        {
            if (!string.IsNullOrEmpty(wordData.vietnameseDescription))
            {
                vietnameseDescriptionText.text = wordData.vietnameseDescription;
                vietnameseDescriptionText.gameObject.SetActive(true);
            }
            else
            {
                vietnameseDescriptionText.gameObject.SetActive(false);
            }
        }

        if (iconImage != null && wordData.icon2D != null)
        {
            iconImage.sprite = wordData.icon2D;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }

        if (playPronunciationButton != null)
        {
            playPronunciationButton.interactable = (wordData.audioPronounce != null);
        }
    }

    public void PlayPronunciation()
    {
        if (currentWordData == null)
        {
            Debug.LogWarning("DictionaryUI: No word data to play pronunciation!");
            return;
        }

        if (currentWordData.audioPronounce != null)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayVoice(currentWordData.audioPronounce);
                Debug.Log($"[DictionaryUI] Playing pronunciation for: {currentWordData.englishName}");
            }
            else
            {
                Debug.LogError("DictionaryUI: AudioManager not found!");
            }
        }
        else
        {
            Debug.LogWarning("DictionaryUI: Pronunciation audio clip is missing!");
        }
    }

    public void ClearDisplay()
    {
        currentWordData = null;
        
        if (englishNameText != null)
            englishNameText.text = "";
        
        if (vietnameseNameText != null)
            vietnameseNameText.text = "";

        if (pronunciationText != null)
        {
            pronunciationText.text = "";
            pronunciationText.gameObject.SetActive(false);
        }
        
        if (descriptionText != null)
            descriptionText.text = "";

        if (vietnameseDescriptionText != null)
        {
            vietnameseDescriptionText.text = "";
            vietnameseDescriptionText.gameObject.SetActive(false);
        }
        
        if (iconImage != null)
            iconImage.gameObject.SetActive(false);
    }
}