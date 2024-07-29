using UnityEngine;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    [SerializeField] private GameObject[] imagesToActivate; // Array of images
    [SerializeField] private Button[] buttons; // Array of buttons
    [SerializeField] private Sprite[] levelSprites; // Array of sprites for different levels
    [SerializeField] private int[] thresholds; // Value thresholds for changing sprites

    void Start()
    {
        // Set up button listeners
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }

    public void disableAllImages()
    {
        foreach (GameObject image in imagesToActivate)
        {
            image.SetActive(false);
        }
    }

    private void OnButtonClick(Button clickedButton)
    {
        // Get the name of the clicked button
        string buttonName = clickedButton.name;

        // Activate the image associated with the clicked button
        foreach (GameObject image in imagesToActivate)
        {
            if (image.name == buttonName)
            {
                image.SetActive(true);

                // Check if the image already has a Text component
                Text textComponent = image.GetComponentInChildren<Text>();
                if (textComponent == null)
                {
                    // Add a Text component if it doesn't exist
                    GameObject textObj = new GameObject("Text");
                    textObj.transform.SetParent(image.transform);

                    // Set up the Text component
                    textComponent = textObj.AddComponent<Text>();
                    textComponent.text = "0"; // Initialize with 0
                    textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    textComponent.alignment = TextAnchor.MiddleCenter;
                    textComponent.color = Color.black; // Set font color to black
                    textComponent.fontSize = 30; // Set font size to 30

                    // Set the RectTransform properties
                    RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = Vector2.zero;
                    rectTransform.sizeDelta = image.GetComponent<RectTransform>().sizeDelta;
                }

                // Update the text value
                if (int.TryParse(textComponent.text, out int currentValue))
                {
                    int newValue = currentValue + 20;
                    textComponent.text = newValue.ToString();

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
                    Debug.LogWarning("Text component does not contain a valid integer.");
                }

                break; // Exit the loop as we found the matching image
            }
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
        return null; // Return null if no threshold is met
    }
}
