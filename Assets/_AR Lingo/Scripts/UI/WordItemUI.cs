using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI component for a single word item in the dictionary grid
/// </summary>
public class WordItemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI englishNameText;
    [SerializeField] private TextMeshProUGUI vietnameseNameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button itemButton;

    public WordData WordData { get; private set; }
    private DictionaryListView listView;

    void Awake()
    {
        if (itemButton != null)
            itemButton.onClick.AddListener(OnItemClicked);
    }

    public void Setup(WordData wordData, DictionaryListView parentListView)
    {
        WordData = wordData;
        listView = parentListView;

        if (wordData == null)
            return;

        if (englishNameText != null)
            englishNameText.text = wordData.englishName;

        if (vietnameseNameText != null)
            vietnameseNameText.text = wordData.vietnameseName;

        if (iconImage != null && wordData.icon2D != null)
        {
            iconImage.sprite = wordData.icon2D;
            iconImage.gameObject.SetActive(true);
        }
        else if (iconImage != null)
        {
            iconImage.gameObject.SetActive(false);
        }
    }

    private void OnItemClicked()
    {
        if (listView != null && WordData != null)
        {
            listView.ShowWordDetail(WordData);
        }
    }
}

