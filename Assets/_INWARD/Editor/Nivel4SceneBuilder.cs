using System.IO;
using Shatter.CameraSystem;
using Shatter.Levels;
using Shatter.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Nivel4SceneBuilder
{
    private const string BackgroundPath = "Assets/_INWARD/Sprites/Nivel 4/Background";
    private const string RootName = "Background_N4";
    private const float BackgroundPixelsPerUnit = 100f;
    private const float BackgroundScale = 1.42f;

    private static readonly string[] BackgroundFiles =
    {
        "BG_1.png",
        "BG_2.png",
        "BG_3.png"
    };

    private static readonly string[] PhaseNames =
    {
        "Galeria de espejos",
        "Puente de reflejos",
        "Confrontacion"
    };

    [MenuItem("Tools/INWARD/Nivel 4/Apply Full Setup")]
    public static void ApplyFullSetup()
    {
        SetupTilemaps();
        BuildBackground();
        AddPlayerForTesting();
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 4: setup completo aplicado (tilemaps, fondos y jugador de prueba).");
    }

    [MenuItem("Tools/INWARD/Nivel 4/Setup Tilemaps")]
    public static void SetupTilemaps()
    {
        Tilemap piso = FindTilemap("Piso");
        Tilemap dano = FindTilemap("Daño") ?? FindTilemap("Dano");
        Tilemap adorno = FindTilemap("Adorno");

        int sueloLayer = LayerMask.NameToLayer("Suelo");
        if (sueloLayer < 0)
        {
            sueloLayer = 0;
            Debug.LogWarning("Nivel 4: no existe la layer 'Suelo'. Se usara Default para Piso.");
        }

        if (piso != null)
        {
            piso.gameObject.layer = sueloLayer;
            EnsureTilemapCollider(piso.gameObject, isTrigger: false);
            SetTilemapSorting(piso.gameObject, 0);
        }

        if (dano != null)
        {
            dano.gameObject.layer = 0;
            EnsureTilemapCollider(dano.gameObject, isTrigger: true);
            if (dano.GetComponent<Hazard>() == null)
            {
                dano.gameObject.AddComponent<Hazard>();
            }
            SetTilemapSorting(dano.gameObject, 3);
        }

        if (adorno != null)
        {
            TilemapCollider2D collider = adorno.GetComponent<TilemapCollider2D>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }
            SetTilemapSorting(adorno.gameObject, 6);
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Nivel 4: tilemaps configurados. Piso=solido, Daño=trigger Hazard, Adorno=decorativo.");
    }

    [MenuItem("Tools/INWARD/Nivel 4/Build Background")]
    public static void BuildBackground()
    {
        ImportBackgroundSprites();

        GameObject existing = GameObject.Find(RootName);
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        Camera mainCamera = Camera.main;
        GameObject root = new(RootName);
        if (mainCamera != null)
        {
            root.transform.SetParent(mainCamera.transform);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;
        }

        GameObject[] containers = new GameObject[BackgroundFiles.Length];
        for (int i = 0; i < BackgroundFiles.Length; i++)
        {
            GameObject container = new($"Fase_{i + 1}_{PhaseNames[i].Replace(' ', '_')}");
            container.transform.SetParent(root.transform);
            container.transform.localPosition = Vector3.zero;
            container.transform.localRotation = Quaternion.identity;
            container.transform.localScale = Vector3.one;
            containers[i] = container;

            CreateBackgroundLayer(container.transform, BackgroundFiles[i], i);
        }

        ConfigurePhaseController(root, containers, mainCamera);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Nivel 4: fondos aplicados por fases usando BG_1, BG_2 y BG_3.");
    }

    [MenuItem("Tools/INWARD/Nivel 4/Add Player For Testing")]
    public static void AddPlayerForTesting()
    {
        GameObject existing = GameObject.FindWithTag("Player");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        Tilemap piso = FindTilemap("Piso");
        Vector3 spawn = FindSpawnPosition(piso);

        GameObject player = new("Player");
        player.tag = "Player";
        player.transform.position = spawn;
        player.transform.localScale = Vector3.one;

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 5f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;

        BoxCollider2D box = player.AddComponent<BoxCollider2D>();
        box.isTrigger = false;
        box.size = new Vector2(0.625f, 0.96875f);
        box.offset = Vector2.zero;

        SpriteRenderer spriteRenderer = player.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = LoadFirstSprite("Assets/_INWARD/Sprites/Player/Idle/Player Idle 48x48.png");
        spriteRenderer.sortingOrder = 10;

        Animator animator = player.AddComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/_INWARD/Animations/Player/Player.controller");

        PlayerController2D controller = player.AddComponent<PlayerController2D>();
        ConfigurePlayerController(controller);

        player.AddComponent<PlayerHealth>();

        PlayerAnimatorBridge bridge = player.AddComponent<PlayerAnimatorBridge>();
        SerializedObject bridgeSerialized = new(bridge);
        bridgeSerialized.FindProperty("controlador").objectReferenceValue = controller;
        bridgeSerialized.FindProperty("animador").objectReferenceValue = animator;
        bridgeSerialized.FindProperty("renderizadorSprite").objectReferenceValue = spriteRenderer;
        bridgeSerialized.ApplyModifiedPropertiesWithoutUndo();

        ConfigureCamera(player.transform, piso);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"Nivel 4: Player de prueba creado en {spawn}.");
    }

    private static void ImportBackgroundSprites()
    {
        foreach (string fileName in BackgroundFiles)
        {
            string assetPath = $"{BackgroundPath}/{fileName}";
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            }

            if (importer == null)
            {
                Debug.LogWarning($"Nivel 4: no se encontro fondo {assetPath}");
                continue;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = BackgroundPixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }
    }

    private static void CreateBackgroundLayer(Transform parent, string fileName, int index)
    {
        string path = $"{BackgroundPath}/{fileName}";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning($"Nivel 4: no se pudo cargar el sprite {path}");
            return;
        }

        GameObject layer = new(Path.GetFileNameWithoutExtension(fileName));
        layer.transform.SetParent(parent);
        layer.transform.localPosition = new Vector3(0f, 0f, 20f + index);
        layer.transform.localRotation = Quaternion.identity;
        layer.transform.localScale = Vector3.one * BackgroundScale;

        SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.drawMode = SpriteDrawMode.Simple;
        renderer.sortingOrder = -100 + index * 10;
    }

    private static void ConfigurePhaseController(GameObject root, GameObject[] containers, Camera mainCamera)
    {
        LevelBackgroundPhaseController controller = root.AddComponent<LevelBackgroundPhaseController>();
        Bounds bounds = GetLevelBounds();
        float minX = bounds.size.x > 0.01f ? bounds.min.x : 0f;
        float maxX = bounds.size.x > 0.01f ? bounds.max.x : 240f;
        float third = (maxX - minX) / 3f;
        float fade = Mathf.Max(18f, third * 0.2f);

        SerializedObject serialized = new(controller);
        serialized.FindProperty("objetivo").objectReferenceValue = mainCamera != null ? mainCamera.transform : null;
        serialized.FindProperty("ocultarRenderersInvisibles").boolValue = true;

        SerializedProperty phasesProperty = serialized.FindProperty("fases");
        phasesProperty.arraySize = containers.Length;
        for (int i = 0; i < containers.Length; i++)
        {
            SerializedProperty phaseProperty = phasesProperty.GetArrayElementAtIndex(i);
            float start = minX + third * i;
            float end = i == containers.Length - 1 ? maxX : minX + third * (i + 1);

            phaseProperty.FindPropertyRelative("nombre").stringValue = PhaseNames[i];
            phaseProperty.FindPropertyRelative("contenedor").objectReferenceValue = containers[i];
            phaseProperty.FindPropertyRelative("xInicioCompleto").floatValue = start;
            phaseProperty.FindPropertyRelative("xFinCompleto").floatValue = end;
            phaseProperty.FindPropertyRelative("distanciaFade").floatValue = fade;
        }

        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigurePlayerController(PlayerController2D controller)
    {
        SerializedObject serialized = new(controller);
        int sueloLayer = LayerMask.NameToLayer("Suelo");
        int sueloMask = sueloLayer >= 0 ? 1 << sueloLayer : ~0;

        serialized.FindProperty("capaSuelo").intValue = sueloMask;
        serialized.FindProperty("capaPared").intValue = sueloMask;
        serialized.FindProperty("capaUnaDireccion").intValue = 0;
        serialized.FindProperty("tamanoColliderNormal").vector2Value = new Vector2(0.625f, 0.96875f);
        serialized.FindProperty("offsetColliderNormal").vector2Value = Vector2.zero;
        serialized.FindProperty("tamanoColliderAgachado").vector2Value = new Vector2(0.5f, 0.6f);
        serialized.FindProperty("offsetColliderAgachado").vector2Value = new Vector2(0f, -0.3f);
        serialized.FindProperty("offsetChequeoSueloY").floatValue = -0.4f;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigureCamera(Transform target, Tilemap piso)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            return;
        }

        CameraFollow2D follow = mainCamera.GetComponent<CameraFollow2D>();
        if (follow == null)
        {
            follow = mainCamera.gameObject.AddComponent<CameraFollow2D>();
        }

        Bounds bounds = piso != null ? piso.localBounds : new Bounds(Vector3.zero, new Vector3(240f, 40f, 0f));
        SerializedObject serialized = new(follow);
        serialized.FindProperty("objetivo").objectReferenceValue = target;
        serialized.FindProperty("offset").vector3Value = new Vector3(0f, -1.5f, -10f);
        serialized.FindProperty("usarLimites").boolValue = true;
        serialized.FindProperty("limiteMin").vector2Value = new Vector2(bounds.min.x, bounds.min.y - 8f);
        serialized.FindProperty("limiteMax").vector2Value = new Vector2(bounds.max.x, bounds.max.y + 8f);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        mainCamera.transform.position = target.position + new Vector3(0f, -1.5f, -10f);
    }

    private static Vector3 FindSpawnPosition(Tilemap piso)
    {
        if (piso == null)
        {
            return new Vector3(0f, 1.5f, 0f);
        }

        BoundsInt bounds = piso.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMax - 1; y >= bounds.yMin; y--)
            {
                Vector3Int cell = new(x, y, 0);
                if (!piso.HasTile(cell) || piso.HasTile(new Vector3Int(x, y + 1, 0)))
                {
                    continue;
                }

                return piso.GetCellCenterWorld(cell) + new Vector3(0f, 1.1f, 0f);
            }
        }

        return piso.transform.position + new Vector3(0f, 1.5f, 0f);
    }

    private static Bounds GetLevelBounds()
    {
        Tilemap piso = FindTilemap("Piso");
        Tilemap dano = FindTilemap("Daño") ?? FindTilemap("Dano");
        Tilemap adorno = FindTilemap("Adorno");

        Bounds bounds = new(Vector3.zero, Vector3.zero);
        bool initialized = false;
        foreach (Tilemap tilemap in new[] { piso, dano, adorno })
        {
            if (tilemap == null || tilemap.cellBounds.size == Vector3Int.zero)
            {
                continue;
            }

            Bounds localBounds = tilemap.localBounds;
            Bounds worldBounds = new(tilemap.transform.TransformPoint(localBounds.center), localBounds.size);
            if (!initialized)
            {
                bounds = worldBounds;
                initialized = true;
            }
            else
            {
                bounds.Encapsulate(worldBounds);
            }
        }

        return initialized ? bounds : new Bounds(Vector3.zero, new Vector3(240f, 40f, 0f));
    }

    private static Tilemap FindTilemap(string name)
    {
        foreach (Tilemap tilemap in Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None))
        {
            if (tilemap.name == name)
            {
                return tilemap;
            }
        }

        return null;
    }

    private static void EnsureTilemapCollider(GameObject target, bool isTrigger)
    {
        TilemapCollider2D collider = target.GetComponent<TilemapCollider2D>();
        if (collider == null)
        {
            collider = target.AddComponent<TilemapCollider2D>();
        }

        collider.isTrigger = isTrigger;
    }

    private static void SetTilemapSorting(GameObject target, int order)
    {
        TilemapRenderer renderer = target.GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            renderer.sortingOrder = order;
        }
    }

    private static Sprite LoadFirstSprite(string assetPath)
    {
        foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(assetPath))
        {
            if (asset is Sprite sprite)
            {
                return sprite;
            }
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
    }
}
