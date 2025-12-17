using UnityEngine;
using Vuforia;

public class ARContentTrigger : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private string animalName = "Cow";
    [SerializeField] private GameObject animalModel;    // Kéo Pf_Cow vào đây

    [Header("Debug Simulation")]
    public bool isTestMode = true;

    // --- LOGIC XỬ LÝ KHI TÌM THẤY TARGET ---
    public void OnTargetFound()
    {
        Debug.Log($"[AR] Found Target: {animalName}");

        // 1. Hiện con thú
        if (animalModel != null)
        {
            animalModel.SetActive(true);
        }

        // 2. Báo cho GameManager (Thêm kiểm tra Instance != null để tránh lỗi)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnARObjectDetected(animalName);
        }
        else
        {
            // Cảnh báo nhẹ nếu quên chưa bật GameManager
            Debug.LogWarning("GameManager chưa được khởi tạo, nhưng AR vẫn chạy.");
        }
    }

    public void OnTargetLost()
    {
        Debug.Log($"[AR] Lost Target: {animalName}");

        // 1. Ẩn con thú
        if (animalModel != null)
        {
            animalModel.SetActive(false);
        }

        // 2. Báo GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnARObjectLost();
        }
    }

    private void Update()
    {
        
    }
}