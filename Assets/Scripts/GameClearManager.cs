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
    private string finalClearMessage = "All Clear!";

    [SerializeField]
    private int clearFontSize = 100;

    [SerializeField]
    private int finalClearFontSize = 150;

    [SerializeField]
    private AudioClip clearSound;

    [SerializeField]
    private AudioClip[] finalClearSounds;

    [SerializeField]
    private float restartDelay = 3f;

    [SerializeField]
    private float finalClearDelay = 5f;

    [SerializeField]
    private string nextSceneName = "DifficultyScene";

    [SerializeField]
    private string finalClearSceneName = "TitleScene";

    [SerializeField]
    private UnityroomRankingSubmitter rankingSubmitter;

    private int remainingCoins;
    private bool isCleared;
    private bool isFinalClear;

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
        ClearTimeTracker.StartRunIfNeeded();
        remainingCoins = FindObjectsByType<CoinPickup>(FindObjectsInactive.Exclude).Length;

        if (clearText == null)
        {
            clearText = CreateClearText();
        }

        if (clearText != null)
        {
            clearText.font = GetDisplayFont();
            clearText.fontSize = clearFontSize;
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
        isFinalClear = DifficultySettings.AllSwitchesOn;

        if (clearText != null)
        {
            clearText.text = isFinalClear ? finalClearMessage : clearMessage;
            clearText.fontSize = isFinalClear ? finalClearFontSize : clearFontSize;
            clearText.gameObject.SetActive(true);
        }

        Debug.Log(isFinalClear ? finalClearMessage : clearMessage);
        if (clearSound != null)
        {
            AudioSource.PlayClipAtPoint(clearSound, Vector3.zero, GameSettings.SfxVolume);
        }

        if (isFinalClear)
        {
            PlayFinalClearSounds();
        }

        SubmitClearTime();

        StartCoroutine(LoadSceneAfterClear());
    }

    private void SubmitClearTime()
    {
        if (!isFinalClear)
        {
            return;
        }

        float clearTime = ClearTimeTracker.StopRun();

        if (rankingSubmitter == null)
        {
            rankingSubmitter = GetComponent<UnityroomRankingSubmitter>();
        }

        if (rankingSubmitter != null)
        {
            rankingSubmitter.SubmitClearTime(clearTime, isFinalClear);
        }
    }

    [ContextMenu("Preview Final Clear")]
    private void PreviewFinalClear()
    {
        StopAllCoroutines();
        isCleared = true;
        isFinalClear = true;

        if (clearText == null)
        {
            clearText = CreateClearText();
        }

        if (clearText != null)
        {
            clearText.font = GetDisplayFont();
            clearText.fontSize = finalClearFontSize;
            clearText.color = Color.black;
            clearText.text = finalClearMessage;
            clearText.gameObject.SetActive(true);
        }

        if (clearSound != null)
        {
            AudioSource.PlayClipAtPoint(clearSound, Vector3.zero, GameSettings.SfxVolume);
        }

        PlayFinalClearSounds();
    }

    private void PlayFinalClearSounds()
    {
        if (finalClearSounds == null)
        {
            return;
        }

        foreach (AudioClip sound in finalClearSounds)
        {
            if (sound != null)
            {
                AudioSource.PlayClipAtPoint(sound, Vector3.zero, GameSettings.SfxVolume);
            }
        }
    }

    private IEnumerator LoadSceneAfterClear()
    {
        Time.timeScale = 0f;
        
        float delay = isFinalClear ? finalClearDelay : restartDelay;
        yield return new WaitForSecondsRealtime(delay);
        
        Time.timeScale = 1f;

        string sceneName = isFinalClear ? finalClearSceneName : nextSceneName;

        if (string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            yield break;
        }

        SceneManager.LoadScene(sceneName);
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
