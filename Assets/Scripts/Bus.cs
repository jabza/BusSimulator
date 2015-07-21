using UnityEngine;
using System.Collections.Generic;

public class Bus : MonoBehaviour 
{
    public  bool                 Doors
    { 
        get 
        {
            AnimatorStateInfo state = frontDoors.GetCurrentAnimatorStateInfo(0);
            return (state.IsName("Open") && state.normalizedTime >= 1.2f); 
        } 
    }

    public  bool                Bell { get; private set; }

    public  Animator            frontDoors;
    public  AudioClip           bellSFX, doorSFX;

    private List<PassengerAI>   queue = new List<PassengerAI>();
    private List<Passenger>     passengers = new List<Passenger>();

    private bool                doorsOpen = false;

    private Queue<string>       route = new Queue<string>();

    private AudioSource         audio;

	void Awake() 
    {
        audio = gameObject.AddComponent<AudioSource>();
        audio.clip = bellSFX;
	}

    void Start()
    {
        route.Enqueue("Alpha Street");
        route.Enqueue("Beta Street");
    }

	void Update() 
    {
        if(route.Count > 0)
        {
            float dist = Vector2.Distance(transform.position, Station.Stations[route.Peek()].transform.position);

            foreach (Passenger p in passengers)
            {
                if (p.Destination == route.Peek() && dist <= p.DistanceTillRing && !Bell)
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
        if(other.tag == "Station" && other.GetComponent<Station>().name == route.Peek())
        {
            route.Dequeue();

            if(route.Count == 0)
                Debug.Log("TERMINUS.");
            else
                Debug.Log("Next Stop: " + route.Peek());
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

        if(!Doors)
            target.x -= 0.5f;
        else
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
