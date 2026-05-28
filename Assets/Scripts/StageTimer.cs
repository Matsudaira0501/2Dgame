using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMesh))]
public class StageTimer : MonoBehaviour
{
    [System.Serializable]
    private class TimeLimitSetting
    {
        public string label;
        public bool coinIncrease;
        public bool coinPositionHard;
        public bool smallPlatform;
        public bool spikeIncrease;
        public float timeLimit = 6f;
    }

    [SerializeField]
    private float defaultTimeLimit = 6f;

    [SerializeField]
    private Color normalColor = Color.black;

    [SerializeField]
    private Color warningColor = Color.red;

    [SerializeField]
    private TimeLimitSetting[] timeLimitSettings;

    private TextMesh timerText;
    private MeshRenderer timerRenderer;
    private float remainingTime;
    private bool isRunning;
    private bool isRestarting;

    void Awake()
    {
        timerText = GetComponent<TextMesh>();
        timerRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        isRunning = DifficultySettings.TimeLimit;
        SetTimerVisible(isRunning);

        if (!isRunning)
        {
            return;
        }

        remainingTime = GetTimeLimit();
        UpdateTimerText();
    }

    private float GetTimeLimit()
    {
        if (timeLimitSettings == null)
        {
            return defaultTimeLimit;
        }

        foreach (TimeLimitSetting setting in timeLimitSettings)
        {
            if (MatchesCurrentDifficulty(setting))
            {
                return setting.timeLimit;
            }
        }

        return defaultTimeLimit;
    }

    private bool MatchesCurrentDifficulty(TimeLimitSetting setting)
    {
        return setting.coinIncrease == DifficultySettings.CoinIncrease &&
            setting.coinPositionHard == DifficultySettings.CoinPositionHard &&
            setting.smallPlatform == DifficultySettings.SmallPlatform &&
            setting.spikeIncrease == DifficultySettings.SpikeIncrease;
    }

    void Update()
    {
        if (!isRunning || isRestarting)
        {
            return;
        }

        if (GameClearManager.Instance != null && GameClearManager.Instance.IsCleared)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            RestartScene();
            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        timerText.text = $"Time: {Mathf.CeilToInt(remainingTime)}";
        timerText.color = remainingTime <= 1f ? warningColor : normalColor;
    }

    private void SetTimerVisible(bool isVisible)
    {
        if (timerRenderer != null)
        {
            timerRenderer.enabled = isVisible;
        }
    }

    private void RestartScene()
    {
        isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
