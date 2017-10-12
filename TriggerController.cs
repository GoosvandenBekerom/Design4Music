using System;
using UnityEngine;

public enum PlayerState
{
    Idle = 0,
    Moving = 1,
    EnteringRoom = 2,
    IdleInRoom = 3,
    LeavingRoom = 4,
    Following = 5
}

public class TriggerController : MonoBehaviour
{
    public Transform Chagall;
    private Animator chagallAnimator;
    private Vector3 _chagallOffset;

    public static PlayerState State { get; set; }
    
    private float startTime;
    private float journeyLength;
    private float speed = 1.3F;
    private Vector3 startPos;
    private Vector3 destination;

    private Transform _room;

    void Awake()
    {
        MagnetSensor.OnCardboardTrigger += () =>
        {
            if (State == PlayerState.Moving || State == PlayerState.EnteringRoom || State == PlayerState.LeavingRoom) return;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)), out hit))
            {
                if (hit.transform.gameObject.CompareTag("StartButton"))
                {
                    GameManager.Instance.StartGame();
                    return;
                }

                if (!GameManager.Instance.GameStarted) return;

                if (hit.transform.gameObject.CompareTag("PlayButton"))
                {
                    hit.transform.parent.GetComponent<PlaneVideo>().Play();
                    return;
                }

                if (State == PlayerState.Idle)
                {
                    if (hit.transform.gameObject.CompareTag("Door"))
                    {
                        startTime = Time.time;
                        startPos = Chagall.position;
                        destination = hit.transform.Find("ChagallPosition").position;
                        journeyLength = Vector3.Distance(startPos, destination);
                        Chagall.LookAt(destination);
                        chagallAnimator.SetBool("Walking", true);

                        _room = hit.transform.parent;
                        State = PlayerState.Moving;
                    }
                }
                else if (State == PlayerState.IdleInRoom)
                {
                    if (hit.transform.gameObject.CompareTag("Exit"))
                    {
                        startTime = Time.time;
                        startPos = transform.position;
                        destination = Vector3.zero;
                        destination.y = transform.position.y;
                        journeyLength = Vector3.Distance(startPos, destination);
                        State = PlayerState.LeavingRoom;
                        _room.Find("Screen").GetComponent<PlaneVideo>().Stop();
                    }
                }
            }
        };
    }

    void Start()
    {
        State = PlayerState.Following;
        chagallAnimator = Chagall.GetComponent<Animator>();
        _chagallOffset = transform.position - Chagall.position;
    }

    private bool chagallMoving;

    void Update()
    {
        switch (State)
        {
            case PlayerState.Idle:
                // player is idle in the center of the main room
                break;
            case PlayerState.Moving:
                // Chagall is moving towards door
                var distCovered = (Time.time - startTime) * speed;
                var fracJourney = distCovered / journeyLength;
                Chagall.position = Vector3.Lerp(startPos, destination, fracJourney);

                if (fracJourney >= 0.95f)
                {
                    var point = transform.position;
                    point.y = Chagall.position.y;
                    Chagall.LookAt(point);
                    chagallAnimator.SetBool("Walking", false);

                    // Player is going to enter the room
                    State = PlayerState.EnteringRoom;
                    startTime = Time.time;
                    startPos = transform.position;
                    destination = _room.position;
                    destination.y = transform.position.y;
                    journeyLength = Vector3.Distance(startPos, destination);
                }
                break;
            case PlayerState.EnteringRoom:
                // Player is moving towards the center of a room
                var pos1 = transform.position;
                pos1.y = Chagall.transform.position.y;
                Chagall.LookAt(pos1);

                var covered = (Time.time - startTime) * speed;
                var journey = covered / journeyLength;
                transform.position = Vector3.Lerp(startPos, destination, journey);

                if (journey >= 0.95f)
                {
                    State = PlayerState.IdleInRoom;
                    //_room.Find("Screen").GetComponent<PlaneVideo>().Play();
                }
                break;
            case PlayerState.IdleInRoom:
                // Player is looking around in a room
                break;
            case PlayerState.LeavingRoom:
                // Player is exiting room
                var pos2 = transform.position;
                pos2.y = Chagall.transform.position.y;
                Chagall.LookAt(pos2);

                var covered2 = (Time.time - startTime) * speed;
                var journey2 = covered2 / journeyLength;
                transform.position = Vector3.Lerp(startPos, destination, journey2);

                if (journey2 >= 0.95f)
                {
                    State = PlayerState.Idle;
                }
                break;
            case PlayerState.Following:
                // Player is following chagall
                chagallMoving = true;
                transform.position = Chagall.position + _chagallOffset;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}