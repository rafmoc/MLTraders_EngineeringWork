using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FirebaseHandler : MonoBehaviour
{
    public string DeviceName;
    public bool isTestRun;

    private DocumentReference docRef;
    private FirebaseFirestore FireDatabase;

    private string runType;
    private long timestamp = 0;
    private int runId = 0;

    public static FirebaseHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        timestamp = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
        runId++;
    }

    public void SendDataToFireBase(Dictionary<string, int> logsData, int Steps)
    {
        FireDatabase = FirebaseFirestore.DefaultInstance;

        runType = isTestRun ? "Test_" : "Training_";

        if (isTestRun)
        {
            //SystemInfo.deviceUniqueIdentifier
            docRef = FireDatabase.Collection(runType + DeviceName).Document(timestamp + "_" + runId + "_" + Steps);
        }
        else
        {
            //SystemInfo.deviceUniqueIdentifier
            docRef = FireDatabase.Collection(runType + DeviceName).Document(timestamp + "_" + Steps);
        }

        docRef.SetAsync(logsData).ContinueWithOnMainThread(task => { Debug.Log("Data sent"); });
        UpdateFireBaseCollectionsCollection();
    }

    private void UpdateFireBaseCollectionsCollection()
    {
        docRef = FireDatabase.Collection("_uniqCollectionsList").Document(runType + DeviceName);

        Dictionary<string, string> data = new();
        data.Add(timestamp.ToString(), runType + DeviceName);

        docRef.SetAsync(data).ContinueWithOnMainThread(task => { Debug.Log("Unique list updated"); });
    }
}