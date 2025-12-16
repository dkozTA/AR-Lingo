using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Dictionary List View - Shows a scrollable grid of all words when opening Dictionary from Home
/// </summary>
public class DictionaryListView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform wordListContainer; // ScrollView Content transform (must have Grid Layout Group)
    [SerializeField] private GameObject wordItemPrefab;   // Prefab for each word item in the grid
    [SerializeField] private DictionaryUI dictionaryDetailView; // Reference to detail view
    [SerializeField] private GameObject listViewPanel;    // Panel showing the grid
    [SerializeField] private GameObject detailViewPanel;  // Panel showing word details
    [SerializeField] private GameObject listViewBackButton; // Button_Back in Panel_ListView (to hide in detail view)
    [SerializeField] private GameObject header; // Header

    [Header("Database")]
    [SerializeField] private WordDatabase wordDatabase;

    [Header("Search")]
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private Button searchButton;

    [Header("Grid Settings")]
    [SerializeField] private int columnsPerRow = 2; // Number of columns in the grid (default: 2 columns)
    [SerializeField] private Vector2 cellSize = new Vector2(200, 250); // Size of each grid cell
    [SerializeField] private Vector2 spacing = new Vector2(10, 10); // Spacing between grid items

    private List<WordItemUI> currentWordItems = new List<WordItemUI>();

    void OnEnable()
    {
        // Khi DictionaryListView được bật (thường là lúc mở Panel_Dictionary từ Home),
        // luôn đảm bảo hiển thị danh sách trước, ẩn màn detail.
        ShowListView();

        if (searchButton != null)
            searchButton.onClick.AddListener(OnSearchClicked);
        
        if (searchInputField != null)
            searchInputField.onValueChanged.AddListener(OnSearchTextChanged);

        LoadWordList();
    }

    void OnDisable()
    {
        if (searchButton != null)
            searchButton.onClick.RemoveListener(OnSearchClicked);
        
        if (searchInputField != null)
            searchInputField.onValueChanged.RemoveListener(OnSearchTextChanged);
    }

    /// <summary>
    /// Load all words from database and display in grid
    /// </summary>
    public void LoadWordList()
    {
        if (wordDatabase == null)
        {
            Debug.LogWarning("DictionaryListView: WordDatabase is not assigned!");
            return;
        }

        if (wordListContainer == null)
        {
            Debug.LogWarning("DictionaryListView: Word List Container is not assigned!");
            return;
        }

        if (wordItemPrefab == null)
        {
            Debug.LogWarning("DictionaryListView: Word Item Prefab is not assigned!");
            return;
        }

        // Setup Grid Layout Group if it exists
        SetupGridLayout();

        // Clear existing items
        ClearWordList();

        // Get all words from database
        List<WordData> allWords = wordDatabase.GetAllWords();
        
        if (allWords == null || allWords.Count == 0)
        {
            Debug.LogWarning("DictionaryListView: No words found in database!");
            return;
        }

        // Create UI items for each word in the grid
        foreach (WordData wordData in allWords)
        {
            GameObject itemObj = Instantiate(wordItemPrefab, wordListContainer);
            WordItemUI wordItem = itemObj.GetComponent<WordItemUI>();
            
            if (wordItem == null)
            {
                // If prefab doesn't have WordItemUI, add it
                wordItem = itemObj.AddComponent<WordItemUI>();
            }
            
            wordItem.Setup(wordData, this);
            currentWordItems.Add(wordItem);
        }

        Debug.Log($"DictionaryListView: Loaded {allWords.Count} words in grid");
    }

    /// <summary>
    /// Setup Grid Layout Group component on the container
    /// </summary>
    private void SetupGridLayout()
    {
        GridLayoutGroup gridLayout = wordListContainer.GetComponent<GridLayoutGroup>();
        
        if (gridLayout == null)
        {
            // Add Grid Layout Group if it doesn't exist
            gridLayout = wordListContainer.gameObject.AddComponent<GridLayoutGroup>();
            Debug.Log("DictionaryListView: Added Grid Layout Group component to container");
        }

        // Configure grid settings
        gridLayout.cellSize = cellSize;
        gridLayout.spacing = spacing;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.UpperLeft;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columnsPerRow;
    }

    /// <summary>
    /// Show word detail view when a word is clicked
    /// </summary>
    public void ShowWordDetail(WordData wordData)
    {
        if (dictionaryDetailView != null && wordData != null)
        {
            // Hide list, show detail
            if (listViewPanel != null)
                listViewPanel.SetActive(false);
            
            if (detailViewPanel != null)
                detailViewPanel.SetActive(true);

            // Hide the ListView's back button when showing detail
            if (listViewBackButton != null)
                listViewBackButton.SetActive(false);

            // Hide header when in detail view
            if (header != null) {
                header.SetActive(false);
            }

            // Display word in detail view
            dictionaryDetailView.DisplayWord(wordData, fromARView: false);
        }
    }

    /// <summary>
    /// Return to list view from detail view
    /// </summary>
    public void BackToListView()
    {
        ShowListView();
    }

    /// <summary>
    /// Đảm bảo chỉ hiện panel danh sách, ẩn panel chi tiết.
    /// Có thể gọi từ SimpleMenuController khi mở Dictionary từ Home.
    /// </summary>
    public void ShowListView()
    {
        if (listViewPanel != null)
            listViewPanel.SetActive(true);
        
        if (detailViewPanel != null)
            detailViewPanel.SetActive(false);

        // Show the ListView's back button when returning to list
        if (listViewBackButton != null)
            listViewBackButton.SetActive(true);

        // Show header when in list view
        if (header != null)
            header.SetActive(true);
    }

    /// <summary>
    /// Filter words based on search text
    /// </summary>
    private void OnSearchTextChanged(string searchText)
    {
        FilterWords(searchText);
    }

    private void OnSearchClicked()
    {
        if (searchInputField != null)
        {
            FilterWords(searchInputField.text);
        }
    }

    private void FilterWords(string searchText)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            // Show all words
            foreach (var item in currentWordItems)
            {
                if (item != null)
                    item.gameObject.SetActive(true);
            }
            return;
        }

        searchText = searchText.ToLower();
        foreach (var item in currentWordItems)
        {
            if (item != null && item.WordData != null)
            {
                bool matches = item.WordData.englishName.ToLower().Contains(searchText) ||
                              item.WordData.vietnameseName.ToLower().Contains(searchText);
                item.gameObject.SetActive(matches);
            }
        }
    }

    private void ClearWordList()
    {
        foreach (var item in currentWordItems)
        {
            if (item != null)
                Destroy(item.gameObject);
        }
        currentWordItems.Clear();
    }
}