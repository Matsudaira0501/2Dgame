using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endif

public static class CoinFeatureSceneSetup
{
#if UNITY_EDITOR
    [MenuItem("Tools/Coin Collector/Setup Sample Scene")]
    public static void SetupSampleScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");

        GameObject gameManager = EnsureGameManager();
        EnsureScoreManager(gameManager);
        EnsureGameClearManager(gameManager);
        EnsureCoins();
        EnsureScoreUi();

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
    }

    private static GameObject EnsureGameManager()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            return gameManager;
        }

        gameManager = new GameObject("GameManager");
        Undo.RegisterCreatedObjectUndo(gameManager, "Create GameManager");
        return gameManager;
    }

    private static void EnsureScoreManager(GameObject gameManager)
    {
        if (Object.FindAnyObjectByType<ScoreManager>() == null)
        {
            gameManager.AddComponent<ScoreManager>();
        }
    }

    private static void EnsureGameClearManager(GameObject gameManager)
    {
        if (Object.FindAnyObjectByType<GameClearManager>() == null)
        {
            gameManager.AddComponent<GameClearManager>();
        }
    }

    private static void EnsureCoins()
    {
        Sprite sprite = LoadCoinSprite();

        Vector3[] positions =
        {
            new Vector3(-3.38f, -2.05f, 0f),
            new Vector3(-0.38f, -2.05f, 0f),
            new Vector3(2.62f, -2.05f, 0f),
        };

        for (int i = 0; i < positions.Length; i++)
        {
            string coinName = $"Coin_{i + 1}";
            if (GameObject.Find(coinName) != null)
            {
                continue;
            }

            GameObject coin = new GameObject(coinName);
            coin.transform.position = positions[i];
            coin.transform.localScale = Vector3.one * 0.08f;

            SpriteRenderer renderer = coin.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingOrder = 1;

            CircleCollider2D collider = coin.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 4.75f;
            collider.offset = new Vector2(4.745f, 4.75f);

            coin.AddComponent<CoinPickup>();
            Undo.RegisterCreatedObjectUndo(coin, $"Create {coinName}");
        }
    }

    private static Sprite LoadCoinSprite()
    {
        const string coinPath = "Assets/images/coin_transparent.png";
        Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(coinPath);
        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite && sprite.name == "coin_transparent_0")
            {
                return sprite;
            }
        }

        foreach (Object asset in assets)
        {
            if (asset is Sprite sprite)
            {
                return sprite;
            }
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(
            AssetDatabase.GUIDToAssetPath("311925a002f4447b3a28927169b83ea6"));
    }

    private static void EnsureScoreUi()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create Canvas");
        }

        if (GameObject.Find("ScoreText") == null)
        {
            GameObject textObject = new GameObject("ScoreText");
            textObject.transform.SetParent(canvas.transform, false);

            Text text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 32;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            text.text = "Score: 0";
            textObject.AddComponent<ScoreText>();

            RectTransform rectTransform = textObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(0f, 1f);
            rectTransform.pivot = new Vector2(0f, 1f);
            rectTransform.anchoredPosition = new Vector2(24f, -24f);
            rectTransform.sizeDelta = new Vector2(260f, 64f);

            Undo.RegisterCreatedObjectUndo(textObject, "Create ScoreText");
        }

        GameObject clearTextObject = GameObject.Find("ClearText");
        if (clearTextObject == null)
        {
            clearTextObject = new GameObject("ClearText");
            clearTextObject.transform.SetParent(canvas.transform, false);

            Text text = clearTextObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 48;
            text.color = Color.yellow;
            text.alignment = TextAnchor.MiddleCenter;
            text.text = "Game Clear!";

            RectTransform rectTransform = clearTextObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(420f, 96f);
            clearTextObject.SetActive(false);

            Undo.RegisterCreatedObjectUndo(clearTextObject, "Create ClearText");
        }

        GameClearManager gameClearManager = Object.FindAnyObjectByType<GameClearManager>();
        if (gameClearManager != null)
        {
            SerializedObject serializedObject = new SerializedObject(gameClearManager);
            serializedObject.FindProperty("clearText").objectReferenceValue =
                clearTextObject.GetComponent<Text>();
            serializedObject.ApplyModifiedProperties();
        }

        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
        }
    }
#endif
}
