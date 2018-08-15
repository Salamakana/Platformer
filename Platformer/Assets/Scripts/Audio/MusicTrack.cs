using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class MusicTrack : Sound
{
    public bool IsLooping;

    public override void SetSource(AudioSource audioSource, AudioMixerGroup audioMixerGroup)
    {
        base.SetSource(audioSource, audioMixerGroup);
        audioSource.loop = IsLooping;
        SoundName = AudioClip.name;
    }

    public override void Play()
    {
        audioSource.volume = Volume;
        audioSource.pitch = Pitch;
        base.Play();
    }
}

