using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import the TextMeshPro namespace
using System.Collections; // Required for IEnumerator

public class BetManager : MonoBehaviour
{
    [SerializeField] private GameObject[] imagesToActivate; // Array of images
    [SerializeField] private Button[] buttons; // Array of buttons
    [SerializeField] private Sprite[] levelSprites; // Array of sprites for different levels
    [SerializeField] private int[] thresholds; // Value thresholds for changing sprites
    [SerializeField] private Button[] betValueButtons; // These hold the values for bet increment amount
    [SerializeField] private CanvasGroup bannerCanvasGroup; // CanvasGroup for the banner to fade
    [SerializeField] private TextMeshProUGUI totalBetValueText; // TMP Text for total bet value
    private int selectedBetValue = 0; // Default value

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

        // Deactivate images initially
        foreach (GameObject image in imagesToActivate)
        {
            image.SetActive(false);
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
        }
        else
        {
            Debug.LogError("Invalid bet value format or TextMeshPro component missing.");
        }
    }

    private void OnButtonClick(Button clickedButton)
    {
        // Get the name of the clicked button
        string buttonName = clickedButton.name;

        // Check if a bet value has been selected
        if (selectedBetValue == 0)
        {
            ShowBanner();
            return; // Exit the function to prevent further processing
        }

        // Activate the image associated with the clicked button
        foreach (GameObject image in imagesToActivate)
        {
            if (image.name == buttonName)
            {
                image.SetActive(true);

                // Check if the image already has a TMP component
                TextMeshProUGUI tmpComponent = image.GetComponentInChildren<TextMeshProUGUI>();
                if (tmpComponent == null)
                {
                    // Add a TMP component if it doesn't exist
                    GameObject tmpObj = new GameObject("TMPText");
                    tmpObj.transform.SetParent(image.transform);

                    // Set up the TMP component
                    tmpComponent = tmpObj.AddComponent<TextMeshProUGUI>();
                    tmpComponent.text = "0"; // Initialize with 0
                    tmpComponent.alignment = TextAlignmentOptions.Center;
                    tmpComponent.color = Color.black; // Set font color to black
                    tmpComponent.fontSize = 20; // Set font size to 30

                    // Set the RectTransform properties
                    RectTransform rectTransform = tmpComponent.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.sizeDelta = image.GetComponent<RectTransform>().sizeDelta;
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
                        Image imgComponent = image.GetComponent<Image>();
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

                break; // Exit the loop as we found the matching image
            }
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
        Invoke("FadeOutBanner", 1f); // Fade out after 1 seconds
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
        totalBetValueText.text = $"Curent Play: {totalValue}";
    }
}
