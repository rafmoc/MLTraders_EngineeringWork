using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trade.PlanetsData
{
    //Types of goods
    [Serializable]
    public enum Goods
    {
        Iron,
        Copper,
        Coal,
        Food,
        Water,
        Medicine,
        RareMetals,
        HighEnergyFuel,
        RawCrystals
    }

    //Class to hold quantity and prices of goods
    [Serializable]
    public class TradingGoods
    {
        public Goods Name;
        public int Quantity;
        public int CommonPrice;
        public int ActualPrice;

        public TradingGoods(Goods name, int quantity, int commonPrice)
        {
            Name = name;
            Quantity = quantity;
            CommonPrice = commonPrice;
        }
    }

    //Class to hold informations if number of goods on planet increse or decrese per world production tick
    [Serializable]
    public class GoodsProduction
    {
        public Goods Name;
        public int Value;

        public GoodsProduction(Goods name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
