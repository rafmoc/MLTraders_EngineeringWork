using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class MLLogs : MonoBehaviour
{
    // This is Singleton pattern
    // ------
    public static MLLogs Instance { get; private set; }

    [SerializeField]
    private int numberOfTrainingCells;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // ------

    [SerializeField]
    private int LogCounter = 0;
    private int headerCounter = 0;

    //---- For logs only
    public int[] products = new int[9];
    public int[] picked = new int[3];
    public int[] earnedCredits = new int[3]; //0 - lost, 1 - same, 2 - earned
    public int[] skipping = new int[2]; //selling / buying
    public int[] planetsTraveled = new int[6];
    public int failedBuy = 0;
    public int tradeBalance = 0;
    //----

    private int TimeForWriteToFIleCounter = 0;

    public void WriteLogs(MLTradersManager mLTrader)
    {
        for (int i = 0; i < 9; i++)
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

        TimeForWriteToFIleCounter++;
        if (TimeForWriteToFIleCounter >= numberOfTrainingCells) //Wait for data from all managers
        {
            TimeForWriteToFIleCounter = 0;
            WriteToFile();
            ClearVariables();
        }
    }

    private void ClearVariables()
    {
        //---- For chart only
        products = new int[9];
        picked = new int[3];
        earnedCredits = new int[3]; //0 - lost, 1 - same, 2 - earned
        skipping = new int[2]; //selling / buying
        planetsTraveled = new int[6];
        failedBuy = 0;
        tradeBalance = 0;
        //----
    }

    private void WriteToFile()
    {
        LogCounter++;
        int steps = 100000 * LogCounter;

        string header = "\n\nSteps Product0 Product1 Product2 Product3 Product4 Product5 Product6 Product7 Product8 " +
                "Picked0 Picked1 Picked2 " +
                "PlanetVisited0 PlanetVisited1 PlanetVisited2 PlanetVisited3 PlanetVisited4 PlanetVisited5 " +
                "LostCredits SameCredits EarnedCredits " +
                "FailedBuy " +
                "TradeBalance " +
                "SkippingSell SkippingBuy";

        string data = steps + " ";
        for (int i = 0; i < 9; i++) { data += products[i] + " "; }
        for (int i = 0; i < 3; i++) { data += picked[i] + " "; }
        for (int i = 0; i < 6; i++) { data += planetsTraveled[i] + " "; }
        for (int i = 0; i < 3; i++) { data += earnedCredits[i] + " "; }
        data += failedBuy + " ";
        data += tradeBalance + " ";
        for (int i = 0; i < 2; i++) { data += skipping[i] + " "; }

        string destination = Path.Combine(Application.persistentDataPath, "MLTraderLog.txt");
        if (!File.Exists(destination)) { using (File.Create(destination)) ; }

        using (StreamWriter writeText = new(destination, true))
        {
            if (headerCounter % 10 == 0)
            {
                writeText.WriteLine(header);
            }
            writeText.WriteLine(data);
        }

        headerCounter++;
    }
}
