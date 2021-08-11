using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;

    [SerializeField] private AudioSource musicAudioSource;
    
    // Music audio clips
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;


    // One shot audio clips
    [SerializeField] private AudioClip flipSwitch;
    [SerializeField] private AudioClip startGame;
    [SerializeField] private AudioClip backToMainMenu;
    [SerializeField] private AudioClip setDifficulty;    
    [SerializeField] private AudioClip youWin;

    // One shot audio source
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayFlipSound()
    {
        _audioSource.PlayOneShot(flipSwitch, 0.65f);
    }

    public void PlayStartGameSound()
    {
        _audioSource.PlayOneShot(startGame, 0.5f);
    }

    public void PlaySetDifficultyGameSound()
    {
        _audioSource.PlayOneShot(setDifficulty, 0.45f);
    }

    public void PlayMainMenuSound()
    {
        _audioSource.PlayOneShot(backToMainMenu, 0.35f);
    }

    public void PlayYouWinSound()
    {
        _audioSource.PlayOneShot(youWin, 0.35f);
    }

    public void SetMenuMusic()
    {
        musicAudioSource.clip = menuMusic;
        musicAudioSource.Play();
    }

    public void SetGameplayMusic()
    {
        musicAudioSource.clip = gameplayMusic;
        musicAudioSource.Play();
    }

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void ToggleSound(bool on)
    {
        AudioSource[] allAudio = GetComponents<AudioSource>();

        for (int i = 0; i < allAudio.Length; ++i)
        {
            allAudio[i].mute = on;
        }
    }
}
