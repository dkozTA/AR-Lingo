using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Dùng LINQ để tìm kiếm nhanh

[CreateAssetMenu(fileName = "WordDatabase", menuName = "AR Lingo/Database")]
public class WordDatabase : ScriptableObject
{
    [SerializeField] private List<WordData> allWords;

    // Hàm tìm kiếm con vật dựa trên ID (Dùng khi AR Camera scan được target)
    public WordData GetWordByID(string searchID)
    {
        // Tìm trong list xem có con nào trùng ID không
        return allWords.FirstOrDefault(w => w.id == searchID);
    }

    // Hàm lấy danh sách ngẫu nhiên để làm Quiz (Sprint sau)
    public List<WordData> GetRandomWords(int count)
    {
        // Shuffle và lấy count phần tử (Logic giả định, cần viết kỹ hơn sau này)
        return allWords.OrderBy(x => Random.value).Take(count).ToList();
    }
}