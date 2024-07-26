using System.Collections;
using UnityEngine;
using TMPro;

public class RandomNumberGenerator : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultText;
    public Transform rotatingImage;
    [SerializeField] private float countdownTime = 40f;
    public static RandomNumberGenerator instance;

    void Start()
    {
        resultText.gameObject.SetActive(false);
    }

    public IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            timerText.text = "Time Left : " + countdownTime.ToString("F0"); // Display the countdown as an integer
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }
        timerText.gameObject.SetActive(false);
    }

    public void display(int number)
    {
        resultText.gameObject.SetActive(true);
        resultText.text = number.ToString();
    }
}
