using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game instance;

    public double currentMoney;
    public double startingMoney;
    public enum BuyMode { One, Ten, Max};
    public BuyMode currentBuyMode;
    public int amountToBuy;

    [SerializeField] TextMeshProUGUI availableCash;
    [SerializeField] Text buyModeText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        InitializeGame();
    }

    private void Update()
    {
        availableCash.text = "Available cash: " + Math.Round(currentMoney, 2, MidpointRounding.AwayFromZero);
    }

    public void InitializeGame()
    {
        currentMoney = startingMoney;
        currentBuyMode = BuyMode.One;
        SetBuyButtonText(currentBuyMode);
    }

    public void AddMoney(double amount)
    {
        currentMoney += amount;
    }

    public void SubtractMoney(double amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
        }
        else
        {
            Debug.LogError("Subtracted more money than available");
            currentMoney = 0;
        }
    }

    public void ToggleBuyMode()
    {
        int newBuyMode = (int)currentBuyMode + 1;
        if (newBuyMode > 2)
        {
            newBuyMode = 0;
        }
        currentBuyMode = (BuyMode)newBuyMode;
        SetBuyButtonText(currentBuyMode);
    }

    private void SetBuyButtonText(BuyMode buyMode)
    {
        switch (buyMode)
        {
            case BuyMode.One:
                buyModeText.text = "One";
                amountToBuy = 1;
                break;
            case BuyMode.Ten:
                buyModeText.text = "Ten";
                amountToBuy = 10;
                break;
            case BuyMode.Max:
                buyModeText.text = "Max";
                break;
            default:
                Debug.LogError("Invalid BuyMode");
                break;
        }
    }
}
