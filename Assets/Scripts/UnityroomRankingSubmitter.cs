using System.Reflection;
using unityroom.Api;
using UnityEngine;

public class UnityroomRankingSubmitter : MonoBehaviour
{
    [SerializeField]
    private string hmacKey = "";

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

        ConfigureHmacKey(client);
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
        ConfigureHmacKey(clientComponent);

        clientObject.SetActive(true);
        return clientComponent;
    }

    private void ConfigureHmacKey(UnityroomApiClient client)
    {
        if (string.IsNullOrEmpty(hmacKey))
        {
            Debug.LogWarning("Unityroom Hmac Key is empty. Set it on UnityroomRankingSubmitter or UnityroomApiClient.");
            return;
        }

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        PropertyInfo property = typeof(UnityroomApiClient).GetProperty("HmacKey", flags);
        if (property != null && property.CanWrite)
        {
            property.SetValue(client, hmacKey);
            return;
        }

        FieldInfo field = typeof(UnityroomApiClient).GetField("HmacKey", flags);
        if (field != null)
        {
            field.SetValue(client, hmacKey);
            return;
        }

        Debug.LogWarning("Unityroom Hmac Key field was not found. Set it on the UnityroomApiClient inspector.");
    }
}
