using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Required for EventTrigger
using System.Collections;
using System.Collections.Generic;

public class BetManager : MonoBehaviour
{
    [SerializeField] private GameObject imagePrefab;
    [SerializeField] private Button[] buttons;
    [SerializeField] private Sprite[] levelSprites;
    [SerializeField] private int[] thresholds;
    [SerializeField] private Button[] betValueButtons;
    [SerializeField] private CanvasGroup bannerCanvasGroup;
    [SerializeField] private TextMeshProUGUI totalBetValueText;
    private int selectedBetValue = 0;

    private List<GameObject> imagesToActivate = new List<GameObject>();

    void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }

        foreach (Button betButton in betValueButtons)
        {
            betButton.onClick.AddListener(() => OnBetValueButtonClick(betButton));
        }

        enableButtons();
    }

    private void OnBetValueButtonClick(Button betButton)
    {
        TextMeshProUGUI tmpText = betButton.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null && int.TryParse(tmpText.text, out int value))
        {
            selectedBetValue = value;
            Debug.Log($"Bet value button pressed: {betButton.name}, Value: {selectedBetValue}");
        }
        else
        {
            Debug.LogError("Invalid bet value format or TextMeshPro component missing.");
        }
    }

    private void OnButtonClick(Button clickedButton)
    {
        Debug.Log($"Button pressed: {clickedButton.name}");

        if (selectedBetValue == 0)
        {
            ShowBanner();
            return;
        }

        GameObject newImage = Instantiate(imagePrefab, clickedButton.transform.parent);
        RectTransform buttonRectTransform = clickedButton.GetComponent<RectTransform>();
        RectTransform newImageRectTransform = newImage.GetComponent<RectTransform>();

        newImageRectTransform.anchoredPosition = buttonRectTransform.anchoredPosition;
        newImage.transform.localRotation = Quaternion.identity;
        newImage.transform.localScale = Vector3.one * 0.5f;

        imagesToActivate.Add(newImage);

        TextMeshProUGUI tmpComponent = newImage.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpComponent == null)
        {
            GameObject tmpObj = new GameObject("TMPText");
            tmpObj.transform.SetParent(newImage.transform);

            tmpComponent = tmpObj.AddComponent<TextMeshProUGUI>();
            tmpComponent.text = "0";
            tmpComponent.alignment = TextAlignmentOptions.Center;
            tmpComponent.color = Color.black;
            tmpComponent.fontSize = 20;

            RectTransform rectTransform = tmpComponent.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(40, 40);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        if (int.TryParse(tmpComponent.text, out int currentValue))
        {
            int newValue = currentValue + selectedBetValue;
            tmpComponent.text = newValue.ToString();

            Sprite appropriateSprite = GetSpriteForValue(newValue);
            if (appropriateSprite != null)
            {
                Image imgComponent = newImage.GetComponent<Image>();
                if (imgComponent != null)
                {
                    imgComponent.sprite = appropriateSprite;
                }
            }
        }
        else
        {
            Debug.LogWarning("TextMeshPro component does not contain a valid integer.");
        }

        AddClickEventToChip(newImage);

        UpdateTotalBetValue();
    }

    private void AddClickEventToChip(GameObject chip)
    {
        EventTrigger trigger = chip.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((data) => { OnChipClick((PointerEventData)data, chip); });
        trigger.triggers.Add(entry);
    }

    private void OnChipClick(PointerEventData data, GameObject chip)
    {
        TextMeshProUGUI tmpComponent = chip.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpComponent != null && int.TryParse(tmpComponent.text, out int currentValue))
        {
            int newValue = currentValue + selectedBetValue;
            tmpComponent.text = newValue.ToString();

            Sprite appropriateSprite = GetSpriteForValue(newValue);
            if (appropriateSprite != null)
            {
                Image imgComponent = chip.GetComponent<Image>();
                if (imgComponent != null)
                {
                    imgComponent.sprite = appropriateSprite;
                }
            }

            UpdateTotalBetValue();
        }
    }

    private Sprite GetSpriteForValue(int value)
    {
        for (int i = thresholds.Length - 1; i >= 0; i--)
        {
            if (value >= thresholds[i])
            {
                return levelSprites[i];
            }
        }
        return null;
    }

    public void invokeDisableButtons(float time)
    {
        Invoke("DisableButtons", time);
    }

    public void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void enableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }

    private void ShowBanner()
    {
        bannerCanvasGroup.alpha = 1;
        bannerCanvasGroup.gameObject.SetActive(true);
        Invoke("FadeOutBanner", 1f);
    }

    private void FadeOutBanner()
    {
        StartCoroutine(FadeCanvasGroup(bannerCanvasGroup, 1f, 0f, 3f));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cg.alpha = end;
        if (end == 0)
        {
            cg.gameObject.SetActive(false);
        }
    }

    private void UpdateTotalBetValue()
    {
        int totalValue = 0;

        foreach (GameObject image in imagesToActivate)
        {
            TextMeshProUGUI tmpComponent = image.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpComponent != null && int.TryParse(tmpComponent.text, out int value))
            {
                totalValue += value;
            }
        }

        totalBetValueText.text = $"Current Play: {totalValue}";
    }
}
