using UnityEngine;
using System.ComponentModel;
using System.Collections;

public class AudibleVehicle : MonoBehaviour 
{
    public  AudioClip   engineSFX, breakSFX, breakReleaseSFX, indicatorSFX;

    private Vehicle     vehicle;
    private AudioSource engineAudio, breakAudio, indicatorAudio;

    private bool        breakPlayed = false;

	void Awake() 
    {
        vehicle = GetComponent<Vehicle>();
        vehicle.PropertyChanged += OnPropertyChanged;

        engineAudio = gameObject.AddComponent<AudioSource>();
        engineAudio.loop = true;
        engineAudio.clip = engineSFX;
        engineAudio.volume = 0.15f;

        breakAudio = gameObject.AddComponent<AudioSource>();
        breakAudio.clip = breakSFX;

        indicatorAudio = gameObject.AddComponent<AudioSource>();
        indicatorAudio.loop = true;
        indicatorAudio.clip = indicatorSFX;
	}

    void Start()
    {
        engineAudio.Play();
    }

	void Update() 
    {
        float pitch = (vehicle.Speed / vehicle.PeakSpeed) + 1;

        if(float.IsNaN(pitch) || pitch > 2 || pitch < 0)
            engineAudio.pitch = 1;
        else
            engineAudio.pitch = pitch;
	}

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case "Break": UpdateBreak(); break;
            case "Indicator": UpdateIndicator(); break;
        }
    }

    void UpdateBreak()
    {
        if(vehicle.Break)
        {
            if(vehicle.Speed >= 10)
            {
                breakAudio.volume = (vehicle.Speed / vehicle.PeakSpeed) * 0.25f;
                breakAudio.Play();
                breakPlayed = true;
            }
        }
        else if(breakPlayed)
        {
            breakAudio.Stop();

            if(vehicle.Speed <= 1)
            {
                breakAudio.volume = 1;
                breakAudio.PlayOneShot(breakReleaseSFX);
            }

            breakPlayed = false;
        }
    }

    void UpdateIndicator()
    {
        if(vehicle.Indicator != Vehicle.EIndicator.OFF)
            indicatorAudio.Play();
        else
            indicatorAudio.Stop();
    }
}
