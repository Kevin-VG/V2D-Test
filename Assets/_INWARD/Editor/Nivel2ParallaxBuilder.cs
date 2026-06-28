using System.IO;
using Shatter.CameraSystem;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Nivel2ParallaxBuilder
{
    private const string BackgroundPath = "Assets/_INWARD/Sprites/Nivel 2/Background";
    private const string RootName = "Background_N2";
    private const float BackgroundCenterY = 0f;
    private const float BackgroundScale = 1.28f;
    private const float BackgroundPixelsPerUnit = 100f;

    private readonly struct PhaseConfig
    {
        public PhaseConfig(string name, string containerName, float start, float end, string sprite, float parallaxX)
        {
            Name = name;
            ContainerName = containerName;
            Start = start;
            End = end;
            Sprite = sprite;
            ParallaxX = parallaxX;
        }

        public string Name { get; }
        public string ContainerName { get; }
        public float Start { get; }
        public float End { get; }
        public string Sprite { get; }
        public float ParallaxX { get; }
    }

    private static readonly PhaseConfig[] Phases =
    {
        new(
            "Archivo dorado",
            "Fase_Archivo_Dorado",
            -20f,
            185f,
            "BG_1.png",
            0.68f),
        new(
            "Archivo flotante",
            "Fase_Archivo_Flotante",
            185f,
            345f,
            "BG_2.png",
            0.76f),
        new(
            "Gran Mesa",
            "Fase_Gran_Mesa",
            340f,
            470f,
            "BG_3.png",
            0.84f)
    };

    [MenuItem("Tools/INWARD/Nivel 2/Build Parallax Background")]
    public static void Build()
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

        var controller = root.AddComponent<LevelBackgroundPhaseController>();

        GameObject[] containers = new GameObject[Phases.Length];
        for (int i = 0; i < Phases.Length; i++)
        {
            PhaseConfig phase = Phases[i];
            GameObject container = new(phase.ContainerName);
            container.transform.SetParent(root.transform);
            container.transform.localPosition = Vector3.zero;
            containers[i] = container;

            CreateLayer(container.transform, phase, i, mainCamera);
        }

        ConfigurePhaseController(controller, containers, mainCamera);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 2: fondos parallax construidos y aplicados en la escena activa.");
    }

    private static void ImportBackgroundSprites()
    {
        foreach (string path in Directory.GetFiles(BackgroundPath, "*.png"))
        {
            string assetPath = path.Replace("\\", "/");
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
                importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            }

            if (importer == null)
            {
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

    private static void CreateLayer(Transform parent, PhaseConfig phase, int phaseIndex, Camera mainCamera)
    {
        string fileName = phase.Sprite;
        string spritePath = $"{BackgroundPath}/{fileName}";
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if (sprite == null)
        {
            Debug.LogWarning($"Nivel 2: no se encontro sprite de fondo {spritePath}");
            return;
        }

        GameObject layer = new(Path.GetFileNameWithoutExtension(fileName));
        layer.transform.SetParent(parent);
        layer.transform.localPosition = new Vector3(0f, BackgroundCenterY, 20f + phaseIndex);
        layer.transform.localScale = Vector3.one * BackgroundScale;

        SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.drawMode = SpriteDrawMode.Simple;
        renderer.sortingOrder = -100 + phaseIndex * 10;
    }

    private static void ConfigurePhaseController(LevelBackgroundPhaseController controller, GameObject[] containers, Camera mainCamera)
    {
        SerializedObject serialized = new(controller);
        serialized.FindProperty("objetivo").objectReferenceValue = mainCamera != null ? mainCamera.transform : null;
        serialized.FindProperty("ocultarRenderersInvisibles").boolValue = true;

        SerializedProperty phasesProperty = serialized.FindProperty("fases");
        phasesProperty.arraySize = Phases.Length;
        for (int i = 0; i < Phases.Length; i++)
        {
            SerializedProperty phaseProperty = phasesProperty.GetArrayElementAtIndex(i);
            phaseProperty.FindPropertyRelative("nombre").stringValue = Phases[i].Name;
            phaseProperty.FindPropertyRelative("contenedor").objectReferenceValue = containers[i];
            phaseProperty.FindPropertyRelative("xInicioCompleto").floatValue = Phases[i].Start;
            phaseProperty.FindPropertyRelative("xFinCompleto").floatValue = Phases[i].End;
            phaseProperty.FindPropertyRelative("distanciaFade").floatValue = 45f;
        }

        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
