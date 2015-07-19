using UnityEngine;
using System.Collections;

public class Vehicle : MonoBehaviour
{
    public  enum            EGear { REVERSE = 0, NEUTRAL, FIRST, SECOND };
    public  enum            EIndicator { OFF = 0, LEFT, RIGHT, HAZARD };

    public  float           Accelerator
    {
        set { force.relativeForce = new Vector2(0, gears[(int)currentGear] * value); }
    }

    public  float           Steering
    {
        get { return wheels[0].transform.localEulerAngles.z; }
        set
        {
            wheels[0].transform.localEulerAngles = new Vector3(0, 0, turnAngle * value);
            wheels[1].transform.localEulerAngles = new Vector3(0, 0, turnAngle * value);
        }
    }

    public  bool            Break
    {
        get { return (chassis.drag == breakDrag); }
        set 
        {
            if(value && !Break)
            {
                chassis.drag = breakDrag;

                if(Velocity.y >= 3)
                {
                    breakSFXPlayed = true;
                    breakAudio.Play();
                }
            }
            else if(!value)
            {
                chassis.drag = drag;

                if(breakAudio.isPlaying)
                    breakAudio.Stop();

                if(breakSFXPlayed && Velocity.y <= 0.5f)
                    miscAudio.PlayOneShot(hissSFX);

                breakSFXPlayed = false;
            }
        }
    }

    public  EIndicator      Indicator
    {
        get { return currentIndicator; }
        set
        {
            if(value == EIndicator.OFF)
                miscAudio.Stop();
            else
                miscAudio.Play();

            currentIndicator = value;
        }
    }

    public  Vector2         Velocity
    {
        get { return GetLocalVelocity(chassis); }
    }

    public  Vector2         enginePosition;
    public  EGear           currentGear = EGear.NEUTRAL;
    public  int[]           gears;

    public  Rigidbody2D[]   wheels;
    public  float           turnAngle;
    public  float           breakDrag;

    public  AudioClip       engineSFX, breakSFX, hissSFX, indicatorSFX;

    private Rigidbody2D     chassis;
    private ConstantForce2D force;
    private AudioSource     engineAudio, breakAudio, miscAudio;

    private EIndicator      currentIndicator = EIndicator.OFF;

    private float           drag;
    private float           dropVelocity;
    private float           peakVelocity;
    private bool            breakSFXPlayed = false;

    void Awake()
    {
        chassis = GetComponent<Rigidbody2D>();
        force = GetComponent<ConstantForce2D>();

        SetupAudio();
        SetupPhysics();

        ChangeGear((int)currentGear);
    }

    void SetupAudio()
    {
        engineAudio = gameObject.AddComponent<AudioSource>();
        breakAudio = gameObject.AddComponent<AudioSource>();
        miscAudio = gameObject.AddComponent<AudioSource>();

        engineAudio.loop = true;
        engineAudio.clip = engineSFX;
        engineAudio.volume = 0.15f;

        breakAudio.clip = breakSFX;

        miscAudio.loop = true;
        miscAudio.clip = indicatorSFX;
        miscAudio.volume = 0.5f;
    }

    void SetupPhysics()
    {
        drag = chassis.drag;
        chassis.centerOfMass = enginePosition;
    }

    void Start()
    {
        engineAudio.Play();
    }

    void Update()
    {
        UpdateGearBox();
        UpdateAudio();
    }

    void FixedUpdate()
    {
        foreach(Rigidbody2D wheel in wheels)
        {
            KillOrthogonalVelocity(wheel);
        }
    }

    void UpdateGearBox()
    {
        if(currentGear != EGear.REVERSE)
        {
            if(Velocity.y >= peakVelocity || (currentGear == EGear.NEUTRAL && Input.GetAxis("Accelerator") > 0))
                ChangeGear((int)currentGear + 1);
            else if((int)currentGear >= 2 && Velocity.y <= dropVelocity)
                ChangeGear((int)currentGear - 1);
        }
    }

    void UpdateAudio()
    {
        float pitch = (Velocity.y / peakVelocity) + 1;

        if(pitch != pitch || pitch > 2 || pitch < 0)
            engineAudio.pitch = 1;
        else
            engineAudio.pitch = pitch;

        breakAudio.volume = (Velocity.y / peakVelocity) * 0.25f;
    }

    void ChangeGear(int newGear)
    {
        if(newGear != (int)EGear.REVERSE)
        {
            peakVelocity = ((gears[newGear] / chassis.drag) - Time.fixedDeltaTime * gears[newGear]) / chassis.mass;
            dropVelocity = ((gears[newGear - 1] / chassis.drag) - Time.fixedDeltaTime * gears[newGear - 1]) / chassis.mass;
        }

        currentGear = (EGear)newGear;
    }

    void KillOrthogonalVelocity(Rigidbody2D wheel)
    {
        Vector2 relVel = GetLocalVelocity(wheel);
        relVel.y = 0f;

        wheel.AddRelativeForce(-relVel * wheel.mass, ForceMode2D.Impulse);
        wheel.angularVelocity = 0;
    }

    Vector2 GetLocalVelocity(Rigidbody2D body)
    {
        return body.transform.InverseTransformDirection(body.velocity);
    }
}
