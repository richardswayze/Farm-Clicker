using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ClickableObject : MonoBehaviour
{
    [SerializeField] ClickerObjects clickerObject;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] Button workButton;
    [SerializeField] Image completionBar;
    [SerializeField] TextMeshProUGUI payPerClick;
    [SerializeField] TextMeshProUGUI amountOwnedLabel;
    [SerializeField] Button buyButton;
    [SerializeField] Button autoClickButton;
    [SerializeField] Button breedButton;
    [SerializeField] Image breedCompletionIcon;

    private string objectName;
    private float timeToPayout;
    private double basePay;
    private double cost;
    private int amountOwned = 0;
    private int amountBought = 0;
    private bool payoutInProgress;
    private float payProgress;
    private Text buyButtonText;
    private bool autoClickEnabled;
    private bool breedingInProgress;
    private float breedingProgress;
    private float timeToBreed;

    private void Awake()
    {
        objectName = clickerObject.Name;
        timeToPayout = clickerObject.TimeToPayout;
        basePay = clickerObject.BasePay;
        cost = clickerObject.BaseCost;
        timeToBreed = clickerObject.TimeToBreed;

        nameText.text = objectName;
        buyButtonText = buyButton.GetComponentInChildren<Text>();
        payPerClick.text = basePay + "/Click";
        amountOwnedLabel.text = "Total Owned: " + amountOwned;
        workButton.GetComponentInChildren<Text>().text = "Click to work";
        payoutInProgress = false;
        autoClickEnabled = false;
        breedingInProgress = false;
    }

    public void StartPayout()
    {
        if (payoutInProgress || amountOwned < 1)
        {
            return;
        }
        payoutInProgress = true;
        payProgress = 0f;
    }

    public void BuyClicker()
    {
        if (Game.instance.currentBuyMode == Game.BuyMode.Max)
        {
            if (Game.instance.currentMoney >= CalculateCost(CalculateMax(Game.instance.currentMoney)))
            {
                int amountToBuy = CalculateMax(Game.instance.currentMoney);
                Game.instance.SubtractMoney(CalculateCost(amountToBuy));
                amountOwned += amountToBuy;
                amountBought += amountToBuy;
                cost = clickerObject.BaseCost * Mathf.Pow(clickerObject.CostIncrease, amountBought);
            }
        }
        else if (Game.instance.currentMoney >= CalculateCost(Game.instance.amountToBuy))
        {
            int amountToBuy = Game.instance.amountToBuy;
            Game.instance.SubtractMoney(CalculateCost(amountToBuy));
            amountOwned += amountToBuy;
            amountBought += amountToBuy;
            cost = clickerObject.BaseCost * Mathf.Pow(clickerObject.CostIncrease, amountBought);
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public void EnableAutoClick()
    {
        autoClickEnabled = true;
        autoClickButton.interactable = false;
    }

    public void Breed()
    {
        if (breedingInProgress || amountOwned < 2)
        {
            return;
        }
        breedingInProgress = true;
        breedingProgress = 0f;
    }

    private double CalculateCost(int amountToBuy)
    {
        if (amountToBuy < 1)
        {
            amountToBuy = 1;
        }
        double itemCost = clickerObject.BaseCost * ((Mathf.Pow(clickerObject.CostIncrease, amountBought) * (Mathf.Pow(clickerObject.CostIncrease, amountToBuy) - 1) / ((clickerObject.CostIncrease - 1))));
        return itemCost;  
    }

    private int CalculateMax(double availableMoney)
    {
        return Mathf.FloorToInt(Mathf.Log((((float)availableMoney * (clickerObject.CostIncrease - 1)) / ((float)clickerObject.BaseCost * Mathf.Pow(clickerObject.CostIncrease, amountBought))) + 1, clickerObject.CostIncrease));
    }

    public void Update()
    {
        if (Game.instance.currentBuyMode == Game.BuyMode.Max)
        {
            buyButton.interactable = Game.instance.currentMoney >= CalculateCost(CalculateMax(Game.instance.currentMoney));
        }
        else
        {
            buyButton.interactable = Game.instance.currentMoney >= CalculateCost(Game.instance.amountToBuy);
        }
        workButton.interactable = !payoutInProgress && amountOwned > 0;
        breedButton.interactable = amountOwned > 1 && !breedingInProgress;

        if (!autoClickEnabled)
        {
            autoClickButton.interactable = Game.instance.currentMoney >= clickerObject.AutoClickCost;
        }
        else if (!payoutInProgress)
        {
            StartPayout();
        }
        
        if (payoutInProgress)
        {
            payProgress += Time.deltaTime; 
            if (payProgress >= timeToPayout)
            {
                payoutInProgress = false;
                payProgress = 0f;
                clickerObject.Pay(amountOwned);
            }
        }

        if (breedingInProgress)
        {
            breedingProgress += Time.deltaTime;
            if (breedingProgress >= timeToBreed)
            {
                breedingInProgress = false;
                breedingProgress = 0;
                amountOwned++;
            }
        }

        amountOwnedLabel.text = amountOwned.ToString();
        payPerClick.text = Math.Round((basePay * amountOwned), 2, MidpointRounding.AwayFromZero).ToString();
        completionBar.fillAmount = payProgress / timeToPayout;
        breedCompletionIcon.fillAmount = breedingProgress / timeToBreed;
        
        if (Game.instance.currentBuyMode == Game.BuyMode.Max)
        {
            buyButtonText.text = "Cost: " + Math.Round(CalculateCost(CalculateMax(Game.instance.currentMoney)), 2, MidpointRounding.AwayFromZero);
        }
        else
        {
            buyButtonText.text = "Cost: " + Math.Round(CalculateCost(Game.instance.amountToBuy), 2, MidpointRounding.AwayFromZero);
        }

    }
}
