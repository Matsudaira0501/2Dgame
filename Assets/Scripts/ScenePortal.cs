using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class ScenePortal : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName = "SampleScene";

    private bool isLoading;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isLoading || other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        isLoading = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(targetSceneName);
    }

    public void SetTargetScene(string sceneName)
    {
        targetSceneName = sceneName;
    }
}
