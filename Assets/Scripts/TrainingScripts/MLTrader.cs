using System;
using System.Collections;
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
    [SerializeField]
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


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(cyclesEnd - cyclesCount);
        sensor.AddObservation(StartingCredits);
        sensor.AddObservation(Credits);
        sensor.AddObservation(buyingTurn);
        sensor.AddOneHotObservation(planet, 6);
        for(int i = tradingIndex; i < tradingIndex + 3; i++)
        {
            sensor.AddObservation(planetsStats.planets[planet].tradingGoods[i].Quantity);
            sensor.AddObservation(planetsStats.planets[planet].tradingGoods[i].ActualPrice);
            sensor.AddObservation(tradingGoods[i].Quantity);
        }
        for (int i = 0; i < 3; i++)
        {
            sensor.AddObservation(boughtPrice[i]);
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
            //Then buy / sell
        }
        if (secondGood != 0)
        {
            //Then buy / sell
        }
        if (thirdGood != 0)
        {
            //Then buy / sell
        }
        if (firstGood + secondGood + thirdGood == 0)
        {
            if(buyingTurn)
            {
                AddReward(-0.002f);
            }
            else
            {
                AddReward(-0.002f);
            }
        }

        if (newPlanet != planet && buyingTurn)
        {
            planet = newPlanet;
            simplifiedInteractions.MoveToPlanet(transform, planet, flySpeed);
        }

        if (cyclesCount >= cyclesEnd)
        {
            if (StartingCredits != Credits) 
            {
                if(StartingCredits > Credits)
                {
                    //BigReward
                }
                else
                {
                    //SmallReward
                }
            }

            EndEpisode();
        }

        //Some sort of static fee. For fuel, workers, ect.
        if (Credits > 200) { Credits -= 500 / cyclesEnd; }

        buyingTurn = buyingTurn == true ? false : true;
    }

    public override void OnEpisodeBegin()
    {
        for(int i = 0; i < 9; i++)
        {
            tradingGoods[i].Quantity = 0;
        }
        for (int i = 0; i < 3; i++)
        {
            boughtPrice[i] = 0;
            boughtAmount[i] = 0;
        }
        cyclesCount = 0;
        cyclesEnd = Random.Range(8, 20) * 2;
        planet = Random.Range(0, 6);
        tradingIndex = Random.Range(0, 7);
        buyingTurn = true;
        StartingCredits = 30000 + Random.Range(-10000, 10001);
        Credits = StartingCredits;
        simplifiedInteractions.MoveToPlanet(transform, planet, flySpeed);
    }
}