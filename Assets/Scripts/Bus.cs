using UnityEngine;
using System.ComponentModel;
using System.Collections.Generic;

public class Bus : MonoBehaviour, INotifyPropertyChanged
{
    public  enum                EDoorState { OPEN = 0, SHUT, MOVING }

    public  event               PropertyChangedEventHandler PropertyChanged;

    public  bool                Bell
    {
        get { return bell; }
        private set
        {
            if(value != bell)
            {
                bell = value;
                NotifyPropertyChanged("Bell");
            }
        }
    }

    public  EDoorState          Door
    {
        get
        {
            AnimatorStateInfo state = frontDoors.GetCurrentAnimatorStateInfo(0);

            if(state.IsName("Open") && state.normalizedTime >= 1.2f)
                return EDoorState.OPEN;
            else if(state.IsName("Idle"))
                return EDoorState.SHUT;
            else
                return EDoorState.MOVING;
        }
    }

    public  Animator            frontDoors;

    private List<PassengerAI>   queue = new List<PassengerAI>();
    private List<Passenger>     passengers = new List<Passenger>();

    private Queue<string>       route = new Queue<string>();
    private bool                bell = false;


    void Start()
    {
        route.Enqueue("Alpha");
        route.Enqueue("Beta");
    }

	void Update() 
    {
        if(route.Count > 0)
        {
            float dist = Vector2.Distance(transform.position, Station.Stations[route.Peek()].transform.position);

            foreach(Passenger p in passengers)
            {
                if(p.Destination == route.Peek() && dist <= p.DistanceTillRing)
                    Bell = true;
            }

            ProcessQueue();
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Passenger")
        {
            PassengerAI pai = other.GetComponent<PassengerAI>();
            if(queue[0] == pai)
                ProcessPassenger(pai);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Station")
        {
            if(route.Count > 0 && other.GetComponent<Station>().name == route.Peek())
            {
                route.Dequeue();

                if(route.Count == 0)
                    Debug.Log("Route Complete.");
                else
                    Debug.Log("Next Stop: " + route.Peek());
            }
        }
    }

    void ProcessPassenger(PassengerAI pai)
    {
        queue.Remove(pai);
        passengers.Add(pai.Passenger);
        Destroy(pai.gameObject, 0);
    }

    void ProcessQueue()
    {
        Vector2 target = frontDoors.transform.localPosition;

        if(Door == EDoorState.SHUT || Door == EDoorState.MOVING)
            target.x -= 0.5f;
        else if(Door == EDoorState.OPEN)
            target.x += 0.5f;

        foreach(PassengerAI pai in queue)
        {
            pai.Target = frontDoors.transform.TransformPoint(target);
            target.x -= 0.5f;
        }
    }

    public void Queue(PassengerAI pai)
    {
        queue.Add(pai);
    }

    public void LeaveQueue(PassengerAI pai)
    {
        queue.Remove(pai);
    }

    public void ToggleDoors()
    {
        frontDoors.SetTrigger("Toggle");
        NotifyPropertyChanged("Door");
    }

    void NotifyPropertyChanged(string propName = "")
    {
        if(PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }
}
