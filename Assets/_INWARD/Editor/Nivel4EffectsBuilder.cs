using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Nivel4EffectsBuilder
{
    private const string EffectsPath = "Assets/_INWARD/Sprites/Nivel 4/Effects";
    private const string RootName = "Effects_N4";

    [MenuItem("Tools/INWARD/Nivel 4/Build Effects")]
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

        CreateMirrorHaze(root.transform);
        CreateCrackFlares(root.transform);
        CreateShardDust(root.transform);
        CreateReflectionGlints(root.transform);
        CreateRedPulse(root.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 4: efectos visuales aplicados (haze, grietas, polvo de cristal, reflejos y pulso rojo).");
    }

    private static void EnsureEffectAssets()
    {
        Directory.CreateDirectory(EffectsPath);
        CreateRadialTexture($"{EffectsPath}/N4_Red_Glow.png", 128, new Color(1f, 0.12f, 0.16f, 0.72f));
        CreateRadialTexture($"{EffectsPath}/N4_White_Glint.png", 96, new Color(1f, 0.92f, 0.86f, 0.82f));
        CreateShardTexture($"{EffectsPath}/N4_Glass_Shard.png", 32, 32);
        CreateHazeTexture($"{EffectsPath}/N4_Mirror_Haze.png", 256, 96);
        CreateCrackTexture($"{EffectsPath}/N4_Light_Crack.png", 96, 256);
        AssetDatabase.Refresh();

        ImportEffectSprite($"{EffectsPath}/N4_Red_Glow.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N4_White_Glint.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N4_Glass_Shard.png", 32f);
        ImportEffectSprite($"{EffectsPath}/N4_Mirror_Haze.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N4_Light_Crack.png", 100f);

        CreateMaterial($"{EffectsPath}/N4_Shard.mat", $"{EffectsPath}/N4_Glass_Shard.png", new Color(1f, 0.84f, 0.86f, 1f));
        CreateMaterial($"{EffectsPath}/N4_Glint.mat", $"{EffectsPath}/N4_White_Glint.png", new Color(1f, 0.9f, 0.9f, 1f));
    }

    private static void CreateMirrorHaze(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N4_Mirror_Haze.png");
        for (int i = 0; i < 4; i++)
        {
            GameObject haze = new($"Haze_Espejo_{i + 1}");
            haze.transform.SetParent(parent);
            haze.transform.localPosition = new Vector3(-9f + i * 6f, -3.55f + (i % 2) * 0.42f, 24f + i);
            haze.transform.localRotation = Quaternion.identity;
            haze.transform.localScale = new Vector3(7.2f, 1.18f, 1f);

            SpriteRenderer renderer = haze.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.72f, 0.12f, 0.18f, 0.16f);
            renderer.sortingOrder = 3 + i;
        }
    }

    private static void CreateCrackFlares(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N4_Light_Crack.png");
        for (int i = 0; i < 4; i++)
        {
            GameObject crack = new($"Grieta_Luz_{i + 1}");
            crack.transform.SetParent(parent);
            crack.transform.localPosition = new Vector3(-8.5f + i * 5.8f, 0.55f + (i % 2) * 1.3f, 21f + i);
            crack.transform.localRotation = Quaternion.Euler(0f, 0f, -18f + i * 12f);
            crack.transform.localScale = new Vector3(0.9f, 1.45f, 1f);

            SpriteRenderer renderer = crack.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(1f, 0.78f, 0.78f, 0.13f);
            renderer.sortingOrder = -10 + i;
        }
    }

    private static void CreateShardDust(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N4_Shard.mat");
        ParticleSystem shards = CreateParticleSystem("Polvo_Cristal", parent, new Vector3(0f, 0.1f, 17f), material, 8);

        ParticleSystem.MainModule main = shards.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(4f, 8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.03f, 0.26f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.025f, 0.075f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.82f, 0.82f, 0.08f),
            new Color(1f, 0.18f, 0.24f, 0.16f));
        main.maxParticles = 42;

        ParticleSystem.EmissionModule emission = shards.emission;
        emission.rateOverTime = 3.5f;

        ParticleSystem.ShapeModule shape = shards.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(22f, 9f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = shards.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.14f, 0.14f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.05f, 0.18f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        ParticleSystem.RotationOverLifetimeModule rotation = shards.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-2.6f, 2.6f);

        ParticleSystem.NoiseModule noise = shards.noise;
        noise.enabled = true;
        noise.strength = 0.26f;
        noise.frequency = 0.34f;
    }

    private static void CreateReflectionGlints(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N4_Glint.mat");
        ParticleSystem glints = CreateParticleSystem("Destellos_Reflejo", parent, new Vector3(0f, 0.35f, 16f), material, 9);

        ParticleSystem.MainModule main = glints.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.4f, 3.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.12f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.035f, 0.11f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 1f, 0.95f, 0.2f),
            new Color(1f, 0.46f, 0.52f, 0.32f));
        main.maxParticles = 38;

        ParticleSystem.EmissionModule emission = glints.emission;
        emission.rateOverTime = 4f;

        ParticleSystem.ShapeModule shape = glints.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(20f, 8f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = glints.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.08f, 0.08f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.02f, 0.08f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);
    }

    private static void CreateRedPulse(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N4_Red_Glow.png");
        Vector3[] positions =
        {
            new(-7.8f, 2.35f, 22f),
            new(0f, 2.8f, 22f),
            new(7.8f, 2.25f, 22f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject pulse = new($"Pulso_Rojo_{i + 1}");
            pulse.transform.SetParent(parent);
            pulse.transform.localPosition = positions[i];
            pulse.transform.localRotation = Quaternion.identity;
            pulse.transform.localScale = Vector3.one * (0.92f + i * 0.08f);

            SpriteRenderer renderer = pulse.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(1f, 0.05f, 0.12f, 0.11f);
            renderer.sortingOrder = -6 + i;
        }
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

    private static void CreateShardTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        Color clear = new(0f, 0f, 0f, 0f);
        Color edge = new(1f, 0.86f, 0.88f, 0.95f);
        Color body = new(0.9f, 0.34f, 0.38f, 0.7f);

        Vector2 a = new(7f, 5f);
        Vector2 b = new(25f, 13f);
        Vector2 c = new(11f, 27f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 p = new(x, y);
                if (!PointInTriangle(p, a, b, c))
                {
                    texture.SetPixel(x, y, clear);
                    continue;
                }

                float edgeDistance = Mathf.Min(DistanceToLine(p, a, b), Mathf.Min(DistanceToLine(p, b, c), DistanceToLine(p, c, a)));
                Color pixel = edgeDistance < 1.2f ? edge : body;
                texture.SetPixel(x, y, pixel);
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreateHazeTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float vertical = Mathf.Sin((float)y / (height - 1) * Mathf.PI);
            for (int x = 0; x < width; x++)
            {
                float wave = 0.7f + Mathf.Sin(x * 0.05f) * 0.16f + Mathf.Sin((x - y) * 0.029f) * 0.1f;
                float alpha = Mathf.Clamp01(vertical * wave) * 0.5f;
                texture.SetPixel(x, y, new Color(0.7f, 0.08f, 0.14f, alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreateCrackTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            float center = width * (0.48f + Mathf.Sin(t * 14f) * 0.1f + Mathf.Sin(t * 31f) * 0.04f);
            for (int x = 0; x < width; x++)
            {
                float d = Mathf.Abs(x - center);
                float alpha = Mathf.Clamp01(1f - d / 4.2f) * Mathf.Sin(t * Mathf.PI) * 0.9f;
                if (d > 5f)
                {
                    alpha *= 0.12f;
                }

                texture.SetPixel(x, y, new Color(1f, 0.82f, 0.82f, alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static bool PointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
    {
        float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
        float s = 1f / (2f * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
        float t = 1f / (2f * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
        return s >= 0f && t >= 0f && 1f - s - t >= 0f;
    }

    private static float DistanceToLine(Vector2 p, Vector2 a, Vector2 b)
    {
        return Mathf.Abs((b.y - a.y) * p.x - (b.x - a.x) * p.y + b.x * a.y - b.y * a.x) / Vector2.Distance(a, b);
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
            Debug.LogWarning($"Nivel 4: no se pudo importar efecto {path}");
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
