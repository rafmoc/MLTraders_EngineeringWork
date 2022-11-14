using System;
using System.Collections.Generic;
using Trade.PlanetsData;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class MLTrader : Agent
{
    public List<TradingGoods> tradingGoods = new();
    [SerializeField]
    private PlanetsStats planetsStats;
    [SerializeField]
    private SimplifiedInteractionsML simplifiedInteractions;
    [Tooltip("0 - Resources, 3 - Supply, 6 - luxury")]
    public int tradingIndex;

    public float flySpeed;
    public int planet;
    public int StartingCredits;
    public int Credits;
    public int newPlanet, firstGood, secondGood, thirdGood;
    public int[] boughtPrice = new int[3];
    public int[] boughtAmount = new int[3];
    public int cyclesCount = 0;
    public int cyclesEnd;
    public bool buyingTurn;
    public TradingGoods planetTradingGoods;

    //---- For logs only
    public int[] products = new int[9];
    public int[] picked = new int[3];
    public int[] earnedCredits = new int[3]; //0 - lost, 1 - same, 2 - earned
    public int[] skipping = new int[2]; //selling / buying
    public int[] planetsTraveled = new int[6];
    public int failedBuy = 0;
    public int tradeBalance = 0;
    //----

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(cyclesEnd - cyclesCount);
        sensor.AddObservation(StartingCredits);
        sensor.AddObservation(Credits);
        sensor.AddObservation(buyingTurn);
        sensor.AddOneHotObservation(planet, 6);                 //Planet number we are on
        for(int i = tradingIndex; i < tradingIndex + 3; i++)    //everything about planet we are on
        {
            sensor.AddObservation(planetsStats.planets[planet].tradingGoods[i].Quantity);
            sensor.AddObservation(planetsStats.planets[planet].tradingGoods[i].ActualPrice);
            sensor.AddObservation(tradingGoods[i].Quantity);
        }
        for (int i = 0; i < 3; i++) //If bought any goods - for how much it was bought
        {
            sensor.AddObservation(boughtPrice[i]);
        }
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        actionMask.SetActionEnabled(0, planet, false); //Masking planet we are on. Trader MUST choose other planet to travel too.

        //Masking all forbiden actions
        if (buyingTurn)
        {
            for (int j = 0; j < 3; j++)
            {
                planetTradingGoods = planetsStats.planets[planet].tradingGoods[tradingIndex + j];

                int maskingValue = Math.Min((((Credits - planetTradingGoods.ActualPrice - 1) / 3) / planetTradingGoods.ActualPrice), planetTradingGoods.Quantity);

                for (int i = 30; i > maskingValue; i--)
                {
                    actionMask.SetActionEnabled(1 + j, i, false);
                }
            }
        }
        else
        {
            for(int j = 0; j < 3; j++)
            {
                for (int i = 30; i > tradingGoods[tradingIndex + j].Quantity; i--)
                {
                    actionMask.SetActionEnabled(1 + j, i, false);
                }
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        cyclesCount++;
        newPlanet = actions.DiscreteActions[0]; //planet id to travel after buying
        firstGood = actions.DiscreteActions[1]; // 0 - 30
        secondGood = actions.DiscreteActions[2];// 0 - 30
        thirdGood = actions.DiscreteActions[3]; // 0 - 30 

        if (firstGood != 0)
        {
            if (buyingTurn) { Buy(0 + tradingIndex, firstGood); } //Buy
            else            { Sell(0 + tradingIndex, firstGood); } //Sell
        }
        if (secondGood != 0)
        {
            if (buyingTurn) { Buy(1 + tradingIndex, secondGood); } //Buy
            else            { Sell(1 + tradingIndex, secondGood); } //Sell
        }
        if (thirdGood != 0)
        {
            if (buyingTurn) { Buy(2 + tradingIndex, thirdGood); } //Buy
            else            { Sell(2 + tradingIndex, thirdGood); } //Sell
        }

        if (firstGood + secondGood + thirdGood == 0)
        {
            if(buyingTurn)
            {
                skipping[1]++;      //skipping buy
                AddReward(-0.002f); //small penalty for not doing any business
            }
            else
            {
                skipping[0]++;      //skipping sell
                AddReward(-0.002f); //small penalty for not doing any business
            }
        }

        if (newPlanet != planet && buyingTurn)
        {
            planet = newPlanet;
            planetsTraveled[newPlanet]++;
            simplifiedInteractions.MoveToPlanet(transform, planet, flySpeed);
        }

        if (cyclesCount >= cyclesEnd)
        {
            if (StartingCredits != Credits) 
            {
                if(StartingCredits < Credits)
                {
                    AddReward(0.4f);
                    AddReward(0.05f * (int)((Credits - StartingCredits) / 200));
                    earnedCredits[2]++; //Earned
                }
                else
                {
                    AddReward(0.1f - Mathf.Clamp(0.005f * (int)((StartingCredits - Credits) / 200), -100f, 0.2f));
                    earnedCredits[0]++; //Lost
                }
            }
            else
            {
                earnedCredits[1]++; //Same credits
            }

            tradeBalance += Credits - StartingCredits;

            Sell(0 + tradingIndex, tradingGoods[0 + tradingIndex].Quantity);
            Sell(1 + tradingIndex, tradingGoods[1 + tradingIndex].Quantity);
            Sell(2 + tradingIndex, tradingGoods[2 + tradingIndex].Quantity);

            EndEpisode();
        }

        //Some sort of static fee. For fuel, workers, ect.
        if (Credits > 200) { Credits -= 500 / cyclesEnd; }

        buyingTurn = buyingTurn == true ? false : true;
    }

    public override void OnEpisodeBegin()
    {
        //Clearing tables
        for(int i = 0; i < 9; i++)
        {
            tradingGoods[i].Quantity = 0;
        }
        for (int i = 0; i < 3; i++) 
        {
            boughtPrice[i] = 0;
            boughtAmount[i] = 0;
        }
        //Setting values
        cyclesCount = 0;                      //Counter of agent life
        cyclesEnd = Random.Range(8, 20) * 2;  //How many turns agent will live
        planet = Random.Range(0, 6);          //Starting planet
        tradingIndex = Random.Range(0, 7);    //What goods it will trade
        buyingTurn = true;                    //Always starts with buying turn
        Credits = StartingCredits;
        StartingCredits = 30000 + Random.Range(-10000, 10001);              //Some random but big amount of starting credits
        simplifiedInteractions.MoveToPlanet(transform, planet, flySpeed);   //Move agent ship to planet he is on
    }

    private void Buy(int tradingGood, int amount)
    {
        planetTradingGoods = planetsStats.planets[planet].tradingGoods[tradingGood]; //Take newest data about goods on planet

        if (planetTradingGoods.Quantity >= amount && planetTradingGoods.ActualPrice * amount <= Credits)
        {
            planetTradingGoods.Quantity -= amount;
            Credits -= planetTradingGoods.ActualPrice * amount;
            tradingGoods[tradingGood].Quantity += amount;

            //Calculating boughtPrice
            boughtPrice[tradingGood - tradingIndex] = ((boughtPrice[tradingGood - tradingIndex] * boughtAmount[tradingGood - tradingIndex]) + (planetTradingGoods.ActualPrice * amount)) / (boughtAmount[tradingGood - tradingIndex] + amount);
            boughtAmount[tradingGood - tradingIndex] += amount;
        }
        else //Forbiden move. Reset Agent and add -1 reward.
        {
            AddReward(-1f);
            Debug.Log("Failed Buy");
            failedBuy++;

            Sell(0 + tradingIndex, tradingGoods[0 + tradingIndex].Quantity);
            Sell(1 + tradingIndex, tradingGoods[1 + tradingIndex].Quantity);
            Sell(2 + tradingIndex, tradingGoods[2 + tradingIndex].Quantity);

            EndEpisode();
        }
    }
    private void Sell(int tradingGood, int amount)
    {
        planetTradingGoods = planetsStats.planets[planet].tradingGoods[tradingGood]; //Take newest data about goods on planet

        if (tradingGoods[tradingGood].Quantity >= amount)
        {
            planetTradingGoods.Quantity += amount;
            Credits += planetTradingGoods.ActualPrice * amount;
            tradingGoods[tradingGood].Quantity -= amount;

            products[tradingGood]++; picked[tradingGood - tradingIndex]++;

            //Calculating boughtPrice
            boughtAmount[tradingGood - tradingIndex] -= amount;
            if (boughtAmount[tradingGood - tradingIndex] == 0)
            {
                boughtPrice[tradingGood - tradingIndex] = 0;
            }
        }
        else //Forbiden move. Reset Agent and add -1 reward.
        {
            AddReward(-1f);
            Debug.Log("Failed Sell");

            Sell(0 + tradingIndex, tradingGoods[0 + tradingIndex].Quantity);
            Sell(1 + tradingIndex, tradingGoods[1 + tradingIndex].Quantity);
            Sell(2 + tradingIndex, tradingGoods[2 + tradingIndex].Quantity);

            EndEpisode();
        }
    }
}