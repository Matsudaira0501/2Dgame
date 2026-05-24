using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    public static GameClearManager Instance { get; private set; }

    [SerializeField]
    private Text clearText;

    [SerializeField]
    private Font displayFont;

    [SerializeField]
    private string clearMessage = "Game Clear!";

    [SerializeField]
    private AudioClip clearSound;

    [SerializeField]
    private float restartDelay = 3f;

    [SerializeField]
    private string nextSceneName = "DifficultyScene";

    private int remainingCoins;
    private bool isCleared;

    public bool IsCleared => isCleared;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        remainingCoins = FindObjectsByType<CoinPickup>(FindObjectsInactive.Exclude).Length;

        if (clearText == null)
        {
            clearText = CreateClearText();
        }

        if (clearText != null)
        {
            clearText.font = GetDisplayFont();
            clearText.fontSize = 100;
            clearText.color = Color.black;
            clearText.text = clearMessage;
            clearText.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void CoinCollected()
    {
        if (isCleared || remainingCoins <= 0)
        {
            return;
        }

        remainingCoins--;

        if (remainingCoins <= 0)
        {
            ClearGame();
        }
    }

    private void ClearGame()
    {
        isCleared = true;

        if (clearText != null)
        {
            clearText.gameObject.SetActive(true);
        }

        Debug.Log(clearMessage);
        if (clearSound != null)
        {
            AudioSource.PlayClipAtPoint(clearSound, Vector3.zero);
        }

        StartCoroutine(LoadSceneAfterClear());
    }

    private IEnumerator LoadSceneAfterClear()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(restartDelay);
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            yield break;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private Text CreateClearText()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("ClearText");
        textObject.transform.SetParent(canvas.transform, false);

        Text text = textObject.AddComponent<Text>();
        text.font = GetDisplayFont();
        text.fontSize = 100;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(100000f, 100000f);

        return text;
    }

    private Font GetDisplayFont()
    {
        return displayFont != null
            ? displayFont
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}
