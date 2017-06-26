using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteImporter : AssetPostprocessor
{
    private const string SPRITE_TAG = "Assets/FvsT/Sprites";
    void OnPostprocessTexture(Texture2D texture)
    {
        DirectoryInfo path = new DirectoryInfo(Path.GetDirectoryName(assetPath));
        if (path.ToString().Contains(SPRITE_TAG))
        {
            TextureImporter textureImporter = assetImporter as TextureImporter;
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spritePackingTag = path.Name;
            textureImporter.alphaIsTransparency = true;
            textureImporter.mipmapEnabled = false;
            textureImporter.SetPlatformTextureSettings("iPhone", 2048, TextureImporterFormat.PVRTC_RGBA4);
            textureImporter.SetPlatformTextureSettings("Android", 2048, TextureImporterFormat.ETC2_RGBA8);
        }
    }
}
