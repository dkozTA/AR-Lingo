using UnityEngine;
using System; // Để dùng Action (Event)

public enum AppState
{
    Home,
    ARScanning,     // Đang tìm mặt phẳng
    ARObjectFound,  // Đã hiện thú, hiện UI tương tác
    Quiz,
    Dictionary
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private AppState _currentState;

    // Event để UI lắng nghe và thay đổi theo (Observer Pattern)
    public event Action<AppState> OnStateChanged;

    private void Awake()
    {
        // Singleton Pattern chuẩn chỉ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Giữ sống qua các Scene
    }

    private void Start()
    {
        ChangeState(AppState.Home); // Khởi đầu ở Home
    }

    public void ChangeState(AppState newState)
    {
        _currentState = newState;
        Debug.Log($"[GameManager] State Changed to: {newState}");

        // Bắn event báo cho UI biết
        OnStateChanged?.Invoke(newState);
    }

    public void OnARObjectDetected(string animalID)
    {
        // Chuyển State để UI biết mà hiện nút Dictionary/Quiz
        ChangeState(AppState.ARObjectFound);

        Debug.Log($"[GameManager] UI should show controls for: {animalID}");
        // TODO: Gửi animalID sang cho UIManager để load đúng data (Tên, Tiếng Anh...)
    }

    public void OnARObjectLost()
    {
        // Quay về trạng thái quét
        ChangeState(AppState.ARScanning);
    }

    // --- CÁC HÀM MOCKUP LOGIC (GIẢ LẬP) ---

    public void StartARScan()
    {
        // Giả vờ chuyển sang màn hình Scan
        ChangeState(AppState.ARScanning);

        // MOCK: Sau 2 giây tự tìm thấy con thú (để test UI khi chưa có AR thật)
        Invoke(nameof(MockObjectFound), 2.0f);
    }

    private void MockObjectFound()
    {
        if (_currentState == AppState.ARScanning)
        {
            ChangeState(AppState.ARObjectFound);
        }
    }
}