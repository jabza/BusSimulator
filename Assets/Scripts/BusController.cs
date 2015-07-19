using UnityEngine;
using System.Collections;

public class BusController : MonoBehaviour 
{
    public  Animator            frontDoors;
    public  AudioClip           doorSFX;

    private Vehicle             vehicle;
    private AudioSource         audio;

    private bool doorsOpen =    false;

	void Awake() 
    {
        vehicle = GetComponent<Vehicle>();
        audio = gameObject.AddComponent<AudioSource>();
	}

    void Update()
    {
        UpdateControls();
    }

    void UpdateControls()
    {
        vehicle.Accelerator = Input.GetAxis("Accelerator");
        vehicle.Steering = Input.GetAxis("Steering");
        vehicle.Break = Input.GetButton("Break");

        if(Input.GetButtonUp("IndicateLeft"))
        {
            if(vehicle.Indicator == Vehicle.EIndicator.OFF)
                vehicle.Indicator = Vehicle.EIndicator.LEFT;
            else
                vehicle.Indicator = Vehicle.EIndicator.OFF;
        }
        else if (Input.GetButtonUp("IndicateRight"))
        {
            if(vehicle.Indicator == Vehicle.EIndicator.OFF)
                vehicle.Indicator = Vehicle.EIndicator.RIGHT;
            else
                vehicle.Indicator = Vehicle.EIndicator.OFF;
        }

        if(Input.GetButtonUp("ToggleDoor"))
            ToggleDoors();
    }

    void ToggleDoors()
    {
        if(doorsOpen)
        {
            frontDoors.SetTrigger("close");
            audio.clip = doorSFX;
            audio.Play();
        }
        else
        {
            frontDoors.SetTrigger("open");
            audio.clip = doorSFX;
            audio.Play();
        }

        doorsOpen = !doorsOpen;
    }
}