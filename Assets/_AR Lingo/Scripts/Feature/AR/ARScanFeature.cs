using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AR Scan Feature - Integrates with Vuforia AR detection
/// Listens to GameManager events for real AR detection
/// </summary>
public class ARScanFeature : MonoBehaviour
{
    [Header("1. UI Elements")]
    public GameObject viewInfoButton;
    public GameObject quizButton;
    public GameObject resetButton;
    public GameObject scanInstruction;
    public GameObject scanFrame;
    public GameObject walkButton;
    public GameObject attackButton;

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
    public AppStateManager gameManager;

    [Header("4. AR Detection Settings")]
    public Camera arCamera;
    [SerializeField] private bool autoShowButtonsOnDetection = true;

    [Header("5. Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private WordData currentDetectedWord;
    private bool hasDetectedObject = false;
    private bool isViewingDictionary = false; // NEW: Track if user is viewing dictionary

    void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera != null && enableDebugLogs)
                Debug.Log($"[ARScanFeature] Auto-found camera: {arCamera.name}");
        }
    }

    void OnEnable()
    {
        if (gameManager == null)
        {
            gameManager = AppStateManager.Instance;
        }

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

        ResetScanState();
    }

    void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnAnimalDetected -= HandleAnimalDetected;
            gameManager.OnARObjectLost -= HandleARObjectLost;
        }
    }

    private void HandleAnimalDetected(string wordID)
    {
        if (enableDebugLogs)
            Debug.Log($"[ARScanFeature] AR Target detected: {wordID}");

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

    private void HandleARObjectLost()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] AR Target lost");

        // NEW: Don't reset if user is viewing dictionary detail
        if (isViewingDictionary)
        {
            if (enableDebugLogs)
                Debug.Log("[ARScanFeature] User is viewing dictionary - keeping UI as is");

            // Just hide the buttons, keep dictionary open
            if (viewInfoButton != null) viewInfoButton.SetActive(false);
            if (quizButton != null) quizButton.SetActive(false);
            if (walkButton != null) walkButton.SetActive(false);
            if (attackButton != null) attackButton.SetActive(false);

            hasDetectedObject = false;
            return;
        }

        // Normal reset for scan panel
        ResetScanState();
    }

    private void ShowDetectedObject()
    {
        hasDetectedObject = true;
        isViewingDictionary = false; // Reset dictionary flag

        if (scanInstruction != null) scanInstruction.SetActive(false);
        if (scanFrame != null) scanFrame.SetActive(false);
        if (resetButton != null) resetButton.SetActive(false);

        if (autoShowButtonsOnDetection)
        {
            ShowActionButtons();
        }
        else
        {
            if (viewInfoButton != null) viewInfoButton.SetActive(false);
            if (quizButton != null) quizButton.SetActive(false);
            if (walkButton != null) walkButton.SetActive(false);
            if (attackButton != null) attackButton.SetActive(false);

            if (enableDebugLogs)
                Debug.Log("[ARScanFeature] Waiting for tap on model to show buttons...");
        }
    }

    void Update()
    {
        if (!hasDetectedObject || isViewingDictionary) return; // Don't detect clicks when viewing dictionary

        bool inputDetected = false;
        Vector3 inputPosition = Vector3.zero;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputDetected = true;
            inputPosition = Input.GetTouch(0).position;

            if (enableDebugLogs)
                Debug.Log($"[ARScanFeature] Touch detected at: {inputPosition}");
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
            inputPosition = Input.mousePosition;

            if (enableDebugLogs)
                Debug.Log($"[ARScanFeature] Mouse click detected at: {inputPosition}");
        }

        if (inputDetected)
        {
            if (arCamera != null)
            {
                Ray ray = arCamera.ScreenPointToRay(inputPosition);
                RaycastHit hit;

                if (enableDebugLogs)
                    Debug.Log($"[ARScanFeature] Raycasting from {ray.origin} in direction {ray.direction}");

                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (enableDebugLogs)
                        Debug.Log($"[ARScanFeature] ✓ Hit object: {hit.collider.gameObject.name}");

                    if (hit.collider.gameObject.name.StartsWith("PF_") ||
                        hit.collider.transform.root.name.StartsWith("ImageTarget"))
                    {
                        ShowActionButtons();
                    }
                }
                else
                {
                    if (enableDebugLogs)
                        Debug.Log("[ARScanFeature] Raycast hit nothing - showing buttons anyway (fallback)");

                    ShowActionButtons();
                }
            }
            else
            {
                Debug.LogWarning("[ARScanFeature] AR Camera not assigned!");
                ShowActionButtons();
            }
        }
    }

    private void ShowActionButtons()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Showing action buttons");

        if (viewInfoButton != null) viewInfoButton.SetActive(true);
        if (walkButton != null) walkButton.SetActive(true);
        if (attackButton != null) attackButton.SetActive(true);

        if (quizButton != null) quizButton.SetActive(false);
    }

    public void OnWalkButtonClicked()
    {
        Debug.Log("[ARScanFeature] Walk button clicked - Animation will be added later");
    }

    public void OnAttackButtonClicked()
    {
        Debug.Log("[ARScanFeature] Attack button clicked - Animation will be added later");
    }

    public void OnOpenDictionaryClicked()
    {
        if (currentDetectedWord == null)
        {
            Debug.LogWarning("ARScanFeature: No word detected to display!");
            return;
        }

        if (enableDebugLogs)
            Debug.Log($"[ARScanFeature] Opening dictionary for: {currentDetectedWord.englishName}");

        // NEW: Set flag that we're viewing dictionary
        isViewingDictionary = true;

        if (scanPanel != null) scanPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(true);

        if (dictionaryDetailViewPanel != null)
            dictionaryDetailViewPanel.SetActive(true);
        if (dictionaryListViewPanel != null)
            dictionaryListViewPanel.SetActive(false);

        if (dictionaryListViewBackButton != null)
            dictionaryListViewBackButton.SetActive(false);
        if (dictionaryHeader != null)
            dictionaryHeader.SetActive(false);

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

    public void OnResetClicked()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Resetting scan state");

        // NEW: Reset dictionary flag
        isViewingDictionary = false;

        ResetScanState();
    }

    // NEW: Call this when user clicks Back button in dictionary
    public void OnBackFromDictionary()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Back from dictionary to scan panel");

        // Reset dictionary flag
        isViewingDictionary = false;

        // Hide dictionary, show scan panel
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
        if (dictionaryDetailViewPanel != null) dictionaryDetailViewPanel.SetActive(false);
        if (scanPanel != null) scanPanel.SetActive(true);

        // Reset to clean scan state
        ResetScanState();
    }

    private void ResetScanState()
    {
        hasDetectedObject = false;
        currentDetectedWord = null;
        isViewingDictionary = false; // Make sure flag is reset

        if (scanInstruction != null) scanInstruction.SetActive(true);
        if (scanFrame != null) scanFrame.SetActive(true);

        if (viewInfoButton != null) viewInfoButton.SetActive(false);
        if (quizButton != null) quizButton.SetActive(false);
        if (resetButton != null) resetButton.SetActive(false);
        if (walkButton != null) walkButton.SetActive(false);
        if (attackButton != null) attackButton.SetActive(false);

        // Only hide dictionary panels if not currently viewing
        if (!isViewingDictionary)
        {
            if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
            if (dictionaryDetailViewPanel != null) dictionaryDetailViewPanel.SetActive(false);
        }
    }
}