using UnityEngine;
using System.Collections.Generic;

public class Passenger
{
    public enum     EPayment { CARD = 0, CASH };

    public EPayment Payment { get; private set; }
    public string   Destination { get; private set; }
    public float    DistanceTillRing { get; private set; }

    public Passenger(List<string> destinations)
    {
        Payment = (EPayment)Random.Range(0, 1);
        Destination = destinations[Random.Range(0, destinations.Count)];
        DistanceTillRing = Random.Range(16, 32);
    }
}
