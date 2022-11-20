using Trade.PlanetsData;
using UnityEngine;

public class SpaceTickSystem : MonoBehaviour
{
    [SerializeField] private PlanetsStats planetsData;
    [SerializeField] private int ticksToChange = 2;
    [SerializeField] private int ticksToChangeResource = 8;
    private int tick = 0;
    private int PlanetResourceTick = 0;

    public void MakeTick()
    {
        tick++;
        if(tick >= ticksToChange)
        {
            MakeGlobalChanges();
            tick = 0;
        }
    }
    private void MakeGlobalChanges()
    {
        PlanetTick();
        PricesUpdate();
    }

    private void PlanetTick()
    {
        foreach(PlanetStats planet in planetsData.planets)
        {
            planet.planetCredits += (int)(Random.Range(10, 100) + Random.Range(1, 5) * planet.technologyModifier + Mathf.Sqrt(planet.population));
            planet.population += (long)(Mathf.Sqrt(planet.population));

            //TradingGoods production and consumption
            for (int i = 0; i < planet.tradingGoods.Count; i++)
            {
                PlanetResourceTick++;
                if (PlanetResourceTick >= ticksToChangeResource)
                {
                    planet.tradingGoods[i].Quantity += planet.goodsProduction[i].Value;
                    PlanetResourceTick = 0;
                }

                if (planet.tradingGoods[i].Quantity < 0)
                {
                    planet.tradingGoods[i].Quantity = 0;

                    switch (planet.tradingGoods[i].Name)
                    {
                        case Goods.Iron:
                        case Goods.Copper:
                        case Goods.Coal:            
                            planet.planetCredits -= (int)(Random.Range(10, 1000) - Random.Range(1, 5) * planet.technologyModifier + Mathf.Sqrt(planet.population)); break;
                        case Goods.Food:            planet.population -= (long)(Random.Range(1, 3) * Mathf.Sqrt(planet.population)); break;
                        case Goods.Water:           planet.population -= (long)(Random.Range(4, 8) * Mathf.Sqrt(planet.population)); break;
                        case Goods.Medicine:        
                            planet.planetCredits -= (int)(Random.Range(10, 1000) - Random.Range(1, 5) * planet.technologyModifier + Mathf.Sqrt(planet.population));
                            planet.population -= (long)(Random.Range(1, 2) * Mathf.Sqrt(planet.population));
                            break;
                        case Goods.RareMetals:
                        case Goods.HighEnergyFuel:
                        case Goods.RawCrystals:
                            planet.planetCredits -= (int)(Random.Range(10, 1000) - Random.Range(1, 3) * planet.technologyModifier + Mathf.Sqrt(planet.population)); break;
                    }
                }
            }

            if(planet.planetCredits < 0 && planet.population > Mathf.Pow(10, 4))
            {
                planet.planetCredits += (int)(Random.Range(10, 1000) + Random.Range(2, 10) * planet.technologyModifier + Mathf.Sqrt(planet.population));
                planet.population -= (long)(Random.Range(2, 4) * Mathf.Sqrt(planet.population));
            }

            planet.population = (long)Mathf.Clamp(planet.population, Mathf.Pow(10, 4), 2 * Mathf.Pow(10, 12));
            planet.planetCredits = Mathf.Clamp(planet.planetCredits, 0, 2000000000);
        }
    }

    public void PricesUpdate()
    {
        foreach (PlanetStats planet in planetsData.planets)
        {
            //TradingGoods production and consumption
            foreach (TradingGoods tradingGood in planet.tradingGoods)
            {
                float priceModifier = 3 - ((float)tradingGood.Quantity / 1800);
                priceModifier = tradingGood.CommonPrice * priceModifier;
                priceModifier += Random.Range(0, 80);

                //Clamping values
                tradingGood.ActualPrice = (int)Mathf.Clamp(priceModifier, 10, 12000);
            }
        }
    }
}