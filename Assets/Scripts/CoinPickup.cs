using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    [SerializeField]
    private int scoreValue = 1;

    [SerializeField]
    private AudioClip collectSound;

    [SerializeField]
    [Range(0f, 2f)]
    private float collectSoundVolume = 1.5f;

    [SerializeField]
    private bool destroyOnCollect = true;

    private bool collected;

    void Reset()
    {
        SetTriggerCollider();
    }

    void OnValidate()
    {
        SetTriggerCollider();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.TryGetComponent(out PlayerController player))
        {
            return;
        }

        Collect(player.transform.position);
    }

    public void Collect(Vector3 collectorPosition)
    {
        collected = true;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }
        else
        {
            Debug.LogWarning("ScoreManager is missing from the scene. Score was not added.", this);
        }

        if (GameClearManager.Instance != null)
        {
            GameClearManager.Instance.CoinCollected();
        }

        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, collectorPosition, collectSoundVolume);
        }

        if (destroyOnCollect)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void SetTriggerCollider()
    {
        if (TryGetComponent(out Collider2D coinCollider))
        {
            coinCollider.isTrigger = true;
        }
    }
}
