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
    [SerializeField] private bool autoShowButtonsOnDetection = false; // Auto show buttons without tap

    [Header("5. Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private WordData currentDetectedWord;
    private bool hasDetectedObject = false;

    void Start()
    {
        // Auto-find AR Camera if not assigned
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera != null && enableDebugLogs)
                Debug.Log($"[ARScanFeature] Auto-found camera: {arCamera.name}");
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
            
            if (enableDebugLogs)
                Debug.Log("[ARScanFeature] Subscribed to GameManager events");
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
        if (enableDebugLogs)
            Debug.Log($"[ARScanFeature] AR Target detected: {wordID}");

        // Get word data from database
        if (wordDatabase != null)
        {
            currentDetectedWord = wordDatabase.GetWordByID(wordID);
            
            if (currentDetectedWord != null)
            {
                if (enableDebugLogs)
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
        if (enableDebugLogs)
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

        // Auto-show buttons or wait for tap
        if (autoShowButtonsOnDetection)
        {
            ShowActionButtons();
        }
        else
        {
            // Buttons will appear after user taps on the 3D model
            if (viewInfoButton != null) viewInfoButton.SetActive(false);
            if (quizButton != null) quizButton.SetActive(false);
            
            if (enableDebugLogs)
                Debug.Log("[ARScanFeature] Waiting for tap on model to show buttons...");
        }
    }

    /// <summary>
    /// Detect tap on 3D model to show action buttons
    /// Uses raycasting to detect clicks on AR models
    /// </summary>
    void Update()
    {
        if (!hasDetectedObject) return;

        // Detect touch or mouse click
        bool inputDetected = false;
        Vector3 inputPosition = Vector3.zero;

        // Check for touch input (mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputDetected = true;
            inputPosition = Input.GetTouch(0).position;
            
            if (enableDebugLogs)
                Debug.Log($"[ARScanFeature] Touch detected at: {inputPosition}");
        }
        // Check for mouse click (editor/PC)
        else if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
            inputPosition = Input.mousePosition;
            
            if (enableDebugLogs)
                Debug.Log($"[ARScanFeature] Mouse click detected at: {inputPosition}");
        }

        if (inputDetected)
        {
            // Try raycast with AR Camera
            if (arCamera != null)
            {
                Ray ray = arCamera.ScreenPointToRay(inputPosition);
                RaycastHit hit;
                
                if (enableDebugLogs)
                    Debug.Log($"[ARScanFeature] Raycasting from {ray.origin} in direction {ray.direction}");
                
                // First try with layer mask
                if (Physics.Raycast(ray, out hit, 100f, arModelLayer))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[ARScanFeature] ✓ Hit AR model with layer: {hit.collider.gameObject.name}");
                    
                    ShowActionButtons();
                }
                // Then try without layer mask (any collider)
                else if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[ARScanFeature] ✓ Hit object: {hit.collider.gameObject.name} on layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
                    
                    // Check if it's one of our AR models by name
                    if (hit.collider.gameObject.name.StartsWith("PF_") || 
                        hit.collider.transform.root.name.StartsWith("ImageTarget"))
                    {
                        ShowActionButtons();
                    }
                    else if (enableDebugLogs)
                    {
                        Debug.Log($"[ARScanFeature] Hit object is not an AR model");
                    }
                }
                else
                {
                    if (enableDebugLogs)
                        Debug.Log("[ARScanFeature] Raycast hit nothing - showing buttons anyway (fallback)");
                    
                    // Fallback: Show buttons on any tap when object is detected
                    ShowActionButtons();
                }
            }
            else
            {
                Debug.LogWarning("[ARScanFeature] AR Camera not assigned!");
                // Fallback: Show buttons anyway
                ShowActionButtons();
            }
        }
    }

    /// <summary>
    /// Show View Info and Quiz buttons
    /// </summary>
    private void ShowActionButtons()
    {
        if (enableDebugLogs)
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

        if (enableDebugLogs)
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
        if (enableDebugLogs)
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