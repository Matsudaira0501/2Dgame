using UnityEngine;
using UnityEngine.SceneManagement;

public class SpikeHazard : MonoBehaviour
{
    private bool isRestarting;

    void OnTriggerEnter2D(Collider2D other)
    {
        RestartIfPlayer(other.GetComponentInParent<PlayerController>());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        RestartIfPlayer(collision.collider.GetComponentInParent<PlayerController>());
    }

    private void RestartIfPlayer(PlayerController player)
    {
        if (isRestarting || player == null)
        {
            return;
        }

        isRestarting = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
