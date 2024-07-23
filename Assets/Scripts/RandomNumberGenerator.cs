using System.Collections;
using UnityEngine;
using TMPro;

public class RandomNumberGenerator : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public Transform rotatingImage;
    [SerializeField] private float countdownTime = 40f;
    private bool isRotating = false;
    private int randomNumber;

    // Assuming the numbers on the wheel are in the same order as seen in the image.
    private int[] numberOrder = new int[] { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };

    void Start()
    {
        resultText.gameObject.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            timerText.text = "Time Left : " + countdownTime.ToString("F0"); // Display the countdown as an integer
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        // Generate a random number between 0 and 36
        randomNumber = Random.Range(0, 37);
        StartCoroutine(RotateImage());
    }

    IEnumerator RotateImage()
    {
        isRotating = true;
        float initialSpeed = 500f; // Initial speed
        float slowDownDuration = 7f; // Time in seconds to slow down
        float elapsedTime = 0f;
        float speed = initialSpeed;

        // Rotate for 7 seconds and slow down gradually
        while (elapsedTime < slowDownDuration)
        {
            rotatingImage.Rotate(Vector3.forward, speed * Time.deltaTime);
            speed = Mathf.Lerp(initialSpeed, 0f, elapsedTime / slowDownDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the image stops at the selected random number
        float anglePerNumber = 360f / numberOrder.Length; // Calculate angle per number
        int targetIndex = System.Array.IndexOf(numberOrder, randomNumber); // Find the index of the random number
        float targetAngle = targetIndex * anglePerNumber;
        Debug.Log("targetAngle :" + targetAngle);
        Debug.Log("targetIndex :" + targetIndex);
        rotatingImage.rotation = Quaternion.Euler(0, 0, -targetAngle); // Apply negative sign to rotate clockwise

        // Display the result
        resultText.text = "Random Number: " + randomNumber.ToString();
        resultText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(false);
        isRotating = false;
    }
}
