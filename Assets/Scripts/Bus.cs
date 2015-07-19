using UnityEngine;
using System.Collections.Generic;

public class Bus : MonoBehaviour 
{
    public  AudioClip       bellSFX;

    private List<Passenger> passengers = new List<Passenger>();
    private bool            bellRang = false;
    private int             nextDestination;

    private AudioSource     audio;

	void Awake() 
    {
        audio = GetComponent<AudioSource>();
        audio.clip = bellSFX;
	}
	
	void Update() 
    {
        foreach(Passenger p in passengers)
        {
            if(p.GetDestinationID() == nextDestination && !bellRang)
            {
                audio.Play();
                bellRang = true;
            }
        }
	}

    public void LoadPassenger(Passenger p)
    {

    }

    public void UnloadPassengers(int destination, Vector2 exitPoistion)
    {
        foreach(Passenger p in passengers)
        {
            if(p.GetDestinationID() == destination)
            {

            }
        }


    }
}
