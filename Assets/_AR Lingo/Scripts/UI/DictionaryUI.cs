using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DictionaryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI englishNameText;
    [SerializeField] private TextMeshProUGUI vietnameseNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button playPronunciationButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button homeButton;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // AudioSource component for playing pronunciation

    [Header("Navigation")]
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject scanPanel; // AR View panel
    [SerializeField] private SimpleMenuController menuController;
    [SerializeField] private MockScanFeature mockScanFeature; // optional: reset scan state when leaving
    [SerializeField] private DictionaryListView listView;    // optional: to go back to list view from detail

    // Current word data being displayed
    private WordData currentWordData;
    private bool cameFromARView = false; // Track if we came from AR View or Home

    void Awake()
    {
        // Ensure AudioSource exists
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnEnable()
    {
        // Setup button listeners
        if (playPronunciationButton != null)
            playPronunciationButton.onClick.AddListener(PlayPronunciation);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        
        if (homeButton != null)
            homeButton.onClick.AddListener(OnHomeClicked);
    }

    void OnDisable()
    {
        // Remove button listeners
        if (playPronunciationButton != null)
            playPronunciationButton.onClick.RemoveListener(PlayPronunciation);
        
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
        
        if (homeButton != null)
            homeButton.onClick.RemoveListener(OnHomeClicked);
    }

    /// <summary>
    /// Display word data in the dictionary panel
    /// </summary>
    /// <param name="wordData">The word data to display</param>
    /// <param name="fromARView">True if opened from AR View, false if from Home</param>
    public void DisplayWord(WordData wordData, bool fromARView = false)
    {
        if (wordData == null)
        {
            Debug.LogWarning("DictionaryUI: WordData is null!");
            return;
        }

        currentWordData = wordData;
        cameFromARView = fromARView;

        // DON'T show header in detail view - it's managed by DictionaryListView

        // Update UI elements
        if (englishNameText != null)
            englishNameText.text = wordData.englishName;

        if (vietnameseNameText != null)
            vietnameseNameText.text = wordData.vietnameseName;

        if (descriptionText != null)
            descriptionText.text = wordData.description;

        // Update icon
        if (iconImage != null && wordData.icon2D != null)
        {
            iconImage.sprite = wordData.icon2D;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }

        // Enable/disable pronunciation button based on audio availability
        if (playPronunciationButton != null)
        {
            playPronunciationButton.interactable = (wordData.audioPronounce != null);
        }
    }

    /// <summary>
    /// Play pronunciation audio
    /// </summary>
    public void PlayPronunciation()
    {
        if (currentWordData == null)
        {
            Debug.LogWarning("DictionaryUI: No word data to play pronunciation!");
            return;
        }

        if (currentWordData.audioPronounce != null && audioSource != null)
        {
            audioSource.PlayOneShot(currentWordData.audioPronounce);
            Debug.Log($"Playing pronunciation for: {currentWordData.englishName}");
        }
        else
        {
            Debug.LogWarning("DictionaryUI: Pronunciation audio clip is missing!");
        }
    }

    /// <summary>
    /// Handle back button click - returns to AR View if came from there, otherwise goes to List View
    /// </summary>
    public void OnBackClicked()
    {
        if (cameFromARView && scanPanel != null)
        {
            // Return to AR View (Scan Panel)
            gameObject.SetActive(false);
            
            // Also hide the dictionary panel if it's separate from this detail panel
            Transform parentPanel = transform.parent;
            if (parentPanel != null && parentPanel.gameObject != gameObject)
            {
                parentPanel.gameObject.SetActive(false);
            }
            
            scanPanel.SetActive(true);
            
            // DON'T call OnResetClicked() here - we want to keep the detected object visible
        }
        else
        {
            // Return to list view (came from Dictionary home/list)
            if (listView != null)
            {
                gameObject.SetActive(false);
                listView.BackToListView();
            }
            else
            {
                Debug.LogWarning("DictionaryUI: listView reference is not assigned! Cannot go back to list.");
            }
        }
    }

    /// <summary>
    /// Handle home button click - always returns to Home
    /// </summary>
    public void OnHomeClicked()
    {
        if (menuController != null)
        {
            if (mockScanFeature != null && cameFromARView) 
                mockScanFeature.OnResetClicked(); // only reset scan state if came from AR view
            menuController.BackToHome();
        }
        else
        {
            // Fallback: manually handle navigation
            if (homePanel != null)
                homePanel.SetActive(true);
            
            gameObject.SetActive(false);
            
            if (scanPanel != null)
                scanPanel.SetActive(false);
                
            if (mockScanFeature != null && cameFromARView) 
                mockScanFeature.OnResetClicked(); // only reset scan state if came from AR view
        }
    }

    /// <summary>
    /// Clear current word data (useful when closing panel)
    /// </summary>
    public void ClearDisplay()
    {
        currentWordData = null;
        
        if (englishNameText != null)
            englishNameText.text = "";
        
        if (vietnameseNameText != null)
            vietnameseNameText.text = "";
        
        if (descriptionText != null)
            descriptionText.text = "";
        
        if (iconImage != null)
            iconImage.gameObject.SetActive(false);
    }
}