using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BusController : MonoBehaviour 
{
    public  float       degreesPerRevolution, degreesPerKM;
    public  Transform   wheelUI;
    public  Transform   speedUI;
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
        {
            vehicle.Indicator = Vehicle.EIndicator.LEFT;
            indicatorLeft.SetTrigger("Indicate");

            if(indicatorRight.GetCurrentAnimatorStateInfo(0).IsName("Indicating"))
                indicatorRight.SetTrigger("Indicate");
        }
        else if(Input.GetButtonUp("IndicateRight"))
        {
            vehicle.Indicator = Vehicle.EIndicator.RIGHT;
            indicatorRight.SetTrigger("Indicate");

            if(indicatorLeft.GetCurrentAnimatorStateInfo(0).IsName("Indicating"))
                indicatorLeft.SetTrigger("Indicate");
        }

        if(Input.GetButtonUp("ToggleDoor"))
            bus.ToggleDoors();
    }

    void UpdateUI()
    {
        wheelUI.localRotation = Quaternion.Euler(0, 0, (vehicle.Steering / degreesPerRevolution)*360);
        speedUI.localRotation = Quaternion.Euler(0, 0, ((-vehicle.Speed * degreesPerKM) + 136));

        if(bus.Bell)
            bellUI.sprite = bellLit;
        else
            bellUI.sprite = bellDim;
    }
}