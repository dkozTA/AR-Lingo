using UnityEngine;

[RequireComponent(typeof(Animator))] // Bắt buộc phải có Animator
[RequireComponent(typeof(Collider))]
public class AnimalInteraction : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Tiếng kêu động vật (Vd: Moo, Bark)")]
    [SerializeField] private AudioClip sfxAnimal;

    [Tooltip("Tiếng đọc tên (Vd: This is a Cow)")]
    [SerializeField] private AudioClip sfxName;

    [Header("Animation Settings")]
    [Tooltip("Tên Trigger trong Animator Controller (Vd: DoAction)")]
    [SerializeField] private string actionTriggerName = "DoAction";

    private Animator _animator;

    void Awake()
    {
        // Tự động tìm Animator trên chính object này hoặc object con
        _animator = GetComponent<Animator>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
    }

    // Hàm này chạy khi Camera soi thấy thẻ (Image Target bật Model lên)
    private void OnEnable()
    {
        // 1. Reset Animation về Idle (để tránh bị kẹt ở pose cũ)
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        // 2. GỌI AUDIO MANAGER PHÁT VOICE
        // Thay vì tự phát, hãy nhờ Sếp (Manager) phát đúng kênh Voice
        if (sfxName != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayVoice(sfxName);
        }
    }

    // Hàm này chạy khi người dùng BẤM vào con vật
    private void OnMouseDown()
    {
        PerformAction();
    }

    public void PerformAction()
    {
        // 1. GỌI AUDIO MANAGER PHÁT SFX
        // Nhờ Sếp phát đúng kênh SFX
        if (sfxAnimal != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(sfxAnimal, true);
        }

        // 2. Kích hoạt Animation Action
        if (_animator != null)
        {
            // Reset trigger cũ để tránh lặp lệnh (Optional)
            _animator.ResetTrigger(actionTriggerName);
            _animator.SetTrigger(actionTriggerName);
            // Debug.Log($"[AnimalInteraction] Triggered animation: {actionTriggerName}");
        }
    }
}