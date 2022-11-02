using System;
using System.Collections.Generic;
using Trade.PlanetsData;
using UnityEngine;


namespace Trade.PlanetsData
{
    [Serializable]
    public class PlanetsStats : MonoBehaviour
    {
        public List<PlanetStats> planets = new List<PlanetStats>();
    }
}