using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField]
    private int score;

    public int Score => score;

    public event Action<int> ScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Start()
    {
        ScoreChanged?.Invoke(score);
    }

    public void AddScore(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        score += amount;
        ScoreChanged?.Invoke(score);
        Debug.Log($"Score: {score}");
    }

    public void ResetScore()
    {
        score = 0;
        ScoreChanged?.Invoke(score);
    }
}
