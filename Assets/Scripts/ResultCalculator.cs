using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultCalculator : MonoBehaviour
{
    [SerializeField] private TMP_Text amountWonText;
    private int amountWon = 0;
    [SerializeField] private Dictionary<int, int> winMultiplier = new Dictionary<int, int>();
    public void calculateResult(int numberDrawn, Dictionary<string, int> betPlacedPositions)
    {
        foreach (var betPlacedPosition in betPlacedPositions)
        {
            if (betPlacedPosition.Key == numberDrawn.ToString())
            {
                Debug.Log("You won");
                amountWon += betPlacedPosition.Value * 10;
                amountWonText.text = "You won: " + amountWon;
            }
        }
    }
    public void setWinMultiplier()
    {
        winMultiplier.Clear();
        
    }
}
