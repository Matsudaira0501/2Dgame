using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private Text tutorialText;

    [SerializeField]
    private Font displayFont;

    [SerializeField]
    [TextArea(3, 6)]
    private string tutorialMessage =
        "移動: ← → / A D\n" +
        "ジャンプ: ↑ / Space\n" +
        "目的: コインを全部取るとクリア！！";

    void Start()
    {
        if (DifficultySettings.AnySwitchOn)
        {
            if (tutorialText != null)
            {
                tutorialText.gameObject.SetActive(false);
            }

            return;
        }

        if (tutorialText == null)
        {
            tutorialText = CreateTutorialText();
        }

        if (tutorialText != null)
        {
            tutorialText.font = GetDisplayFont();
            tutorialText.fontSize = 55;
            tutorialText.color = Color.black;
            tutorialText.text = tutorialMessage;
        }
    }

    private Text CreateTutorialText()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("TutorialText");
        textObject.transform.SetParent(canvas.transform, false);

        Text text = textObject.AddComponent<Text>();
        text.font = GetDisplayFont();
        text.fontSize = 55;
        text.color = Color.black;
        text.alignment = TextAnchor.UpperLeft;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        Shadow shadow = textObject.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.7f);
        shadow.effectDistance = new Vector2(2f, -2f);

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = new Vector2(24f, -24f);
        rectTransform.sizeDelta = new Vector2(10000f, 10000f);

        return text;
    }

    private Font GetDisplayFont()
    {
        return displayFont != null
            ? displayFont
            : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
                ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
    }
}
