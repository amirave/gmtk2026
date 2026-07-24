using UnityEngine;
using UnityEditor;
using System.IO;

// Place this script inside a folder named "Editor" anywhere in your Assets folder,
// e.g. Assets/Editor/TextureTrimmer.cs

public class TextureTrimmer
{
    [MenuItem("Assets/Trim Transparent Padding", true)]
    private static bool ValidateTrim()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D) return true;
        }
        return false;
    }

    [MenuItem("Assets/Trim Transparent Padding")]
    private static void TrimSelected()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is Texture2D tex)
            {
                TrimTexture(tex);
            }
        }
        AssetDatabase.Refresh();
    }

    private static void TrimTexture(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogWarning($"No TextureImporter found for {path}, skipping.");
            return;
        }

        bool wasReadable = importer.isReadable;
        TextureImporterCompression prevCompression = importer.textureCompression;

        // Temporarily force readable + uncompressed so we can read raw pixels
        importer.isReadable = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Texture2D readableTex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        int width = readableTex.width;
        int height = readableTex.height;
        Color32[] pixels = readableTex.GetPixels32();

        int minX = width, minY = height, maxX = 0, maxY = 0;
        bool found = false;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (pixels[y * width + x].a > 0)
                {
                    found = true;
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (!found)
        {
            Debug.LogWarning($"{path}: fully transparent, skipping.");
            RestoreImporter(importer, path, wasReadable, prevCompression);
            return;
        }

        int trimmedWidth = maxX - minX + 1;
        int trimmedHeight = maxY - minY + 1;

        if (trimmedWidth == width && trimmedHeight == height)
        {
            Debug.Log($"{path}: no padding to trim.");
            RestoreImporter(importer, path, wasReadable, prevCompression);
            return;
        }

        Color32[] trimmedPixels = new Color32[trimmedWidth * trimmedHeight];
        for (int y = 0; y < trimmedHeight; y++)
        {
            for (int x = 0; x < trimmedWidth; x++)
            {
                trimmedPixels[y * trimmedWidth + x] = pixels[(y + minY) * width + (x + minX)];
            }
        }

        Texture2D trimmed = new Texture2D(trimmedWidth, trimmedHeight, TextureFormat.RGBA32, false);
        trimmed.SetPixels32(trimmedPixels);
        trimmed.Apply();

        File.WriteAllBytes(path, trimmed.EncodeToPNG());
        Object.DestroyImmediate(trimmed);

        RestoreImporter(importer, path, wasReadable, prevCompression);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Debug.Log($"Trimmed {path}: {width}x{height} -> {trimmedWidth}x{trimmedHeight}");
    }

    private static void RestoreImporter(TextureImporter importer, string path, bool wasReadable, TextureImporterCompression prevCompression)
    {
        importer.isReadable = wasReadable;
        importer.textureCompression = prevCompression;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}
