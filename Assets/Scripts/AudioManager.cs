using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public AudioClip flipSwitch;
    public AudioClip gameOver;
    public AudioClip restartGame;

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayFlipSound()
    {
        if (_audioSource != null)
        {
            _audioSource.pitch = Random.Range(0.80f, 1.0f);
            _audioSource.PlayOneShot(flipSwitch, 0.55f);
        }
    }

    public void PlayRestartGameSound()
    {
        _audioSource.PlayOneShot(restartGame, 0.5f);
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
