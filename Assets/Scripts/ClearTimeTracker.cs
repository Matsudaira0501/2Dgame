using UnityEngine;

public static class ClearTimeTracker
{
    private static float startTime;
    private static bool isRunning;

    public static bool IsRunning => isRunning;

    public static float ElapsedTime => isRunning ? Time.realtimeSinceStartup - startTime : 0f;

    public static void StartRun()
    {
        startTime = Time.realtimeSinceStartup;
        isRunning = true;
    }

    public static void StartRunIfNeeded()
    {
        if (!isRunning)
        {
            StartRun();
        }
    }

    public static float StopRun()
    {
        float elapsedTime = ElapsedTime;
        isRunning = false;
        return elapsedTime;
    }
}
