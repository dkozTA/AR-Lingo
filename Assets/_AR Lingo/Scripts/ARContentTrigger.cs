using UnityEngine;
using Vuforia;

/// <summary>
/// AR Content Trigger - Handles Vuforia image target detection
/// This script should be attached to each Image Target prefab
/// The OnTargetFound/OnTargetLost methods are called by DefaultObserverEventHandler via Unity Events
/// </summary>
public class ARContentTrigger : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private string wordID = "Bull"; // Must match WordDatabase ID
    [SerializeField] private GameObject ar3DModel; // The 3D model (e.g., PF_Bull)

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private void Start()
    {
        if (enableDebugLogs)
            Debug.Log($"[ARContentTrigger] Initialized for target: {wordID}");
        
        if (ar3DModel != null)
        {
            // DEBUG: Log model info at start
            Debug.Log($"[ARContentTrigger] Model Name: {ar3DModel.name}");
            Debug.Log($"[ARContentTrigger] Model Position: {ar3DModel.transform.localPosition}");
            Debug.Log($"[ARContentTrigger] Model Scale: {ar3DModel.transform.localScale}");
            Debug.Log($"[ARContentTrigger] Model Active Self: {ar3DModel.activeSelf}");
            
            // Check for renderer
            Renderer[] renderers = ar3DModel.GetComponentsInChildren<Renderer>();
            Debug.Log($"[ARContentTrigger] Found {renderers.Length} renderers in model");
            foreach (var r in renderers)
            {
                Debug.Log($"[ARContentTrigger] Renderer: {r.name}, Enabled: {r.enabled}, Material: {(r.material != null ? r.material.name : "None")}");
            }
            
            // Make sure model starts hidden
            ar3DModel.SetActive(false);
            if (enableDebugLogs)
                Debug.Log($"[ARContentTrigger] 3D Model '{ar3DModel.name}' set to inactive");
        }
        else
        {
            Debug.LogError($"[ARContentTrigger] AR 3D Model is not assigned for {wordID}!");
        }
    }

    /// <summary>
    /// Called by DefaultObserverEventHandler when target is found
    /// This is connected via Unity Events in the Inspector
    /// </summary>
    public void OnTargetFound()
    {
        if (enableDebugLogs)
            Debug.Log($"[ARContentTrigger] ✓ Target FOUND: {wordID}");

        // Show the 3D model
        if (ar3DModel != null)
        {
            ar3DModel.SetActive(true);
            
            // DEBUG: Check after activation
            Debug.Log($"[ARContentTrigger] ✓ Model activated - Active: {ar3DModel.activeSelf}, ActiveInHierarchy: {ar3DModel.activeInHierarchy}");
            Debug.Log($"[ARContentTrigger] ✓ Model World Position: {ar3DModel.transform.position}");
            Debug.Log($"[ARContentTrigger] ✓ ImageTarget Position: {transform.position}");
            
            if (enableDebugLogs)
                Debug.Log($"[ARContentTrigger] ✓ 3D Model '{ar3DModel.name}' activated");
        }
        else
        {
            Debug.LogError($"[ARContentTrigger] Cannot show model - ar3DModel is null!");
        }

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnARObjectDetected(wordID);
            if (enableDebugLogs)
                Debug.Log($"[ARContentTrigger] ✓ Notified GameManager about {wordID}");
        }
        else
        {
            Debug.LogWarning("[ARContentTrigger] GameManager not found!");
        }
    }

    /// <summary>
    /// Called by DefaultObserverEventHandler when target is lost
    /// This is connected via Unity Events in the Inspector
    /// </summary>
    public void OnTargetLost()
    {
        if (enableDebugLogs)
            Debug.Log($"[ARContentTrigger] ✗ Target LOST: {wordID}");

        // Hide the 3D model
        if (ar3DModel != null)
        {
            ar3DModel.SetActive(false);
            if (enableDebugLogs)
                Debug.Log($"[ARContentTrigger] ✗ 3D Model '{ar3DModel.name}' deactivated");
        }

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyARObjectLost();
        }
    }
}