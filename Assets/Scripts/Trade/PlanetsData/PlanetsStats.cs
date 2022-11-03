using System;
using System.Collections.Generic;
using System.IO;
using Trade.PlanetsData;
using UnityEngine;


namespace Trade.PlanetsData
{
    [Serializable]
    public class PlanetsStats : MonoBehaviour
    {
        public List<PlanetStats> planets = new List<PlanetStats>();

        //Saving planets infotmations to json file called Planets.dat in unity persistentDataPath
        public void SaveData()
        {
            string destination = Path.Combine(Application.persistentDataPath, "Planets.dat");

            if (!File.Exists(destination)) { using (File.Create(destination)) { }; }

            string jsonData = JsonUtility.ToJson(this, true);
            using (StreamWriter writetext = new StreamWriter(destination))
            {
                writetext.WriteLine(jsonData);
            }
        }

        //Loading planets infotmations to json file called Planets.dat in unity persistentDataPath
        public void LoadData()
        {
            string destination = Path.Combine(Application.persistentDataPath, "Planets.dat");
            string jsonData;

            if (File.Exists(destination))
            {
                using (StreamReader readtext = new StreamReader(destination))
                {
                    jsonData = readtext.ReadToEnd();
                }
                JsonUtility.FromJsonOverwrite(jsonData, this);
            }
        }
    }
}