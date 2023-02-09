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
        FirebaseFirestore FireDatabase = FirebaseFirestore.DefaultInstance;
        if (isTestRun)
        {
            //SystemInfo.deviceUniqueIdentifier
            docRef = FireDatabase.Collection("Test_" + DeviceName).Document(timestamp + "_" + runId + "_" + Steps);
        }
        else
        {
            //SystemInfo.deviceUniqueIdentifier
            docRef = FireDatabase.Collection("Training_" + DeviceName).Document(timestamp + "_" + Steps);
        }
        /*Dictionary<string, int> data = new Dictionary<string, int> { };

        foreach (KeyValuePair<string, int> score in logsData)
        {
            data.Add(score.Key, score.Value);
        }*/

        docRef.SetAsync(logsData).ContinueWithOnMainThread(task => { Debug.Log("Data sent"); });
    }
}
