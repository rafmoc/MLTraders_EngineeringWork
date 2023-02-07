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

    public void Start()
    {
        Dictionary<string, int> testDic = new();
        testDic.Add("Test", 1);
        SendDataToFireBase(testDic, 10);
    }

    public void SendDataToFireBase(Dictionary<string, int> scores, int Steps)
    {
        FirebaseFirestore FireDatabase = FirebaseFirestore.DefaultInstance;
        if (isTestRun)
        {
            docRef = FireDatabase.Collection("Test" + DeviceName).Document();
        }
        else
        {
            docRef = FireDatabase.Collection("Training" + DeviceName).Document();
        }
        Dictionary<string, int> data = new Dictionary<string, int> { };

        foreach (KeyValuePair<string, int> score in scores)
        {
            data.Add(score.Key, score.Value);
        }

        docRef.SetAsync(data).ContinueWithOnMainThread(task => { Debug.Log("Data sent"); });
    }
}
