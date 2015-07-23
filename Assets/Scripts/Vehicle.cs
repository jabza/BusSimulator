using UnityEngine;
using System.ComponentModel;
using System.Collections;

public class Vehicle : MonoBehaviour, INotifyPropertyChanged
{
    public  enum            EGear { REVERSE = 0, NEUTRAL, FIRST, SECOND };
    public  enum            EIndicator { OFF = 0, LEFT, RIGHT, HAZARD };

    public  event           PropertyChangedEventHandler PropertyChanged;

    public  float           Accelerator
    {
        get { return force.relativeForce.y / gears[(int)currentGear]; }
        set 
        {
            force.relativeForce = new Vector2(0, gears[(int)currentGear] * value);
        }
    }

    public  float           Steering
    {
        get { return tyres[0].transform.localEulerAngles.z / turnAngle; }
        set
        {
            tyres[0].transform.localEulerAngles = new Vector3(0, 0, turnAngle * value);
            tyres[1].transform.localEulerAngles = new Vector3(0, 0, turnAngle * value);
        }
    }

    public  bool            Break
    {
        get { return (chassis.drag == breakDrag); }
        set 
        {
            bool oldValue = Break;

            if(value)
                chassis.drag = breakDrag;
            else
                chassis.drag = drag;

            if(oldValue != Break)
                NotifyPropertyChanged("Break");
        }
    }

    public  EIndicator      Indicator
    {
        get { return currentIndicator; }
        set
        {
            EIndicator oldValue = Indicator;

            if(value == Indicator)
                currentIndicator = EIndicator.OFF;
            else
                currentIndicator = value;

            if(oldValue != Indicator)
                NotifyPropertyChanged("Indicator");
        }
    }

    public  float           Speed { get { return MpsToKph(GetLocalVelocity(chassis).y); } }
    public  float           PeakSpeed { get; private set; }
    public  float           DropSpeed { get; private set; }

    public  EGear           currentGear = EGear.NEUTRAL;
    public  int[]           gears;
    public  Rigidbody2D[]   tyres;
    public  float           turnAngle, breakDrag;

    private float           drag;
    private Rigidbody2D     chassis;
    private ConstantForce2D force;
    private EIndicator      currentIndicator = EIndicator.OFF;

    void Awake()
    {
        chassis = GetComponent<Rigidbody2D>();
        force = GetComponent<ConstantForce2D>();

        drag = chassis.drag;

        ChangeGear((int)currentGear);
    }

    void Update()
    {
        UpdateGearBox();
    }

    void FixedUpdate()
    {
        foreach(Rigidbody2D tyre in tyres)
        {
            KillOrthogonalVelocity(tyre);
        }
    }

    void UpdateGearBox()
    {
        if(currentGear != EGear.REVERSE)
        {
            if(Speed >= PeakSpeed || (currentGear == EGear.NEUTRAL && Input.GetAxis("Accelerator") > 0))
                ChangeGear((int)currentGear + 1);
            else if((int)currentGear >= 2 && Speed <= DropSpeed)
                ChangeGear((int)currentGear - 1);
        }
    }

    void ChangeGear(int newGear)
    {
        if(newGear != (int)EGear.REVERSE)
        {
            PeakSpeed = MpsToKph(((gears[newGear] / chassis.drag) - Time.fixedDeltaTime * gears[newGear]) / chassis.mass);
            DropSpeed = MpsToKph(((gears[newGear - 1] / chassis.drag) - Time.fixedDeltaTime * gears[newGear - 1]) / chassis.mass);
        }

        currentGear = (EGear)newGear;
    }

    void KillOrthogonalVelocity(Rigidbody2D tyre)
    {
        Vector2 relVel = GetLocalVelocity(tyre);
        relVel.y = 0f;

        tyre.AddRelativeForce(-relVel * tyre.mass, ForceMode2D.Impulse);
    }

    Vector2 GetLocalVelocity(Rigidbody2D body)
    {
        return body.transform.InverseTransformDirection(body.velocity);
    }

    float MpsToKph(float mps)
    {
        return (mps * 60 * 60) / 1000f;
    }

    void NotifyPropertyChanged(string propName = "")
    {
        if(PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }
}
