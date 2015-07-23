using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;
using System.Collections;

public class BusController : MonoBehaviour 
{
    public  float       wheelRevolutions, degreesPerKM;
    public  Transform   wheelUI, speedUI;
    public  Button      doorsUI;
    public  Image       bellUI;

    public  Sprite      bellDim, bellLit;
    public  Animator    indicatorLeft, indicatorRight;

    private Vehicle     vehicle;
    private Bus         bus;

	void Awake() 
    {
        vehicle = GetComponent<Vehicle>();
        bus = GetComponent<Bus>();

        vehicle.PropertyChanged += OnPropertyChanged;
        bus.PropertyChanged += OnPropertyChanged;

        doorsUI.onClick.AddListener(() => bus.ToggleDoors());
	}

    void Update()
    {
        UpdateControls();
        UpdateUI();
    }

    void UpdateControls()
    {
        vehicle.Accelerator = Input.GetAxis("Accelerator");
        vehicle.Steering = Input.GetAxis("Steering");
        vehicle.Break = Input.GetButton("Break");

        if(Input.GetButtonUp("IndicateLeft"))
            vehicle.Indicator = Vehicle.EIndicator.LEFT;
        else if(Input.GetButtonUp("IndicateRight"))
            vehicle.Indicator = Vehicle.EIndicator.RIGHT;

        if(Input.GetButtonUp("ToggleDoor"))
            bus.ToggleDoors();
    }

    void UpdateUI()
    {
        wheelUI.localRotation = Quaternion.Euler(0, 0, vehicle.Steering * (wheelRevolutions * 360));
        speedUI.localRotation = Quaternion.Euler(0, 0, (-vehicle.Speed * degreesPerKM) + 136);
    }

    void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case "Bell": UpdateBell(); break;
            case "Indicator": UpdateIndicator(); break;
        }
    }

    void UpdateBell()
    {
        if(bus.Bell)
            bellUI.sprite = bellLit;
        else
            bellUI.sprite = bellDim;
    }

    void UpdateIndicator()
    {
        if(indicatorLeft.GetCurrentAnimatorStateInfo(0).IsName("Indicating") && (vehicle.Indicator == Vehicle.EIndicator.RIGHT || vehicle.Indicator == Vehicle.EIndicator.OFF))
            indicatorLeft.SetTrigger("Stop");
        if(indicatorRight.GetCurrentAnimatorStateInfo(0).IsName("Indicating") && (vehicle.Indicator == Vehicle.EIndicator.LEFT || vehicle.Indicator == Vehicle.EIndicator.OFF))
            indicatorRight.SetTrigger("Stop");

        if(vehicle.Indicator == Vehicle.EIndicator.LEFT)
            indicatorLeft.SetTrigger("Start");
        else if(vehicle.Indicator == Vehicle.EIndicator.RIGHT)
            indicatorRight.SetTrigger("Start");
    }
}