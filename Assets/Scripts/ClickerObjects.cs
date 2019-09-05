using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Object", menuName = "Clicker Object", order = 1)]
public class ClickerObjects : ScriptableObject
{
    public string Name;
    public float TimeToPayout;
    public double BasePay;
    public double BaseCost;
    public float CostIncrease;
    public double AutoClickCost;
    public float TimeToBreed;

    public void Pay(int totalObjects)
    {
        Game.instance.AddMoney(BasePay * totalObjects);
    }
}
