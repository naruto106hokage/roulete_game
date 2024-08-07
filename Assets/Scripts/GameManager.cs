using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RandomNumberGenerator randomNumberGenerator;
    [SerializeField] private ResultWheelHandler resultWheelHandler;
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private ListManager listManager;
    [SerializeField] private BetManager betManager;
    [SerializeField] private ResultCalculator resultCalculator;


    private float repeatInterval = 40f;
    private float playAgain = 5f;

    private const string PlayerPrefsKey = "RandomNumbersList";
    private const int MaxRandomNumbers = 13;

    private List<int> randomNumbers = new List<int>();
    private Dictionary<string,int> betPositions = new Dictionary<string, int>();

    private void Awake()
    {
        if (randomNumberGenerator == null || resultWheelHandler == null || rouletteWheel == null || boardManager == null || betManager == null)
        {
            Debug.LogError("Reference not set");
        }

        LoadRandomNumbers();
    }

    void Start()
    {
        listManager.setNumberToList(randomNumbers);
        StartCoroutine("RepeatedStartGame");
    }

    IEnumerator RepeatedStartGame()
    {
        while (true)
        {
            yield return StartCoroutine(startGame());
            yield return new WaitForSeconds(playAgain);
        }
    }

    IEnumerator startGame()
    {
        resultWheelHandler.disableAll();
        boardManager.disableAllMarker();
        randomNumberGenerator.disableResultNumber();
        betManager.DestroyAllImages();
        betManager.EnableButtons();
        betManager.InvokeDisableButtons(repeatInterval - 1);
        yield return StartCoroutine(randomNumberGenerator.StartCountdown(repeatInterval));
        betPositions = betManager.displayBetPositions();
        betManager.DeactivateAllImages();
        int randomNumber = 5;//Random.Range(0, 37);
        Debug.Log("Random Number: " + randomNumber);
        betManager.DisableButtons();
        AddRandomNumber(randomNumber);
        resultWheelHandler.disableAll();
        yield return StartCoroutine(resultWheelHandler.rotateTheWheel());
        resultCalculator.calculateResult(randomNumber, betPositions);
        resultWheelHandler.displayResult(randomNumber);
        randomNumberGenerator.display(randomNumber);
        boardManager.setBoardMarker(randomNumber);
        listManager.setNumberToList(randomNumbers);
    }

    void AddRandomNumber(int number)
    {
        if (randomNumbers.Count >= MaxRandomNumbers)
        {
            randomNumbers.RemoveAt(0); // Remove the oldest number
        }
        randomNumbers.Add(number);
        SaveRandomNumbers();
    }

    void SaveRandomNumbers()
    {
        string json = JsonUtility.ToJson(new Serialization<int>(randomNumbers));
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    void LoadRandomNumbers()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            randomNumbers = JsonUtility.FromJson<Serialization<int>>(json).ToList();
        }
    }
}

[System.Serializable]
public class Serialization<T>
{
    public List<T> items;

    public Serialization(List<T> items)
    {
        this.items = items;
    }

    public List<T> ToList()
    {
        return items;
    }
}
