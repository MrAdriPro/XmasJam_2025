using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public bool musicManager = false;
    
    [SerializeField] AudioSource audioSource;
    public AudioClip[] audioClips;
    
    public int songToPlay;

    private void Start()
    {
        if (musicManager == true)
        {
            PlaySong(songToPlay);
        }
    }

    public void PlaySong(int clip)
    {
        if (audioClips.Length + 1 < clip) return;
        
        audioSource.clip = audioClips[clip];
        
        audioSource.Play();
    }
}
