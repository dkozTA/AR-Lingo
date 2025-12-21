using UnityEngine;
using UnityEngine.UI;

// IMPORTANT: Only import UnityEditor in Editor mode
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SimpleMenuController : MonoBehaviour
{
    [Header("Kéo các Panel vào đây")]
    public GameObject homePanel;
    public GameObject scanPanel;
    public GameObject dictionaryPanel;
    public GameObject quizPanel;
    public GameObject settingPanel;

    // --- HÀM CHO CÁC NÚT Ở MÀN HÌNH HOME ---

    public void OpenScan()
    {
        homePanel.SetActive(false);
        scanPanel.SetActive(true);
        
        ARScanFeature.ClearARScanFlag();
    }

    public void OpenDictionary()
    {
        homePanel.SetActive(false);
        dictionaryPanel.SetActive(true);
        
        ARScanFeature.ClearARScanFlag();
    }

    public void OpenQuiz()
    {
        homePanel.SetActive(false);
        quizPanel.SetActive(true);
        
        ARScanFeature.ClearARScanFlag();
    }

    public void OpenSetting()
    {
        homePanel.SetActive(false);
        settingPanel.SetActive(true);
    }

    /// <summary>
    /// Quit the application
    /// </summary>
    public void QuitApp()
    {
        Debug.Log("[SimpleMenuController] Quitting application...");

        #if UNITY_EDITOR
        // In Unity Editor: Stop play mode
        EditorApplication.isPlaying = false;
        #else
        // On device/build: Quit application
        Application.Quit();
        #endif
    }

    // --- HÀM CHO NÚT BACK ---

    public void BackToHome()
    {
        ARScanFeature.ClearARScanFlag();
        
        scanPanel.SetActive(false);
        dictionaryPanel.SetActive(false);
        quizPanel.SetActive(false);
        settingPanel.SetActive(false);

        homePanel.SetActive(true);
    }
}