using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Nivel5TilesetImporter
{
    private const string TilesetPath = "Assets/_INWARD/Sprites/Nivel 5/Piso/N5_Jardin_Tileset_32x32.png";
    private const string TilesOutputPath = "Assets/_INWARD/Sprites/Nivel 5/Piso/Tiles";
    private const string PalettePrefabPath = "Assets/_INWARD/Palettes/Nivel 5 - Jardin.prefab";
    private const int TileSize = 32;
    private const int Columns = 8;
    private const int Rows = 4;

    private static readonly string[] TileNames =
    {
        "grass_top", "grass_left", "grass_right", "grass_single", "trunk_left", "trunk_right", "fill_leaves", "fill_shadow",
        "young_tree", "big_tree", "lantern_branch", "sanctuary_seed", "hopscotch_light", "cookie", "memory_fragment", "seed_platform",
        "musical_bridge", "music_notes", "leaf_slide", "warm_light", "memory_mateo", "flower_cluster", "guitar_icon", "lookout_floor",
        "permanent_platform", "bruno_window", "paper_lantern", "white_fade", "corner_top_left", "corner_top_right", "corner_bottom_left", "corner_bottom_right"
    };

    [MenuItem("Tools/INWARD/Nivel 5/Import Tileset 32x32")]
    public static void ImportTileset()
    {
        AssetDatabase.ImportAsset(TilesetPath, ImportAssetOptions.ForceUpdate);

        TextureImporter importer = AssetImporter.GetAtPath(TilesetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError($"Nivel 5 Tileset: No se pudo encontrar el importer de {TilesetPath}");
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = TileSize;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;

        SpriteRect[] sprites = new SpriteRect[Columns * Rows];
        List<SpriteNameFileIdPair> nameFileIdPairs = new(Columns * Rows);
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                int index = row * Columns + col;
                string spriteName = $"n5_jardin_{index:00}_{TileNames[index]}";
                GUID spriteId = GUID.Generate();
                sprites[index] = new SpriteRect
                {
                    name = spriteName,
                    rect = new Rect(col * TileSize, (Rows - 1 - row) * TileSize, TileSize, TileSize),
                    alignment = SpriteAlignment.Center,
                    pivot = new Vector2(0.5f, 0.5f),
                    spriteID = spriteId
                };
                nameFileIdPairs.Add(new SpriteNameFileIdPair(spriteName, spriteId));
            }
        }

        SpriteDataProviderFactories factory = new();
        factory.Init();
        ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();
        dataProvider.SetSpriteRects(sprites);
        ISpriteNameFileIdDataProvider nameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
        nameFileIdDataProvider.SetNameFileIdPairs(nameFileIdPairs);
        dataProvider.Apply();
        importer.SaveAndReimport();

        Directory.CreateDirectory(TilesOutputPath);
        Object[] importedAssets = AssetDatabase.LoadAllAssetsAtPath(TilesetPath);
        int created = 0;

        foreach (Object asset in importedAssets)
        {
            if (asset is not Sprite sprite || !sprite.name.StartsWith("n5_jardin_"))
            {
                continue;
            }

            string tilePath = $"{TilesOutputPath}/{sprite.name}.asset";
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                AssetDatabase.CreateAsset(tile, tilePath);
                created++;
            }

            tile.sprite = sprite;
            tile.colliderType = Tile.ColliderType.Sprite;
            EditorUtility.SetDirty(tile);
        }

        AssetDatabase.SaveAssets();
        CreatePalettePrefab();
        AssetDatabase.Refresh();
        Debug.Log($"Nivel 5 Tileset importado: {Columns * Rows} sprites 32x32, {created} tiles nuevos en {TilesOutputPath}.");
    }

    private static void CreatePalettePrefab()
    {
        GameObject gridObject = new("Nivel 5 - Jardin");
        Grid grid = gridObject.AddComponent<Grid>();
        grid.cellSize = Vector3.one;

        GameObject tilemapObject = new("Paleta");
        tilemapObject.transform.SetParent(gridObject.transform);
        Tilemap tilemap = tilemapObject.AddComponent<Tilemap>();
        tilemapObject.AddComponent<TilemapRenderer>();

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                int index = row * Columns + col;
                string tilePath = $"{TilesOutputPath}/n5_jardin_{index:00}_{TileNames[index]}.asset";
                Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
                if (tile != null)
                {
                    tilemap.SetTile(new Vector3Int(col, -row, 0), tile);
                }
            }
        }

        Directory.CreateDirectory(Path.GetDirectoryName(PalettePrefabPath));
        PrefabUtility.SaveAsPrefabAsset(gridObject, PalettePrefabPath);
        Object.DestroyImmediate(gridObject);
    }
}
