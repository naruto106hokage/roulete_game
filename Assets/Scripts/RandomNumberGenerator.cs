using System.Collections;
using UnityEngine;
using TMPro;

public class RandomNumberGenerator : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public Transform rotatingImage;
    [SerializeField] private float countdownTime = 40f;
    private int randomNumber;

    ////public WheelRotationManager rotationManager;

    // Assuming the numbers on the wheel are in the same order as seen in the image.
    //private int[] numberOrder = new int[] { 0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27, 13, 36, 11, 30, 8, 23, 10, 5, 24, 16, 33, 1, 20, 14, 31, 9, 22, 18, 29, 7, 28, 12, 35, 3, 26 };

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
        Debug.Log("randomNumber :"+ randomNumber);
        //rotationManager.SetWinningPosition(360/randomNumber);
        //rotationManager.Rotate();
    }
}
