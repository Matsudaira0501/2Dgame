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
    private string clearMessage = "Game Clear!";

    [SerializeField]
    private float restartDelay = 3f;

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
        StartCoroutine(RestartAfterClear());
    }

    private IEnumerator RestartAfterClear()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(restartDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
            ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 48;
        text.color = Color.yellow;
        text.alignment = TextAnchor.MiddleCenter;

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(420f, 96f);

        return text;
    }
}
