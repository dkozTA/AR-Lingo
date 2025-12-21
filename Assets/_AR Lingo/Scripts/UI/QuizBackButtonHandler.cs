using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles Back button in Quiz panel - navigates to correct panel based on source
/// </summary>
public class QuizBackButtonHandler : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject quizPanel;
    public GameObject scanPanel;
    public GameObject homePanel;
    public SimpleMenuController menuController;
    public ARScanFeature arScanFeature;

    [Header("Button")]
    public Button backButton; // Keep for reference, but don't add listener in code

    // The button is already connected in Unity Inspector, so we don't need to add listener in code

    public void OnBackClicked()
    {
        Debug.Log("[QuizBackButtonHandler] ========== OnBackClicked START ==========");
        
        // Check if opened from AR Scan using static flag
        bool openedFromAR = ARScanFeature.IsOpenedFromARScan();
        
        Debug.Log($"[QuizBackButtonHandler] Back clicked. OpenedFromAR: {openedFromAR}");

        if (openedFromAR)
        {
            // Came from AR Scan → Go back to AR Scan panel
            Debug.Log("[QuizBackButtonHandler] Going to AR Scan panel (openedFromAR = TRUE)");
            
            if (quizPanel != null) 
            {
                quizPanel.SetActive(false);
                Debug.Log("[QuizBackButtonHandler] Quiz panel deactivated");
            }
            
            if (scanPanel != null) 
            {
                scanPanel.SetActive(true);
                Debug.Log("[QuizBackButtonHandler] Scan panel activated");
            }

            if (arScanFeature != null)
            {
                arScanFeature.OnBackFromQuiz();
                Debug.Log("[QuizBackButtonHandler] Called arScanFeature.OnBackFromQuiz()");
            }
            else
            {
                Debug.LogError("[QuizBackButtonHandler] arScanFeature is NULL!");
            }
        }
        else
        {
            // Came from Home menu → Go back to Home
            Debug.Log("[QuizBackButtonHandler] Going to Home panel (openedFromAR = FALSE)");
            
            if (menuController != null)
            {
                menuController.BackToHome();
                Debug.Log("[QuizBackButtonHandler] Called menuController.BackToHome()");
            }
            else
            {
                Debug.LogWarning("[QuizBackButtonHandler] menuController is NULL - using fallback");
                
                // Fallback: manually hide quiz and show home
                if (quizPanel != null) quizPanel.SetActive(false);
                if (homePanel != null) homePanel.SetActive(true);
            }
        }
        
        Debug.Log("[QuizBackButtonHandler] ========== OnBackClicked END ==========");
    }
}