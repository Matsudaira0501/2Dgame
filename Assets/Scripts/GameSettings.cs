using UnityEngine;

public static class GameSettings
{
    public static float SfxVolume { get; private set; } = 1f;

    public static void SetSfxVolume(float volume)
    {
        SfxVolume = Mathf.Clamp01(volume);
    }
}
