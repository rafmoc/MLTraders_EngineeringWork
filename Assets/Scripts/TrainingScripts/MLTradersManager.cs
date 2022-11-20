using System.Collections;
using System.Collections.Generic;
using Trade.PlanetsData;
using Trade.PlanetsMechanics;
using UnityEngine;

public class MLTradersManager : MonoBehaviour
{
    [SerializeField] private GameObject MLTradersParent;
    [SerializeField] private GameObject planetUIsParent;
    [SerializeField] private InitializeNewPlanetsData PlanetsData;
    [SerializeField] private SpaceTickSystem mLSpaceTickSystem;
    [SerializeField] private int numberOfTrainingCells;
    [SerializeField] private int numberOfTraderInTrainingCell;

    private List<MLTrader> MLTraders = new();

    private int MLAgentesActionCounter = 0;
    private int MLAgentesLogsCounter = 0;

    //---- For logs only
    public int[] products = new int[9];
    public int[] picked = new int[3];
    public int[] earnedCredits = new int[3]; //0 - lost, 1 - same, 2 - earned
    public int[] skipping = new int[2]; //selling / buying
    public int[] planetsTraveled = new int[6];
    public int failedBuy = 0;
    public int tradeBalance = 0;
    //----

    private void Awake()
    {
        //On awake we add all MLTraders that this manager should manage to its list of MLTraders.
        foreach (Transform child in MLTradersParent.transform)
        {
            MLTraders.Add(child.GetComponent<MLTrader>());
        }
    }

    private void Start()
    {
        foreach (MLTrader mLTrader in MLTraders)
        {
            foreach (Goods good in System.Enum.GetValues(typeof(Goods)))
            {
                // We fill tradingGoods list of our MLTraders with all existing goods types.
                // I do it here insted of in MLTrader.cs just to be 100% sure
                // that tradingGoods list will be filled before any agent action (observation).
                mLTrader.tradingGoods.Add(new TradingGoods(good, 0, 0)); 
            }
        }
        PlanetsData.PlanetsData(); // Setting up planets
        StartCoroutine(MLAction()); // Starting Agents Loop
    }

    private void WriteLogs()
    {
        //---- For logs only
        products = new int[9];
        picked = new int[3];
        earnedCredits = new int[3]; //0 - lost, 1 - same, 2 - earned
        skipping = new int[2]; //selling / buying
        planetsTraveled = new int[6];
        failedBuy = 0;
        tradeBalance = 0;
        //----
        foreach (MLTrader mLTrader in MLTraders)
        {
            for(int i = 0; i < 9; i++)
            {
                products[i] += mLTrader.products[i];
                mLTrader.products[i] = 0;
            }
            for (int i = 0; i < 3; i++)
            {
                picked[i] += mLTrader.picked[i];
                earnedCredits[i] += mLTrader.earnedCredits[i];
                mLTrader.picked[i] = 0;
                mLTrader.earnedCredits[i] = 0;
            }
            for (int i = 0; i < 6; i++)
            {
                planetsTraveled[i] += mLTrader.planetsTraveled[i];
                mLTrader.planetsTraveled[i] = 0;
            }
            for (int i = 0; i < 2; i++)
            {
                skipping[i] += mLTrader.skipping[i];
                mLTrader.skipping[i] = 0;
            }
            failedBuy += mLTrader.failedBuy;
            mLTrader.failedBuy = 0;
            tradeBalance += mLTrader.tradeBalance;
            mLTrader.tradeBalance = 0;
        }

        MLLogs.Instance.WriteLogs(this);
    }

    IEnumerator MLAction()
    {
        // Refreshing the whole environment from time to time.
        // I'm setting this to make referesh every 5000 steps.
        // So it will be 1000 divided by number of training cells working at once because all of them are making steps.
        MLAgentesActionCounter++;
        if(MLAgentesActionCounter >= 1000 / numberOfTraderInTrainingCell) 
        {
            MLAgentesActionCounter = 0;
            PlanetsData.PlanetsData(); // Generate new, fresh planets data
        }

        // Making logs from time to time
        // I want to have logs 1 time per 100 000 steps.
        MLAgentesLogsCounter++;
        if (MLAgentesLogsCounter >= (100000 / numberOfTraderInTrainingCell ) / numberOfTrainingCells)
        {
            MLAgentesLogsCounter = 0;
            WriteLogs();
        }

        yield return new WaitForSeconds(0.001f); // Giving more than enaugh time for changes.

        foreach (MLTrader mLTrader in MLTraders) // For each Trader this Managrer have:
        {
            // Request decision means going throug steps: observation -> (masking) -> decision -> action -> reward
            mLTrader.RequestDecision();
            mLSpaceTickSystem.PricesUpdate(); // Updating prices after every agent as they can be on the same planet
            yield return new WaitForSeconds(0.0001f); // Giving more than enaugh time for prices update.
        }
        mLSpaceTickSystem.MakeTick(); // Global planets tick
        foreach (Transform child in planetUIsParent.transform)//Update viusals
        {
            child.GetComponent<PlanetUI>().UpdatePlanetsUI();
        }
        StartCoroutine(MLAction()); // Starting this coroutine again
    }
}
