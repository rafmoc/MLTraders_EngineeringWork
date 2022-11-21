using Trade.PlanetsData;
using UnityEngine;
using UnityEngine.UI;

public class PlanetUI : MonoBehaviour
{
    [SerializeField] private int planetNumber;
    [SerializeField] private PlanetsStats planetsStats;

    [SerializeField] private Image IronBar;
    [SerializeField] private Image CopperBar;
    [SerializeField] private Image CoalBar;
    [SerializeField] private Image FoodBar;
    [SerializeField] private Image WaterBar;
    [SerializeField] private Image MedicineBar;
    [SerializeField] private Image RareMetalsBar;
    [SerializeField] private Image HighENergyFuelBar;
    [SerializeField] private Image RawCrystalsBar;

    private PlanetStats planetStats;
    //Max is 5000 but Hardly ever there is more than 3000.
    //So i set it like that for better visual look
    private float maxResources = 3000; 

    public void UpdatePlanetsUI()
    {
        if(planetStats == null)
        {
            planetStats = planetsStats.planets[planetNumber-1];
        }

        IronBar.fillAmount = planetStats.tradingGoods[0].Quantity / maxResources;
        CopperBar.fillAmount = planetStats.tradingGoods[1].Quantity / maxResources;
        CoalBar.fillAmount = planetStats.tradingGoods[2].Quantity / maxResources;
        FoodBar.fillAmount = planetStats.tradingGoods[3].Quantity / maxResources;
        WaterBar.fillAmount = planetStats.tradingGoods[4].Quantity / maxResources;
        MedicineBar.fillAmount = planetStats.tradingGoods[5].Quantity / maxResources;
        RareMetalsBar.fillAmount = planetStats.tradingGoods[6].Quantity / maxResources;
        HighENergyFuelBar.fillAmount = planetStats.tradingGoods[7].Quantity / maxResources;
        RawCrystalsBar.fillAmount = planetStats.tradingGoods[8].Quantity / maxResources;
    }
}
