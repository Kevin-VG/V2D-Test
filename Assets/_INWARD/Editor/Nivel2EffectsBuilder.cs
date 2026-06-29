using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Nivel2EffectsBuilder
{
    private const string EffectsPath = "Assets/_INWARD/Sprites/Nivel 2/Effects";
    private const string RootName = "Effects_N2";

    [MenuItem("Tools/INWARD/Nivel 2/Build Effects")]
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

        CreateDust(root.transform);
        CreateVoiceFog(root.transform);
        CreateLampGlows(root.transform);
        CreateFloatingPapers(root.transform);
        CreateMemorySpecks(root.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Nivel 2: efectos visuales aplicados (polvo, niebla, brillos, papeles y motas de memoria).");
    }

    private static void EnsureEffectAssets()
    {
        Directory.CreateDirectory(EffectsPath);
        CreateRadialTexture($"{EffectsPath}/N2_Glow_Dorado.png", 128, new Color(1f, 0.63f, 0.18f, 0.82f));
        CreateRadialTexture($"{EffectsPath}/N2_Dust_Mote.png", 64, new Color(0.92f, 0.73f, 0.42f, 0.58f));
        CreateFogTexture($"{EffectsPath}/N2_Voice_Fog.png", 256, 80);
        CreatePaperTexture($"{EffectsPath}/N2_Paper.png", 32, 32);
        AssetDatabase.Refresh();

        ImportEffectSprite($"{EffectsPath}/N2_Glow_Dorado.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N2_Dust_Mote.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N2_Voice_Fog.png", 100f);
        ImportEffectSprite($"{EffectsPath}/N2_Paper.png", 32f);

        CreateMaterial($"{EffectsPath}/N2_Dust.mat", $"{EffectsPath}/N2_Dust_Mote.png", new Color(0.96f, 0.75f, 0.42f, 1f));
        CreateMaterial($"{EffectsPath}/N2_Paper.mat", $"{EffectsPath}/N2_Paper.png", new Color(0.9f, 0.76f, 0.53f, 1f));
        CreateMaterial($"{EffectsPath}/N2_Gold.mat", $"{EffectsPath}/N2_Glow_Dorado.png", new Color(1f, 0.66f, 0.22f, 1f));
    }

    private static void CreateDust(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N2_Dust.mat");
        ParticleSystem dust = CreateParticleSystem("Polvo_Archivo", parent, new Vector3(0f, 0.3f, 17f), material, 7);

        ParticleSystem.MainModule main = dust.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(7f, 13f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.03f, 0.22f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.018f, 0.055f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.82f, 0.63f, 0.34f, 0.16f),
            new Color(1f, 0.86f, 0.48f, 0.34f));
        main.maxParticles = 140;

        ParticleSystem.EmissionModule emission = dust.emission;
        emission.rateOverTime = 18f;

        ParticleSystem.ShapeModule shape = dust.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(24f, 10.5f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = dust.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.05f, 0.1f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.02f, 0.1f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        ParticleSystem.NoiseModule noise = dust.noise;
        noise.enabled = true;
        noise.strength = 0.14f;
        noise.frequency = 0.23f;
        noise.scrollSpeed = 0.08f;
    }

    private static void CreateVoiceFog(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N2_Voice_Fog.png");
        for (int i = 0; i < 4; i++)
        {
            GameObject fog = new($"Niebla_Voz_{i + 1}");
            fog.transform.SetParent(parent);
            fog.transform.localPosition = new Vector3(-9f + i * 6f, -3.9f + (i % 2) * 0.45f, 23f + i);
            fog.transform.localRotation = Quaternion.identity;
            fog.transform.localScale = new Vector3(6.8f, 1.2f, 1f);

            SpriteRenderer renderer = fog.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(0.28f, 0.18f, 0.35f, 0.24f);
            renderer.sortingOrder = 3 + i;
        }
    }

    private static void CreateLampGlows(Transform parent)
    {
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{EffectsPath}/N2_Glow_Dorado.png");
        Vector3[] positions =
        {
            new(-7.8f, 2.65f, 21f),
            new(-1.8f, 2.3f, 21f),
            new(4.1f, 2.55f, 21f),
            new(8.5f, 1.85f, 21f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject glow = new($"Brillo_Foco_{i + 1}");
            glow.transform.SetParent(parent);
            glow.transform.localPosition = positions[i];
            glow.transform.localRotation = Quaternion.identity;
            glow.transform.localScale = Vector3.one * (0.72f + i * 0.05f);

            SpriteRenderer renderer = glow.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = new Color(1f, 0.68f, 0.24f, 0.11f);
            renderer.sortingOrder = -7 + i;
        }
    }

    private static void CreateFloatingPapers(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N2_Paper.mat");
        ParticleSystem papers = CreateParticleSystem("Papeles_Flotantes", parent, new Vector3(0f, 4.8f, 18f), material, 8);

        ParticleSystem.MainModule main = papers.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(6f, 12f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.12f, 0.35f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.13f, 0.28f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.86f, 0.7f, 0.48f, 0.28f),
            new Color(1f, 0.86f, 0.58f, 0.42f));
        main.maxParticles = 24;

        ParticleSystem.EmissionModule emission = papers.emission;
        emission.rateOverTime = 1.8f;

        ParticleSystem.ShapeModule shape = papers.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(22f, 1f, 0.1f);

        ParticleSystem.VelocityOverLifetimeModule velocity = papers.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.x = new ParticleSystem.MinMaxCurve(-0.18f, 0.22f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.5f, -0.15f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        ParticleSystem.RotationOverLifetimeModule rotation = papers.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-1.8f, 1.8f);

        ParticleSystem.NoiseModule noise = papers.noise;
        noise.enabled = true;
        noise.strength = 0.42f;
        noise.frequency = 0.2f;
    }

    private static void CreateMemorySpecks(Transform parent)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>($"{EffectsPath}/N2_Gold.mat");
        ParticleSystem specks = CreateParticleSystem("Motas_Memoria", parent, new Vector3(0f, -0.4f, 16f), material, 6);

        ParticleSystem.MainModule main = specks.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(3.5f, 7f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.02f, 0.18f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.035f, 0.09f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(1f, 0.52f, 0.16f, 0.18f),
            new Color(1f, 0.88f, 0.38f, 0.38f));
        main.maxParticles = 55;

        ParticleSystem.EmissionModule emission = specks.emission;
        emission.rateOverTime = 7f;

        ParticleSystem.ShapeModule shape = specks.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(18f, 7f, 0.1f);

        ParticleSystem.NoiseModule noise = specks.noise;
        noise.enabled = true;
        noise.strength = 0.28f;
        noise.frequency = 0.34f;
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

    private static void CreateFogTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        for (int y = 0; y < height; y++)
        {
            float vertical = Mathf.Sin((float)y / (height - 1) * Mathf.PI);
            for (int x = 0; x < width; x++)
            {
                float wave = 0.7f + Mathf.Sin(x * 0.052f) * 0.18f + Mathf.Sin((x - y) * 0.027f) * 0.12f;
                float alpha = Mathf.Clamp01(vertical * wave) * 0.62f;
                texture.SetPixel(x, y, new Color(0.28f, 0.18f, 0.36f, alpha));
            }
        }

        SaveTexture(texture, path);
    }

    private static void CreatePaperTexture(string path, int width, int height)
    {
        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);
        Color clear = new(0f, 0f, 0f, 0f);
        Color edge = new(0.45f, 0.31f, 0.19f, 1f);
        Color body = new(0.82f, 0.67f, 0.45f, 1f);
        Color light = new(0.95f, 0.83f, 0.6f, 1f);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool inside = x >= 6 && x <= 25 && y >= 5 && y <= 26;
                if (!inside)
                {
                    texture.SetPixel(x, y, clear);
                    continue;
                }

                bool border = x == 6 || x == 25 || y == 5 || y == 26;
                Color pixel = border ? edge : body;
                if (x > 17 && y > 16)
                {
                    pixel = Color.Lerp(pixel, light, 0.5f);
                }

                texture.SetPixel(x, y, pixel);
            }
        }

        for (int x = 10; x < 22; x++)
        {
            texture.SetPixel(x, 13, edge);
            texture.SetPixel(x, 18, edge);
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
            Debug.LogWarning($"Nivel 2: no se pudo importar efecto {path}");
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
