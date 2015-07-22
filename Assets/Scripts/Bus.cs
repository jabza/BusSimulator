using UnityEngine;
using System.Collections.Generic;

public class Bus : MonoBehaviour 
{
    public  enum                EDoorState { OPEN = 0, SHUT, MOVING }

    public  EDoorState          Doors
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

    public  bool                Bell { get; private set; }

    public  Animator            frontDoors;
    public  AudioClip           bellSFX, doorSFX;

    private List<PassengerAI>   queue = new List<PassengerAI>();
    private List<Passenger>     passengers = new List<Passenger>();

    private Queue<string>       route = new Queue<string>();

    private AudioSource         audio;

	void Awake() 
    {
        audio = gameObject.AddComponent<AudioSource>();
        audio.clip = bellSFX;
	}

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
                if(p.Destination == route.Peek() && dist <= p.DistanceTillRing && !Bell)
                {
                    audio.clip = bellSFX;
                    audio.Play();
                    Bell = true;
                }
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

        if(Doors == EDoorState.SHUT || Doors == EDoorState.MOVING)
            target.x -= 0.5f;
        else if(Doors == EDoorState.OPEN)
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
        frontDoors.SetTrigger("toggle");
        audio.clip = doorSFX;
        audio.Play();
    }
}
