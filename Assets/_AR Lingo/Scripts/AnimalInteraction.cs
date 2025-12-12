using UnityEngine;

public class AnimalInteraction : MonoBehaviour
{
    public AudioClip animalSound; // Kéo file mp3 vào đây
    public AudioClip nameSound;   // Kéo file đọc tên vào đây (nếu muốn)
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // Đảm bảo có Collider để nhận tương tác
        if (GetComponent<Collider>() == null)
        {
             Debug.LogError("Thiếu Collider trên con vật: " + gameObject.name);
        }
    }

    void OnMouseDown()
    {
        // Hàm này tự động chạy khi click chuột hoặc chạm ngón tay vào vật thể có Collider
        if (animalSound != null)
        {
            audioSource.PlayOneShot(animalSound);
            Debug.Log("Đã chạm vào: " + gameObject.name);
        }
    }
}