using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Tự động thêm AudioSource nếu quên
[RequireComponent(typeof(Collider))]    // Bắt buộc có Collider
public class AnimalInteraction : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip sfxAnimal; // Đổi tên cho rõ nghĩa
    [SerializeField] private AudioClip sfxName;   // Voice đọc tên

    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        PlayInteraction(); // Gọi hàm phát tiếng ngay lập tức
    }

    private void OnMouseDown()
    {
        // TODO: Sau này sẽ thay bằng AR Raycast system
        PlayInteraction();
    }

    public void PlayInteraction()
    {
        if (sfxAnimal != null)
        {
            // Stop âm thanh cũ nếu đang chạy để tránh ồn
            if (_audioSource.isPlaying) _audioSource.Stop();

            _audioSource.PlayOneShot(sfxAnimal);
            Debug.Log($"[AnimalInteraction] Playing SFX for: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"[AnimalInteraction] Missing SFX for: {gameObject.name}");
        }
    }
}