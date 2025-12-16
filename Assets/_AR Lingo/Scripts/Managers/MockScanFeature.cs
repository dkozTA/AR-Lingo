using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MockScanFeature : MonoBehaviour
{
    [Header("1. Các vật thể")]
    public GameObject model3D;          // Kéo Mock_Apple vào đây
    public GameObject viewInfoButton;   // Kéo Btn_ViewInfo vào đây (Dictionary)
    public GameObject quizButton;       // (Tuỳ chọn) Kéo Btn_Quiz vào đây
    public GameObject resetButton;      // Kéo Btn_Reset / Rescan vào đây
    public GameObject stimulateButton;  // Kéo Btn_Stimulate / Scan vào đây
    public GameObject scanInstruction;  // Kéo cái Bottom_Panel (hướng dẫn quét) vào đây
    public GameObject scanFrame;        // Kéo Scan Frame (khung quét) vào đây

    [Header("2. Kết nối UI")]
    public GameObject dictionaryPanel; // Kéo Panel_Dictionary vào đây
    public GameObject scanPanel;       // Kéo Panel_Scan vào đây
    public DictionaryUI dictionaryUI;   // Reference to DictionaryUI component
    public GameObject dictionaryListViewPanel;  // Panel list (nếu dùng chế độ 2 panel)
    public GameObject dictionaryDetailViewPanel; // Panel detail (nếu tách riêng)
    public GameObject dictionaryListViewBackButton; // Button_Back in Panel_ListView (to hide when opening from AR)
    public GameObject dictionaryHeader;       // Header in Dictionary panel (to hide when opening from AR)
    
    [Header("3. Word Database")]
    public WordDatabase wordDatabase;  // Kéo WordDatabase asset vào đây
    public string detectedWordID = "animal_cow"; // ID của từ được phát hiện (sẽ được set khi AR scan)
    
    [Header("4. Legacy - Dữ liệu mẫu (Fallback nếu không dùng WordData)")]
    public string objectName = "Quả Táo Đỏ";
    [TextArea] public string objectDescription = "Đây là quả táo đỏ tươi, rất ngon và giàu vitamin.";

    [Header("5. Mock Scan Settings")]
    public float fakeScanDelay = 2.0f;   // Thời gian chờ giả lập (2-3s)
    public GameObject scanFX;           // (Tuỳ chọn) Hiệu ứng particle / khung quét

    private WordData currentDetectedWord; // Word data của vật thể đang được phát hiện
    private bool isScanning = false;

    // --- HÀM 1: Gắn vào nút SIMULATE / SCAN ---
    public void OnSimulateClicked()
    {
        if (isScanning) return; // Đang quét rồi thì bỏ qua
        StartCoroutine(MockScanRoutine());
    }

    // --- HÀM 2: Gắn vào nút VIEW INFO (Xem từ điển) ---
    public void OnOpenDictionaryClicked()
    {
        // 1. Chuyển màn hình
        scanPanel.SetActive(false);
        dictionaryPanel.SetActive(true);

        // Bật panel Detail, tắt panel List nếu có cấu hình
        if (dictionaryDetailViewPanel != null) 
            dictionaryDetailViewPanel.SetActive(true);
        if (dictionaryListViewPanel != null) 
            dictionaryListViewPanel.SetActive(false);

        // Hide the ListView's back button when opening from AR scan
        if (dictionaryListViewBackButton != null)
            dictionaryListViewBackButton.SetActive(false);

        // Hide the header when opening from AR scan
        if (dictionaryHeader != null)
            dictionaryHeader.SetActive(false);

        // 2. Điền dữ liệu vào Dictionary
        if (dictionaryUI != null)
        {
            dictionaryUI.gameObject.SetActive(true); // Đảm bảo panel detail hiện
            if (currentDetectedWord != null)
            {
                dictionaryUI.DisplayWord(currentDetectedWord, fromARView: true);
            }
            else
            {
                // Fallback: dùng dữ liệu legacy (objectName/objectDescription) nếu chưa có WordData
                var temp = ScriptableObject.CreateInstance<WordData>();
                temp.englishName = string.IsNullOrEmpty(objectName) ? "Unknown" : objectName;
                temp.vietnameseName = string.IsNullOrEmpty(objectName) ? "Không rõ" : objectName;
                temp.description = objectDescription;
                dictionaryUI.DisplayWord(temp, fromARView: true);
                Debug.LogWarning("Dictionary opened with legacy data (no WordData set).");
            }
        }
        else
        {
            Debug.LogWarning("DictionaryUI component not found! Make sure to assign it in the inspector.");
        }
        
        // DON'T reset scan state here - only reset when actually going back
    }

    /// <summary>
    /// Set the detected word when AR camera detects a card
    /// This should be called by your Vuforia AR detection system
    /// </summary>
    public void SetDetectedWord(string wordID)
    {
        detectedWordID = wordID;
        if (wordDatabase != null)
        {
            currentDetectedWord = wordDatabase.GetWordByID(wordID);
            if (currentDetectedWord != null)
            {
                Debug.Log($"Word detected: {currentDetectedWord.englishName} ({currentDetectedWord.vietnameseName})");
            }
        }
    }

    // --- HÀM 4: Gắn vào nút RESET / RESCAN ---
    public void OnResetClicked()
    {
        ResetScanState();
    }

    /// <summary>
    /// Đưa màn Scan về trạng thái ban đầu (chưa quét gì)
    /// </summary>
    private void ResetScanState()
    {
        isScanning = false;
        currentDetectedWord = null;

        // Hiện panel hướng dẫn và scan frame
        if (scanInstruction != null) scanInstruction.SetActive(true);
        if (scanFrame != null) scanFrame.SetActive(true);

        // Ẩn model 3D và các nút
        if (model3D != null) model3D.SetActive(false);
        if (viewInfoButton != null) viewInfoButton.SetActive(false);
        if (quizButton != null) quizButton.SetActive(false);
        
        // Reset button should be HIDDEN in initial state (only show when object is detected)
        if (resetButton != null) resetButton.SetActive(false);
        
        // Stimulate button should be VISIBLE in initial state
        if (stimulateButton != null) stimulateButton.SetActive(true);

        // Tắt hiệu ứng scan
        if (scanFX != null) scanFX.SetActive(false);
        
        // Also hide dictionary panels when resetting
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
        if (dictionaryDetailViewPanel != null) dictionaryDetailViewPanel.SetActive(false);
    }

    /// <summary>
    /// Coroutine giả lập quá trình quét: hiện FX + chờ 2-3s rồi mới thấy model 3D
    /// </summary>
    private System.Collections.IEnumerator MockScanRoutine()
    {
        isScanning = true;

        // Bật hướng dẫn + hiệu ứng quét + scan frame
        if (scanInstruction != null) scanInstruction.SetActive(true);
        if (scanFX != null) scanFX.SetActive(true);
        if (scanFrame != null) scanFrame.SetActive(true); // Show scan frame during scanning
        if (stimulateButton != null) stimulateButton.SetActive(false); // Ẩn nút Scan khi đang quét
        if (resetButton != null) resetButton.SetActive(false); // Ẩn nút Reset khi đang quét

        // Ẩn model & các nút trong lúc đang quét
        if (model3D != null) model3D.SetActive(false);
        if (viewInfoButton != null) viewInfoButton.SetActive(false);
        if (quizButton != null) quizButton.SetActive(false);

        // Chờ thời gian giả lập
        yield return new WaitForSeconds(fakeScanDelay);

        // Giả lập: đã nhận diện xong object
        if (scanInstruction != null) scanInstruction.SetActive(false);
        if (scanFX != null) scanFX.SetActive(false);
        if (scanFrame != null) scanFrame.SetActive(false); // Hide scan frame when model is detected

        if (model3D != null) model3D.SetActive(true);

        // Lấy dữ liệu từ database
        if (wordDatabase != null && !string.IsNullOrEmpty(detectedWordID))
        {
            currentDetectedWord = wordDatabase.GetWordByID(detectedWordID);
            if (currentDetectedWord != null)
            {
                Debug.Log($"Detected word: {currentDetectedWord.englishName}");
            }
        }

        // After detection: Hide Stimulate button, show Reset button
        if (stimulateButton != null) stimulateButton.SetActive(false);
        if (resetButton != null) resetButton.SetActive(true);

        // Cho phép bấm Dictionary / Quiz sau khi người dùng chạm vào model (xử lý trong Update)
        isScanning = false;
    }

    // --- HÀM 3: Tự động phát hiện click vào vật thể 3D ---
    void Update()
    {
        // Nếu vật thể đang hiện, kiểm tra chuột click
        if (model3D != null && model3D.activeSelf && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == model3D)
                {
                    Debug.Log("Đã bấm vào quả táo!");
                    if (viewInfoButton != null) viewInfoButton.SetActive(true); // Hiện nút xem thông tin
                    if (quizButton != null) quizButton.SetActive(true); // Hiện nút quiz (optional)
                }
            }
        }
    }
}