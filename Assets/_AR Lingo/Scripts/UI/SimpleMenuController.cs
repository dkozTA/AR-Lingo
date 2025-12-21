using UnityEngine;
using UnityEngine.UI;

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

        Application.Quit();
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