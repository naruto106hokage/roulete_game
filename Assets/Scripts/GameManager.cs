using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RandomNumberGenerator randomNumberGenerator;
    [SerializeField] private ResultWheelHandler resultWheelHandler;
    [SerializeField] private RouletteWheel rouletteWheel;
    [SerializeField] private BoardManager boardManager;

    int randomNumber;

    private void Awake()
    {
        if (randomNumberGenerator == null || resultWheelHandler == null || rouletteWheel == null || boardManager == null)
        {
            Debug.LogError("Reference not set");
        }
    }

    void Start()
    {
        StartCoroutine("startGame");
    }

    IEnumerator startGame()
    {
        resultWheelHandler.disableAll();
        boardManager.disableAllMarker();
        yield return StartCoroutine(randomNumberGenerator.StartCountdown());
        randomNumber = Random.Range(0, 37);
        Debug.Log("random Number :" + randomNumber);
        resultWheelHandler.disableAll();
        yield return StartCoroutine(resultWheelHandler.rotateTheWheel());
        resultWheelHandler.displayResult(randomNumber);
        randomNumberGenerator.display(randomNumber);
        boardManager.setBoardMarker(randomNumber);
    }   


}
