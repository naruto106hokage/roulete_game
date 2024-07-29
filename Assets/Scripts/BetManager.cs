using UnityEngine;
using UnityEngine.UI;

public class BetManager : MonoBehaviour
{
    [SerializeField] private GameObject[] imagesToActivate; // Array of images
    [SerializeField] private Button[] buttons; // Array of buttons

    public void disableAllImages()
    {
        foreach (GameObject image in imagesToActivate)
        {
            image.SetActive(false);
        }

        // Set up button listeners
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
        }
    }

    private void OnButtonClick(Button clickedButton)
    {
        // Get the name of the clicked button
        string buttonName = clickedButton.name;

        // Activate the image associated with the clicked button
        foreach (GameObject image in imagesToActivate)
        {
            // Activate the image if its name matches the button's name
            if (image.name == buttonName)
            {
                image.SetActive(true);
                break; // Exit the loop as we found the matching image
            }
        }
    }
}
