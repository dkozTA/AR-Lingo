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
        homePanel.SetActive(false); // Tắt Home
        scanPanel.SetActive(true);  // Bật Scan
    }

    public void OpenDictionary()
    {
        homePanel.SetActive(false);
        dictionaryPanel.SetActive(true);
    }

    public void OpenQuiz()
    {
        homePanel.SetActive(false);
        quizPanel.SetActive(true);
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
        // Stop playing in Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Quit application on device
        Application.Quit();
        #endif
    }

    // --- HÀM CHO NÚT BACK ---

    public void BackToHome()
    {
        // Tắt hết tất cả các panel con đi cho chắc ăn
        scanPanel.SetActive(false);
        dictionaryPanel.SetActive(false);
        quizPanel.SetActive(false);
        settingPanel.SetActive(false);

        // Bật lại Home
        homePanel.SetActive(true);
    }
}