using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimplifiedInteractionsML : MonoBehaviour
{
    public GameObject Tradeable;
    //public LineControler lineControler;
    //public DrawLine drawLine;

    private List<Transform> tradableTransforms = new();

    public void MoveToPlanet(Transform trader, int planet, float flySpeed)
    {
        if(tradableTransforms.Count == 0)
        {
            foreach (Transform child in Tradeable.transform)
            {
                tradableTransforms.Add(child);
            }
        }
        StartCoroutine(MoveTo(trader, tradableTransforms[planet], flySpeed));
    }

    private IEnumerator MoveTo( Transform trader, Transform destinationTransform, float flySpeed)
    {
        Vector3 destinationPosition = destinationTransform.position;
        Vector3 currentPosition = trader.position;
        float t = 0f;
        while (t < 1)
        {
            t += 1f; //Time.deltaTime * flySpeed;
            trader.position = Vector3.Lerp(currentPosition, destinationPosition, t);
            //drawLine.points[0] = trader.position;
            //drawLine.points[1] = destinationPosition;
            //lineControler.DrawLines();
            yield return null;
        }
    }
}
