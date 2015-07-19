using UnityEngine;
using System.Collections.Generic;

public class Passenger
{
    public  enum        EPayment { CARD = 0, CASH };

    private EPayment    payment;
    private int         destinationID;
    private float       distanceTillRing;

    public Passenger(int[] stops)
    {
        payment = (EPayment)Random.Range(0, 1);
        destinationID = stops[Random.Range(0, stops.Length)];
        distanceTillRing = Random.Range(4, 10);
    }

    public EPayment GetPaymentMethod()
    {
        return payment;
    }

    public int GetDestinationID()
    {
        return destinationID;
    }

    public float GetDistanceTillRing()
    {
        return distanceTillRing;
    }
}
