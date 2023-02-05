using System.Collections.Generic;
using UnityEngine;

namespace Trade.TradeUtils
{
    /*
    This class is able to make my custom Random Ragne with Chance. 
    To make it work properly remember to have chances sum to 100.
    example on this planetType below:
    - new TradeRandom.RandomWithChance(0, 0, 60) - it means that there is 60% chance that random value will be in range 0 to 0
    - new TradeRandom.RandomWithChance(1, 2, 30) - it means that there is 30% chance that random value will be in range 1 to 2
    - new TradeRandom.RandomWithChance(3, 3, 10) - it means that there is 10% chance that random value will be in range 3 to 3
    So you change the chance of randomness in specific set

    planets[planetsCount].planetType = (PlanetType)TradeRandom.RandomRangeChance(
                    new TradeRandom.RandomWithChance(0, 0, 60), new TradeRandom.RandomWithChance(1, 2, 30), new TradeRandom.RandomWithChance(3, 3, 10));
    */
    public static class TradeRandom
    {
        public struct RandomWithChance
        {
            public int min;
            public int max;
            public int percent;
            public RandomWithChance(int minRandom, int maxRandom, int percentChance)
            {
                this.min = minRandom;
                this.max = maxRandom;
                this.percent = percentChance;
            }
        }

        public static int RandomRangeChance(params RandomWithChance[] randoms)
        {
            int precentChance = Random.Range(0, 101);
            int sum = 0;

            foreach (RandomWithChance selection in randoms)
            {
                sum += selection.percent;
                if (precentChance <= sum)
                {
                    return Random.Range(selection.min, selection.max + 1);
                }
            }
            Debug.LogWarning("Wrong Random Sum");
            return -1;
        }
    }

    static class List
    {
        public static List<T> AddAll<T>(this List<T> parentList, List<T> toAddList)
        {
            for (int i = 0; i < toAddList.Count; i++)
            {
                parentList.Add(toAddList[i]);
            }
            return parentList;
        }
    }
}