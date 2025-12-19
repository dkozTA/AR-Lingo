using UnityEngine;
using System.IO;

public class CardCapture : MonoBehaviour
{
    public Camera targetCamera; // Kéo Main Camera vào đây
    public string animalName = "Dog"; // Gõ tên con vật vào đây trước khi chụp

    [ContextMenu("Chụp Ngay (Click Me)")] // Tạo nút bấm trên Inspector
    public void CaptureScreenshot()
    {
        // 1. Tạo RenderTexture tạm thời chuẩn size 767x767
        RenderTexture rt = new RenderTexture(767, 767, 24);
        targetCamera.targetTexture = rt;

        // 2. Render
        Texture2D screenShot = new Texture2D(767, 767, TextureFormat.RGB24, false);
        targetCamera.Render();
        RenderTexture.active = rt;

        // 3. Đọc pixel và lưu
        screenShot.ReadPixels(new Rect(0, 0, 767, 767), 0, 0);
        targetCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        // 4. Lưu ra file JPG
        byte[] bytes = screenShot.EncodeToJPG(100); // Chất lượng 100%
        string filename = Application.dataPath + "/_AR Lingo/Sprites/Flashcards/Img_" + animalName + ".jpg";

        // Tạo folder nếu chưa có
        Directory.CreateDirectory(Path.GetDirectoryName(filename));

        File.WriteAllBytes(filename, bytes);
        Debug.Log("✅ Đã chụp xong: " + filename);

        // Refresh Unity để thấy file
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}