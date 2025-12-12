using UnityEngine;

// Menu giúp click chuột phải tạo file nhanh: Create -> AR Lingo -> Word Data
[CreateAssetMenu(fileName = "NewWordData", menuName = "AR Lingo/Word Data")]
public class WordData : ScriptableObject
{
    [Header("Identity")]
    public string id;              // ID duy nhất (vd: "animal_lion")

    [Header("Content")]
    public string englishName;     // Vd: Lion
    public string vietnameseName;  // Vd: Sư tử
    [TextArea]
    public string description;     // Mô tả ngắn

    [Header("Assets")]
    public GameObject modelPrefab; // Prefab 3D con vật (có Animator)
    public Sprite icon2D;          // Ảnh đại diện cho Dictionary
    public AudioClip audioPronounce; // File âm thanh đọc tên (Anh/Việt)
    public AudioClip audioSFX;       // Tiếng kêu con vật (Gầm, sủa...)

    // Helper: Lấy tên theo ngôn ngữ đang chọn (Sprint sau sẽ cần)
    public string GetName(bool isEnglish)
    {
        return isEnglish ? englishName : vietnameseName;
    }
}