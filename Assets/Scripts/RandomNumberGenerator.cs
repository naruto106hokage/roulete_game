using System.Collections;
using UnityEngine;
using TMPro;

public class RandomNumberGenerator : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public Transform rotatingImage;
    [SerializeField] private float countdownTime = 40f;
    public int randomNumber;
    public static RandomNumberGenerator instance;

    void Start()
    {
        resultText.gameObject.SetActive(false);
        //StartCoroutine(StartCountdown());
    }

    public IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            timerText.text = "Time Left : " + countdownTime.ToString("F0"); // Display the countdown as an integer
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        // Generate a random number between 0 and 36
        //randomNumber = Random.Range(0, 37);
        //Debug.Log("randomNumber :" + randomNumber);
        //rotationManager.SetWinningPosition(360/randomNumber);
        //rotationManager.Rotate();
    }
}
