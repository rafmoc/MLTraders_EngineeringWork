using UnityEngine;
using Trade.TradeUtils;
using System.Collections.Generic;
using Trade.PlanetsData;

namespace Trade.PlanetsMechanics
{
    public class InitializeNewPlanetsData : MonoBehaviour
    {
        public int RandomizingSeed;

        [SerializeField]
        [Tooltip("Reference here an object that holds all tradable objects / planets as childs")]
        private GameObject TradeablePlanetsParent;

        [SerializeField] public PlanetsStats planetsStats;

        private List<Transform> tradableTransforms = new();
        private List<PlanetStats> planets = new();

        private void Start()
        {
            Random.InitState(RandomizingSeed);
            //PlanetsData();
        }

        public void PlanetsData()
        {
            tradableTransforms.Clear();
            planets.Clear();

            foreach (Transform child in TradeablePlanetsParent.transform)
            {
                tradableTransforms.Add(child);
            }

            InitializePlanetsData();
            BalancePlanets();

            planetsStats.planets = planets;
        }

        private void BalancePlanets()
        {
            int planetsCount = -1;
            int goodsCount = 9;
            int[] tab = new int[goodsCount];
            foreach (Transform tradeableTransform in tradableTransforms)
            {
                planetsCount++;

                for (int i = 0; i < goodsCount; i++)
                {
                    tab[i] += planets[planetsCount].goodsProduction[i].Value;
                }
            }

            int balancingCount = 8;

            foreach (Transform tradeableTransform in tradableTransforms)
            {
                balancingCount -= 2;
                if (balancingCount > 0)
                {
                    for (int i = 0; i < goodsCount; i++)
                    {
                        if (tab[i] % 2 == 0)
                        {
                            planets[planetsCount].goodsProduction[i].Value -= tab[i] / balancingCount;
                            tab[i] -= tab[i] / balancingCount;
                        }
                        else
                        {
                            planets[planetsCount].goodsProduction[i].Value--;
                            tab[i]--;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < goodsCount; i++)
                    {
                        planets[planetsCount].goodsProduction[i].Value -= tab[i];
                        tab[i] = 0;
                    }
                    continue;
                }
                planetsCount--;
            }
        }

        private void InitializePlanetsData()
        {
            int planetsCount = -1;
            //For each planet that will have PlanetStats
            foreach (Transform tradeableTransform in tradableTransforms)
            {
                planetsCount++;
                planets.Add(new PlanetStats(tradeableTransform.name));

                //Planet goods modifiers
                int oresModifier = 0;
                int rareOresModifier = 0;
                int organicModifier = 0;
                int technologyModifier = 0;
                int rawCrystalsModifier = 0;
                long populationModifier = 0;
                int dangerModifier = 0;

                /*
                Here i use my custom Random Ragne with Chance. 
                To make it work properly remember to have chances sum to 100.
                example on this planetType below:
                - new TradeRandom.RandomWithChance(0, 0, 60) - it means that there is 60% chance that random value will be in range 0 to 0
                - new TradeRandom.RandomWithChance(1, 2, 30) - it means that there is 30% chance that random value will be in range 1 to 2
                - new TradeRandom.RandomWithChance(3, 3, 10) - it means that there is 10% chance that random value will be in range 3 to 3
                So you change the chance of randomness in specific set
                */

                planets[planetsCount].planetType = (PlanetType)TradeRandom.RandomRangeChance(
                    new TradeRandom.RandomWithChance(0, 0, 60), new TradeRandom.RandomWithChance(1, 2, 30), new TradeRandom.RandomWithChance(3, 3, 10));
                planets[planetsCount].planetTech = (PlanetTech)TradeRandom.RandomRangeChance(
                    new TradeRandom.RandomWithChance(0, 1, 10), new TradeRandom.RandomWithChance(2, 2, 80), new TradeRandom.RandomWithChance(3, 4, 10));
                planets[planetsCount].planetResources = (PlanetResources)TradeRandom.RandomRangeChance(
                    new TradeRandom.RandomWithChance(0, 1, 10), new TradeRandom.RandomWithChance(2, 2, 80), new TradeRandom.RandomWithChance(3, 4, 10));
                planets[planetsCount].planetState = (PlanetState)TradeRandom.RandomRangeChance(
                    new TradeRandom.RandomWithChance(0, 0, 60), new TradeRandom.RandomWithChance(1, 2, 30), new TradeRandom.RandomWithChance(3, 3, 10));


                switch (planets[planetsCount].planetType)
                {
                    case PlanetType.Normal: organicModifier += 100; populationModifier += 2000; break;
                    case PlanetType.Desert: organicModifier += 10; populationModifier += 100; break;
                    case PlanetType.Ocean: organicModifier += 200; populationModifier += 1000; break;
                    case PlanetType.NuclearWasteland:
                        organicModifier += 1; populationModifier += 10;
                        planets[planetsCount].planetTech = PlanetTech.LowTech;
                        planets[planetsCount].planetState = PlanetState.Destroyed; break;
                }

                switch (planets[planetsCount].planetTech)
                {
                    case PlanetTech.Primitive: technologyModifier += 1; populationModifier += 10; break;
                    case PlanetTech.LowTech: technologyModifier += 5; populationModifier += 100; break;
                    case PlanetTech.MidTech: technologyModifier += 10; populationModifier += 500; dangerModifier += 1; break;
                    case PlanetTech.HighTech: technologyModifier += 50; populationModifier += 5000; dangerModifier += 2; break;
                    case PlanetTech.TechnoPolis: technologyModifier += 100; populationModifier += 10000; dangerModifier += 3; break;
                }

                switch (planets[planetsCount].planetResources)
                {
                    case PlanetResources.Drained: oresModifier += 1; break;
                    case PlanetResources.ResourcePoor: oresModifier += 50; rareOresModifier += 1; break;
                    case PlanetResources.ResourcesNormal: oresModifier += 200; rareOresModifier += 5; rawCrystalsModifier = 1; break;
                    case PlanetResources.ResourcePlenty: oresModifier += 500; rareOresModifier += 10; rawCrystalsModifier = 1; break;
                    case PlanetResources.ResourceRich: oresModifier += 1000; rareOresModifier += 20; rawCrystalsModifier = 2; break;
                }

                switch (planets[planetsCount].planetState)
                {
                    case PlanetState.Peacefull: populationModifier += 400; technologyModifier += 20; break;
                    case PlanetState.Epidemy: populationModifier -= 200; technologyModifier += 5; dangerModifier += 1; break;
                    case PlanetState.AtWar: populationModifier -= 200; technologyModifier += 5; dangerModifier += 1; break;
                    case PlanetState.Destroyed: populationModifier = 1; technologyModifier -= 4; dangerModifier += 1; break;
                }

                //Randomizing population on planet
                populationModifier *= Random.Range(100, 10000);

                Mathf.Clamp(populationModifier, Mathf.Pow(10, 4), 2 * Mathf.Pow(10, 12));
                Mathf.Clamp(technologyModifier, 0, 200);

                planets[planetsCount].technologyModifier = technologyModifier;
                planets[planetsCount].population = populationModifier;
                planets[planetsCount].dangerLevel = dangerModifier;
                planets[planetsCount].planetCredits = 4 * (int)(Random.Range(10, 1000) * technologyModifier + Mathf.Sqrt(populationModifier));

                //Here we add resources to all planets with specific amount and common price.
                planets[planetsCount].tradingGoods.AddAll(new List<TradingGoods>
            {
                new TradingGoods(Goods.Iron,           Random.Range(oresModifier, 10 * oresModifier), 50),
                new TradingGoods(Goods.Copper,         Random.Range(oresModifier, 10 * oresModifier), 75),
                new TradingGoods(Goods.Coal,           Random.Range(oresModifier, 10 * oresModifier), 150),
                new TradingGoods(Goods.Food,           Random.Range(organicModifier, 10 * organicModifier), 250),
                new TradingGoods(Goods.Water,          Random.Range(organicModifier, 10 * organicModifier), 400),
                new TradingGoods(Goods.Medicine,       Random.Range(technologyModifier, 10 * technologyModifier), 600),
                new TradingGoods(Goods.RareMetals,     Random.Range(rareOresModifier, 10 * rareOresModifier), 800),
                new TradingGoods(Goods.HighEnergyFuel, Random.Range(technologyModifier, 10 * technologyModifier), 1200),
                new TradingGoods(Goods.RawCrystals,    Random.Range(rawCrystalsModifier, 10 * rawCrystalsModifier), 2000)
            });

                //Here we define production on planet. Every planet tick planet will lose or get specific amount of resources randomized here
                //This simulate static planets needs. Can be combined or repleaced by some buildings / factorys system
                planets[planetsCount].goodsProduction.AddAll(new List<GoodsProduction>
            {
                new GoodsProduction(Goods.Iron,           Random.Range(-6, 6)),
                new GoodsProduction(Goods.Copper,         Random.Range(-6, 6)),
                new GoodsProduction(Goods.Coal,           Random.Range(-5, 5)),
                new GoodsProduction(Goods.Food,           Random.Range(-5, 5)),
                new GoodsProduction(Goods.Water,          TradeRandom.RandomRangeChance(new TradeRandom.RandomWithChance(0, 0, 40), new TradeRandom.RandomWithChance(-4, 4, 60))),
                new GoodsProduction(Goods.Medicine,       TradeRandom.RandomRangeChance(new TradeRandom.RandomWithChance(0, 0, 50), new TradeRandom.RandomWithChance(-2, 2, 50))),
                new GoodsProduction(Goods.RareMetals,     TradeRandom.RandomRangeChance(new TradeRandom.RandomWithChance(0, 0, 60), new TradeRandom.RandomWithChance(-2, 2, 40))),
                new GoodsProduction(Goods.HighEnergyFuel, TradeRandom.RandomRangeChance(new TradeRandom.RandomWithChance(0, 0, 70), new TradeRandom.RandomWithChance(-1, 1, 30))),
                new GoodsProduction(Goods.RawCrystals,    TradeRandom.RandomRangeChance(new TradeRandom.RandomWithChance(0, 0, 80), new TradeRandom.RandomWithChance(-1, 1, 20)))
            });

                //For every resource in planet
                for (int i = 0; i < planets[planetsCount].tradingGoods.Count; i++)
                {
                    //Calculate ActualPrice of a good that depends on quantity of this good on planet
                    //Calculate function 3 - x/1800 where x is quantity of trading good.
                    //This is because it fits perfect my max limit of resource hardcaped at 5 000.
                    //If change is needed it is the best to change both hardcap and function to calculate ActualPrice
                    float priceModifier = 3 - ((float)planets[planetsCount].tradingGoods[i].Quantity / 1800);
                    priceModifier = planets[planetsCount].tradingGoods[i].CommonPrice * priceModifier;
                    priceModifier += Random.Range(0, 40);

                    //Clamping values
                    planets[planetsCount].tradingGoods[i].Quantity = Mathf.Clamp(planets[planetsCount].tradingGoods[i].Quantity, 0, 5000);
                    planets[planetsCount].tradingGoods[i].ActualPrice = (int)Mathf.Clamp(priceModifier, 10, 12000);
                }
            }
        }
    }
}