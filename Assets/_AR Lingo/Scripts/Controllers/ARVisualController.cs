using UnityEngine;
using System.Collections;
using TMPro;

public class ARVisualController : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Thời gian để con vật hiện ra hoàn toàn")]
    [SerializeField] private float spawnDuration = 0.5f;
    [Tooltip("Đường cong chuyển động (Nên chọn kiểu ElasticOut)")]
    [SerializeField] private AnimationCurve spawnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("UI Components")]
    [Tooltip("Kéo cái World Space Canvas trên đầu con vật vào đây")]
    [SerializeField] private Canvas infoCanvas;

    private Vector3 _originalScale;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _originalScale = transform.localScale; // Lưu lại kích thước chuẩn (ví dụ 1,1,1)
    }

    private void OnEnable()
    {
        // 1. Reset về bé tí hon
        transform.localScale = Vector3.zero;

        // 2. Bắt đầu hiệu ứng phình to ra
        StartCoroutine(SpawnAnimation());

        // 3. Nếu có UI, bật nó lên
        if (infoCanvas != null) infoCanvas.gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        // Giữ cho bảng tên luôn quay mặt về phía Camera
        if (infoCanvas != null && _mainCamera != null)
        {
            // Kỹ thuật Billboard: Chỉ xoay trục Y để chữ không bị nghiêng ngả
            infoCanvas.transform.LookAt(infoCanvas.transform.position + _mainCamera.transform.rotation * Vector3.forward,
            _mainCamera.transform.rotation * Vector3.up);
        }
    }

    private IEnumerator SpawnAnimation()
    {
        float timer = 0f;
        while (timer < spawnDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / spawnDuration;

            // Tính toán scale dựa trên đường cong
            float curveValue = spawnCurve.Evaluate(progress);
            transform.localScale = _originalScale * curveValue;

            yield return null;
        }
        transform.localScale = _originalScale; // Chốt hạ kích thước chuẩn
    }
}