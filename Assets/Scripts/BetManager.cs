using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import the TextMeshPro namespace
using System.Collections; // Required for IEnumerator
using System.Collections.Generic; // Required for List

public class BetManager : MonoBehaviour
{
    [SerializeField] private GameObject imagePrefab; // Prefab of the image to instantiate
    [SerializeField] private Button[] buttons; // Array of buttons
    [SerializeField] private Sprite[] levelSprites; // Array of sprites for different levels
    [SerializeField] private int[] thresholds; // Value thresholds for changing sprites
    [SerializeField] private Button[] betValueButtons; // These hold the values for bet increment amount
    [SerializeField] private CanvasGroup bannerCanvasGroup; // CanvasGroup for the banner to fade
    [SerializeField] private TextMeshProUGUI totalBetValueText; // TMP Text for total bet value
    private int selectedBetValue = 0; // Default value

    // List to keep track of instantiated images
    private List<GameObject> imagesToActivate = new List<GameObject>();

    void Start()
    {
        // Set up button listeners for images
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }

        // Set up button listeners for bet values
        foreach (Button betButton in betValueButtons)
        {
            betButton.onClick.AddListener(() => OnBetValueButtonClick(betButton));
        }

        enableButtons();
    }

    private void OnBetValueButtonClick(Button betButton)
    {
        // Parse the bet value from the button's TMP text component
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

        // Check if a bet value has been selected
        if (selectedBetValue == 0)
        {
            ShowBanner();
            return; // Exit the function to prevent further processing
        }

        // Instantiate the image prefab
        GameObject newImage = Instantiate(imagePrefab, clickedButton.transform.parent);

        // Set the position of the new image to the position of the clicked button
        RectTransform buttonRectTransform = clickedButton.GetComponent<RectTransform>();
        RectTransform newImageRectTransform = newImage.GetComponent<RectTransform>();

        newImageRectTransform.anchoredPosition = buttonRectTransform.anchoredPosition;

        // Reset the rotation and set the scale to half
        newImage.transform.localRotation = Quaternion.identity;
        newImage.transform.localScale = Vector3.one * 0.5f; // Set scale to half

        // Add the new image to the list
        imagesToActivate.Add(newImage);

        // Check if the new image already has a TMP component
        TextMeshProUGUI tmpComponent = newImage.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpComponent == null)
        {
            // Add a TMP component if it doesn't exist
            GameObject tmpObj = new GameObject("TMPText");
            tmpObj.transform.SetParent(newImage.transform);

            // Set up the TMP component
            tmpComponent = tmpObj.AddComponent<TextMeshProUGUI>();
            tmpComponent.text = "0"; // Initialize with 0
            tmpComponent.alignment = TextAlignmentOptions.Center;
            tmpComponent.color = Color.black; // Set font color to black
            tmpComponent.fontSize = 20; // Set font size to 20

            // Set the RectTransform properties
            RectTransform rectTransform = tmpComponent.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(40, 40); // Adjust size as needed
            rectTransform.anchoredPosition = Vector2.zero; // Center it
        }

        // Update the TMP text value
        if (int.TryParse(tmpComponent.text, out int currentValue))
        {
            int newValue = currentValue + selectedBetValue;
            tmpComponent.text = newValue.ToString();

            // Determine the appropriate sprite based on the value
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

        // Calculate the total bet value and update the TMP text component
        UpdateTotalBetValue();
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
        return null; // Return null if no threshold is met
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
        Invoke("FadeOutBanner", 1f); // Fade out after 1 second
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

        // Update the TMP text component with the total bet value
        totalBetValueText.text = $"Current Play: {totalValue}";
    }
}
