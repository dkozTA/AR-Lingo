using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AR Scan Feature - Integrates with Vuforia AR detection
/// Listens to GameManager events for real AR detection
/// </summary>
public class ARScanFeature : MonoBehaviour
{
    [Header("1. UI Elements - Bottom Action Buttons")]
    public GameObject viewInfoButton;
    public GameObject walkButton;
    public GameObject attackButton;
    public GameObject resetButton;

    [Header("1.2 NEW: Top-Right Navigation Buttons")]
    public GameObject dictionaryButtonAR;
    public GameObject quizButtonAR;

    [Header("2. UI Instructions")]
    public GameObject scanInstruction;
    public GameObject scanFrame;

    [Header("3. Panel References")]
    public GameObject dictionaryPanel;
    public GameObject scanPanel;
    public GameObject quizPanel;
    public DictionaryUI dictionaryUI;
    public GameObject dictionaryListViewPanel;
    public GameObject dictionaryDetailViewPanel;
    public GameObject dictionaryListViewBackButton;
    public GameObject dictionaryHeader;
    public DictionaryListView dictionaryListView;

    [Header("4. Data")]
    public WordDatabase wordDatabase;
    public AppStateManager gameManager;

    [Header("5. AR Detection Settings")]
    public Camera arCamera;
    [SerializeField] private bool autoShowButtonsOnDetection = true;

    [Header("6. Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private WordData currentDetectedWord;
    private bool hasDetectedObject = false;
    private bool isViewingDictionary = false;
    
    // NEW: Store reference to current detected model's animator
    private AnimalAnimationController currentAnimalController;

    private static bool _openedFromARScan = false;

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

                // NEW: Find the AnimalAnimationController on the detected model
                FindCurrentAnimalController(wordID);
                
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

    // Find the AnimalAnimationController for the detected animal
    private void FindCurrentAnimalController(string wordID)
    {
        // Search for the 3D model in the scene
        string modelName = "PF_" + wordID.Replace("animal_", "");
        
        // FASTEST: Direct search by name
        GameObject modelObject = GameObject.Find(modelName);
        
        if (modelObject != null)
        {
            currentAnimalController = modelObject.GetComponentInChildren<AnimalAnimationController>();
            
            if (currentAnimalController != null)
            {
                if (enableDebugLogs)
                    Debug.Log($"[ARScanFeature] Found AnimalAnimationController on: {modelObject.name}");
                
                // Set custom animation names from WordData if available
                if (currentDetectedWord != null && currentDetectedWord.SupportsAnimations())
                {
                    currentAnimalController.SetAnimationNames(
                        currentDetectedWord.walkAnimationName,
                        currentDetectedWord.attackAnimationName
                    );
                }
                return;
            }
        }

        if (enableDebugLogs)
            Debug.LogWarning($"[ARScanFeature] AnimalAnimationController not found for: {modelName}");
    }

    private void HandleARObjectLost()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] AR Target lost");

        if (isViewingDictionary)
        {
            if (enableDebugLogs)
                Debug.Log("[ARScanFeature] User is viewing dictionary - keeping UI as is");

            if (viewInfoButton != null) viewInfoButton.SetActive(false);
            if (walkButton != null) walkButton.SetActive(false);
            if (attackButton != null) attackButton.SetActive(false);
            
            if (dictionaryButtonAR != null) dictionaryButtonAR.SetActive(false);
            if (quizButtonAR != null) quizButtonAR.SetActive(false);

            hasDetectedObject = false;
            currentAnimalController = null; // Clear reference
            return;
        }

        ResetScanState();
    }

    private void ShowDetectedObject()
    {
        hasDetectedObject = true;
        isViewingDictionary = false;

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
            if (walkButton != null) walkButton.SetActive(false);
            if (attackButton != null) attackButton.SetActive(false);
            
            if (dictionaryButtonAR != null) dictionaryButtonAR.SetActive(false);
            if (quizButtonAR != null) quizButtonAR.SetActive(false);

            if (enableDebugLogs)
                Debug.Log("[ARScanFeature] Waiting for tap on model to show buttons...");
        }
    }

    void Update()
    {
        if (!hasDetectedObject || isViewingDictionary) return;

        if (!TryGetInputPosition(out var inputPosition)) return;

        if (arCamera == null)
        {
            Debug.LogWarning("[ARScanFeature] AR Camera not assigned!");
            ShowActionButtons();
            return;
        }

        var ray = arCamera.ScreenPointToRay(inputPosition);
        LogDebug($"[ARScanFeature] Raycasting from {ray.origin} in direction {ray.direction}");

        if (Physics.Raycast(ray, out var hit, 100f))
        {
            LogDebug($"[ARScanFeature] ✓ Hit object: {hit.collider.gameObject.name}");
            if (IsARTarget(hit.collider.transform))
            {
                ShowActionButtons();
            }
        }
        else
        {
            LogDebug("[ARScanFeature] Raycast hit nothing - showing buttons anyway (fallback)");
            ShowActionButtons();
        }
    }

    private bool TryGetInputPosition(out Vector3 inputPosition)
    {
        inputPosition = Vector3.zero;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            inputPosition = Input.GetTouch(0).position;
            LogDebug($"[ARScanFeature] Touch detected at: {inputPosition}");
            return true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
            LogDebug($"[ARScanFeature] Mouse click detected at: {inputPosition}");
            return true;
        }

        return false;
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs) Debug.Log(message);
    }

    private static bool IsARTarget(Transform t)
    {
        return t.gameObject.name.StartsWith("PF_") || t.root.name.StartsWith("ImageTarget");
    }

    private void ShowActionButtons()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Showing action buttons");

        if (viewInfoButton != null) viewInfoButton.SetActive(true);
        
        // NEW: Only show Walk/Attack buttons if the animal supports animations
        bool hasAnimations = currentDetectedWord != null && currentDetectedWord.SupportsAnimations();
        
        if (walkButton != null) 
            walkButton.SetActive(hasAnimations);
        
        if (attackButton != null) 
            attackButton.SetActive(hasAnimations);

        if (dictionaryButtonAR != null) dictionaryButtonAR.SetActive(true);
        if (quizButtonAR != null) quizButtonAR.SetActive(true);
    }

    // UPDATED: Walk button now triggers animation
    public void OnWalkButtonClicked()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Walk button clicked");

        if (currentAnimalController != null)
        {
            currentAnimalController.PlayWalk();
            
            if (enableDebugLogs)
                Debug.Log($"[ARScanFeature] Playing Walk animation on: {currentAnimalController.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("[ARScanFeature] No AnimalAnimationController found - cannot play Walk animation");
        }
    }

    // UPDATED: Attack button now triggers animation
    public void OnAttackButtonClicked()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Attack button clicked");

        if (currentAnimalController != null)
        {
            currentAnimalController.PlayAttack();
            
            if (enableDebugLogs)
                Debug.Log($"[ARScanFeature] Playing Attack animation on: {currentAnimalController.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("[ARScanFeature] No AnimalAnimationController found - cannot play Attack animation");
        }
    }

    public void OnViewInfoButtonClicked()
    {
        if (currentDetectedWord == null)
        {
            Debug.LogWarning("ARScanFeature: No word detected to display!");
            return;
        }

        if (enableDebugLogs)
            Debug.Log($"[ARScanFeature] Opening dictionary DETAIL for: {currentDetectedWord.englishName}");

        isViewingDictionary = true;
        _openedFromARScan = true;

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

    public void OnDictionaryListButtonClicked()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Opening dictionary LIST view");

        isViewingDictionary = true;
        _openedFromARScan = true;

        if (scanPanel != null) scanPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(true);

        if (dictionaryListViewPanel != null)
            dictionaryListViewPanel.SetActive(true);
        if (dictionaryDetailViewPanel != null)
            dictionaryDetailViewPanel.SetActive(false);

        if (dictionaryListViewBackButton != null)
            dictionaryListViewBackButton.SetActive(true);
        if (dictionaryHeader != null)
            dictionaryHeader.SetActive(true);

        if (dictionaryListView != null)
        {
            dictionaryListView.gameObject.SetActive(true);
            dictionaryListView.ShowListView();
        }
        else
        {
            Debug.LogWarning("ARScanFeature: DictionaryListView component not found!");
        }
    }

    public void OnQuizButtonClicked()
    {
        Debug.Log("[ARScanFeature] OnQuizButtonClicked called");
        
        _openedFromARScan = true;
        Debug.Log($"[ARScanFeature] Set _openedFromARScan = {_openedFromARScan}");

        if (scanPanel != null) scanPanel.SetActive(false);
        if (quizPanel != null) 
        {
            quizPanel.SetActive(true);
            
            var quizManager = quizPanel.GetComponent<QuizManager>();
            if (quizManager != null)
            {
                quizManager.ResetQuiz();
            }
        }
        else
        {
            Debug.LogWarning("ARScanFeature: Quiz panel not assigned!");
        }
    }

    public void OnResetClicked()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Resetting scan state");

        isViewingDictionary = false;

        ResetScanState();
    }

    public void OnBackFromDictionary()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Back from dictionary to scan panel");

        isViewingDictionary = false;
        _openedFromARScan = false;

        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
        if (dictionaryDetailViewPanel != null) dictionaryDetailViewPanel.SetActive(false);
        if (dictionaryListViewPanel != null) dictionaryListViewPanel.SetActive(false);
        
        if (scanPanel != null) scanPanel.SetActive(true);

        ResetScanState();
    }

    public void OnBackFromQuiz()
    {
        if (enableDebugLogs)
            Debug.Log("[ARScanFeature] Back from quiz to scan panel");

        _openedFromARScan = false;

        if (quizPanel != null) quizPanel.SetActive(false);
        if (scanPanel != null) scanPanel.SetActive(true);

        ResetScanState();
    }

    public static bool IsOpenedFromARScan()
    {
        Debug.Log($"[ARScanFeature] IsOpenedFromARScan called - returning: {_openedFromARScan}");
        return _openedFromARScan;
    }

    public static void ClearARScanFlag()
    {
        _openedFromARScan = false;
        Debug.Log("[ARScanFeature] ClearARScanFlag called - set to FALSE");
    }

    private void ResetScanState()
    {
        hasDetectedObject = false;
        currentDetectedWord = null;
        isViewingDictionary = false;
        currentAnimalController = null; // NEW: Clear controller reference

        if (scanInstruction != null) scanInstruction.SetActive(true);
        if (scanFrame != null) scanFrame.SetActive(true);

        if (viewInfoButton != null) viewInfoButton.SetActive(false);
        if (walkButton != null) walkButton.SetActive(false);
        if (attackButton != null) attackButton.SetActive(false);
        if (resetButton != null) resetButton.SetActive(false);

        if (dictionaryButtonAR != null) dictionaryButtonAR.SetActive(false);
        if (quizButtonAR != null) quizButtonAR.SetActive(false);

        if (!isViewingDictionary)
        {
            if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
            if (dictionaryDetailViewPanel != null) dictionaryDetailViewPanel.SetActive(false);
            if (dictionaryListViewPanel != null) dictionaryListViewPanel.SetActive(false);
        }
    }
}