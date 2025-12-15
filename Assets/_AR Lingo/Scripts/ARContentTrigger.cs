using UnityEngine;
using Vuforia; // Nếu chưa cài Vuforia thì comment dòng này lại

public class ARContentTrigger : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private string animalName = "Cow"; // Tên để gửi cho UI
    [SerializeField] private GameObject animalModel;    // Kéo Pf_Cow vào đây

    [Header("Debug Simulation")]
    public bool isTestMode = true; // Bật cái này để test bằng phím Space

    // --- LOGIC XỬ LÝ KHI TÌM THẤY TARGET ---
    public void OnTargetFound()
    {
        Debug.Log($"[AR] Found Target: {animalName}");

        // 1. Hiện con thú
        if (animalModel != null) animalModel.SetActive(true);

        // 2. Báo cho GameManager biết là đã tìm thấy -> Để nó bật UI
        // (Giả sử GameManager có hàm này, tôi sẽ update GameManager bên dưới)
        GameManager.Instance.OnARObjectDetected(animalName);
    }

    public void OnTargetLost()
    {
        Debug.Log($"[AR] Lost Target: {animalName}");

        // 1. Ẩn con thú
        if (animalModel != null) animalModel.SetActive(false);

        // 2. Báo GameManager quay về trạng thái Scan
        GameManager.Instance.OnARObjectLost();
    }

    // --- GIẢ LẬP (MOCKING) CHO UI DEV ---
    private void Update()
    {
        if (isTestMode)
        {
            // Ấn phím F (Found) để giả lập tìm thấy
            if (Input.GetKeyDown(KeyCode.F)) OnTargetFound();

            // Ấn phím L (Lost) để giả lập mất dấu
            if (Input.GetKeyDown(KeyCode.L)) OnTargetLost();
        }
    }
}