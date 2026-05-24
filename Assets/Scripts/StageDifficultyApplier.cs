using UnityEngine;

public class StageDifficultyApplier : MonoBehaviour
{
    [System.Serializable]
    private class CoinPositionSetting
    {
        public Transform coin;
        public Vector3 normalPosition;
        public Transform hardPosition;
        public Transform spikeOnlyHardPosition;
    }

    [SerializeField]
    private GameObject[] coinIncreaseCoins;

    [SerializeField]
    private CoinPositionSetting[] coinPositionSettings;

    [SerializeField]
    private GameObject[] normalPlatformObjects;

    [SerializeField]
    private GameObject[] hardPlatformObjects;

    [SerializeField]
    private GameObject[] spikeObjects;

    void Awake()
    {
        SetObjectsActive(coinIncreaseCoins, DifficultySettings.CoinIncrease);
        ApplyCoinPositions();
        ApplyPlatformDifficulty();
        ApplySpikeDifficulty();
    }

    private void SetObjectsActive(GameObject[] objects, bool isActive)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject targetObject in objects)
        {
            if (targetObject != null)
            {
                targetObject.SetActive(isActive);
            }
        }
    }

    private void ApplyCoinPositions()
    {
        if (coinPositionSettings == null)
        {
            return;
        }

        foreach (CoinPositionSetting setting in coinPositionSettings)
        {
            if (setting.coin == null)
            {
                continue;
            }

            Transform targetPosition = GetCoinTargetPosition(setting);

            if (targetPosition != null)
            {
                setting.coin.position = targetPosition.position;
            }
            else
            {
                setting.coin.localPosition = setting.normalPosition;
            }
        }
    }

    private Transform GetCoinTargetPosition(CoinPositionSetting setting)
    {
        if (!DifficultySettings.CoinPositionHard)
        {
            return null;
        }

        if (DifficultySettings.SpikeIncrease &&
            !DifficultySettings.SmallPlatform &&
            setting.spikeOnlyHardPosition != null)
        {
            return setting.spikeOnlyHardPosition;
        }

        return setting.hardPosition;
    }

    private void ApplyPlatformDifficulty()
    {
        SetObjectsActive(normalPlatformObjects, !DifficultySettings.SmallPlatform);
        SetObjectsActive(hardPlatformObjects, DifficultySettings.SmallPlatform);
    }

    private void ApplySpikeDifficulty()
    {
        SetObjectsActive(spikeObjects, DifficultySettings.SpikeIncrease);
    }

    void OnDrawGizmos()
    {
        if (coinPositionSettings == null)
        {
            return;
        }

        foreach (CoinPositionSetting setting in coinPositionSettings)
        {
            DrawPositionGizmo(setting.hardPosition, Color.yellow, 0.25f);
            DrawPositionGizmo(setting.spikeOnlyHardPosition, Color.cyan, 0.3f);
        }
    }

    private void DrawPositionGizmo(Transform target, Color color, float radius)
    {
        if (target == null)
        {
            return;
        }

        Gizmos.color = color;
        Gizmos.DrawWireSphere(target.position, radius);
    }
}
