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

    public void UpdatePlanetsUI()
    {
        
    }
}
