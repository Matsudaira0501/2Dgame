using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField]
    private string startSceneName = "SampleScene";

    [SerializeField]
    private TextMesh volumeText;

    [SerializeField]
    [Range(0f, 1f)]
    private float initialSfxVolume = 1f;

    [SerializeField]
    private float volumeStep = 0.1f;

    [SerializeField]
    private AudioClip volumePreviewSound;

    void Start()
    {
        Time.timeScale = 1f;
        DifficultySettings.ResetAll();
        GameSettings.SetSfxVolume(initialSfxVolume);
        UpdateVolumeText();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
        {
            ChangeSfxVolume(-volumeStep);
        }

        if (keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
        {
            ChangeSfxVolume(volumeStep);
        }

        if (keyboard.leftShiftKey.wasPressedThisFrame || keyboard.rightShiftKey.wasPressedThisFrame)
        {
            ClearTimeTracker.StartRun();
            SceneManager.LoadScene(startSceneName);
        }
    }

    private void ChangeSfxVolume(float amount)
    {
        GameSettings.SetSfxVolume(GameSettings.SfxVolume + amount);
        UpdateVolumeText();
        PlayVolumePreviewSound();
    }

    private void UpdateVolumeText()
    {
        if (volumeText == null)
        {
            return;
        }

        int percent = Mathf.RoundToInt(GameSettings.SfxVolume * 100f);
        volumeText.text = $"SE Volume: {percent}%";
    }

    private void PlayVolumePreviewSound()
    {
        if (volumePreviewSound != null)
        {
            AudioSource.PlayClipAtPoint(volumePreviewSound, Vector3.zero, GameSettings.SfxVolume);
        }
    }
}
