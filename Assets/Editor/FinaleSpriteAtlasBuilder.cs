// [UNITY-SKILL:SPRITEATLAS]
// [TYPE:PREBUILD] Version 1.0.0 - built-in atlas delivery.
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.U2D;
using UnityEngine;

// Editor-only: refresh the two atlas assets before each player build.
public sealed class FinaleSpriteAtlasBuilder : IPreprocessBuildWithReport
{
    public const string AtlasFolder = "Assets/Atlases";
    public const string RewardsPath = AtlasFolder + "/RewardsAtlas.spriteatlasv2";
    public const string UIPath = AtlasFolder + "/UIAtlas.spriteatlasv2";
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report) => GenerateAtlases();

    public static void GenerateAtlases()
    {
        EditorSettings.spritePackerMode = SpritePackerMode.SpriteAtlasV2;
        if (EditorSettings.spritePackerMode != SpritePackerMode.SpriteAtlasV2)
            throw new BuildFailedException("Sprite Atlas V2 paketlemesi etkinleştirilemedi.");

        if (!AssetDatabase.IsValidFolder(AtlasFolder))
            AssetDatabase.CreateFolder("Assets", "Atlases");

        // Only project sprites belong here. TMP fonts and the large backgrounds stay separate.
        var paths = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/UI", "Assets/Art" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => path.StartsWith("Assets/", StringComparison.Ordinal)
                && Path.GetFileNameWithoutExtension(path).IndexOf("background", StringComparison.OrdinalIgnoreCase) < 0)
            .Distinct()
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        // Compress the finished atlas, not its inputs: avoid two lossy compression passes.
        foreach (var path in paths)
        {
            var sourceImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            if (sourceImporter != null && sourceImporter.textureCompression != TextureImporterCompression.Uncompressed)
            {
                sourceImporter.textureCompression = TextureImporterCompression.Uncompressed;
                sourceImporter.crunchedCompression = false;
                sourceImporter.SaveAndReimport();
            }
        }

        var rewards = paths.Where(IsReward).ToArray();
        var ui = paths.Where(path => !IsReward(path)).ToArray();
        GenerateAtlas(RewardsPath, rewards);
        GenerateAtlas(UIPath, ui);
        AssetDatabase.SaveAssets();
    }

    private static bool IsReward(string path) =>
        Path.GetFileNameWithoutExtension(path).IndexOf("_icon", StringComparison.OrdinalIgnoreCase) >= 0;

    private static void GenerateAtlas(string atlasPath, string[] spritePaths)
    {
        var sprites = spritePaths.Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .Where(sprite => sprite != null)
            .Cast<UnityEngine.Object>()
            .ToArray();
        if (sprites.Length == 0)
            throw new BuildFailedException("Atlas için sprite bulunamadı: " + atlasPath);

        // Rebuild this generated asset from its source list; its existing .meta GUID stays intact.
        var atlas = new SpriteAtlasAsset();
        atlas.Add(sprites);
        SpriteAtlasAsset.Save(atlas, atlasPath);
        AssetDatabase.ImportAsset(atlasPath, ImportAssetOptions.ForceUpdate);

        var importer = AssetImporter.GetAtPath(atlasPath) as SpriteAtlasImporter;
        if (importer == null)
            throw new BuildFailedException("SpriteAtlasImporter bulunamadı: " + atlasPath);

        importer.includeInBuild = true;
        var packing = importer.packingSettings;
        packing.enableRotation = false;
        packing.enableTightPacking = false;
        packing.padding = 4;
        packing.enableAlphaDilation = true;
        importer.packingSettings = packing;

        var texture = importer.textureSettings;
        texture.generateMipMaps = false;
        texture.readable = false;
        texture.filterMode = FilterMode.Bilinear;
        importer.textureSettings = texture;

        var defaults = importer.GetPlatformSettings("DefaultTexturePlatform");
        defaults.maxTextureSize = 2048;
        defaults.textureCompression = TextureImporterCompression.Compressed;
        defaults.compressionQuality = 80;
        importer.SetPlatformSettings(defaults);
        importer.SetPlatformSettings(new TextureImporterPlatformSettings
        {
            name = "Android",
            overridden = true,
            maxTextureSize = 2048,
            format = TextureImporterFormat.ASTC_6x6,
            compressionQuality = 80
        });
        importer.SaveAndReimport();
    }
}
