using UnityEngine;

public class MainMenuMusicLooper : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip song1;
    public AudioClip song2;

    private bool playingFirst = true; //Determines which song will be played

    void Start()
    {
        PlaySong(song1);
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            if (playingFirst)
            {
                PlaySong(song2);
            }
            else
            {
                PlaySong(song1);
            }

            playingFirst = !playingFirst;
        }
    }

    void PlaySong(AudioClip clip) // Subprogram to play song
    {
        audioSource.clip = clip;
        audioSource.Play();
    }
}
