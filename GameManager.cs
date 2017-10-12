using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public bool GameStarted { get; private set; }

    // Game Objects
    public Canvas StartCanvas;
    public GameObject Chagall;

    // Chagall components
    private PlayableDirector timeline;
    private VoiceController voice;
    
    private bool timelinePlaying;

	void Start ()
	{
	    Instance = this;
        DontDestroyOnLoad(gameObject);
	    voice = Chagall.GetComponent<VoiceController>();
        timeline = Chagall.GetComponent<PlayableDirector>();
	    GameStarted = false;
	}

    public void StartGame()
    {
        GameStarted = true;
        StartCanvas.gameObject.SetActive(false);
        voice.PlayEntranceClip();
        timeline.Play();
        timelinePlaying = true;
    }

    void Update()
    {
        if (!timelinePlaying) return;

        if (timeline.state != PlayState.Playing)
        {
            voice.PlayCenterClip();
            timelinePlaying = false;
            TriggerController.State = PlayerState.Idle;
        }
    }
}
