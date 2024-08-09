using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
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
    [SerializeField] private Image[] neighbourBetImages;
    [SerializeField] private Button clearBet;
    [SerializeField] private Button doubleBet;
    [SerializeField] private Button removeBetButton; // Button to activate delete mode
    private int selectedBetValue = 0;
    private bool isDeleteModeActive = false;

    private List<GameObject> imagesToActivate = new List<GameObject>();
    private Dictionary<string, int> betsPlaced = new Dictionary<string, int>();
    private Dictionary<string, int> positionOfBetPlaced = new Dictionary<string, int>(); // Dictionary to store image names and bet amounts

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

        clearBet.onClick.AddListener(() => DestroyAllImages());
        doubleBet.onClick.AddListener(() => DoubleAllBets());
        removeBetButton.onClick.AddListener(() => ToggleDeleteMode());

        EnableButtons();
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
        newImage.name = clickedButton.name; // Set the name of the new image to the button's name
        RectTransform buttonRectTransform = clickedButton.GetComponent<RectTransform>();
        RectTransform newImageRectTransform = newImage.GetComponent<RectTransform>();

        newImageRectTransform.anchoredPosition = buttonRectTransform.anchoredPosition;
        newImage.transform.localRotation = Quaternion.identity;

        foreach (var neighbourBetImage in neighbourBetImages)
        {
            if (string.Equals(neighbourBetImage.name, clickedButton.name, System.StringComparison.OrdinalIgnoreCase))
            {
                newImageRectTransform.sizeDelta = neighbourBetImage.GetComponent<RectTransform>().sizeDelta;
                newImageRectTransform.anchoredPosition = neighbourBetImage.GetComponent<RectTransform>().anchoredPosition;
                newImageRectTransform.localScale = neighbourBetImage.GetComponent<RectTransform>().localScale;
                newImageRectTransform.pivot = neighbourBetImage.GetComponent<RectTransform>().pivot;
                newImageRectTransform.anchorMin = neighbourBetImage.GetComponent<RectTransform>().anchorMin;
                newImageRectTransform.anchorMax = neighbourBetImage.GetComponent<RectTransform>().anchorMax;
                newImageRectTransform.rotation = neighbourBetImage.GetComponent<RectTransform>().rotation;
                break;
            }
        }

        imagesToActivate.Add(newImage);

        TextMeshProUGUI tmpComponent = newImage.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpComponent == null)
        {
            GameObject tmpObj = new GameObject("TMPText");
            tmpObj.transform.SetParent(newImage.transform);

            tmpComponent = tmpObj.AddComponent<TextMeshProUGUI>();
            tmpComponent.text = "0";
            tmpComponent.alignment = TextAlignmentOptions.Center;
            tmpComponent.color = UnityEngine.Color.black;
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

            // Update the positionOfBetPlaced dictionary with the new bet amount
            positionOfBetPlaced[newImage.name] = newValue;
        }
        else
        {
            Debug.LogWarning("TextMeshPro component does not contain a valid integer.");
        }

        AddClickEventToChip(newImage);

        UpdateBets(clickedButton.name, selectedBetValue);
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
        if (isDeleteModeActive)
        {
            Destroy(chip);
            imagesToActivate.Remove(chip);
            positionOfBetPlaced.Remove(chip.name); // Remove the chip's name from the dictionary

            string buttonName = chip.transform.parent.name;
            if (betsPlaced.ContainsKey(buttonName))
            {
                betsPlaced[buttonName] -= selectedBetValue;
                if (betsPlaced[buttonName] <= 0)
                {
                    betsPlaced.Remove(buttonName);
                }
                UpdateTotalBetValue();
            }
            return;
        }

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

            // Update the positionOfBetPlaced dictionary with the new bet amount
            positionOfBetPlaced[chip.name] = newValue;

            UpdateBets(chip.transform.parent.name, selectedBetValue);
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

    public void InvokeDisableButtons(float time)
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

    public void EnableButtons()
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
            if (image != null)
            {
                TextMeshProUGUI tmpComponent = image.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpComponent != null && int.TryParse(tmpComponent.text, out int value))
                {
                    totalValue += value;
                }
            }
        }

        totalBetValueText.text = totalValue.ToString();
    }

    private void UpdateBets(string buttonName, int betValue)
    {
        if (betsPlaced.ContainsKey(buttonName))
        {
            betsPlaced[buttonName] += betValue;
        }
        else
        {
            betsPlaced.Add(buttonName, betValue);
        }
    }

    public void DestroyAllImages()
    {
        Debug.Log("Destroying all images");

        foreach (var bet in betsPlaced)
        {
            Debug.Log($"Button: {bet.Key}, Bet: {bet.Value}");
        }

        foreach (GameObject image in imagesToActivate)
        {
            Destroy(image);
        }
        imagesToActivate.Clear();
        positionOfBetPlaced.Clear(); // Clear the dictionary of image names
        betsPlaced.Clear();
        UpdateTotalBetValue();
    }

    private void ToggleDeleteMode()
    {
        isDeleteModeActive = !isDeleteModeActive;
        UnityEngine.Color buttonColor = isDeleteModeActive ? new Color32(255, 115, 110, 255) : UnityEngine.Color.white;
        removeBetButton.GetComponent<Image>().color = buttonColor;
    }

    private void DoubleAllBets()
    {
        foreach (GameObject image in imagesToActivate)
        {
            if (image != null)
            {
                TextMeshProUGUI tmpComponent = image.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpComponent != null && int.TryParse(tmpComponent.text, out int currentValue))
                {
                    int newValue = currentValue * 2;
                    tmpComponent.text = newValue.ToString();

                    Sprite appropriateSprite = GetSpriteForValue(newValue);
                    if (appropriateSprite != null)
                    {
                        Image imgComponent = image.GetComponent<Image>();
                        if (imgComponent != null)
                        {
                            imgComponent.sprite = appropriateSprite;
                        }
                    }

                    // Update the positionOfBetPlaced dictionary with the new bet amount
                    positionOfBetPlaced[image.name] = newValue;
                }
            }
        }

        foreach (var key in new List<string>(betsPlaced.Keys))
        {
            betsPlaced[key] *= 2;
        }

        UpdateTotalBetValue();
    }

    public void DeactivateAllImages()
    {
        foreach (GameObject image in imagesToActivate)
        {
            if (image != null)
            {
                image.SetActive(false);
            }
        }
    }

    public Dictionary<string, int> displayBetPositions()
    {
        Debug.Log("<color=red>Bet was placed on</color>");
        foreach (var betPlaced in positionOfBetPlaced)
        {
            Debug.Log("<color=blue> Number : " + betPlaced.Key + " , Bet Amount: " + betPlaced.Value + " </color>");
        }
        return positionOfBetPlaced;
    }
}
