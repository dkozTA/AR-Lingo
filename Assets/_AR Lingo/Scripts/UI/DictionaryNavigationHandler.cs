using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles back button navigation in Dictionary panel
/// Determines whether to go back to List View or AR Scan panel
/// </summary>
public class DictionaryNavigationHandler : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject dictionaryPanel;
    public GameObject scanPanel;
    public GameObject homePanel;

    [Header("Components")]
    public DictionaryListView listView;
    public SimpleMenuController menuController;
    public ARScanFeature arScanFeature;

    [Header("Button")]
    public Button backButton;

    void OnEnable()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners(); // Clear any existing listeners
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    void OnDisable()
    {
        if (backButton != null)
        {
            backButton.onClick.RemoveListener(OnBackClicked);
        }
    }

    public void OnBackClicked()
    {
        // Check if opened from AR Scan using static flag
        bool openedFromAR = ARScanFeature.IsOpenedFromARScan();
        
        Debug.Log($"[DictionaryNavigationHandler] Back clicked. OpenedFromAR: {openedFromAR}");

        if (openedFromAR)
        {
            // Came from AR Scan → Go back to AR Scan panel
            Debug.Log("[DictionaryNavigationHandler] Returning to AR Scan panel");
            
            if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
            if (scanPanel != null) scanPanel.SetActive(true);

            if (arScanFeature != null)
            {
                arScanFeature.OnBackFromDictionary();
            }
        }
        else
        {
            // Came from Home menu → Go back to List View (or Home if already in list view)
            Debug.Log("[DictionaryNavigationHandler] Returning to List View or Home");
            
            if (listView != null)
            {
                // Check if we're currently in detail view
                if (listView.IsInDetailView())
                {
                    // In detail view → go back to list view
                    listView.BackToListView();
                }
                else
                {
                    // Already in list view → go back to home
                    if (menuController != null)
                    {
                        menuController.BackToHome();
                    }
                    else
                    {
                        // Fallback: manually hide dictionary and show home
                        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
                        if (homePanel != null) homePanel.SetActive(true);
                    }
                }
            }
            else
            {
                Debug.LogWarning("DictionaryNavigationHandler: listView reference is not assigned!");
            }
        }
    }
}