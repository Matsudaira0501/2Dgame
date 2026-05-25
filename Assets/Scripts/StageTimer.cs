using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMesh))]
public class StageTimer : MonoBehaviour
{
    [SerializeField]
    private float timeLimit = 7f;

    [SerializeField]
    private Color normalColor = Color.black;

    [SerializeField]
    private Color warningColor = Color.red;

    private TextMesh timerText;
    private float remainingTime;
    private bool isRunning;
    private bool isRestarting;

    void Awake()
    {
        timerText = GetComponent<TextMesh>();
    }

    void Start()
    {
        isRunning = DifficultySettings.TimeLimit;
        gameObject.SetActive(isRunning);

        if (!isRunning)
        {
            return;
        }

        remainingTime = timeLimit;
        UpdateTimerText();
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

    private void RestartScene()
    {
        isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
