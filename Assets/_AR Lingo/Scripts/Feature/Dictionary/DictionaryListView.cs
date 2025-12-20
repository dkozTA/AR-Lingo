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
    [SerializeField] private Transform wordListContainer;
    [SerializeField] private GameObject wordItemPrefab;
    [SerializeField] private DictionaryUI dictionaryDetailView;
    [SerializeField] private GameObject listViewPanel;
    [SerializeField] private GameObject detailViewPanel;
    [SerializeField] private GameObject listViewBackButton;
    [SerializeField] private GameObject header;

    [Header("Database")]
    [SerializeField] private WordDatabase wordDatabase;

    [Header("Search")]
    [SerializeField] private TMP_InputField searchInputField;
    [SerializeField] private Button searchButton;

    [Header("Grid Settings")]
    [SerializeField] private int columnsPerRow = 2;
    [SerializeField] private Vector2 cellSize = new Vector2(180, 220);
    [SerializeField] private Vector2 spacing = new Vector2(10, 10);
    
    [Header("Grid Padding")]
    [SerializeField] private int paddingLeft = 10;
    [SerializeField] private int paddingRight = 10;
    [SerializeField] private int paddingTop = 10;
    [SerializeField] private int paddingBottom = 10;

    private List<WordItemUI> currentWordItems = new List<WordItemUI>();
    private bool isInitialized = false;
    private GridLayoutGroup cachedGridLayout;

    void Awake()
    {
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
        ShowListView();

        if (searchButton != null)
            searchButton.onClick.AddListener(OnSearchClicked);
        
        if (searchInputField != null)
            searchInputField.onValueChanged.AddListener(OnSearchTextChanged);

        if (!isInitialized || currentWordItems.Count == 0)
        {
            LoadWordList();
        }
    }

    void OnDisable()
    {
        if (searchButton != null)
            searchButton.onClick.RemoveListener(OnSearchClicked);
        
        if (searchInputField != null)
            searchInputField.onValueChanged.RemoveListener(OnSearchTextChanged);
    }

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

        if (!isInitialized)
        {
            SetupGridLayout();
        }

        ClearWordList();

        List<WordData> allWords = wordDatabase.GetAllWords();
        
        if (allWords == null || allWords.Count == 0)
        {
            Debug.LogWarning("DictionaryListView: No words found in database!");
            return;
        }

        foreach (WordData wordData in allWords)
        {
            GameObject itemObj = Instantiate(wordItemPrefab, wordListContainer);
            WordItemUI wordItem = itemObj.GetComponent<WordItemUI>();
            
            if (wordItem == null)
            {
                wordItem = itemObj.AddComponent<WordItemUI>();
            }
            
            wordItem.Setup(wordData, this);
            currentWordItems.Add(wordItem);
        }

        isInitialized = true;
        Debug.Log($"DictionaryListView: Loaded {allWords.Count} words in grid");
    }

    private void SetupGridLayout()
    {
        if (cachedGridLayout == null)
        {
            Debug.LogError("DictionaryListView: Grid Layout Group is not initialized!");
            return;
        }

        cachedGridLayout.cellSize = cellSize;
        cachedGridLayout.spacing = spacing;
        cachedGridLayout.padding = new RectOffset(paddingLeft, paddingRight, paddingTop, paddingBottom);
        cachedGridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        cachedGridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        cachedGridLayout.childAlignment = TextAnchor.UpperLeft;
        cachedGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        cachedGridLayout.constraintCount = columnsPerRow;

        Debug.Log($"Grid Layout configured: CellSize={cellSize}, Spacing={spacing}, Columns={columnsPerRow}");
    }

    public void ShowWordDetail(WordData wordData)
    {
        if (dictionaryDetailView != null && wordData != null)
        {
            if (listViewPanel != null)
                listViewPanel.SetActive(false);
            
            if (detailViewPanel != null)
                detailViewPanel.SetActive(true);

            if (listViewBackButton != null)
                listViewBackButton.SetActive(false);

            if (header != null)
                header.SetActive(false);

            dictionaryDetailView.DisplayWord(wordData, fromARView: false);
        }
    }

    public void BackToListView()
    {
        ShowListView();
    }

    public void ShowListView()
    {
        if (listViewPanel != null)
            listViewPanel.SetActive(true);
        
        if (detailViewPanel != null)
            detailViewPanel.SetActive(false);

        if (listViewBackButton != null)
            listViewBackButton.SetActive(true);

        if (header != null)
            header.SetActive(true);
        
        if (!ARScanFeature.IsOpenedFromARScan())
        {
            Debug.Log("[DictionaryListView] Showing list view from Home");
        }
    }

    // NEW: Check if currently in detail view
    public bool IsInDetailView()
    {
        if (detailViewPanel != null)
        {
            return detailViewPanel.activeSelf;
        }
        return false;
    }

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
            foreach (var wordItem in currentWordItems)
            {
                if (wordItem != null)
                    wordItem.gameObject.SetActive(true);
            }
            return;
        }

        searchText = searchText.ToLower();

        foreach (var wordItem in currentWordItems)
        {
            if (wordItem != null && wordItem.WordData != null)
            {
                bool matches = wordItem.WordData.englishName.ToLower().Contains(searchText) ||
                              wordItem.WordData.vietnameseName.ToLower().Contains(searchText);
                
                wordItem.gameObject.SetActive(matches);
            }
        }
    }

    private void ClearWordList()
    {
        if (wordListContainer != null)
        {
            foreach (Transform child in wordListContainer)
            {
                if (child != null && child.gameObject != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        if (currentWordItems != null)
        {
            currentWordItems.Clear();
        }
        else
        {
            currentWordItems = new List<WordItemUI>();
        }
    }

    public void ReloadWordList()
    {
        isInitialized = false;
        LoadWordList();
    }
}