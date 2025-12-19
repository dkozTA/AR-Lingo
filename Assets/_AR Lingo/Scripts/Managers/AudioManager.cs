using UnityEngine;
using UnityEngine.Audio; // Bắt buộc để dùng Mixer

public class AudioManager : MonoBehaviour
{
    // Singleton: Đảm bảo chỉ có 1 AudioManager tồn tại
    public static AudioManager Instance;

    [Header("Components")]
    [Tooltip("Kéo file MainMixer vào đây")]
    public AudioMixer mainMixer;

    // THÊM MỚI: 2 Nguồn phát riêng biệt
    [Header("Audio Sources")]
    [Tooltip("Kéo AudioSource dành cho SFX vào đây")]
    public AudioSource sfxSource;

    [Tooltip("Kéo AudioSource dành cho Voice/Pronunciation vào đây")]
    public AudioSource voiceSource;

    [Header("Settings Keys")]
    // Tên tham số bạn đã đặt trong bước Expose Parameter
    private const string MIXER_SFX = "SFXVol";
    private const string MIXER_VOICE = "VoiceVol";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ cho nó sống khi chuyển cảnh
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Tải setting cũ khi bật game. Mặc định là 1 (Max volume)
        float savedSFX = PlayerPrefs.GetFloat("Settings_SFX", 1f);
        float savedVoice = PlayerPrefs.GetFloat("Settings_Voice", 1f);

        SetSFXLevel(savedSFX);
        SetVoiceLevel(savedVoice);
    }

    // --- HÀM CHO UI GỌI VÀO ---

    // Slider UI sẽ truyền giá trị từ 0 đến 1 vào đây
    public void SetSFXLevel(float sliderValue)
    {
        mainMixer.SetFloat(MIXER_SFX, LogarithmicDb(sliderValue));
        PlayerPrefs.SetFloat("Settings_SFX", sliderValue); // Lưu lại
        PlayerPrefs.Save();
    }

    public void SetVoiceLevel(float sliderValue)
    {
        mainMixer.SetFloat(MIXER_VOICE, LogarithmicDb(sliderValue));
        PlayerPrefs.SetFloat("Settings_Voice", sliderValue); // Lưu lại
        PlayerPrefs.Save();
    }

    // --- THÊM MỚI: HÀM PHÁT ÂM THANH ---

    public void PlaySFX(AudioClip clip, bool interrupt = false)
    {
        if (clip == null || sfxSource == null) return;

        // Nếu yêu cầu ngắt (interrupt = true), dừng tiếng đang kêu
        if (interrupt)
        {
            sfxSource.Stop();
        }

        // Phát tiếng mới
        sfxSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        if (clip != null && voiceSource != null)
        {
            // Với giọng đọc, ta nên dừng câu cũ trước khi đọc câu mới (tùy chọn)
            voiceSource.Stop();
            voiceSource.PlayOneShot(clip);
        }
    }

    // --- CÔNG THỨC TOÁN HỌC ---
    // Chuyển đổi 0-1 (Slider) thành -80dB đến 0dB (Mixer)
    private float LogarithmicDb(float linear)
    {
        linear = Mathf.Clamp(linear, 0.0001f, 1f); // Tránh log(0) gây lỗi
        return Mathf.Log10(linear) * 20;
    }
}