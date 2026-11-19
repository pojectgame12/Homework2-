#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BuildHomework2Demo
{
    [MenuItem("Tools/Homework 2/Create Mini-Map Demo Scene")]
    public static void CreateScene()
    {
        EnsureFolder("Assets/Generated");
        EnsureFolder("Assets/Generated/Sprites");
        EnsureFolder("Assets/Scenes");

        Sprite ground = MakeSprite("Ground", new Color(0.11f, 0.18f, 0.25f));
        Sprite wall = MakeSprite("Wall", new Color(0.27f, 0.38f, 0.53f));
        Sprite playerSprite = MakeSprite("Player", new Color(0.17f, 0.86f, 0.98f));
        Sprite star = MakeSprite("Star", new Color(1f, 0.79f, 0.19f));
        Sprite obstacle = MakeSprite("Obstacle", new Color(0.53f, 0.42f, 0.72f));
        Sprite exit = MakeSprite("Exit", new Color(0.14f, 0.77f, 0.38f));

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var world = new GameObject("World").transform;
        Visual("Floor", ground, new Vector3(0f, 0f, 0f), new Vector2(24f, 14f), world, -5);
        Solid("Top wall", wall, new Vector2(0f, 7f), new Vector2(24f, 0.7f), world);
        Solid("Bottom wall", wall, new Vector2(0f, -7f), new Vector2(24f, 0.7f), world);
        Solid("Left wall", wall, new Vector2(-12f, 0f), new Vector2(0.7f, 14f), world);
        Solid("Right wall", wall, new Vector2(12f, 0f), new Vector2(0.7f, 14f), world);
        Solid("Obstacle 1", obstacle, new Vector2(-2.5f, -2f), new Vector2(1f, 6f), world);
        Solid("Obstacle 2", obstacle, new Vector2(4.5f, 2.5f), new Vector2(1f, 6f), world);
        Solid("Obstacle 3", obstacle, new Vector2(1f, 5.5f), new Vector2(7f, 0.7f), world);
        Visual("Goal marker", exit, new Vector3(10f, 5.5f, 0f), new Vector2(1.2f, 1.2f), world, 2);

        GameObject player = Visual("Player", playerSprite, new Vector3(-9.5f, -4.5f, 0f),
            new Vector2(0.85f, 0.85f), null, 3);
        player.tag = "Player";
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        player.AddComponent<BoxCollider2D>();
        player.AddComponent<PlayerMovement2D>();

        Vector2[] starPositions =
        {
            new Vector2(-7f, 4f), new Vector2(-3f, 4f), new Vector2(1f, 0f),
            new Vector2(7.5f, -4f), new Vector2(9f, 4f)
        };
        foreach (Vector2 position in starPositions)
        {
            var item = Visual("Star", star, new Vector3(position.x, position.y, 0f),
                new Vector2(0.5f, 0.5f), world, 1);
            item.AddComponent<CircleCollider2D>().isTrigger = true;
            item.AddComponent<CollectibleStar>();
        }
        var hud = new GameObject("GameManager").AddComponent<StarCounter>();

        GameObject mainCameraObject = new GameObject("Main Camera");
        mainCameraObject.tag = "MainCamera";
        Camera mainCamera = mainCameraObject.AddComponent<Camera>();
        mainCamera.transform.position = new Vector3(-9.5f, -4.5f, -10f);
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 5f;
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = new Color(0.04f, 0.07f, 0.12f);
        mainCameraObject.AddComponent<AudioListener>();
        mainCameraObject.AddComponent<KeepOrthographicScale>();
        mainCameraObject.AddComponent<CameraFollow2D>().SetTarget(player.transform);

        // RenderTexture works for both the Built-in pipeline and Universal Render Pipeline.
        string rtPath = "Assets/Generated/MinimapTexture.renderTexture";
        if (AssetDatabase.LoadAssetAtPath<RenderTexture>(rtPath) != null)
            AssetDatabase.DeleteAsset(rtPath);
        var rt = new RenderTexture(512, 512, 16, RenderTextureFormat.ARGB32);
        rt.name = "MinimapTexture";
        AssetDatabase.CreateAsset(rt, rtPath);

        GameObject minimapObject = new GameObject("MiniMap Camera");
        Camera miniCamera = minimapObject.AddComponent<Camera>();
        miniCamera.transform.position = new Vector3(0f, 0f, -10f);
        miniCamera.orthographic = true;
        miniCamera.orthographicSize = 13f;
        miniCamera.clearFlags = CameraClearFlags.SolidColor;
        miniCamera.backgroundColor = new Color(0.03f, 0.05f, 0.07f);
        miniCamera.targetTexture = rt;
        miniCamera.cullingMask = mainCamera.cullingMask;
        miniCamera.enabled = true;

        var canvasObj = new GameObject("Minimap Canvas", typeof(RectTransform));
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        var frame = new GameObject("Minimap Frame", typeof(RectTransform));
        frame.transform.SetParent(canvasObj.transform, false);
        var rect = frame.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = new Vector2(245f, 245f);
        var border = frame.AddComponent<Image>();
        border.color = new Color(0.65f, 0.85f, 0.95f);

        var map = new GameObject("MiniMap Image", typeof(RectTransform));
        map.transform.SetParent(frame.transform, false);
        var mapRect = map.GetComponent<RectTransform>();
        mapRect.anchorMin = Vector2.zero;
        mapRect.anchorMax = Vector2.one;
        mapRect.offsetMin = new Vector2(5f, 5f);
        mapRect.offsetMax = new Vector2(-5f, -5f);
        var raw = map.AddComponent<RawImage>();
        raw.texture = rt;
        raw.raycastTarget = false;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MiniMapDemo.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeGameObject = player;
        EditorUtility.DisplayDialog("Homework 2",
            "Mini-map scene created! Open Assets/Scenes/MiniMapDemo and press Play.\n" +
            "Requires package: Input System (com.unity.inputsystem).", "OK");
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path)?.Replace('\\','/');
        string name = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, name);
    }

    private static Sprite MakeSprite(string name, Color color)
    {
        string path = "Assets/Generated/Sprites/" + name + ".png";
        if (!File.Exists(path))
        {
            var texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            Color[] colors = new Color[32 * 32];
            for (int i = 0; i < colors.Length; i++) colors[i] = color;
            texture.SetPixels(colors);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
        }
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        if (importer.textureType != TextureImporterType.Sprite || importer.spritePixelsPerUnit != 32)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = 32;
            importer.filterMode = FilterMode.Point;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static GameObject Visual(string name, Sprite sprite, Vector3 position,
                                     Vector2 scale, Transform parent, int order)
    {
        GameObject go = new GameObject(name);
        go.transform.position = position;
        go.transform.localScale = new Vector3(scale.x, scale.y, 1f);
        if (parent != null) go.transform.SetParent(parent, true);
        var renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.sortingOrder = order;
        return go;
    }

    private static void Solid(string name, Sprite sprite, Vector2 position,
                              Vector2 scale, Transform parent)
    {
        GameObject go = Visual(name, sprite, new Vector3(position.x, position.y, 0f),
                               scale, parent, 1);
        go.AddComponent<BoxCollider2D>();
    }
}
#endif
