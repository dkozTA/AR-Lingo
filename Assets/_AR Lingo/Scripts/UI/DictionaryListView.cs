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
    [SerializeField] private GameObject header; // Header for list view

    [Header("Database")]
    [SerializeField] private WordDatabase wordDatabase;

    [Header("Search")]
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private Button searchButton;

    [Header("Grid Settings")]
    [SerializeField] private int columnsPerRow = 2; // Number of columns in the grid (default: 2 columns)
    [SerializeField] private Vector2 cellSize = new Vector2(180, 220); // Size of each grid cell
    [SerializeField] private Vector2 spacing = new Vector2(10, 10); // Spacing between grid items
    
    [Header("Grid Padding")]
    [SerializeField] private int paddingLeft = 10;
    [SerializeField] private int paddingRight = 10;
    [SerializeField] private int paddingTop = 10;
    [SerializeField] private int paddingBottom = 10;

    private List<WordItemUI> currentWordItems = new List<WordItemUI>();
    private bool isInitialized = false; // Track if we've loaded words already
    private GridLayoutGroup cachedGridLayout; // Cache the grid layout component

    void Awake()
    {
        // Cache the grid layout component
        if (wordListContainer != null)
        {
            cachedGridLayout = wordListContainer.GetComponent<GridLayoutGroup>();
            if (cachedGridLayout == null)
            {
                cachedGridLayout = wordListContainer.gameObject.AddComponent<GridLayoutGroup>();
            }
        }
    }

    void OnEnable()
    {
        // Show list view first
        ShowListView();

        // Setup search listeners
        if (searchButton != null)
            searchButton.onClick.AddListener(OnSearchClicked);
        
        if (searchInputField != null)
            searchInputField.onValueChanged.AddListener(OnSearchTextChanged);

        // Only load word list once or if list is empty
        if (!isInitialized || currentWordItems.Count == 0)
        {
            LoadWordList();
        }
    }

    void OnDisable()
    {
        // Remove search listeners
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

        // Setup Grid Layout Group ONCE
        if (!isInitialized)
        {
            SetupGridLayout();
        }

        // Clear existing items safely
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

        isInitialized = true;
        Debug.Log($"DictionaryListView: Loaded {allWords.Count} words in grid");
    }

    /// <summary>
    /// Setup Grid Layout Group component on the container - ONLY CALLED ONCE
    /// </summary>
    private void SetupGridLayout()
    {
        if (cachedGridLayout == null)
        {
            Debug.LogError("DictionaryListView: Grid Layout Group is not initialized!");
            return;
        }

        // Configure grid settings using the inspector values
        cachedGridLayout.cellSize = cellSize;
        cachedGridLayout.spacing = spacing;
        
        // Set padding using individual values
        cachedGridLayout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        
        cachedGridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        cachedGridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        cachedGridLayout.childAlignment = TextAnchor.UpperLeft;
        cachedGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cachedGridLayout.constraintCount = columnsPerRow;

        Debug.Log($"Grid Layout configured: CellSize={cellSize}, Spacing={spacing}, Columns={columnsPerRow}, Padding=({paddingLeft},{paddingRight},{paddingTop},{paddingBottom})");
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

            // Hide header in detail view
            if (header != null)
                header.SetActive(false);

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

        // Show header in list view
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
                if (item != null && item.gameObject != null)
                    item.gameObject.SetActive(true);
            }
            return;
        }

        searchText = searchText.ToLower();
        foreach (var item in currentWordItems)
        {
            if (item != null && item.gameObject != null && item.WordData != null)
            {
                bool matches = item.WordData.englishName.ToLower().Contains(searchText) ||
                              item.WordData.vietnameseName.ToLower().Contains(searchText);
                item.gameObject.SetActive(matches);
            }
        }
    }

    /// <summary>
    /// Clear word list safely - handles null references
    /// </summary>
    private void ClearWordList()
    {
        // Destroy all child objects in the container
        if (wordListContainer != null)
        {
            // Get all children and destroy them
            foreach (Transform child in wordListContainer)
            {
                if (child != null && child.gameObject != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        // Clear the list (remove null references too)
        if (currentWordItems != null)
        {
            currentWordItems.Clear();
        }
        else
        {
            // Initialize list if it was null
            currentWordItems = new List<WordItemUI>();
        }
    }

    /// <summary>
    /// Force reload the word list
    /// </summary>
    public void ReloadWordList()
    {
        isInitialized = false;
        LoadWordList();
    }
}