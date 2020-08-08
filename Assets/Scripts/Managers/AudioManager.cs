using UnityEngine;
using UnityEngine.Audio;

public abstract class Sound
{
    protected AudioSource audioSource;
    public string SoundName;
    public AudioClip AudioClip;
    [Range(0,1)]
    public float Volume = 0.5f;
    [Range(0.9f, 1.1f)]
    public float Pitch = 1f;

    public virtual void SetSource(AudioSource audioSource, AudioMixerGroup audioMixerGroup)
    {
        this.audioSource = audioSource;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.clip = AudioClip;
        audioSource.playOnAwake = false;
    }

    public virtual void Play()
    {
        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }
}

public class AudioManager : Singelton<AudioManager>
{
    private AudioMixer masterAudioMixer;

    private AudioMixerGroup[] audioMixerGroups;
    private float minVolume = 0.001f;

    private void Awake()
    {
        GetReferences();
    }

    private void GetReferences()
    {
        masterAudioMixer = Resources.Load<AudioMixer>("Audio/AudioMixers/MasterAudioMixer");
        audioMixerGroups = masterAudioMixer.FindMatchingGroups("Master");     
    }

    public AudioMixerGroup GetAudioMixerOutputChannel(string outputChannelName)
    {
        foreach (var audioMixerGroup in audioMixerGroups)
        {
            if (audioMixerGroup.name.Equals(outputChannelName))
            {
                return audioMixerGroup;
            }
        }

        return null;
    }

    public GameObject CreateNewGameObjectContainer(string containerName, Transform parent)
    {
        GameObject newGameObject = new GameObject(containerName);
        newGameObject.transform.SetParent(parent);
        return newGameObject;
    }

    public void MasterVolume(float volume)
    {
        volume = volume > minVolume ? volume : minVolume;
        masterAudioMixer.SetFloat("MasterVolume", Mathf.Log(volume) * 20);
    }

    public void SfxVolume(float volume)
    {
        volume = volume > minVolume ? volume : minVolume;
        masterAudioMixer.SetFloat("SfxVolume", Mathf.Log(volume) * 20);
    }

    public void MusicVolume(float volume)
    {
        volume = volume > minVolume ? volume : minVolume;
        masterAudioMixer.SetFloat("MusicVolume", Mathf.Log(volume) * 20);
    }
}
