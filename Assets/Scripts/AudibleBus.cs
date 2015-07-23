using UnityEngine;
using System.ComponentModel;
using System.Collections;

public class AudibleBus : MonoBehaviour 
{
    public  AudioClip   doorSFX, bellSFX;

    private Bus         bus;
    private AudioSource audio;

	void Awake() 
    {
        bus = GetComponent<Bus>();
        bus.PropertyChanged += OnPropertyChanged;

        audio = gameObject.AddComponent<AudioSource>();
	}

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case "Door": audio.Stop(); audio.PlayOneShot(doorSFX); break;
            case "Bell": if(bus.Bell)  audio.PlayOneShot(bellSFX); break;
        }
    }
}
