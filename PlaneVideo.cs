using UnityEngine;
using UnityEngine.Video;

public class PlaneVideo : MonoBehaviour
{
    private VideoPlayer video;
    private GvrAudioSource audioSource;

    private GameObject playButton;

    void Start()
    {
        video = GetComponent<VideoPlayer>();
        audioSource = GetComponent<GvrAudioSource>();
        playButton = transform.Find("PlayButton").gameObject;
    }

    public void Play()
    {
        video.Play();
        audioSource.Play();
        playButton.SetActive(false);
    }

    public void Stop()
    {
        video.Stop();
        audioSource.Stop();
        playButton.SetActive(true);
    }
}