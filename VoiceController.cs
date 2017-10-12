using UnityEngine;

public class VoiceController : MonoBehaviour
{
    private GvrAudioSource source;

    public AudioClip EntranceClip;
    public AudioClip CenterClip;

    void Start()
    {
        source = GetComponent<GvrAudioSource>();
    }

    public void PlayEntranceClip()
    {
        PlayClip(EntranceClip);
    }

    public void PlayCenterClip()
    {
        PlayClip(CenterClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (source.isPlaying) return;

        source.clip = clip;
        source.Play();
    }
}
