using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Nivel3EffectsBuilder
{
    private const string EffectsPath = "Assets/_INWARD/Sprites/Nivel 3/Effects";
    private const string RootName = "Effects_N3";

    [MenuItem("Tools/INWARD/Nivel 3/Build Effects")]
    public static void BuildEffects()
    {
        EnsureEffectAssets();

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

        CreateDeepFog(root.transform);
        CreateWaterStreaks(root.transform);
        CreateBubbles(root.transform);
        CreateSoftFireflies(root.transform);
        CreateSurfaceGlow(root.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 3: efectos visuales aplicados (niebla profunda, corrientes, burbujas, luciernagas y luz de superficie).");
    }

    private static void EnsureEffectAssets()
    {
        Directory.CreateDirectory(EffectsPath);
        CreateRadialTexture($"{EffectsPath}/N3_Bubble.png", 64, new Color(0.62f, 0.86f, 1f, 0.58f), hollow: true);
        CreateRadialTexture($"{EffectsPath}/N3_Firefly.png", 96, new Color(0.55f, 0.82f, 1f, 0.72f), hollow: false);
        CreateFogTexture($"{EffectsPath}/N3_Deep_Fog.png", 256, 96);
        CreateStreakTexture($"{EffectsPath}/N3_Water_Streak.png", 64, 256);
        CreateSurfaceTexture($"{EffectsPath}/N3_Surface_Glow.png", 256, 96);
        AssetDatabase.Refresh();

        ImportEffectSprite($"{EffectsPath}/N3_Bubble.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N3_Firefly.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N3_Deep_Fog.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N3_Water_Streak.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N3_Surface_Glow.png", 100f);

        CreateMaterial($"{EffectsPath}/N3_Bubble.mat", $"{EffectsPath}/N3_Bubble.png", new Color(0.72f, 0.9f, 1f, 1f));
        CreateMaterial($"{EffectsPath}/N3_Firefly.mat", $"{EffectsPath}/N3_Firefly.png", new Color(0.5f, 0.76f, 1f, 1f));
    }

    private static void CreateDeepFog(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N3_Deep_Fog.png");
        Color[] colors =
        {
            new(0.16f, 0.18f, 0.23f, 0.26f),
            new(0.12f, 0.23f, 0.31f, 0.22f),
            new(0.25f, 0.22f, 0.36f, 0.18f)
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject fog = new($"Niebla_Mar_{i + 1}");
            fog.transform.SetParent(parent);
            fog.transform.localPosition = new Vector3(-9f + i * 6f, -3.75f + (i % 2) * 0.42f, 23f + i);
            fog.transform.localRotation = Quaternion.identity;
            fog.transform.localScale = new Vector3(7.4f, 1.45f, 1f);

            SpriteRenderer renderer = fog.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = colors[i % colors.Length];
            renderer.sortingOrder = 3 + i;
        }
    }

    private static void CreateWaterStreaks(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N3_Water_Streak.png");
        for (int i = 0; i < 5; i++)
        {
            GameObject streak = new($"Corriente_Lenta_{i + 1}");
            streak.transform.SetParent(parent);
            streak.transform.localPosition = new Vector3(-10f + i * 5f, 0.4f + (i % 2) * 1.1f, 20f + i);
            streak.transform.localRotation = Quaternion.Euler(0f, 0f, -3f + i * 1.5f);
            streak.transform.localScale = new Vector3(1.35f, 1.65f, 1f);

            SpriteRenderer renderer = streak.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.34f, 0.58f, 0.78f, 0.13f);
            renderer.sortingOrder = -12 + i;
        }
    }

    private static void CreateBubbles(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N3_Bubble.mat");
        ParticleSystem bubbles = CreateParticleSystem("Burbujas_Lentas", parent, new Vector3(0f, -4.2f, 17f), material, 7);

        ParticleSystem.MainModule main = bubbles.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 12f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.08f, 0.28f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.035f, 0.13f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.55f, 0.82f, 1f, 0.16f),
            new Color(0.76f, 0.95f, 1f, 0.36f));
        main.maxParticles = 70;

        ParticleSystem.EmissionModule emission = bubbles.emission;
        emission.rateOverTime = 7f;

        ParticleSystem.ShapeModule shape = bubbles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(21f, 1.2f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = bubbles.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.08f, 0.08f);
        velocity.y = new ParticleSystem.MinMaxCurve(0.18f, 0.48f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        ParticleSystem.NoiseModule noise = bubbles.noise;
        noise.enabled = true;
        noise.strength = 0.18f;
        noise.frequency = 0.25f;
        noise.scrollSpeed = 0.1f;
    }

    private static void CreateSoftFireflies(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N3_Firefly.mat");
        ParticleSystem fireflies = CreateParticleSystem("Luciernagas_Azules", parent, new Vector3(0f, 0.35f, 16f), material, 8);

        ParticleSystem.MainModule main = fireflies.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.03f, 0.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.11f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.44f, 0.66f, 1f, 0.16f),
            new Color(0.8f, 0.65f, 1f, 0.32f));
        main.maxParticles = 42;

        ParticleSystem.EmissionModule emission = fireflies.emission;
        emission.rateOverTime = 4.5f;

        ParticleSystem.ShapeModule shape = fireflies.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(20f, 8f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = fireflies.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.12f, 0.12f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.04f, 0.16f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        ParticleSystem.NoiseModule noise = fireflies.noise;
        noise.enabled = true;
        noise.strength = 0.22f;
        noise.frequency = 0.3f;
    }

    private static void CreateSurfaceGlow(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N3_Surface_Glow.png");
        GameObject glow = new("Luz_Superficie_Suave");
        glow.transform.SetParent(parent);
        glow.transform.localPosition = new Vector3(0f, 4.25f, 22f);
        glow.transform.localRotation = Quaternion.identity;
        glow.transform.localScale = new Vector3(5.8f, 1.25f, 1f);

        SpriteRenderer renderer = glow.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = new Color(0.42f, 0.58f, 1f, 0.055f);
        renderer.sortingOrder = -6;
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

    private static void CreateRadialTexture(string path, int size, Color color, bool hollow)
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
                if (hollow)
                {
                    float ring = Mathf.Clamp01(1f - Mathf.Abs(distance - 0.62f) * 7f);
                    alpha = Mathf.Max(ring * 0.8f, alpha * 0.12f);
                }
                else
                {
                    alpha *= alpha;
                }

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
                float wave = 0.76f + Mathf.Sin(x * 0.045f) * 0.16f + Mathf.Sin((x + y) * 0.026f) * 0.12f;
                float alpha = Mathf.Clamp01(vertical * wave) * 0.58f;
                texture.SetPixel(x, y, new Color(0.16f, 0.23f, 0.34f, alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreateStreakTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        float centerX = (width - 1) * 0.5f;

        for (int y = 0; y < height; y++)
        {
            float vertical = Mathf.Sin((float)y / (height - 1) * Mathf.PI);
            for (int x = 0; x < width; x++)
            {
                float horizontal = Mathf.Clamp01(1f - Mathf.Abs(x - centerX) / centerX);
                float alpha = horizontal * horizontal * vertical * 0.36f;
                texture.SetPixel(x, y, new Color(0.42f, 0.72f, 0.92f, alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreateSurfaceTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float vertical = 1f - (float)y / (height - 1);
            for (int x = 0; x < width; x++)
            {
                float edge = Mathf.Sin((float)x / (width - 1) * Mathf.PI);
                float wave = 0.75f + Mathf.Sin(x * 0.08f) * 0.18f;
                float alpha = Mathf.Clamp01(vertical * wave * edge) * 0.46f;
                texture.SetPixel(x, y, new Color(0.5f, 0.62f, 1f, alpha));
            }
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
            Debug.LogWarning($"Nivel 3: no se pudo importar efecto {path}");
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
}
