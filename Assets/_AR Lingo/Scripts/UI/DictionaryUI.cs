using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dictionary UI - Displays word information in detail view
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
    [SerializeField] private Button backButton;
    [SerializeField] private Button homeButton;

    // REMOVED: No longer need local audioSource
    // [Header("Audio")]
    // [SerializeField] private AudioSource audioSource;

    [Header("Navigation")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject scanPanel;
    [SerializeField] private SimpleMenuController menuController;
    [SerializeField] private ARScanFeature arScanFeature;
    [SerializeField] private DictionaryListView listView;

    private WordData currentWordData;
    private bool cameFromARView = false;

    void Awake()
    {
        // REMOVED: No longer create local AudioSource
        // if (audioSource == null)
        // {
        //     audioSource = gameObject.AddComponent<AudioSource>();
        //     audioSource.playOnAwake = false;
        // }
    }

    void OnEnable()
    {
        if (playPronunciationButton != null)
            playPronunciationButton.onClick.AddListener(PlayPronunciation);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        
        if (homeButton != null)
            homeButton.onClick.AddListener(OnHomeClicked);
    }

    void OnDisable()
    {
        if (playPronunciationButton != null)
            playPronunciationButton.onClick.RemoveListener(PlayPronunciation);
        
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
        
        if (homeButton != null)
            homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    public void DisplayWord(WordData wordData, bool fromARView = false)
    {
        if (wordData == null)
        {
            Debug.LogWarning("DictionaryUI: WordData is null!");
            return;
        }

        currentWordData = wordData;
        cameFromARView = fromARView;

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

    // FIXED: Use AudioManager instead of local AudioSource
    public void PlayPronunciation()
    {
        if (currentWordData == null)
        {
            Debug.LogWarning("DictionaryUI: No word data to play pronunciation!");
            return;
        }

        if (currentWordData.audioPronounce != null)
        {
            // Use AudioManager's Voice channel so it respects volume settings
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayVoice(currentWordData.audioPronounce);
                Debug.Log($"Playing pronunciation for: {currentWordData.englishName}");
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

    public void OnBackClicked()
    {
        if (cameFromARView && scanPanel != null)
        {
            gameObject.SetActive(false);
            
            Transform parentPanel = transform.parent;
            if (parentPanel != null && parentPanel.gameObject != gameObject)
            {
                parentPanel.gameObject.SetActive(false);
            }
            
            scanPanel.SetActive(true);
            
            if (arScanFeature != null)
            {
                arScanFeature.OnBackFromDictionary();
            }
        }
        else
        {
            if (listView != null)
            {
                gameObject.SetActive(false);
                listView.BackToListView();
            }
            else
            {
                Debug.LogWarning("DictionaryUI: listView reference is not assigned!");
            }
        }
    }

    public void OnHomeClicked()
    {
        if (menuController != null)
        {
            menuController.BackToHome();
        }
        else
        {
            Debug.LogWarning("DictionaryUI: menuController reference is not assigned!");
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