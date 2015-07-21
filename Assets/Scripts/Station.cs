using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Station : MonoBehaviour
{
    public static Dictionary<string, Station>   Stations = new Dictionary<string, Station>();

    public string                               name;
    public Text                                 sign;
    public GameObject                           passengerPrefab;

	void Awake() 
    {
        sign.text = name;
        Stations.Add(name, this);
	}

    void Start()
    {
        int passengerCount = Random.Range(0, 5);

        List<string> destinations = new List<string>(Stations.Keys);
        destinations.Remove(name);

        for (int i = 0; i < passengerCount; i++)
        {
            GameObject pai = Instantiate(passengerPrefab, transform.position, Quaternion.identity) as GameObject;
            pai.transform.parent = transform;

            pai.transform.Translate(Random.Range(-1f, 0f), Random.Range(-1f, 1f), 0, Space.Self);
            pai.GetComponent<PassengerAI>().Passenger = new Passenger(destinations);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Bus")
        {
            foreach(PassengerAI p in transform.GetComponentsInChildren<PassengerAI>())
            {
                p.OnBusEnter(other.GetComponent<Bus>());
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Bus")
        {
            foreach(PassengerAI p in transform.GetComponentsInChildren<PassengerAI>())
            {
                p.OnBusExit(other.GetComponent<Bus>());
            }
        }
    }
}
