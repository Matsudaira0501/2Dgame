public static class DifficultySettings
{
    public static bool CoinIncrease { get; private set; }
    public static bool CoinPositionHard { get; private set; }
    public static bool SmallPlatform { get; private set; }
    public static bool SpikeIncrease { get; private set; }
    public static bool ForceJump { get; private set; }
    public static bool TimeLimit { get; private set; }

    public static bool AnySwitchOn =>
        CoinIncrease || CoinPositionHard || SmallPlatform || SpikeIncrease || ForceJump || TimeLimit;

    public static void SetSwitch(DifficultySwitchType switchType, bool isOn)
    {
        switch (switchType)
        {
            case DifficultySwitchType.CoinIncrease:
                CoinIncrease = isOn;
                break;
            case DifficultySwitchType.CoinPositionHard:
                CoinPositionHard = isOn;
                break;
            case DifficultySwitchType.SmallPlatform:
                SmallPlatform = isOn;
                break;
            case DifficultySwitchType.SpikeIncrease:
                SpikeIncrease = isOn;
                break;
            case DifficultySwitchType.ForceJump:
                ForceJump = isOn;
                break;
            case DifficultySwitchType.TimeLimit:
                TimeLimit = isOn;
                break;
        }
    }

    public static void ResetAll()
    {
        CoinIncrease = false;
        CoinPositionHard = false;
        SmallPlatform = false;
        SpikeIncrease = false;
        ForceJump = false;
        TimeLimit = false;
    }
}
