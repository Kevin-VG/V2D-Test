using System.IO;
using Shatter.CameraSystem;
using Shatter.Levels;
using Shatter.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Nivel5SceneBuilder
{
    private const string BackgroundPath = "Assets/_INWARD/Sprites/Nivel 5/Background";
    private const string EffectsPath = "Assets/_INWARD/Sprites/Nivel 5/Effects";
    private const string RootName = "Background_N5";
    private const string EffectsRootName = "Effects_N5";
    private const float BackgroundPixelsPerUnit = 100f;
    private const float BackgroundScale = 1.48f;
    private const float BackgroundCenterY = 0.15f;

    private static readonly string[] BackgroundFiles =
    {
        "BG_1.png",
        "BG_2.png"
    };

    private static readonly string[] PhaseNames =
    {
        "Jardin al amanecer",
        "Arbol del mirador"
    };

    [MenuItem("Tools/INWARD/Nivel 5/Build Background")]
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
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 5: fondos aplicados por fases usando BG_1 y BG_2.");
    }

    [MenuItem("Tools/INWARD/Nivel 5/Setup Tilemaps")]
    public static void SetupTilemaps()
    {
        Tilemap entorno = FindTilemap("Entorno");
        Tilemap adorno = FindTilemap("Adorno");

        int sueloLayer = LayerMask.NameToLayer("Suelo");
        if (sueloLayer < 0)
        {
            sueloLayer = 0;
            Debug.LogWarning("Nivel 5: no existe la layer 'Suelo'. Se usara Default para Entorno.");
        }

        if (entorno != null)
        {
            entorno.gameObject.layer = sueloLayer;
            EnsureTilemapCollider(entorno.gameObject, isTrigger: false);
            SetTilemapSorting(entorno.gameObject, 0);
        }
        else
        {
            Debug.LogWarning("Nivel 5: no se encontro tilemap 'Entorno'.");
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
        else
        {
            Debug.LogWarning("Nivel 5: no se encontro tilemap 'Adorno'.");
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 5: tilemaps configurados. Entorno=contacto fisico, Adorno=visual sin collider.");
    }

    [MenuItem("Tools/INWARD/Nivel 5/Add Player For Testing")]
    public static void AddPlayerForTesting()
    {
        GameObject existing = GameObject.FindWithTag("Player");
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        SetupTilemaps();

        Tilemap entorno = FindTilemap("Entorno");
        Vector3 spawn = FindSpawnPosition(entorno);

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

        ConfigureCamera(player.transform, entorno);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log($"Nivel 5: Player de prueba creado en {spawn}.");
    }

    [MenuItem("Tools/INWARD/Nivel 5/Build Effects")]
    public static void BuildEffects()
    {
        EnsureEffectAssets();

        GameObject existing = GameObject.Find(EffectsRootName);
        if (existing != null)
        {
            Object.DestroyImmediate(existing);
        }

        Camera mainCamera = Camera.main;
        GameObject root = new(EffectsRootName);
        if (mainCamera != null)
        {
            root.transform.SetParent(mainCamera.transform);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;
        }

        CreateFog(root.transform);
        CreateLightRays(root.transform);
        CreateGlow(root.transform);
        CreatePollen(root.transform);
        CreateLeaves(root.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 5: efectos visuales aplicados (polen, niebla, rayos, brillos y hojas).");
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
                Debug.LogWarning($"Nivel 5: no se encontro fondo {assetPath}");
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
            Debug.LogWarning($"Nivel 5: no se pudo cargar el sprite {path}");
            return;
        }

        GameObject layer = new(Path.GetFileNameWithoutExtension(fileName));
        layer.transform.SetParent(parent);
        layer.transform.localPosition = new Vector3(0f, BackgroundCenterY, 20f + index);
        layer.transform.localRotation = Quaternion.identity;
        layer.transform.localScale = Vector3.one * BackgroundScale;

        SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.drawMode = SpriteDrawMode.Simple;
        renderer.sortingOrder = -120 + index * 10;
    }

    private static void ConfigurePhaseController(GameObject root, GameObject[] containers, Camera mainCamera)
    {
        LevelBackgroundPhaseController controller = root.AddComponent<LevelBackgroundPhaseController>();
        Bounds bounds = GetLevelBounds();
        float minX = bounds.size.x > 0.01f ? bounds.min.x : -12f;
        float maxX = bounds.size.x > 0.01f ? bounds.max.x : 263f;
        float half = (maxX - minX) * 0.5f;
        float fade = Mathf.Max(28f, (maxX - minX) * 0.12f);

        SerializedObject serialized = new(controller);
        serialized.FindProperty("objetivo").objectReferenceValue = mainCamera != null ? mainCamera.transform : null;
        serialized.FindProperty("ocultarRenderersInvisibles").boolValue = true;

        SerializedProperty phasesProperty = serialized.FindProperty("fases");
        phasesProperty.arraySize = containers.Length;
        for (int i = 0; i < containers.Length; i++)
        {
            SerializedProperty phaseProperty = phasesProperty.GetArrayElementAtIndex(i);
            float start = minX + half * i;
            float end = i == containers.Length - 1 ? maxX : minX + half * (i + 1);

            phaseProperty.FindPropertyRelative("nombre").stringValue = PhaseNames[i];
            phaseProperty.FindPropertyRelative("contenedor").objectReferenceValue = containers[i];
            phaseProperty.FindPropertyRelative("xInicioCompleto").floatValue = start;
            phaseProperty.FindPropertyRelative("xFinCompleto").floatValue = end;
            phaseProperty.FindPropertyRelative("distanciaFade").floatValue = fade;
        }

        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Bounds GetLevelBounds()
    {
        Tilemap[] tilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include);
        Bounds bounds = new(Vector3.zero, Vector3.zero);
        bool initialized = false;

        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap == null || tilemap.GetComponent<TilemapRenderer>() == null)
            {
                continue;
            }

            tilemap.CompressBounds();
            Bounds localBounds = tilemap.localBounds;
            if (localBounds.size.x <= 0.01f && localBounds.size.y <= 0.01f)
            {
                continue;
            }

            Vector3 min = tilemap.transform.TransformPoint(localBounds.min);
            Vector3 max = tilemap.transform.TransformPoint(localBounds.max);
            Bounds worldBounds = new((min + max) * 0.5f, max - min);

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

        return initialized ? bounds : new Bounds(Vector3.zero, new Vector3(275f, 24f, 0f));
    }

    private static void EnsureEffectAssets()
    {
        Directory.CreateDirectory(EffectsPath);
        CreateRadialTexture($"{EffectsPath}/N5_Glow.png", 128, new Color(1f, 0.82f, 0.34f, 0.8f));
        CreateFogTexture($"{EffectsPath}/N5_Fog.png", 256, 64);
        CreateRayTexture($"{EffectsPath}/N5_LightRay.png", 96, 256);
        CreateLeafTexture($"{EffectsPath}/N5_Leaf.png", 32, 32);
        AssetDatabase.Refresh();

        ImportEffectSprite($"{EffectsPath}/N5_Glow.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N5_Fog.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N5_LightRay.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N5_Leaf.png", 32f);

        CreateMaterial($"{EffectsPath}/N5_Glow.mat", $"{EffectsPath}/N5_Glow.png", new Color(1f, 0.86f, 0.45f, 1f));
        CreateMaterial($"{EffectsPath}/N5_Leaf.mat", $"{EffectsPath}/N5_Leaf.png", new Color(0.98f, 0.84f, 0.35f, 1f));
    }

    private static void CreateFog(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N5_Fog.png");
        for (int i = 0; i < 3; i++)
        {
            GameObject fog = new($"Niebla_Baja_{i + 1}");
            fog.transform.SetParent(parent);
            fog.transform.localPosition = new Vector3((i - 1) * 8f, -3.8f + i * 0.25f, 24f + i);
            fog.transform.localRotation = Quaternion.identity;
            fog.transform.localScale = new Vector3(8.5f, 1.35f, 1f);

            SpriteRenderer renderer = fog.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.86f, 0.95f, 0.78f, 0.18f);
            renderer.sortingOrder = 4 + i;
        }
    }

    private static void CreateLightRays(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N5_LightRay.png");
        for (int i = 0; i < 3; i++)
        {
            GameObject ray = new($"Rayo_Luz_{i + 1}");
            ray.transform.SetParent(parent);
            ray.transform.localPosition = new Vector3(-6f + i * 5.5f, 1.1f, 22f + i);
            ray.transform.localRotation = Quaternion.Euler(0f, 0f, -11f + i * 5f);
            ray.transform.localScale = new Vector3(1.15f, 1.75f, 1f);

            SpriteRenderer renderer = ray.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(1f, 0.9f, 0.48f, 0.16f);
            renderer.sortingOrder = -18 + i;
        }
    }

    private static void CreateGlow(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N5_Glow.png");
        Vector3[] positions =
        {
            new(-7.5f, 1.9f, 23f),
            new(0.5f, 2.35f, 23f),
            new(7.0f, 1.7f, 23f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject glow = new($"Brillo_Linterna_{i + 1}");
            glow.transform.SetParent(parent);
            glow.transform.localPosition = positions[i];
            glow.transform.localRotation = Quaternion.identity;
            glow.transform.localScale = Vector3.one * (1.1f + i * 0.12f);

            SpriteRenderer renderer = glow.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(1f, 0.76f, 0.28f, 0.24f);
            renderer.sortingOrder = -8 + i;
        }
    }

    private static void CreatePollen(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N5_Glow.mat");
        ParticleSystem pollen = CreateParticleSystem("Polen_Luz", parent, new Vector3(0f, 0.1f, 18f), material, 8);

        ParticleSystem.MainModule main = pollen.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 11f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.08f, 0.32f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.025f, 0.075f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.78f, 0.32f, 0.18f),
            new Color(0.9f, 1f, 0.58f, 0.34f));
        main.maxParticles = 95;

        ParticleSystem.EmissionModule emission = pollen.emission;
        emission.rateOverTime = 12f;

        ParticleSystem.ShapeModule shape = pollen.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(24f, 10f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = pollen.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.08f, 0.16f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.02f, 0.12f);

        ParticleSystem.NoiseModule noise = pollen.noise;
        noise.enabled = true;
        noise.strength = 0.18f;
        noise.frequency = 0.28f;
        noise.scrollSpeed = 0.12f;
    }

    private static void CreateLeaves(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N5_Leaf.mat");
        ParticleSystem leaves = CreateParticleSystem("Hojas_Suaves", parent, new Vector3(0f, 4.8f, 19f), material, 7);

        ParticleSystem.MainModule main = leaves.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(8f, 15f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.15f, 0.45f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.24f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.95f, 0.68f, 0.22f, 0.42f),
            new Color(0.55f, 0.84f, 0.28f, 0.38f));
        main.maxParticles = 28;

        ParticleSystem.EmissionModule emission = leaves.emission;
        emission.rateOverTime = 2.3f;

        ParticleSystem.ShapeModule shape = leaves.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(22f, 1f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = leaves.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.35f, -0.05f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.55f, -0.18f);

        ParticleSystem.RotationOverLifetimeModule rotation = leaves.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-1.4f, 1.4f);

        ParticleSystem.NoiseModule noise = leaves.noise;
        noise.enabled = true;
        noise.strength = 0.32f;
        noise.frequency = 0.18f;
    }

    private static ParticleSystem CreateParticleSystem(string name, Transform parent, Vector3 localPosition, Material material, int sortingOrder)
    {
        GameObject go = new(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        ParticleSystem particleSystem = go.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSystem.main;
        main.loop = true;
        main.playOnAwake = true;
        main.prewarm = true;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.scalingMode = ParticleSystemScalingMode.Hierarchy;

        ParticleSystemRenderer renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = sortingOrder;
        renderer.material = material;

        particleSystem.Play();
        return particleSystem;
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

    private static void ConfigureCamera(Transform target, Tilemap entorno)
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

        Bounds bounds = entorno != null ? entorno.localBounds : new Bounds(Vector3.zero, new Vector3(275f, 24f, 0f));
        SerializedObject serialized = new(follow);
        serialized.FindProperty("objetivo").objectReferenceValue = target;
        serialized.FindProperty("offset").vector3Value = new Vector3(0f, -1.5f, -10f);
        serialized.FindProperty("usarLimites").boolValue = true;
        serialized.FindProperty("limiteMin").vector2Value = new Vector2(bounds.min.x, bounds.min.y - 8f);
        serialized.FindProperty("limiteMax").vector2Value = new Vector2(bounds.max.x, bounds.max.y + 8f);
        serialized.ApplyModifiedPropertiesWithoutUndo();

        mainCamera.transform.position = target.position + new Vector3(0f, -1.5f, -10f);
    }

    private static Vector3 FindSpawnPosition(Tilemap entorno)
    {
        if (entorno == null)
        {
            return new Vector3(0f, 1.5f, 0f);
        }

        BoundsInt bounds = entorno.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMax - 1; y >= bounds.yMin; y--)
            {
                Vector3Int cell = new(x, y, 0);
                if (!entorno.HasTile(cell) || entorno.HasTile(new Vector3Int(x, y + 1, 0)))
                {
                    continue;
                }

                return entorno.GetCellCenterWorld(cell) + new Vector3(0f, 1.1f, 0f);
            }
        }

        return entorno.transform.position + new Vector3(0f, 1.5f, 0f);
    }

    private static Tilemap FindTilemap(string name)
    {
        Tilemap[] tilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsInactive.Include);
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap != null && tilemap.gameObject.name == name)
            {
                return tilemap;
            }
        }

        return null;
    }

    private static void EnsureTilemapCollider(GameObject target, bool isTrigger)
    {
        TilemapCollider2D tilemapCollider = target.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            tilemapCollider = target.AddComponent<TilemapCollider2D>();
        }

        tilemapCollider.isTrigger = isTrigger;
        tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;

        Rigidbody2D rigidbody = target.GetComponent<Rigidbody2D>();
        if (rigidbody == null)
        {
            rigidbody = target.AddComponent<Rigidbody2D>();
        }

        rigidbody.bodyType = RigidbodyType2D.Static;
        rigidbody.simulated = true;

        CompositeCollider2D composite = target.GetComponent<CompositeCollider2D>();
        if (composite == null)
        {
            composite = target.AddComponent<CompositeCollider2D>();
        }

        composite.isTrigger = isTrigger;
        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
    }

    private static void SetTilemapSorting(GameObject target, int sortingOrder)
    {
        TilemapRenderer renderer = target.GetComponent<TilemapRenderer>();
        if (renderer == null)
        {
            return;
        }

        renderer.sortingOrder = sortingOrder;
    }

    private static void CreateRadialTexture(string path, int size, Color color)
    {
        Texture2D texture = new(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new((size - 1) * 0.5f, (size - 1) * 0.5f);
        float radius = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / radius;
                float alpha = Mathf.Clamp01(1f - distance);
                alpha *= alpha;
                texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreateFogTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float vertical = Mathf.Sin((float)y / (height - 1) * Mathf.PI);
            for (int x = 0; x < width; x++)
            {
                float wave = 0.78f + Mathf.Sin(x * 0.07f) * 0.12f + Mathf.Sin((x + y) * 0.031f) * 0.1f;
                float alpha = Mathf.Clamp01(vertical * wave) * 0.55f;
                texture.SetPixel(x, y, new Color(0.82f, 0.95f, 0.78f, alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreateRayTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        float centerX = (width - 1) * 0.5f;

        for (int y = 0; y < height; y++)
        {
            float vertical = 1f - (float)y / (height - 1);
            for (int x = 0; x < width; x++)
            {
                float horizontal = Mathf.Clamp01(1f - Mathf.Abs(x - centerX) / centerX);
                float alpha = horizontal * horizontal * vertical * 0.65f;
                texture.SetPixel(x, y, new Color(1f, 0.88f, 0.46f, alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreateLeafTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        Color clear = new(0f, 0f, 0f, 0f);
        Color edge = new(0.34f, 0.48f, 0.15f, 1f);
        Color body = new(0.68f, 0.88f, 0.27f, 1f);
        Color light = new(0.96f, 0.78f, 0.26f, 1f);
        Vector2 center = new(width * 0.5f, height * 0.5f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (x - center.x) / 12f;
                float dy = (y - center.y) / 5f;
                float value = dx * dx + dy * dy;
                if (value > 1f)
                {
                    texture.SetPixel(x, y, clear);
                    continue;
                }

                Color pixel = value > 0.72f ? edge : body;
                if (x > center.x && y > center.y - 2f)
                {
                    pixel = Color.Lerp(pixel, light, 0.55f);
                }

                texture.SetPixel(x, y, pixel);
            }
        }

        for (int i = 8; i < 25; i++)
        {
            int y = Mathf.RoundToInt(16 + (i - 16) * 0.25f);
            texture.SetPixel(i, y, edge);
        }

        SaveTexture(texture, path);
    }

    private static void SaveTexture(Texture2D texture, string path)
    {
        File.WriteAllBytes(path, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);
    }

    private static void ImportEffectSprite(string path, float pixelsPerUnit)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            importer = AssetImporter.GetAtPath(path) as TextureImporter;
        }

        if (importer == null)
        {
            Debug.LogWarning($"Nivel 5: no se pudo importar efecto {path}");
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = pixelsPerUnit;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Bilinear;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
    }

    private static void CreateMaterial(string materialPath, string texturePath, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Particles/Standard Unlit");
        }

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (material == null)
        {
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, materialPath);
        }
        else
        {
            material.shader = shader;
        }

        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        material.mainTexture = texture;
        if (material.HasProperty("_BaseMap"))
        {
            material.SetTexture("_BaseMap", texture);
        }

        material.color = color;
        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", color);
        }

        EditorUtility.SetDirty(material);
        AssetDatabase.SaveAssets();
    }

    private static Sprite LoadFirstSprite(string path)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite != null)
        {
            return sprite;
        }

        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (Object asset in assets)
        {
            if (asset is Sprite childSprite)
            {
                return childSprite;
            }
        }

        Debug.LogWarning($"Nivel 5: no se encontro sprite de jugador en {path}");
        return null;
    }
}
