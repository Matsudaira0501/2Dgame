using UnityEngine;

public class DifficultySceneManager : MonoBehaviour
{
    [SerializeField]
    private bool resetTimeScaleOnStart = true;

    void Awake()
    {
        DifficultySettings.ResetAll();
    }

    void Start()
    {
        if (resetTimeScaleOnStart)
        {
            Time.timeScale = 1f;
        }
    }
}
