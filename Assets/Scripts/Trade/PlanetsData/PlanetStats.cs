using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trade.PlanetsData
{
    [Serializable]
    public class PlanetStats
    {
        public string planetName;
        public int planetCredits;
        public List<TradingGoods> tradingGoods = new();
        public List<GoodsProduction> goodsProduction = new();

        public int dangerLevel;
        public int technologyModifier;
        public long population;
        public PlanetType planetType;
        public PlanetTech planetTech;
        public PlanetResources planetResources;
        public PlanetState planetState;

        public PlanetStats(string PlanetName)
        {
            planetName = PlanetName;
        }
    }

    [Serializable]
    public enum PlanetType
    {
        Normal,
        Ocean,
        Desert,
        NuclearWasteland
    }

    [Serializable]
    public enum PlanetTech
    {
        TechnoPolis,
        HighTech,
        MidTech,
        LowTech,
        Primitive
    }

    [Serializable]
    public enum PlanetResources
    {
        ResourceRich,
        ResourcePlenty,
        ResourcesNormal,
        ResourcePoor,
        Drained
    }

    [Serializable]
    public enum PlanetState
    {
        Peacefull,
        Epidemy,
        AtWar,
        Destroyed
    }
}