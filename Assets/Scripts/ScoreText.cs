using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{
    [SerializeField]
    private ScoreManager scoreManager;

    [SerializeField]
    private string prefix = "Score: ";

    [SerializeField]
    private Font displayFont;

    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
        if (displayFont != null)
        {
            text.font = displayFont;
        }

        text.fontSize = 48;
        text.color = Color.black;
    }

    void OnEnable()
    {
        if (scoreManager == null)
        {
            scoreManager = ScoreManager.Instance != null
                ? ScoreManager.Instance
                : FindAnyObjectByType<ScoreManager>();
        }

        if (scoreManager != null)
        {
            scoreManager.ScoreChanged += UpdateText;
            UpdateText(scoreManager.Score);
        }
    }

    void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreChanged -= UpdateText;
        }
    }

    private void UpdateText(int score)
    {
        text.text = $"{prefix}{score}";
    }
}
