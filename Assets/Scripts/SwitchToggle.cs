using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SwitchToggle : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer statusRenderer;

    [SerializeField]
    private Sprite offSprite;

    [SerializeField]
    private Sprite onSprite;

    [SerializeField]
    private bool isOn;

    [SerializeField]
    private DifficultySwitchType switchType;

    public bool IsOn => isOn;

    void Awake()
    {
        UpdateVisual();
        ApplySetting();
    }

    void OnValidate()
    {
        UpdateVisual();
    }

    void Reset()
    {
        Collider2D switchCollider = GetComponent<Collider2D>();
        switchCollider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
        {
            return;
        }

        isOn = !isOn;
        UpdateVisual();
        ApplySetting();
    }

    private void UpdateVisual()
    {
        if (statusRenderer == null)
        {
            return;
        }

        statusRenderer.sprite = isOn ? onSprite : offSprite;
    }

    private void ApplySetting()
    {
        DifficultySettings.SetSwitch(switchType, isOn);
    }
}
