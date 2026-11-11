using System.Reflection;
using unityroom.Api;
using UnityEngine;

public class UnityroomRankingSubmitter : MonoBehaviour
{
    // Git管理外のResourcesテキストから読み込む（シーンへのキー直書きを防ぐ）。
    private const string HmacResourceName = "unityroom-ranking-hmac";

    [SerializeField]
    private int clearTimeBoardNo = 1;

    [SerializeField]
    private bool submitOnlyFinalClear = true;

    [SerializeField]
    private bool submitTimeAsWholeSeconds = true;

    public void SubmitClearTime(float clearTime, bool isFinalClear)
    {
        if (submitOnlyFinalClear && !isFinalClear)
        {
            return;
        }

        UnityroomApiClient client = GetOrCreateUnityroomApiClientInstance();
        if (client == null)
        {
            Debug.LogWarning("UnityroomApiClient was not found. Check the unityroom client library.");
            return;
        }

        if (!TryConfigureHmacKey(client))
        {
            return;
        }

        float score = ConvertClearTimeToScore(clearTime);
        client.SendScore(clearTimeBoardNo, score, ScoreboardWriteMode.HighScoreAsc);
        Debug.Log($"Sent clear time to unityroom: {clearTime:F2}s Score={score}");
    }

    private float ConvertClearTimeToScore(float clearTime)
    {
        if (!submitTimeAsWholeSeconds)
        {
            return clearTime;
        }

        return Mathf.Ceil(Mathf.Max(0f, clearTime));
    }

    private UnityroomApiClient GetOrCreateUnityroomApiClientInstance()
    {
        UnityroomApiClient existingClient = FindFirstObjectByType<UnityroomApiClient>();
        if (existingClient != null)
        {
            return existingClient;
        }

        GameObject clientObject = new GameObject("UnityroomApiClient");
        clientObject.SetActive(false);

        UnityroomApiClient clientComponent = clientObject.AddComponent<UnityroomApiClient>();
        clientObject.SetActive(true);
        return clientComponent;
    }

    private bool TryConfigureHmacKey(UnityroomApiClient client)
    {
        TextAsset keyText = Resources.Load<TextAsset>(HmacResourceName);
        string hmacKey = keyText != null ? keyText.text.Trim() : "";
        if (string.IsNullOrWhiteSpace(hmacKey))
        {
            Debug.LogWarning("Ranking key is missing. Create Assets/Resources/unityroom-ranking-hmac.txt locally (ignored by Git).");
            return false;
        }

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        PropertyInfo property = typeof(UnityroomApiClient).GetProperty("HmacKey", flags);
        if (property != null && property.CanWrite)
        {
            property.SetValue(client, hmacKey);
            return true;
        }

        FieldInfo field = typeof(UnityroomApiClient).GetField("HmacKey", flags);
        if (field != null)
        {
            field.SetValue(client, hmacKey);
            return true;
        }

        Debug.LogWarning("UnityroomApiClient HmacKey property or field was not found.");
        return false;
    }
}
