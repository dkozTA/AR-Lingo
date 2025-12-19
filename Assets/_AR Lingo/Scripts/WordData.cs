using UnityEngine;

// Menu giúp click chuột phải tạo file nhanh: Create -> AR Lingo -> Word Data
[CreateAssetMenu(fileName = "NewWordData", menuName = "AR Lingo/Word Data")]
public class WordData : ScriptableObject
{
    [Header("Identity")]
    public string id;              // ID duy nhất (vd: "animal_lion")

    [Header("Category")]
    public WordCategory category;  // NEW: Loại từ (Animal, Plant, Object...)

    [Header("Content")]
    public string englishName;     // Vd: Lion
    public string vietnameseName;  // Vd: Sư tử
    public string pronunciation;   // NEW: Phiên âm (vd: "/ˈlaɪən/")
    [TextArea]
    public string description;     // Mô tả ngắn
    [TextArea]
    public string vietnameseDescription; // NEW: Mô tả tiếng Việt

    [Header("Assets")]
    public GameObject modelPrefab; // Prefab 3D con vật (có Animator)
    public Sprite icon2D;          // Ảnh đại diện cho Dictionary
    public AudioClip audioPronounce; // File âm thanh đọc tên (Anh/Việt)
    public AudioClip audioSFX;       // Tiếng kêu con vật (Gầm, sủa...)

    [Header("Animation Settings (Only for Animals)")]
    public bool hasAnimations;     // NEW: Check if this has animations (auto-set based on category)
    public string walkAnimationName = "Walk"; // NEW: Tên animation Walk
    public string attackAnimationName = "Attack"; // NEW: Tên animation Attack

    // Helper: Lấy tên theo ngôn ngữ đang chọn (Sprint sau sẽ cần)
    public string GetName(bool isEnglish)
    {
        return isEnglish ? englishName : vietnameseName;
    }

    // Helper: Check if this word supports animations (only animals)
    public bool SupportsAnimations()
    {
        return category == WordCategory.Animal && hasAnimations;
    }

    // Validate on editor changes
    private void OnValidate()
    {
        // Auto-set hasAnimations based on category
        hasAnimations = (category == WordCategory.Animal);
    }
}

// NEW: Enum for word categories
public enum WordCategory
{
    Animal,    // Con vật - có animation
    Plant,     // Thực vật - không có animation
    Object,    // Đồ vật - không có animation
    Food       // Đồ ăn - không có animation
}