using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{
    [SerializeField]
    private ScoreManager scoreManager;

    [SerializeField]
    private string prefix = "Score: ";

    private Text text;

    void Awake()
    {
        text = GetComponent<Text>();
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
