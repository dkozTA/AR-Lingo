using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AR Scan Feature - Integrates with Vuforia AR detection
/// Listens to GameManager events for real AR detection
/// </summary>
public class ARScanFeature : MonoBehaviour
{
    [Header("1. UI Elements")]
    public GameObject viewInfoButton;   // Button to view dictionary
    public GameObject quizButton;       // Button to start quiz
    public GameObject resetButton;      // Button to reset scan
    public GameObject scanInstruction;  // Scan instruction panel
    public GameObject scanFrame;        // AR scan frame overlay

    [Header("2. Panel References")]
    public GameObject dictionaryPanel;
    public GameObject scanPanel;
    public DictionaryUI dictionaryUI;
    public GameObject dictionaryListViewPanel;
    public GameObject dictionaryDetailViewPanel;
    public GameObject dictionaryListViewBackButton;
    public GameObject dictionaryHeader;
    
    [Header("3. Data")]
    public WordDatabase wordDatabase;
    public GameManager gameManager; // Reference to GameManager for AR events

    [Header("4. AR Detection Settings")]
    public LayerMask arModelLayer; // Layer for AR models to detect clicks
    public Camera arCamera; // Reference to AR Camera

    private WordData currentDetectedWord;
    private bool hasDetectedObject = false;

    void Start()
    {
        // Auto-find AR Camera if not assigned
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }
    }

    void OnEnable()
    {
        // Auto-find GameManager if not assigned
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        // Subscribe to AR detection events from GameManager
        if (gameManager != null)
        {
            gameManager.OnAnimalDetected += HandleAnimalDetected;
            gameManager.OnARObjectLost += HandleARObjectLost;
        }
        else
        {
            Debug.LogError("ARScanFeature: GameManager reference is missing!");
        }

        // Initialize UI state
        ResetScanState();
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (gameManager != null)
        {
            gameManager.OnAnimalDetected -= HandleAnimalDetected;
            gameManager.OnARObjectLost -= HandleARObjectLost;
        }
    }

    /// <summary>
    /// Called when AR camera detects an image target
    /// This is triggered by GameManager.OnAnimalDetected event
    /// </summary>
    private void HandleAnimalDetected(string wordID)
    {
        Debug.Log($"[ARScanFeature] AR Target detected: {wordID}");

        // Get word data from database
        if (wordDatabase != null)
        {
            currentDetectedWord = wordDatabase.GetWordByID(wordID);
            
            if (currentDetectedWord != null)
            {
                Debug.Log($"[ARScanFeature] Word found: {currentDetectedWord.englishName} ({currentDetectedWord.vietnameseName})");
                ShowDetectedObject();
            }
            else
            {
                Debug.LogWarning($"[ARScanFeature] Word with ID '{wordID}' not found in database!");
            }
        }
        else
        {
            Debug.LogError("ARScanFeature: WordDatabase is not assigned!");
        }
    }

    /// <summary>
    /// Called when AR target is lost (camera no longer sees it)
    /// This is triggered by GameManager.OnARObjectLost event
    /// </summary>
    private void HandleARObjectLost()
    {
        Debug.Log("[ARScanFeature] AR Target lost");
        
        // Keep UI visible even when target is lost for better UX
        // User can still interact with buttons until they manually reset
    }

    /// <summary>
    /// Show UI elements when object is detected
    /// </summary>
    private void ShowDetectedObject()
    {
        hasDetectedObject = true;

        // Hide scan instruction and frame
        if (scanInstruction != null) scanInstruction.SetActive(false);
        if (scanFrame != null) scanFrame.SetActive(false);

        // Show reset button
        if (resetButton != null) resetButton.SetActive(true);

        // Buttons will appear after user taps on the 3D model (handled in Update)
        if (viewInfoButton != null) viewInfoButton.SetActive(false);
        if (quizButton != null) quizButton.SetActive(false);
    }

    /// <summary>
    /// Detect tap on 3D model to show action buttons
    /// Uses raycasting to detect clicks on AR models
    /// </summary>
    void Update()
    {
        if (!hasDetectedObject) return;

        // Detect touch or mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray;
            
            // Use AR Camera for raycasting
            if (arCamera != null)
            {
                ray = arCamera.ScreenPointToRay(Input.mousePosition);
            }
            else
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            }

            RaycastHit hit;
            
            // Check if we hit any AR model
            if (Physics.Raycast(ray, out hit, 100f, arModelLayer))
            {
                Debug.Log($"[ARScanFeature] Tapped on AR model: {hit.collider.gameObject.name}");
                ShowActionButtons();
            }
            else
            {
                // Alternative: Show buttons on any tap when object is detected
                // Uncomment this if you want buttons to show on any screen tap
                // ShowActionButtons();
            }
        }
    }

    /// <summary>
    /// Show View Info and Quiz buttons
    /// </summary>
    private void ShowActionButtons()
    {
        Debug.Log("[ARScanFeature] Showing action buttons");
        
        if (viewInfoButton != null) viewInfoButton.SetActive(true);
        if (quizButton != null) quizButton.SetActive(true);
    }

    /// <summary>
    /// Open Dictionary detail view - Called by View Info button
    /// </summary>
    public void OnOpenDictionaryClicked()
    {
        if (currentDetectedWord == null)
        {
            Debug.LogWarning("ARScanFeature: No word detected to display!");
            return;
        }

        Debug.Log($"[ARScanFeature] Opening dictionary for: {currentDetectedWord.englishName}");

        // Switch to Dictionary panel
        if (scanPanel != null) scanPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(true);

        // Show detail view, hide list view
        if (dictionaryDetailViewPanel != null) 
            dictionaryDetailViewPanel.SetActive(true);
        if (dictionaryListViewPanel != null) 
            dictionaryListViewPanel.SetActive(false);

        // Hide ListView's UI elements
        if (dictionaryListViewBackButton != null)
            dictionaryListViewBackButton.SetActive(false);
        if (dictionaryHeader != null)
            dictionaryHeader.SetActive(false);

        // Display word data
        if (dictionaryUI != null)
        {
            dictionaryUI.gameObject.SetActive(true);
            dictionaryUI.DisplayWord(currentDetectedWord, fromARView: true);
        }
        else
        {
            Debug.LogWarning("ARScanFeature: DictionaryUI component not found!");
        }
    }

    /// <summary>
    /// Reset scan state - Called by Reset button
    /// </summary>
    public void OnResetClicked()
    {
        Debug.Log("[ARScanFeature] Resetting scan state");
        ResetScanState();
    }

    /// <summary>
    /// Reset to initial scanning state
    /// </summary>
    private void ResetScanState()
    {
        hasDetectedObject = false;
        currentDetectedWord = null;

        // Show scan instruction and frame
        if (scanInstruction != null) scanInstruction.SetActive(true);
        if (scanFrame != null) scanFrame.SetActive(true);

        // Hide all buttons
        if (viewInfoButton != null) viewInfoButton.SetActive(false);
        if (quizButton != null) quizButton.SetActive(false);
        if (resetButton != null) resetButton.SetActive(false);

        // Hide dictionary panels
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
        if (dictionaryDetailViewPanel != null) dictionaryDetailViewPanel.SetActive(false);
    }
}