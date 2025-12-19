using UnityEngine;

/// <summary>
/// Script này dùng cho trường hợp Model ĐÃ ĐƯỢC GẮN LÀM CON của Image Target.
/// Cập nhật: Đã tích hợp với hệ thống AudioManager tập trung.
/// </summary>
public class SimpleARTrigger : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Kéo object Model (con) vào đây (Vd: PF_Cow)")]
    [SerializeField] private GameObject linkedModel;

    [Header("Optional Audio")]
    [Tooltip("Kéo file âm thanh đọc tên vào đây (Snd_Name_Cow). ĐỂ TRỐNG nếu Model đã tự phát tiếng khi hiện!")]
    [SerializeField] private AudioClip namePronunciation;

    private void Start()
    {
        // 1. Luôn ẩn model lúc bắt đầu game
        if (linkedModel != null)
        {
            linkedModel.SetActive(false);
        }
        else
        {
            Debug.LogError($"[SimpleARTrigger] {gameObject.name} chưa được gắn Model con!");
        }
    }

    // --- GẮN VÀO SỰ KIỆN ON TARGET FOUND CỦA VUFORIA ---
    public void OnTargetFound()
    {
        Debug.Log($"[SimpleARTrigger] Tìm thấy: {gameObject.name}");

        // 1. Hiện Model lên
        if (linkedModel != null)
        {
            linkedModel.SetActive(true);
            // LƯU Ý: Nếu linkedModel có script AnimalInteraction, nó sẽ tự kích hoạt OnEnable và phát tiếng ở đó.
        }

        // 2. (Tùy chọn) Gọi AudioManager phát tiếng
        // Chỉ chạy nếu bạn ĐÃ GẮN AudioClip vào biến namePronunciation
        if (namePronunciation != null)
        {
            if (AudioManager.Instance != null)
            {
                // Dùng kênh Voice cho giọng đọc tên
                AudioManager.Instance.PlayVoice(namePronunciation);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy AudioManager! Hãy đảm bảo đã tạo nó trong Scene.");
            }
        }
    }

    // --- GẮN VÀO SỰ KIỆN ON TARGET LOST CỦA VUFORIA ---
    public void OnTargetLost()
    {
        Debug.Log($"[SimpleARTrigger] Mất dấu: {gameObject.name}");

        // Ẩn Model đi
        if (linkedModel != null)
        {
            linkedModel.SetActive(false);
        }

        // Không cần stop âm thanh. 
        // Nếu đang đọc dở "This is a...", cứ để nó đọc hết câu cho tự nhiên.
    }
}