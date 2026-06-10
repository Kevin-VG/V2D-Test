using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Level3Builder : EditorWindow
{
    private Tilemap tilemap;
    private Grid grid;

    private Tile tile_piso_1;
    private Tile tile_piso_2;
    private Tile tile_piso_3;
    private Tile tile_piso_4;
    private Tile tile_piso_5;
    private Tile tile_piso_6;
    private Tile tile_piso_7;
    private Tile tile_piso_8;
    private Tile tile_piso_9;
    private Tile tile_piso_10;
    private Tile tile_piso_11;
    private Tile tile_piso_12;

    [MenuItem("Tools/INWARD/Build Level 3")]
    public static void ShowWindow()
    {
        GetWindow<Level3Builder>("Level 3 Builder");
    }

    private void OnEnable()
    {
        LoadTiles();
    }

    private void LoadTiles()
    {
        string basePath = "Assets/_INWARD/Sprites/Nivel 3/Piso/";
        tile_piso_1 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_1_n3_0.asset");
        tile_piso_2 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_2_n3_0.asset");
        tile_piso_3 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_3_n3_0.asset");
        tile_piso_4 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_4_n3_0.asset");
        tile_piso_5 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_5_n3_0.asset");
        tile_piso_6 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_6_n3_0.asset");
        tile_piso_7 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_7_n3_0.asset");
        tile_piso_8 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_8_n3_0.asset");
        tile_piso_9 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_9_n3_0.asset");
        tile_piso_10 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_10_n3_0.asset");
        tile_piso_11 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_11_n3_0.asset");
        tile_piso_12 = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "piso_12_n3_0.asset");

        var tm = Object.FindObjectOfType<Tilemap>();
        if (tm != null)
        {
            tilemap = tm;
            grid = tm.GetComponentInParent<Grid>();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Level 3 Builder - EL MAR QUIETO", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (tilemap == null)
        {
            EditorGUILayout.HelpBox("No se encontró Tilemap. Asegúrate de tener un Tilemap en la escena.", MessageType.Warning);
            if (GUILayout.Button("Reintentar"))
            {
                LoadTiles();
            }
            return;
        }

        EditorGUILayout.LabelField("Tilemap: " + tilemap.gameObject.name);
        EditorGUILayout.Space();

        if (GUILayout.Button("Limpiar Todo"))
        {
            ClearAll();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Construir por Sala:", EditorStyles.boldLabel);

        if (GUILayout.Button("R3.1 - La Caída")) BuildR3_1();
        if (GUILayout.Button("R3.2 - Santuario")) BuildR3_2();
        if (GUILayout.Button("R3.3 - Descenso Suave")) BuildR3_3();
        if (GUILayout.Button("R3.4 - Primera Ancla")) BuildR3_4();
        if (GUILayout.Button("R3.5 - Corriente Peligrosa")) BuildR3_5();
        if (GUILayout.Button("R3.6 - Roca (Ancla)")) BuildR3_6();
        if (GUILayout.Button("R3.7 - Banca")) BuildR3_7();
        if (GUILayout.Button("R3.8 - Fragmento: Pulso")) BuildR3_8();
        if (GUILayout.Button("R3.9 - Salto de Fe")) BuildR3_9();
        if (GUILayout.Button("R3.10 - Banca 60s")) BuildR3_10();
        if (GUILayout.Button("R3.11 - Santuario: Concha")) BuildR3_11();
        if (GUILayout.Button("R3.12 - Caracol (Ancla)")) BuildR3_12();
        if (GUILayout.Button("R3.13 - Botella de Bruno")) BuildR3_13();
        if (GUILayout.Button("R3.14 - Hacia la Superficie")) BuildR3_14();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Construcción Completa:", EditorStyles.boldLabel);

        if (GUILayout.Button("CONSTRUIR TODO EL NIVEL"))
        {
            BuildFullLevel();
        }
    }

    private void ClearAll()
    {
        var bounds = tilemap.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
        Debug.Log("Level 3 Builder: Tilemap limpiado.");
    }

    private void SetTile(int x, int y, Tile tile)
    {
        tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    private void SetTileRange(int xStart, int yStart, int width, int height, Tile tile)
    {
        for (int x = xStart; x < xStart + width; x++)
        {
            for (int y = yStart; y < yStart + height; y++)
            {
                SetTile(x, y, tile);
            }
        }
    }

    private void SetWall(int x, int yStart, int height, Tile wallTile)
    {
        SetTileRange(x, yStart, 1, height, wallTile);
    }

    private void SetPlatform(int xStart, int y, int width, Tile platformTile)
    {
        SetTileRange(xStart, y, width, 1, platformTile);
    }

    private void BuildR3_1()
    {
        int ox = 0, oy = 0;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 2, oy - 12, 12, tile_piso_1);
        SetWall(ox + 17, oy - 12, 12, tile_piso_1);
        SetWall(ox + 18, oy - 12, 12, tile_piso_2);
        SetWall(ox + 19, oy - 12, 12, tile_piso_1);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        SetPlatform(ox + 0, oy - 1, 20, tile_piso_2);
        SetPlatform(ox + 5, oy - 3, 6, tile_piso_10);
        SetPlatform(ox + 10, oy - 6, 6, tile_piso_10);
        SetPlatform(ox + 5, oy - 9, 6, tile_piso_10);
        Debug.Log("R3.1 construida.");
    }

    private void BuildR3_2()
    {
        int ox = 0, oy = -15;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 18, oy - 12, 12, tile_piso_1);
        SetWall(ox + 19, oy - 12, 12, tile_piso_2);
        SetPlatform(ox + 6, oy - 6, 8, tile_piso_10);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.2 construida.");
    }

    private void BuildR3_3()
    {
        int ox = 0, oy = -30;
        SetWall(ox + 0, oy - 8, 8, tile_piso_1);
        SetWall(ox + 19, oy - 8, 8, tile_piso_1);
        SetPlatform(ox + 3, oy - 3, 5, tile_piso_10);
        SetPlatform(ox + 12, oy - 6, 5, tile_piso_10);
        SetPlatform(ox + 3, oy - 9, 5, tile_piso_10);
        SetPlatform(ox + 12, oy - 12, 5, tile_piso_10);
        Debug.Log("R3.3 construida.");
    }

    private void BuildR3_4()
    {
        int ox = 0, oy = -45;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 18, oy - 12, 12, tile_piso_1);
        SetWall(ox + 19, oy - 12, 12, tile_piso_2);
        SetPlatform(ox + 6, oy - 6, 8, tile_piso_10);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.4 construida.");
    }

    private void BuildR3_5()
    {
        int ox = 0, oy = -60;
        SetWall(ox + 0, oy - 15, 15, tile_piso_1);
        SetWall(ox + 1, oy - 15, 15, tile_piso_2);
        SetWall(ox + 2, oy - 15, 15, tile_piso_1);
        SetWall(ox + 17, oy - 15, 15, tile_piso_1);
        SetWall(ox + 18, oy - 15, 15, tile_piso_2);
        SetWall(ox + 19, oy - 15, 15, tile_piso_1);
        SetTileRange(ox + 7, oy - 15, 6, 15, tile_piso_5);
        SetPlatform(ox + 3, oy - 5, 4, tile_piso_10);
        SetPlatform(ox + 13, oy - 10, 4, tile_piso_10);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.5 construida.");
    }

    private void BuildR3_6()
    {
        int ox = 0, oy = -78;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 18, oy - 12, 12, tile_piso_1);
        SetWall(ox + 19, oy - 12, 12, tile_piso_2);
        SetPlatform(ox + 6, oy - 6, 8, tile_piso_10);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.6 construida.");
    }

    private void BuildR3_7()
    {
        int ox = 0, oy = -93;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 18, oy - 12, 12, tile_piso_1);
        SetWall(ox + 19, oy - 12, 12, tile_piso_2);
        SetPlatform(ox + 6, oy - 6, 8, tile_piso_10);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.7 construida.");
    }

    private void BuildR3_8()
    {
        int ox = 0, oy = -108;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 18, oy - 12, 12, tile_piso_1);
        SetWall(ox + 19, oy - 12, 12, tile_piso_2);
        SetPlatform(ox + 6, oy - 8, 8, tile_piso_11);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.8 construida.");
    }

    private void BuildR3_9()
    {
        int ox = 0, oy = -123;
        SetWall(ox + 0, oy - 15, 15, tile_piso_1);
        SetWall(ox + 1, oy - 15, 15, tile_piso_2);
        SetWall(ox + 18, oy - 15, 15, tile_piso_1);
        SetWall(ox + 19, oy - 15, 15, tile_piso_2);
        SetPlatform(ox + 2, oy - 6, 5, tile_piso_11);
        SetPlatform(ox + 13, oy - 6, 5, tile_piso_11);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.9 construida.");
    }

    private void BuildR3_10()
    {
        int ox = -5, oy = -141;
        SetWall(ox + 0, oy - 10, 10, tile_piso_1);
        SetWall(ox + 1, oy - 10, 10, tile_piso_2);
        SetWall(ox + 2, oy - 10, 10, tile_piso_1);
        SetWall(ox + 27, oy - 10, 10, tile_piso_1);
        SetWall(ox + 28, oy - 10, 10, tile_piso_2);
        SetWall(ox + 29, oy - 10, 10, tile_piso_1);
        SetPlatform(ox + 0, oy - 3, 30, tile_piso_1);
        SetPlatform(ox + 0, oy - 2, 30, tile_piso_2);
        SetPlatform(ox + 0, oy - 1, 30, tile_piso_1);
        SetPlatform(ox + 3, oy - 6, 4, tile_piso_9);
        SetPlatform(ox + 23, oy - 6, 4, tile_piso_9);
        SetPlatform(ox + 0, oy, 30, tile_piso_1);
        Debug.Log("R3.10 construida.");
    }

    private void BuildR3_11()
    {
        int ox = 0, oy = -156;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 18, oy - 12, 12, tile_piso_1);
        SetWall(ox + 19, oy - 12, 12, tile_piso_2);
        SetPlatform(ox + 6, oy - 6, 8, tile_piso_9);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.11 construida.");
    }

    private void BuildR3_12()
    {
        int ox = 0, oy = -171;
        SetWall(ox + 0, oy - 8, 8, tile_piso_1);
        SetWall(ox + 19, oy - 8, 8, tile_piso_1);
        SetPlatform(ox + 3, oy - 3, 5, tile_piso_12);
        SetPlatform(ox + 12, oy - 6, 5, tile_piso_12);
        SetPlatform(ox + 3, oy - 9, 5, tile_piso_12);
        SetPlatform(ox + 12, oy - 12, 5, tile_piso_12);
        Debug.Log("R3.12 construida.");
    }

    private void BuildR3_13()
    {
        int ox = 0, oy = -186;
        SetWall(ox + 0, oy - 12, 12, tile_piso_1);
        SetWall(ox + 1, oy - 12, 12, tile_piso_2);
        SetWall(ox + 18, oy - 12, 12, tile_piso_1);
        SetWall(ox + 19, oy - 12, 12, tile_piso_2);
        SetPlatform(ox + 6, oy - 6, 8, tile_piso_12);
        SetPlatform(ox + 0, oy, 20, tile_piso_1);
        Debug.Log("R3.13 construida.");
    }

    private void BuildR3_14()
    {
        int ox = 0, oy = -201;
        SetWall(ox + 0, oy - 15, 15, tile_piso_1);
        SetWall(ox + 1, oy - 15, 15, tile_piso_2);
        SetWall(ox + 18, oy - 15, 15, tile_piso_1);
        SetWall(ox + 19, oy - 15, 15, tile_piso_2);
        SetPlatform(ox + 5, oy - 3, 6, tile_piso_12);
        SetPlatform(ox + 10, oy - 6, 6, tile_piso_12);
        SetPlatform(ox + 5, oy - 9, 6, tile_piso_12);
        SetPlatform(ox + 10, oy - 12, 6, tile_piso_12);
        SetPlatform(ox + 0, oy, 3, tile_piso_1);
        SetPlatform(ox + 17, oy, 3, tile_piso_1);
        Debug.Log("R3.14 construida.");
    }

    private void BuildFullLevel()
    {
        ClearAll();
        BuildR3_1();
        BuildR3_2();
        BuildR3_3();
        BuildR3_4();
        BuildR3_5();
        BuildR3_6();
        BuildR3_7();
        BuildR3_8();
        BuildR3_9();
        BuildR3_10();
        BuildR3_11();
        BuildR3_12();
        BuildR3_13();
        BuildR3_14();
        Debug.Log("=== NIVEL 3 COMPLETO CONSTRUIDO ===");
    }
}
