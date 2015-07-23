using UnityEngine;
using System.Collections;

public class PassengerAI : MonoBehaviour 
{
    public  enum        EState { WAITING = 0, BOARDING, ALIGHTING }

    public  EState      State { get; private set; }
    public  Vector2     Target { get; set; }
    public  Passenger   Passenger { get; set; }

    private Vector2     origin;
    private Bus         targetBus;

    void Start()
    {
        State = EState.WAITING;
        origin = transform.position;
    }
	
	void Update() 
    {
        Vector2 direction = Vector2.zero;

        if (State == EState.WAITING)
        {
            if(Vector2.Distance(transform.position, origin) >= 0.1f)
                direction = GetDirection(origin);
        }
        else if (State == EState.BOARDING)
        {
            if(targetBus.GetComponent<Vehicle>().Speed < 1)
            {
                if(Vector2.Distance(transform.position, Target) >= 0.1f)
                    direction = GetDirection(Target);
            }
            else if(Vector2.Distance(transform.position, origin) >= 0.1f)
                direction = GetDirection(origin);
        }

        transform.Translate(direction * Time.deltaTime, Space.World);
	}

    Vector2 GetDirection(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;

        if(float.IsNaN(direction.x) || float.IsNaN(direction.y))
            return Vector2.zero;

        return direction;
    }

    public void OnBusEnter(Bus bus)
    {
        if(State == EState.WAITING)
        {
            State = EState.BOARDING;
            targetBus = bus;
            bus.Queue(this);
        }
    }

    public void OnBusExit(Bus bus)
    {
        State = EState.WAITING;
        bus.LeaveQueue(this);
    }
}
