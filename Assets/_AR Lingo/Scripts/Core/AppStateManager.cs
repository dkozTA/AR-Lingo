using UnityEngine;
using System;

public enum AppState
{
    Home,
    ARScanning,     // Màn hình camera, đang tìm thẻ
    ARObjectFound,  // Đã thấy thẻ, hiện nút Quiz/Dictionary
    Quiz,
    Dictionary
}

public class AppStateManager : MonoBehaviour
{
    public static AppStateManager Instance { get; private set; }

    [Header("Debug Info")]
    [SerializeField] private AppState _currentState;
    [SerializeField] private string _currentAnimalID; // Biến để debug xem đang nhận con gì

    // --- EVENTS (CHO UI LẮNG NGHE) ---
    // 1. Báo tin khi chuyển màn hình (VD: Tắt Home, Bật AR View)
    public event Action<AppState> OnStateChanged;

    // 2. Báo tin khi xác định được con vật cụ thể (để UI load nội dung tương ứng)
    public event Action<string> OnAnimalDetected;

    // 3. Báo tin khi AR target bị mất (camera không còn nhìn thấy thẻ)
    public event Action OnARObjectLost;

    // Property để các script khác có thể lấy tên con vật hiện tại bất cứ lúc nào
    public string CurrentAnimalID => _currentAnimalID;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Mặc định vào game là ở màn hình Home
        ChangeState(AppState.Home);
    }

    // --- CORE LOGIC: QUẢN LÝ TRẠNG THÁI ---

    public void ChangeState(AppState newState)
    {
        _currentState = newState;
        Debug.Log($"[GameManager] State Changed to: {newState}");

        // Bắn tín hiệu cho UIManager biết để bật/tắt Canvas
        OnStateChanged?.Invoke(newState);
    }

    // --- CORE LOGIC: GIAO TIẾP VỚI AR (VUFORIA) ---

    // Hàm này được gọi từ script ARContentTrigger
    public void OnARObjectDetected(string animalID)
    {
        // 1. Lưu lại ID (VD: "Cow", "Fox")
        _currentAnimalID = animalID;

        // 2. Chuyển State sang chế độ tương tác
        ChangeState(AppState.ARObjectFound);

        // 3. Bắn event kèm dữ liệu để UI cập nhật text/âm thanh
        Debug.Log($"[GameManager] Detected: {animalID}. Notify UI to update data.");
        OnAnimalDetected?.Invoke(animalID);
    }

    // Hàm này được gọi từ script ARContentTrigger khi mất target
    public void NotifyARObjectLost()
    {
        // Khi mất dấu thẻ, quay về trạng thái Scanning
        Debug.Log("[GameManager] Target Lost. Resetting to Scan mode.");

        _currentAnimalID = null; // Xóa dữ liệu cũ
        ChangeState(AppState.ARScanning);

        // Bắn event để UI biết
        OnARObjectLost?.Invoke();
    }

    // --- HÀM ĐIỀU KHIỂN LUỒNG (Flow Control) ---

    public void StartARScan()
    {
        // Được gọi khi bấm nút "Start" ở Home Screen
        Debug.Log("[GameManager] Starting AR Session...");
        ChangeState(AppState.ARScanning);

        // LƯU Ý: Đã xóa đoạn Invoke Mockup. 
        // Giờ đây game sẽ đợi tín hiệu thật từ Camera/ARContentTrigger.
    }
}